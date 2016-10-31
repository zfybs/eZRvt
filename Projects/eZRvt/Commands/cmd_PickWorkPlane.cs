using System;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DllActivator;
using RevitStd.Selector;

namespace eZRvt.Commands
{
    /// <summary>
    /// 在界面中选择一个面，并以此来定义工作平面 work plane
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class cmd_PickWorkPlane : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //
            DllActivator_eZRvt dat = new DllActivator_eZRvt();
            dat.ActivateReferences();
            //
            UIApplication uiApp = commandData.Application;
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            Document doc = uiApp.ActiveUIDocument.Document;

            // 是否只根据平面来创建工作平面，而不考虑曲面
            bool planarFaceOnly = true;
            if (planarFaceOnly)
            {
                return SetWorkPlaneFromPlanarFace(uiDoc, doc);
            }
            else
            {
                return SetWorkPlaneFromAnyFace(uiDoc, doc);
            }
        }

        private Result SetWorkPlaneFromPlanarFace(UIDocument uiDoc, Document doc)
        {
            // 先在界面中选择一个平面
            Element elem;
            PlanarFace pf;
            var refe = Selector.SelectPlanarFace(uiDoc, out elem, out pf);

            //
            if (refe != null)
            {
                using (Transaction transDoc = new Transaction(doc, "设置工作平面"))
                {
                    try
                    {
                        transDoc.Start();
                        View v = uiDoc.ActiveView;

                        // 将原工作平面删除
                        SketchPlane sp = v.SketchPlane;
                        if (sp != null && sp.IsValidObject)
                        {
                            // doc.Delete(sp.Id);
                            // 不能删除上面的工作平面，因为有一些模型线是依附于工作平面对象绘制出来的，
                            // 当将工作平面删除后，那些依附在其上的模型线等对象也会被删除。
                        }

                        // 将得到的平面定义为新的工作平面
                        sp = SketchPlane.Create(doc, refe);
                        v.SketchPlane = sp;

                        // 显示工作平面
                        v.ShowActiveWorkPlane();

                        transDoc.Commit();
                        return Result.Succeeded;
                    }
                    catch (Exception ex)
                    {
                        transDoc.RollBack();
                        return Result.Failed;
                    }
                }
            }
            return Result.Succeeded;
        }

        private Result SetWorkPlaneFromAnyFace(UIDocument uiDoc, Document doc)
        {
            // 先在界面中选择一个平面
            Element elem;
            Face fc;
            var refe = Selector.SelectFace(uiDoc, out elem, out fc);

            //
            if (refe != null)
            {
                using (Transaction transDoc = new Transaction(doc, "设置工作平面"))
                {
                    try
                    {
                        transDoc.Start();
                        View v = uiDoc.ActiveView;

                        // 将原工作平面删除
                        SketchPlane sp = v.SketchPlane;
                        if (sp != null && sp.IsValidObject)
                        {
                            // doc.Delete(sp.Id);
                            // 不能删除上面的工作平面，因为有一些模型线是依附于工作平面对象绘制出来的，
                            // 当将工作平面删除后，那些依附在其上的模型线等对象也会被删除。
                        }

                        // 构造一个平面
                        Plane plane = new Plane(norm: fc.ComputeNormal(refe.UVPoint), origin: refe.GlobalPoint);

                        // 将得到的平面定义为新的工作平面
                        sp = SketchPlane.Create(doc, plane);
                        v.SketchPlane = sp;

                        // 显示工作平面
                        v.ShowActiveWorkPlane();

                        transDoc.Commit();
                        return Result.Succeeded;
                    }
                    catch (Exception ex)
                    {
                        transDoc.RollBack();
                        return Result.Failed;
                    }
                }
            }
            return Result.Succeeded;
        }
    }
}