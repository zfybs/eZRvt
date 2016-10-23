using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;

namespace eZRvt
{
    [Transaction(TransactionMode.Manual)]
    public class IECAbout : IExternalCommand
    {
        #region IExternalCommand Members

        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            string aboutString = "Autodesk Revit 快速建模工具集 \r\n" + "by 曾凡云";
            TaskDialog.Show("About", aboutString);
            return Result.Succeeded;
        }

        #endregion
    }
}