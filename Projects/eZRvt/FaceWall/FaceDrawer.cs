using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.Exceptions;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using Autodesk.Revit.UI.Selection;
using eZRvt.GlobalSettings;
using RevitStd;
using InvalidOperationException = System.InvalidOperationException;
using OperationCanceledException = Autodesk.Revit.Exceptions.OperationCanceledException;
using View = Autodesk.Revit.DB.View;
using RevitStd.Geometry;

namespace eZRvt.FaceWall
{
    /// <summary>
    /// 
    /// </summary>
    public class FaceDrawer
    {
        private UIApplication _uiApp;
        private UIDocument _uiDoc;
        private Document _doc;
        private View _view;

        /// <summary>
        /// 当前的绘制操作是否还未完成。如果当前正在绘制，则不能继续绘制。
        /// </summary>
        private static bool _isDrawing;

        public void DrawFaces(UIDocument uiDoc, FaceOptions options)
        {
            _uiDoc = uiDoc;
            _uiApp = uiDoc.Application;

            // 通过 dockable pane 中的按钮点击后，uiDoc.ActiveView.Id.IntegerValue 与 uiDoc.ActiveGraphicalView.Id.IntegerValue 是相等的。
            // 但是此时程序的焦点并不会转到 GraphicalView中，而是处于Pane中。
            if (!_isDrawing)
            {
                _isDrawing = true;

                _doc = _uiDoc.Document;
                _view = _uiDoc.ActiveGraphicalView;

                // 先将必要的参数绑定到类别中
                using (Transaction trans = new Transaction(_doc, "为指定类别添加绑定参数"))
                {
                    try
                    {
                        trans.Start();
                        BindParametersToCategory(trans, _doc, options.CategoryId);
                        trans.Commit();
                    }
                    catch (Exception ex)
                    {
                        trans.RollBack();

                        throw new InvalidOperationException("为指定类别添加绑定参数出错", ex);
                    }
                }
                // 开始绘制
                Result res = Result.Failed;
                do
                {
                    res = options.MultiFaces ? DrawOneElemWithFaces(_uiDoc, options) : DrawFromOneFace(_uiDoc, options);

                } while (res == Result.Succeeded);

                // 此次绘制完成
                _isDrawing = false;
            }
        }

        /// <summary>
        /// 根据选择的一个平面或者曲线来绘制一个面层对象
        /// </summary>
        /// <param name="uiDoc"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private Result DrawFromOneFace(UIDocument uiDoc, FaceOptions options)
        {
            Result res = Result.Failed;

            // 在Revit界面中选择表面
            Reference faceRef = SelectSurface(uiDoc, options);

            if (faceRef != null)
            {
                // 1. 选择的单元
                Element struElement = _doc.GetElement(faceRef);

                // 2. 选择的表面
                GeometryObject obj = struElement.GetGeometryObjectFromReference(faceRef);
                if (!(obj is Face))
                {
                    MessageBox.Show("未检测到选择的面！", "提示");
                    return Result.Failed;
                }
                Face face = (Face)obj;

                // 3. 族实例对象的 变换
                Transform transform = GetElementTransform(struElement);

                // ----------------------------------------------------------------------------------------------------
                // 对不同的选择情况进行分类处理
                using (Transaction tran = new Transaction(_doc, "在选择的面上生成面层"))
                {
                    try
                    {
                        tran.Start();

                        // 根据不同的选择类型进行分类处理
                        if (face is PlanarFace)
                        {
                            PlanarFace pFace = (PlanarFace)face;

                            // 对单元中所有同方向的面均进行绘制进行特殊处理

                            if (options.IncludeSameNormal)
                            {
                                IList<DirectShape> surfaces = ExtendToFacesWithSameNormal(tran, struElement,
                                  pFace, options, transform);

                                // 设置面层对象的位置
                                ElementId[] ids = surfaces.Select(r => r.Id).ToArray();
                                uiDoc.Selection.SetElementIds(ids);
                            }
                            else  //　对于一般的单元进行操作
                            {
                                DirectShape ds = ConstructSurfaceFromPlanarFace(tran, pFace, options, transform);
                                if (ds != null)
                                {
                                    // 设置面层对象的位置
                                    uiDoc.Selection.SetElementIds(new ElementId[] { ds.Id });
                                }
                            }
                        }

                        else
                        {
                            // 将一个曲面转化为多个网格平面
                            SurfaceTriangulator st = new SurfaceTriangulator(face, transform);

                            List<PlanarFace> planarFaces = st.GetMeshedFaces();

                            List<FaceTransform> faceTransforms = planarFaces.Select(r => new FaceTransform(r, Transform.Identity)).ToList();
                            //
                            DirectShape ds = ConstructSurfaceFromPlanarFaces(tran, faceTransforms, options);

                            // throw new InvalidOperationException("无法在不是平面的表面生成面层。");
                        }

                        tran.Commit();
                        return Result.Succeeded;

                    }
                    catch (Exception ex)
                    {
                        // Utils.ShowDebugCatch(ex, "生成面层出错。");
                        tran.RollBack();
                        throw new InvalidOperationException("生成面层出错。", ex);
                        return Result.Failed;
                    }
                }
            }
            return res;
        }

        /// <summary>
        /// 选择多个面，并将对应的多个面层绘制到一个DirectShape中
        /// </summary>
        /// <param name="uiDoc"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        private Result DrawOneElemWithFaces(UIDocument uiDoc, FaceOptions options)
        {
            Result res = Result.Failed;

            // 在Revit界面中选择表面
            var faceRefs = SelectSurfaces(uiDoc, options);



            if (faceRefs != null && faceRefs.Count > 0)
            {

                // 提取每一个选择的表面，以及对应的单元的 Transform 对象
                List<FaceTransform> faceTransforms = new List<FaceTransform>();
                foreach (Reference faceRef in faceRefs)
                {
                    // 1. 选择的单元
                    Element struElement = _doc.GetElement(faceRef);

                    // 2. 选择的表面
                    GeometryObject obj = struElement.GetGeometryObjectFromReference(faceRef);
                    // 3. 族实例对象的 变换
                    Transform transform = GetElementTransform(struElement);

                    if (!(obj is PlanarFace))
                    {
                        Face face = (Face)obj;

                        // 将一个曲面转化为多个网格平面
                        SurfaceTriangulator st = new SurfaceTriangulator(face, transform);

                        List<PlanarFace> planarFaces = st.GetMeshedFaces();
                        // 一个曲面所对应的所有平面网格
                        List<FaceTransform> ft = planarFaces.Select(r => new FaceTransform(r, Transform.Identity)).ToList();

                        // 将这一个曲面所对应的三角形平面集合添加到总的集合中
                        faceTransforms.AddRange(ft);
                    }
                    else
                    {
                        PlanarFace pFace = (PlanarFace)obj;

                        faceTransforms.Add(new FaceTransform(pFace, transform));
                    }

                }

                // ----------------------------------------------------------------------------------------------------
                // 对不同的选择情况进行分类处理
                using (Transaction tran = new Transaction(_doc, "通过多个平面来生成一个面层对象"))
                {
                    try
                    {
                        tran.Start();

                        DirectShape ds = ConstructSurfaceFromPlanarFaces(tran, faceTransforms, options);

                        if (ds != null)
                        {
                            // 设置面层对象的位置
                            uiDoc.Selection.SetElementIds(new ElementId[] { ds.Id });
                        }

                        tran.Commit();
                        return Result.Succeeded;

                    }
                    catch (Exception ex)
                    {
                        //  Utils.ShowDebugCatch(ex, "生成面层出错。");
                        tran.RollBack();

                        throw new InvalidOperationException("生成面层出错。", ex);
                        //  return Result.Failed;
                    }
                }
            }
            return res;
        }


        #region ---   通过各种模式选择面层的表面

        /// <summary>
        /// 选择模型中的结构单元表面
        /// </summary>
        /// <returns></returns>
        private Reference SelectSurface(UIDocument uidoc, FaceOptions fOpt)
        {
            Reference boundaries = null;
            try
            {

                boundaries = uidoc.Selection.PickObject(ObjectType.Face, new FaceExcluder(_doc, fOpt.ExcludeFaceElement, onlyPlanarFace: fOpt.IncludeSameNormal), "选择结构的表面（平面对象）。");

            }
            catch (OperationCanceledException ex)
            {
                return null;
            }
            return boundaries;
        }

        /// <summary>
        /// 选择模型中的结构单元表面
        /// </summary>
        /// <returns></returns>
        private IList<Reference> SelectSurfaces(UIDocument uidoc, FaceOptions fOpt)
        {
            IList<Reference> boundaries = null;
            try
            {
                boundaries = uidoc.Selection.PickObjects(ObjectType.Face, new FaceExcluder(_doc, fOpt.ExcludeFaceElement, false), "选择结构的表面（平面对象）。");

            }
            catch (OperationCanceledException ex)
            {
                return null;
            }
            return boundaries;
        }

        #endregion

        #region ---   确定是否要进行变换


        /// <summary>
        /// 将模型空间中选择到的 pickedFace（此面属性模型空间中的 bindedElement 上） 按指定的变换返回其在模型空间中的重合面
        /// </summary>
        /// <param name="bindedElement"> 用户在模型中选择表面所附着的那个单元，如果此单元不是 FamilyInstance ，
        /// 则其GetTransform()返回的肯定是一个Identity，反之则不是；但是此此单元是否被修改过决定了选择的 Face 的位置是相对于模型空间还是族空间。 </param>
        /// <param name="pickedFace"> 用户在模型中通过“uidoc.Selection.PickObject(ObjectType.Face, "选择结构的表面。")”选择的实例对象的表面 </param>
        /// <returns></returns>
        private Transform GetElementTransform(Element bindedElement)
        {
            if (!(bindedElement is FamilyInstance))
            {
                // 对于不是 FamilyInstance 的对象，其没有空间坐标变换的问题，所以直接返回不变换即可。
                return Transform.Identity;
            }

            FamilyInstance ins = (FamilyInstance)bindedElement;

            if (ins.HasModifiedGeometry())
            {
                return Transform.Identity;
            }
            return ins.GetTransform();
        }

        #endregion

        #region ---   创建面层实体

        /// <summary>
        /// 将楼梯中所有与选择的表面相同方向的面上都生成面层
        /// </summary>
        /// <param name="docTran">  </param>
        /// <param name="elem"> 用来生成面层的单元。此函数会搜索此单元中所有与 baseFace 同法向的面，并进行面层的绘制 </param>
        /// <param name="pickedFace"> 楼梯中用来生成面层的那个面</param>
        /// <param name="faceOp"> 生成面层的选项</param>
        /// <param name="transf"> </param>
        /// <returns></returns>
        private IList<DirectShape> ExtendToFacesWithSameNormal(Transaction docTran, Element elem, PlanarFace pickedFace, FaceOptions faceOp, Transform transf)
        {
            IList<DirectShape> directShapes = new List<DirectShape>();

            // 值中的false 表示solid 是 Family中的solid，后期还要再进行一次transform才能变换 Familyinstance 所对应的位置
            Dictionary<Solid, bool> solids = GeoHelper.GetSolidsInModel(elem, GeoHelper.SolidVolumnConstraint.Positive);

            foreach (Solid solid in solids.Keys)
            {
                foreach (Face face in solid.Faces)
                {
                    if ((face is PlanarFace))
                    {
                        PlanarFace planarFace = (PlanarFace)face;

                        if (GeoHelper.IsSameDirection(planarFace.FaceNormal, transf.OfVector(pickedFace.FaceNormal)))
                        {
                            DirectShape ds = ConstructSurfaceFromPlanarFace(docTran, planarFace, faceOp, Transform.Identity);

                            if (ds != null)
                            {
                                directShapes.Add(ds);
                            }
                        }
                    }
                }
            }

            return directShapes;
        }

        /// <summary>
        /// 从单一平面来生成面层对象
        /// </summary>
        /// <param name="face"> 用来生成面层的那个面 </param>
        /// <param name="faceOp"> 生成面层的选项 </param>
        /// <returns> 如果新生成的面层实体或者进行剪切后的实体体积太小，则认为不会生成此实体（在施工中也不可能考虑如此精细的施工），此时返回 null</returns>
        /// <param name="transform"> 从族几何到实例对象的变换 </param>
        private DirectShape ConstructSurfaceFromPlanarFace(Transaction docTran, PlanarFace face, FaceOptions faceOp, Transform transform)
        {
            Solid solid = ExtrudeSolid(face, transform, faceOp.SurfaceThickness);
            if (solid.Volume < 0.00001)
            {
                // 如果新生成的面层实体体积太小，则认为不会生成此实体（在施工中也不可能考虑如此精细的施工）
                return null;
            }

            docTran.SetName("生成面层");

            // create direct shape and assign the sphere shape

            DirectShape ds = DirectShape.CreateElement(_doc, faceOp.CategoryId,
                                                       "Application id",
                                                       "Geometry object id");
            ds.SetShape(new GeometryObject[] { solid });

            _doc.Regenerate();

            // 判断生成的实体与周围的结构的相交关系
            bool hasIntersect;
            IList<Solid> solids = ExcludeIntersect(ds, new Solid[] { solid }, out hasIntersect);

            if (hasIntersect)
            {
                if (!solids.Any())
                {
                    // 如果新生成的面层实体体积太小，则认为不会生成此实体（在施工中也不可能考虑如此精细的施工）
                    _doc.Delete(ds.Id);
                    return null;
                }

                solid = solids.First();

                // 删除原来的实体，并根据剪切后的实体重新创建
                _doc.Delete(ds.Id);

                if (solid.Volume < 0.000001)
                {
                    // 如果新生成的面层实体被剪切之后体积太小，则认为不会生成此实体（在施工中也不可能考虑如此精细的施工）
                    return null;
                }
                else
                {
                    // create direct shape and assign the sphere shape
                    ds = DirectShape.CreateElement(_doc, faceOp.CategoryId,
                                                               "Application id",
                                                               "Geometry object id");
                    ds.SetShape(new GeometryObject[] { solid });
                }
            }

            PlanarFace pFace = CorrespondFace(solid, face, transform);

            //  设置面层单元的各种参数
            SetParameters(docTran, ds, volumn: solid.Volume, area: pFace.Area, faceOptions: faceOp);

            return ds;
        }

        /// <summary>
        /// 将多个平面生成为一个面层对象
        /// </summary>
        /// <param name="docTran">  </param>
        /// <param name="faceTransforms"> 键表示用来生成面层的多个面，对应的值表示将 face 对象 转换到模型空间中所需要进行的变换 </param>
        /// <param name="faceOp"> 生成面层的选项 </param>
        /// <returns> 如果新生成的面层实体或者进行剪切后的实体体积太小，则认为不会生成此实体（在施工中也不可能考虑如此精细的施工），此时返回 null</returns>
        private DirectShape ConstructSurfaceFromPlanarFaces(Transaction docTran, List<FaceTransform> faceTransforms, FaceOptions faceOp)
        {
            IList<Solid> solids = new List<Solid>();

            foreach (FaceTransform face_Transform in faceTransforms)
            {
                PlanarFace face = face_Transform.planarFace;
                Transform transform = face_Transform.transform;

                Solid solid = ExtrudeSolid(face, transform, faceOp.SurfaceThickness);
                solids.Add(solid);
            }


            docTran.SetName("通过多个平面来生成一个面层对象");
            // create direct shape and assign the sphere shape
            DirectShape ds = DirectShape.CreateElement(_doc, faceOp.CategoryId,
                                                      "Application id",
                                                      "Geometry object id");
            ds.SetShape(solids.Select(r => r as GeometryObject).ToList());

            _doc.Regenerate();

            bool reSetFace = false; // 是否要将当前的面层对象删除后再重新生成


            bool hasIntersect;
            // 将初步生成的面层对象与周围的环境相剪切，并返回剪切后的实体集合。
            // 集合中的Solid的体积可能为零，说明它已经被剪切掉了，但是其定义还在。
            IList<Solid> extrudedSolids = ExcludeIntersect(ds, solids, out hasIntersect);

            // 在将此面层对象中的多个Solid进行组合之前，先获取到面层对象的面积

            double area = 0;
            int index = 0;
            foreach (var faceTransform in faceTransforms)
            {
                PlanarFace f = faceTransform.planarFace;
                Transform transform = faceTransform.transform;
                //
                Solid solid = extrudedSolids[index];
                if (solid.Volume > 0)
                {
                    Face face = CorrespondFace(solid, f, transform);
                    //
                    area += face.Area;
                }
                //
                index += 1;
            }

            IList<Solid> finnalSolids;
            if (faceOp.UnionInnerSolids)
            {
                // 将此面层对象中的多个Solid进行组合，最后集合中就只包含一个用来Union的Solid，以及不能成功被Union的Solids。
                finnalSolids = UnionSolid(extrudedSolids);
            }
            else
            {
                finnalSolids = extrudedSolids;
            }

            // 计算组合之后的最终面层的真实体积。

            double volumn = finnalSolids.Sum(s => s.Volume);

            // 不管原来生成的面层对象与周围环境是否有相交，都删除原来的实体，并根据剪切后的实体重新创建
            _doc.Delete(ds.Id);

            // create direct shape and assign the sphere shape
            ds = DirectShape.CreateElement(_doc, faceOp.CategoryId,
                                                      "Application id",
                                                      "Geometry object id");
            ds.SetShape(finnalSolids.Select(r => r as GeometryObject).ToList());
            // ds.SetShape(new GeometryObject[] { unionedSolid });

            //  设置面层单元的各种参数
            SetParameters(docTran, ds, volumn: volumn, area: area, faceOptions: faceOp);

            return ds;
        }

        /// <summary>
        /// 根据指定的平面与变换关系拉伸出一个实体
        /// </summary>
        /// <param name="face"> </param>
        /// <param name="transform"> 将 face 对象 转换到模型空间中所需要进行的变换</param>
        /// <param name="thickNess"> 面层的厚度，单位为米 </param>
        /// <returns></returns>
        private Solid ExtrudeSolid(PlanarFace face, Transform transform, double thickNess)
        {
            Solid solid = null;

            IList<CurveLoop> curveLoops = face.GetEdgesAsCurveLoops();

            //drawCurve(curveLoops);

            // 要在实体还未创建之前就定位好其在模型空间中的位置，因为后面要依据此位置来进行相交判断。
            foreach (CurveLoop curveLoop in curveLoops)
            {
                curveLoop.Transform(transform);
            }

            //IList<CurveLoop> curveLoops;
            //CurvesFormator.GetContiguousCurvesFromEdgeArrArray(face.EdgeLoops, out curveLoops);

            XYZ extrusionDir = transform.OfVector(face.FaceNormal);

            solid = GeometryCreationUtilities.CreateExtrusionGeometry(
                profileLoops: curveLoops,
                extrusionDir: extrusionDir,
                extrusionDist: UnitUtils.ConvertToInternalUnits(thickNess, DisplayUnitType.DUT_METERS));
            return solid;
        }


        /// <summary>
        /// 根据用户选择的面来匹配模型中实体所对应的面（用来计算实际的面层面积）
        /// </summary>
        /// <param name="solidInModel">此实体由pickedFace拉伸后生成，位于模型空间中的实体，而且其在进行过交叉剪切操作后的实体。</param>
        /// <param name="pickedFace"></param>
        /// <param name="transf"></param>
        /// <returns></returns>
        private PlanarFace CorrespondFace(Solid solidInModel, PlanarFace pickedFace, Transform transf)
        {
            XYZ pickedFaceNormal = transf.OfVector(pickedFace.FaceNormal);
            foreach (Face f in solidInModel.Faces)
            {
                PlanarFace pf = f as PlanarFace;

                // 先比较法向是否相同
                if (pf != null && GeoHelper.IsSameDirection(pf.FaceNormal, pickedFaceNormal))
                {
                    return pf;
                }
            }
            return pickedFace;
        }

        /// <summary>
        /// 判断当前创建出来的那个实体是否与其他单元相交，如果相交，则在原实体中剪除相交的部分，如果没有相交，则直接返回原实体集合（集合中的元素个数与原 originalSolids 集合中元素个数相同）。
        /// </summary>
        /// <param name="directShape"></param>
        /// <param name="originalSolids"> directShape 所对应的实体，由于 ExecuteBooleanOperationModifyingOriginalSolid 函数中的 OriginalSolid 
        /// 不能是直接从Revit的Element中得到的，所以要将前面通过轮廓拉伸出来的实体作为参数传入。</param>
        /// <param name="hasIntersect"></param>剪切后的实体的体积有可能不大于 0 啊
        /// <returns> 返回的集合中的元素个数与原 originalSolids 集合中元素个数相同。剪切后的实体的体积有可能不大于 0 .</returns>
        private IList<Solid> ExcludeIntersect(DirectShape directShape, IList<Solid> originalSolids, out bool hasIntersect)
        {
            // 应用过滤器，在整个文档中搜索与指定Element相交的Element
            FilteredElementCollector collector = new FilteredElementCollector(directShape.Document);
            ElementIntersectsElementFilter elementFilter = new ElementIntersectsElementFilter(element: directShape, inverted: false);
            collector.WherePasses(elementFilter);

            // 排除面层本身
            collector.Excluding(new ElementId[] { directShape.Id });

            if (!collector.Any())
            {
                // 说明没有相交的部分
                hasIntersect = false;
                return originalSolids;
            }

            hasIntersect = true;

            // 将与其相交的实体进行剪切操作
            bool promptWhileError = false;

            foreach (Element interSectElem in collector)
            {
                var interSectSolids = GeoHelper.GetSolidsInModel(interSectElem, GeoHelper.SolidVolumnConstraint.Positive).Keys;  // 与面层对象相交的 Element 中所有的实体
                for (int i = 0; i < originalSolids.Count; i++)
                {
                    Solid originalS = originalSolids[i];

                    foreach (Solid interSectS in interSectSolids)
                    {
                        try
                        {
                            //  在原实体中减去相交的部分
                            BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(originalS, interSectS, BooleanOperationsType.Difference);

                        }
                        catch (Exception ex)
                        {
                            if (promptWhileError)
                            {
                                // 在剪切时如果不能剪切，则不剪切。
                                DialogResult res = MessageBox.Show("实体剪切时出现错误，可能的原因是面层与模型中的其他实体有细微交叉，" +
                                                                   "以致剪切后的实体出现细小锯齿。\n\r （忽略此细微交叉对于面层算量并无明显影响）。" +
                                                   " \n\r 点击“是”以忽略并继续，点击“否”不再提示。", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Hand, MessageBoxDefaultButton.Button2);

                                promptWhileError = (res != DialogResult.No);
                            }

                        }
                    }
                    // 剪切后的实体的体积有可能不大于 0 啊
                    originalSolids[i] = originalS;  // 将剪切完成后的 Solid 再赋值回集合中
                }
            }
            return originalSolids;
        }

        /// <summary>
        /// 将一个directShape中的多个Solid进行合并
        /// </summary>
        /// <param name="originalSolids"></param>
        /// <returns>理想情况下，返回的集合中应该只有一个Solid，但是由于Union操作不一定每次都能成功，所以此集合中还会有出现不成功时的那些Solid。</returns>
        private List<Solid> UnionSolid(IList<Solid> originalSolids)
        {
            List<Solid> solids = new List<Solid>();
            double v1 = 0;
            foreach (Solid s in originalSolids)
            {
                //
                v1 += s.Volume;
            }

            Solid s1 = originalSolids[0];
            solids.Add(s1);

            for (int i = 1; i < originalSolids.Count; i++)
            {
                Solid s2 = originalSolids[i];
                try
                {
                    //  将两个实体进行组合
                    BooleanOperationsUtils.ExecuteBooleanOperationModifyingOriginalSolid(s1, s2, BooleanOperationsType.Union);
                }
                catch (Exception ex)
                {
                    // 说明实体组合不成功，此时应该将未组合进去的Solid记录下来
                    solids.Add(s2);
                }
            }
            return solids;
        }


        #endregion

        #region ---   类别参数的创建 与 实例参数的设置

        /// <summary>
        /// 将外部共享参数创建为项目参数，并将其绑定到指定的类别
        /// </summary>
        /// <param name="transDoc"> bindingMap.Insert 与 bindingMap.ReInsert 方法必须在事务内才能执行</param>
        /// <param name="doc"></param>
        /// <param name="facenCategoryId"> 面层对象所属的类别 </param>
        public void BindParametersToCategory(Transaction transDoc, Document doc, ElementId facenCategoryId)
        {
            // 打开共享文件
            Autodesk.Revit.ApplicationServices.Application app = doc.Application;
            string OriginalSharedFileName = app.SharedParametersFilename; // Revit程序中，原来的共享文件路径
            
            app.SharedParametersFilename = ProjectPath.SharedParameters; // 设置Revit的共享参数文件

            DefinitionFile myDefinitionFile = app.OpenSharedParameterFile(); // 如果没有找到对应的文件，则打开时不会报错，而是直接返回Nothing
            // app.SharedParametersFilename = OriginalSharedFileName; // 将Revit程序中的共享文件路径还原，以隐藏插件程序中的共享参数文件。

            // create a new group in the shared parameters file
            DefinitionGroup myGroup = myDefinitionFile.Groups.get_Item(FaceWallParameters.sp_Group_Face);

            // 提取此共享参数组中的每一个参数，并赋值给指定的类别
            ExternalDefinition exdef_FaceIdTag = (ExternalDefinition)myGroup.Definitions.get_Item(FaceWallParameters.sp_FaceIdTag);
            ExternalDefinition exdef_FaceType = (ExternalDefinition)myGroup.Definitions.get_Item(FaceWallParameters.sp_FaceType);
            ExternalDefinition exdef_Volumn = (ExternalDefinition)myGroup.Definitions.get_Item(FaceWallParameters.sp_Volumn);
            ExternalDefinition exdef_Area = (ExternalDefinition)myGroup.Definitions.get_Item(FaceWallParameters.sp_Area);

            //
            BindingMap bindingMap = doc.ParameterBindings;
            Category myCategory = Category.GetCategory(doc, facenCategoryId);

            // ---------------------------------------------------------------------------------------------------------
            // 判断指定的类别中是否绑定有此 "面层标识 FaceIdTag" 参数
            bool faceParaHasInsertedIntoThatCategory = false;
            bool parameterHasBeenAddedToProject = false;
            if (bindingMap.Contains(exdef_FaceIdTag))
            {
                Autodesk.Revit.DB.Binding parameterBinding = doc.ParameterBindings.get_Item(exdef_FaceIdTag);
                parameterHasBeenAddedToProject = true;

                if (parameterBinding is InstanceBinding)
                {
                    // 外部共享参数 exdef_FaceIdTag 在此文档中 绑定到了哪些类别
                    CategorySet bindedCategories = ((InstanceBinding)parameterBinding).Categories;
                    if (bindedCategories.Contains(myCategory))
                    {
                        faceParaHasInsertedIntoThatCategory = true;
                    }
                }
            }

            // ---------------------------------------------------------------------------------------------------------
            // 如果此参数"面层标识 FaceIdTag"还没有被添加到项目参数中，则先将用 Insert 其进行添加
            if (!parameterHasBeenAddedToProject)
            {
                //  在“项目参数”中添加此参数，并为其指定一个绑定的类别。
                CategorySet myCategories = app.Create.NewCategorySet();
                myCategories.Insert(myCategory);

                //Create an instance of InstanceBinding
                InstanceBinding instanceBinding = app.Create.NewInstanceBinding(myCategories);
                // Get the BingdingMap of current document.

                // Bind the definitions to the document
                bindingMap.Insert(exdef_FaceIdTag, instanceBinding, BuiltInParameterGroup.PG_IDENTITY_DATA);  //  此 Insert 操作必须在事务开启后才能执行
                bindingMap.Insert(exdef_FaceType, instanceBinding, BuiltInParameterGroup.PG_IDENTITY_DATA);
                bindingMap.Insert(exdef_Volumn, instanceBinding, BuiltInParameterGroup.PG_IDENTITY_DATA);
                bindingMap.Insert(exdef_Area, instanceBinding, BuiltInParameterGroup.PG_IDENTITY_DATA);
            }

            // ---------------------------------------------------------------------------------------------------------
            // 如果"面层标识 FaceIdTag" 参数已经绑定到了指定的类别上，就认为其他三个参数也绑定上来了，此时就不需要再绑定了；
            // 如果没有绑定，则要将这四个参数一次性全部绑定到此指定的类别上
            if (parameterHasBeenAddedToProject && !faceParaHasInsertedIntoThatCategory)
            {
                Autodesk.Revit.DB.Binding parameterBinding = doc.ParameterBindings.get_Item(exdef_FaceIdTag);
                ((InstanceBinding)parameterBinding).Categories.Insert(myCategory);  // 将新的类别添加到 binding 的类别集合中

                // 重新进行绑定
                bindingMap.ReInsert(exdef_FaceIdTag, parameterBinding, BuiltInParameterGroup.PG_IDENTITY_DATA);
                bindingMap.ReInsert(exdef_FaceType, parameterBinding, BuiltInParameterGroup.PG_IDENTITY_DATA);
                bindingMap.ReInsert(exdef_Volumn, parameterBinding, BuiltInParameterGroup.PG_IDENTITY_DATA);
                bindingMap.ReInsert(exdef_Area, parameterBinding, BuiltInParameterGroup.PG_IDENTITY_DATA);
            }
        }

        /// <summary>
        /// 为面层实体设置颜色、类型等参数
        /// </summary>
        /// <param name="tran"></param>
        /// <param name="ds"></param>
        /// <param name="volumn"></param>
        /// <param name="area"></param>
        /// <param name="faceOptions"></param>
        private void SetParameters(Transaction tran, DirectShape ds, double volumn, double area, FaceOptions faceOptions)
        {
            // 设置实体对象在指定视力中的填充颜色
            OverrideGraphicSettings gs = _view.GetElementOverrides(ds.Id);

            // 填充颜色
            gs.SetProjectionFillColor(faceOptions.Color);

            // 填充模式

            string fillPatternName = "实体填充";
            FillPatternElement fpe = FillPatternElement.GetFillPatternElementByName(_doc, FillPatternTarget.Drafting, fillPatternName);
            if (fpe == null)
            {
                fpe = FillPatternElement.Create(_doc,
                      new FillPattern(fillPatternName, FillPatternTarget.Drafting, FillPatternHostOrientation.ToHost));
            }

            gs.SetProjectionFillPatternId(fpe.Id);
            gs.SetProjectionFillPatternVisible(true);
            _view.SetElementOverrides(ds.Id, gs);

            // 设置参数
            Parameter p;

            // 面层对象标识
            p = ds.get_Parameter(FaceWallParameters.sp_FaceIdTag_guid);
            if (p != null)
            {
                p.Set(FaceWallParameters.FaceIdentificaion);
            }

            // 面积
            p = ds.get_Parameter(FaceWallParameters.sp_Area_guid);
            if (p != null)
            {
                p.Set(area);
            }

            // 体积
            p = ds.get_Parameter(FaceWallParameters.sp_Volumn_guid);
            if (p != null)
            {
                p.Set(volumn);
            }

            // 类型
            p = ds.get_Parameter(FaceWallParameters.sp_FaceType_guid);
            if (p != null)
            {
                p.Set(faceOptions.FaceType);
            }

        }

        #endregion

        private class FaceTransform
        {
            public readonly PlanarFace planarFace;
            public readonly Transform transform;

            public FaceTransform(PlanarFace pf, Transform trans)
            {
                planarFace = pf;
                transform = trans;
            }
        }
    }

    /// <summary>
    /// 在选择表面时排除WallFace对象
    /// </summary>
    /// <remarks></remarks>
    public class FaceExcluder : ISelectionFilter
    {
        private readonly Document _doc;
        private readonly bool _excludeFaceElement;
        private readonly bool _onlyPlanarFace;
        private Element _elem;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="excludeFaceElement">不选择面层对象</param>
        /// <param name="onlyPlanarFace">只能选择平面几何</param>
        public FaceExcluder(Document doc, bool excludeFaceElement, bool onlyPlanarFace)
        {
            _doc = doc;
            _excludeFaceElement = excludeFaceElement;
            _onlyPlanarFace = onlyPlanarFace;
        }

        public bool AllowElement(Element element)
        {
            _elem = element;

            //
            if (_excludeFaceElement)  // 不选择面层对象
            {
                WallFace wf;
                if (WallFace.IsWallFace(element, out wf))
                {
                    return false;
                }
                return true;
            }
            return true;
        }

        public bool AllowReference(Reference refer, XYZ point)
        {
            if (_onlyPlanarFace && _elem != null)
            {
                var f = _elem.GetGeometryObjectFromReference(refer);
                if (f is PlanarFace)
                {
                    return true;
                }
                return false;
            }
            return true;
        }
    }

}