using System;
using System.Collections.Generic;
using System.Windows.Forms;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;
using Autodesk.Revit.UI;
using DllActivator;
using eZRvt.ConduitLayout;
using View = Autodesk.Revit.DB.View;

namespace eZRvt.Commands
{
    /// <summary> 将指定标高范围内的线管对象进行临时隐藏 </summary>
    [Transaction(TransactionMode.Manual)]
    public class cmd_HideConduit : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            //
            DllActivator_eZRvt dat = new DllActivator_eZRvt();
            dat.ActivateReferences();
            //
            UIDocument uiDoc = commandData.Application.ActiveUIDocument;
            Document doc = commandData.Application.ActiveUIDocument.Document;

            //
            ConduitLevelFilter clf = ConduitLevelFilter.GetUniqueConduitLevelFilter(uiDoc);
            DialogResult res = clf.ShowDialog();
            if (res != DialogResult.OK) return Result.Cancelled;

            //
            FilterOption filterOptions = clf.FilterOptions;


            // 过滤出来的线管与弯头对象
            var elems = uiDoc.Selection.GetElementIds();
            List<ConduitLine> lines = FilterConduits(doc, elems);

            //   uiDoc.Selection.SetElementIds(lines.Select(r => r.Line.Id).ToList());

            //
            using (Transaction transa = new Transaction(doc, "线管布线"))
            {
                try
                {
                    transa.Start();

                    bool inRange;
                    List<ElementId> elemsInRange = new List<ElementId>();
                    List<ElementId> elemsOutRange = new List<ElementId>();
                    foreach (ConduitLine line in lines)
                    {
                        // 检查此线管是否要被隐藏
                        if (filterOptions.CheckAllConnector) // 此线管中只要有一个 Connector 不在视图范围内，则将其隐藏
                        {
                            inRange = true;
                            foreach (Connector connector in line.ConnectorManager.Connectors)
                            {
                                if (!IsWithinElevationRange(connector.Origin.Z, filterOptions))
                                {
                                    inRange = false;
                                    continue;
                                }
                            }
                        }
                        else // 此线管中只要有一个 Connector 在视图范围内，则不将其隐藏
                        {
                            inRange = false;
                            foreach (Connector connector in line.ConnectorManager.Connectors)
                            {
                                if (IsWithinElevationRange(connector.Origin.Z, filterOptions))
                                {
                                    inRange = true;
                                    continue;
                                }
                            }
                        }

                        if (inRange)
                        {
                            elemsInRange.Add(line.Line.Id);
                        }
                        else
                        {
                            elemsOutRange.Add(line.Line.Id);
                        }
                    }

                    View v = uiDoc.ActiveView;
                    // 先解除临时隐藏模式
                    v.DisableTemporaryViewMode(TemporaryViewMode.TemporaryHideIsolate);

                    // 隐藏线管
                    if (filterOptions.ShowElementsInRange)
                    {
                        // if (elemsInRange.Count > 0) v.UnhideElements(elemsInRange);
                        //
                        if (elemsOutRange.Count > 0) v.HideElementsTemporary(elemsOutRange);
                    }
                    else
                    {
                        if (elemsInRange.Count > 0) v.HideElementsTemporary(elemsInRange);
                        // if (elemsOutRange.Count > 0) v.UnhideElements(elemsOutRange);
                    }

                    transa.Commit();
                    return Result.Succeeded;
                    ;
                }
                catch (Exception ex)
                {
                    transa.RollBack();

                    message = ex.Message + "\r\n" + ex.StackTrace;
                    return Result.Failed;
                    ;
                }
            }
        }

        public List<ConduitLine> FilterConduits(Document doc, ICollection<ElementId> elemIds)
        {
            List<ConduitLine> connManagers = new List<ConduitLine>();
            FilteredElementCollector conduitColl = elemIds == null || elemIds.Count <= 0
                ? new FilteredElementCollector(doc)
                : new FilteredElementCollector(doc, elemIds);
            conduitColl.OfClass(typeof(Conduit));

            foreach (Element elem in conduitColl)
            {
                connManagers.Add(new ConduitLine((Conduit)elem));
            }

            FilteredElementCollector elbowColl = elemIds == null || elemIds.Count <= 0
                ? new FilteredElementCollector(doc)
                : new FilteredElementCollector(doc, elemIds);
            elbowColl.OfClass(typeof(FamilyInstance)).OfCategoryId(new ElementId(BuiltInCategory.OST_ConduitFitting));

            foreach (Element elem in elbowColl)
            {
                connManagers.Add(new ConduitLine((FamilyInstance)elem));
            }

            // 将线管与弯头组合起来
            return connManagers;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="elevation">连接件的标高，单位为inch</param>
        /// <param name="filterOption"></param>
        /// <returns></returns>
        public bool IsWithinElevationRange(double elevation, FilterOption filterOption)
        {
            elevation = UnitUtils.ConvertFromInternalUnits(elevation, DisplayUnitType.DUT_METERS);
            return (elevation <= filterOption.Top) && (elevation >= filterOption.Bottom);
        }
    }

    public class ConduitLine
    {
        public ConnectorManager ConnectorManager;

        public Element Line;

        public ConduitLine(Conduit conduit)
        {
            Line = conduit;
            ConnectorManager = conduit.ConnectorManager;
        }

        public ConduitLine(FamilyInstance elbow)
        {
            Line = elbow;
            ConnectorManager = elbow.MEPModel.ConnectorManager;
        }
    }
}