using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using DllActivator;
using RevitStd.Selector;

namespace eZRvt.Commands
{
    /// <summary>
    /// 在界面中选择一个面，并以此来定义工作平面 work plane
    /// </summary>
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    public class cmd_DebugTest : IExternalCommand
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

            using (Transaction transDoc = new Transaction(doc, "DebugTest"))
            {
                try
                {
                    transDoc.Start();

                    MessageBox.Show("进入调试");

                    transDoc.Commit();
                    return Result.Succeeded;
                }
                catch (Exception ex)
                {
                    transDoc.RollBack();
                    return Result.Failed;
                }


                return Result.Succeeded;
            }
        }
    }
}
