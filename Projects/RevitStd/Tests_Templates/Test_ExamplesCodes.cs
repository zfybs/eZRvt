#region

using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

#endregion

namespace RevitStd.Tests_Templates
{
    /// <summary>
    /// 一些效果实现的示例代码
    /// </summary>
    /// <remarks></remarks>
    public class Test_ExamplesCodes
    {
        private UIDocument UIDoc;
        private Document Doc;
        private Application App;

        /// <summary>
        /// 返回Element的几何数据
        /// </summary>
        /// <remarks></remarks>
        public void GetGeometry()
        {
            //选择一根柱子
            Reference ref1 = UIDoc.Selection.PickObject(ObjectType.Element, "Please pick a column");
            Element elem = Doc.GetElement(ref1);
            FamilyInstance column = elem as FamilyInstance;


            Options opt = new Options();
            opt.ComputeReferences = true;
            opt.DetailLevel = ViewDetailLevel.Medium;

            GeometryElement geoElem_Colu = column.get_Geometry(opt);

            // 柱子中的各种图形
            foreach (GeometryObject obj in geoElem_Colu)
            {
                // 如果柱子被切割了，那么此柱子的几何信息与定义此柱子的族的几何信息就不一样了，所以，被切割的柱子的GeometryElement之中就直接包含有Solid
                if (obj is Solid)
                {
                    Solid solid = obj as Solid;

                    // 如果柱子未被切割，则GeometryElement之中就包含的就是GeometryInstance
                }
                else if (obj is GeometryInstance)
                {
                    GeometryInstance geoInstance = obj as GeometryInstance;
                    // 返回族实例的几何数据
                    GeometryElement geoElement = geoInstance.GetInstanceGeometry();
                    //
                    foreach (GeometryObject obj2 in geoElement)
                    {
                        if (obj2 is Solid)
                        {
                            Solid solid2 = obj2 as Solid;
                            //对象几何数据中可能包含没有属性的Solid，需要排除它。
                            if (solid2.Volume > 0) // 判断此Solid是否有效
                            {
                            }
                        }
                    }
                }
            }
        }

        /// <summary> 射线法：找到与梁相交的所有墙对象 </summary>
        public Result FindSupporting()
        {
            Transaction trans = new Transaction(Doc, "ExComm");
            trans.Start();

            // 在界面中选择一个梁
            Selection sel = UIDoc.Selection;
            Reference ref1 = sel.PickObject(ObjectType.Element, "Please pick a beam");
            FamilyInstance beam = Doc.GetElement(ref1) as FamilyInstance;

            //Read the beam's location line
            LocationCurve lc = beam.Location as LocationCurve;
            Curve curve = lc.Curve;

            // 将梁的起点和端点作为射线的原点与方向
            XYZ ptStart = curve.GetEndPoint(0);
            XYZ ptEnd = curve.GetEndPoint(1);

            //move the two point a little bit lower, so the ray can go through the wall
            // 将这两个点向下移动一点点，来让引射线可以穿过墙
            XYZ offset = new XYZ(0, 0, 0.01);
            ptStart = ptStart - offset;
            ptEnd = ptEnd - offset;

            // 将当前3D视图作为ReferenceIntersector的构造参数
            View3D view3d = null;
            view3d = Doc.ActiveView as View3D;
            if (view3d == null)
            {
                TaskDialog.Show("3D view", "current view should be 3D view");
                return Result.Failed;
            }

            // 执行射线相交。注意此射线是无限延长的，如果没有指定ReferenceIntersector中的搜索范围，则会在整个项目中的所有Element中进行相交运算。
            double beamLen = Convert.ToDouble(curve.Length);
            ReferenceIntersector ReferenceIntersector1 = new ReferenceIntersector(view3d);
            IList<ReferenceWithContext> references = ReferenceIntersector1.Find(ptStart, ptEnd - ptStart);

            // 清除已经选择的对象
            sel.SetElementIds(new List<ElementId>());
            // 返回已经选择的对象
            ICollection<ElementId> Ge = sel.GetElementIds();

            // 找到所有相交对象中，与梁相交的墙对象
            double tolerate = 0.00001;
            foreach (ReferenceWithContext reference in references)
            {
                Reference ref2 = reference.GetReference();
                ElementId id = ref2.ElementId;
                Element elem = Doc.GetElement(id);
                //
                if (elem is Wall)
                {
                    // 如果与射线相交的对象到射线原点的距离比梁的长度小，说明，此对象是与梁相交的
                    // 如果相交面与梁的端面重合，则可以设置一个tolerate，这样的话，那个与之重合的面也可以被选中了。
                    if (reference.Proximity < beamLen + tolerate)
                    {
                        Ge.Add(elem.Id);
                    }
                }
            }
            // 整体选择所有与梁相交的墙
            sel.SetElementIds(Ge);
            trans.Commit();
            // Change
            return Result.Succeeded;
        }

        /// <summary> 找到与某Element的几何实体相交的Elements </summary>
        public void FindIntersectWallsByElement()
        {
            Transaction trans = new Transaction(Doc, "ExComm");
            trans.Start();

            // 选择一个柱子
            Selection sel = UIDoc.Selection;
            Reference ref1 = sel.PickObject(ObjectType.Element, "Please pick a column");
            Element column = Doc.GetElement(ref1);

            // 应用过滤器，在整个文档中搜索与指定Element相交的Element
            FilteredElementCollector collector = new FilteredElementCollector(Doc);
            ElementIntersectsElementFilter elementFilter = new ElementIntersectsElementFilter(column, false);
            collector.WherePasses(elementFilter);

            // 排除柱子本身
            List<ElementId> excludes = new List<ElementId>();
            excludes.Add(column.Id);
            collector.Excluding(excludes);

            //将最终的相交结果选中
            List<ElementId> selEle = new List<ElementId>();
            foreach (Element elem in collector)
            {
                selEle.Add(elem.Id);
            }
            sel.SetElementIds(selEle);

            trans.Commit();
        }

        /// <summary>ElementIntersectsSolidFilter: 找到与某Solid相交的Elements </summary>
        public void FindIntersectWallsByGeometry()
        {
            Transaction trans = new Transaction(Doc, "ExComm");
            trans.Start();

            // ---------------- 在API内存中创建一个拉伸Solid的定义（但是并不包含在Document中） ----------------
            //pick a point to draw solid
            Selection sel = UIDoc.Selection;
            XYZ pt = sel.PickPoint("Please pick a point to get the close walls");

            XYZ pttemp1 = sel.PickPoint(ObjectSnapTypes.None, "Pick leader end...");
            XYZ pttemp2 = sel.PickPoint(ObjectSnapTypes.None, "Pick leader elbow...");

            double dBoxLength = 3;
            // 创建进行拉伸的闭合曲线
            XYZ pt1 = new XYZ(pt.X - dBoxLength/2, pt.Y - dBoxLength/2, pt.Z);
            XYZ pt2 = new XYZ(pt.X + dBoxLength/2, pt.Y - dBoxLength/2, pt.Z);
            XYZ pt3 = new XYZ(pt.X + dBoxLength/2, pt.Y + dBoxLength/2, pt.Z);
            XYZ pt4 = new XYZ(pt.X - dBoxLength/2, pt.Y + dBoxLength/2, pt.Z);

            // 闭合曲线的四条边
            Line lineBottom = Line.CreateBound(pt1, pt2);
            Line lineRight = Line.CreateBound(pt2, pt3);
            Line lineTop = Line.CreateBound(pt3, pt4);
            Line lineLeft = Line.CreateBound(pt4, pt1);

            // 将四条边连接起来
            CurveLoop profile = new CurveLoop();
            profile.Append(lineBottom);
            profile.Append(lineRight);
            profile.Append(lineTop);
            profile.Append(lineLeft);

            // 创建闭合的连续曲线段
            List<CurveLoop> loops = new List<CurveLoop>();
            loops.Add(profile);

            // 创建出一个Solid：此Solid只是为了与其他API交互，并未保存在Document中，所以也不可见。
            XYZ vector = new XYZ(0, 0, 1);
            Solid solid = GeometryCreationUtilities.CreateExtrusionGeometry(loops, vector, 10);

            // 在整个文档中搜索与此虚拟定义的Solid相交的Element
            FilteredElementCollector collector = new FilteredElementCollector(Doc);
            ElementIntersectsSolidFilter solidFilter = new ElementIntersectsSolidFilter(solid);
            collector.WherePasses(solidFilter);

            // 将最终的相交结果选中
            List<ElementId> selEle = new List<ElementId>();
            foreach (Element elem in collector)
            {
                selEle.Add(elem.Id);
            }
            sel.SetElementIds(selEle);
            //
            trans.Commit();
        }

        /// <summary>
        /// ElementParameterFilter 参数过滤器：找到一个房间内的所有对象
        /// </summary>
        /// <remarks>提示: 参数过滤条件可能比其他的类型过滤条件要快，但是这要视条件而定。毕竟这是一个慢过，使用时请按照过滤标准的复杂程度而异。</remarks>
        public void ElementParameterFilter_FindOjbectsInSpecificRoom(ExternalCommandData commandData)
        {
            UIApplication app = commandData.Application;
            Document document = app.ActiveUIDocument.Document;
            //pick a room
            Selection sel = app.ActiveUIDocument.Selection;
            Reference ref1 = sel.PickObject(ObjectType.Element, "Please pick a room");
            Room room = document.GetElement(ref1) as Room;

            // 定义要过滤哪个参数 ParameterValueProvider ：表示Element的房间Id的参数。
            ParameterValueProvider provider = new ParameterValueProvider(new ElementId(BuiltInParameter.ELEM_ROOM_ID));
            // 定义过滤规则的表达式 FilterNumericRuleEvaluator 或者 FilterStringRuleEvaluator
            FilterNumericRuleEvaluator evaluator = new FilterNumericEquals();
            // 定义要过滤的类型 FilterRule
            FilterElementIdRule rule = new FilterElementIdRule(provider, evaluator, room.Id);
            // 最终得到参数过滤器 ElementParameterFilter
            ElementParameterFilter filter = new ElementParameterFilter(rule);

            // 在什么范围内进行过滤
            FilteredElementCollector collector = new FilteredElementCollector(document);
            // 执行过滤操作，并得到过滤后的结果
            collector.WherePasses(filter);
        }


        /// <summary>
        /// ElementParameterFilter参数过滤器 Creates an ElementParameter filter to find rooms whose area is greater than specified value
        /// </summary>
        /// <remarks>提示: 参数过滤条件可能比其他的类型过滤条件要快，但是这要视条件而定。毕竟这是一个慢过，使用时请按照过滤标准的复杂程度而异。</remarks>
        public void ElementParameterFilter_FindRooms(Document Document)
        {
            // 以下5个步骤演示了要进行参数过滤的完整过程。
            // provider
            ParameterValueProvider pvp = new ParameterValueProvider(new ElementId(BuiltInParameter.ROOM_AREA));
            // evaluator
            FilterNumericRuleEvaluator fnrv = new FilterNumericGreater();

            // 过滤规则 : 参数“房间面积”的值“大于”“100 Square Foot”。（浮点数比较的容差为0.000001）
            FilterRule fRule = new FilterDoubleRule(pvp, fnrv, 100, 0.000001);

            // Create an ElementParameter filter
            ElementParameterFilter filter = new ElementParameterFilter(fRule);

            // Apply the filter to the elements in the active document
            FilteredElementCollector collector = new FilteredElementCollector(Document);
            IList<Element> rooms = collector.WherePasses(filter).ToElements();

            // 反转过滤条件
            // Find rooms whose area is less than or equal to 100:
            // Use inverted filter to match elements
            ElementParameterFilter lessOrEqualFilter = new ElementParameterFilter(fRule, true);
            collector = new FilteredElementCollector(Document);
            IList<Element> lessOrEqualFounds = collector.WherePasses(lessOrEqualFilter).ToElements();
        }

        public void Test()
        {
        }
    }
}