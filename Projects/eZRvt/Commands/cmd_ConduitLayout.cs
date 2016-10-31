using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using DllActivator;
using eZRvt.ConduitLayout;

namespace eZRvt.Commands
{
    /// <summary> 通过在界面中点选线管A与电气设备B，来新增一段线管C，以将A与B相连 </summary>
    [Transaction(TransactionMode.Manual)]
    public class cmd_ConduitLayout : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //
            DllActivator_eZRvt dat = new DllActivator_eZRvt();
            dat.ActivateReferences();
            //
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = uiDoc.Document;

            var t = uiDoc.ActiveView.Id.IntegerValue == uiDoc.ActiveGraphicalView.Id.IntegerValue;

            // 选择 配电箱 与 线管
            FamilyInstance cab;
            List<Conduit> conduits;

            Result selectionResult;
            selectionResult = SelectCabinet_Conduits(uiDoc, out cab, out conduits, ref message);
            if (selectionResult == Result.Succeeded)
            {
                MEPElectricalEquipment cabinet = new MEPElectricalEquipment(cab);
                Dictionary<ElementId, string> errorConduits = new Dictionary<ElementId, string>();
                // 将每一条线管分别连接到电气设备上
                foreach (Conduit cd in conduits)
                {
                    using (Transaction transa = new Transaction(doc, "线管布线"))
                    {
                        try
                        {
                            transa.Start();

                            ConduitFittingMEP conduitFittingMEP = new ConduitFittingMEP(cabinet, cd);

                            FamilyInstance elbow = conduitFittingMEP.Connect(transa);

                            transa.Commit();
                        }
                        catch (Exception ex)
                        {
                            errorConduits.Add(cd.Id, ex.Message);

                            string elemInfo = "    出错线管 Id ： " + cd.Id.ToString();
                            transa.RollBack();

                            //
                            // Utils.ShowDebugCatch(ex, elemInfo);
                        }
                    }
                }

                //
                if (errorConduits.Count > 0)
                {
                    // 处理出错信息
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("出错的线管与出错原因：");

                    int index = 0;
                    foreach (var errorConduit in errorConduits)
                    {
                        index += 1;
                        sb.AppendLine(index.ToString() + ". " + errorConduit.Key.ToString());
                        sb.AppendLine(errorConduit.Value);
                    }

                    //
                    MessageBox.Show(sb.ToString(), "部分线管绘制出错", MessageBoxButtons.OK, MessageBoxIcon.Warning);

                    uiDoc.Selection.SetElementIds(errorConduits.Keys);
                }
                return Result.Succeeded;
            }

            else
            {
                return selectionResult;
                ;
            }
        }

        #region ---   通过各种模式选择 线管 或者 电气设备

        /// <summary>
        /// 选择模型中的线管或者电气设备
        /// </summary>
        /// <returns></returns>
        private bool SelectCabinet_Conduit(UIDocument uidoc, out FamilyInstance cabinet, out Conduit conduit)
        {
            cabinet = null;
            conduit = null;

            Document doc = uidoc.Document;
            try
            {
                //
                var machineRef = uidoc.Selection.PickObject(ObjectType.Element, new SelectionFilter_Cabinet_Conduit(),
                    "选择线管或者电气设备。");
                var pickedEle = doc.GetElement(machineRef);

                //选择线管。
                if (IsCabinet(pickedEle))
                {
                    cabinet = pickedEle as FamilyInstance;
                    //
                    machineRef = uidoc.Selection.PickObject(ObjectType.Element, new SelectionFilter_Conduit(), "选择线管。");
                    conduit = doc.GetElement(machineRef) as Conduit;
                    return true;
                }
                else if (IsConduit(pickedEle))
                {
                    conduit = pickedEle as Conduit;

                    //选择电气设备
                    machineRef = uidoc.Selection.PickObject(ObjectType.Element, new SelectionFilter_Cabinet(), "选择电气设备。");
                    cabinet = doc.GetElement(machineRef) as FamilyInstance;
                    return true;
                }
            }
            catch (OperationCanceledException ex)
            {
                return false;
            }
            return false;
        }

        /// <summary>
        /// 选择模型中的线管或者电气设备
        /// </summary>
        /// <returns></returns>
        private Result SelectCabinet_Conduits(UIDocument uidoc, out FamilyInstance cabinet, out List<Conduit> conduits,
            ref string errorMessage)
        {
            List<FamilyInstance> cabinets = new List<FamilyInstance>();
            cabinet = null;
            conduits = new List<Conduit>();

            Document doc = uidoc.Document;
            try
            {
                //
                var machineRef = uidoc.Selection.PickObjects(ObjectType.Element, new SelectionFilter_Cabinet_Conduit(),
                    "选择线管或者电气设备。");

                foreach (Reference re in machineRef)
                {
                    Element pickedEle = doc.GetElement(re);
                    if (IsCabinet(pickedEle))
                    {
                        cabinets.Add(pickedEle as FamilyInstance);
                    }
                    else if (IsConduit(pickedEle))
                    {
                        conduits.Add((Conduit) pickedEle);
                    }
                }

                if (cabinets.Count <= 0 || conduits.Count <= 0)
                {
                    errorMessage = "请选择至少一个线管与电气设备";
                    return Result.Failed;
                }

                if (cabinets.Count > 1)
                {
                    errorMessage = "选择了多个电气设备";
                    return Result.Failed;
                }

                cabinet = cabinets[0];

                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                // errorMessage = ex.Message;
                return Result.Cancelled;
            }
        }

        /// <summary>
        /// 选择线管对象
        /// </summary>
        /// <remarks></remarks>
        private class SelectionFilter_Cabinet_Conduit : ISelectionFilter
        {
            public bool AllowElement(Element element)
            {
                return IsCabinet(element) || IsConduit(element);
            }

            public bool AllowReference(Reference refer, XYZ point)
            {
                return true;
            }
        }

        /// <summary>
        /// 在模型中选择配电箱
        /// </summary>
        /// <remarks></remarks>
        private class SelectionFilter_Cabinet : ISelectionFilter
        {
            public bool AllowElement(Element element)
            {
                return IsCabinet(element);
            }

            public bool AllowReference(Reference refer, XYZ point)
            {
                return true;
            }
        }

        /// <summary>
        /// 选择线管对象
        /// </summary>
        /// <remarks></remarks>
        private class SelectionFilter_Conduit : ISelectionFilter
        {
            public bool AllowElement(Element element)
            {
                return IsConduit(element);
            }

            public bool AllowReference(Reference refer, XYZ point)
            {
                return true;
            }
        }

        private static bool IsCabinet(Element element)
        {
            if (element is FamilyInstance &&
                (element.Category.Id == new ElementId(BuiltInCategory.OST_ElectricalEquipment)))
            {
                return true;
            }
            return false;
        }

        private static bool IsConduit(Element element)
        {
            if (element is Conduit)
            {
                return true;
            }
            return false;
        }

        #endregion
    }
}