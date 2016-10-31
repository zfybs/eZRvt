using System;
using System.IO;
using System.Windows.Forms.VisualStyles;
using System.Windows.Media.Imaging;
using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using eZRvt.ConduitLayout;
using eZRvt.GlobalSettings;

namespace eZRvt.ConduitLayout
{

    public class ExternalApplication : IExternalApplication
    {

        string Dll_Projects = "ConduitLayout.dll";

        public Result OnShutdown(UIControlledApplication application)
        {
            //TaskDialog.Show("Revit", "ExternalApplication")
            return Result.Succeeded;
        }

        public Result OnStartup(UIControlledApplication application)
        {
            //Create a custom ribbon tab
            string tabName = ConduitLayoutParameters.AddinTabName;
            application.CreateRibbonTab(tabName);

            // 建模面板
            RibbonPanel ribbonPanelModeling = application.CreateRibbonPanel(tabName, ConduitLayoutParameters.panelName_DrawFace);
            AddPushButtonShowPane(ribbonPanelModeling);
            AddPushButtonHideRange(ribbonPanelModeling);
            // 注册 DockablePane 面板
            //  RegisterPanel(application);

            return Result.Succeeded;
        }

        #region ---   添加按钮

        /// <summary> 添加“打开面层面板”的按钮 </summary>
        private void AddPushButtonShowPane(RibbonPanel panel)
        {
            // Create a new push button
            PushButton pushButton =
                panel.AddItem(new PushButtonData("ConduitLayout", "线管连接", Path.Combine(ProjectPath.Dlls, Dll_Projects),
                    "ConduitLayout.cmds_ConduitLayout")) as PushButton;
            pushButton.ToolTip = "将线管与电气设备进行连接";
            // Set Contextual help
            ContextualHelp contextHelp = new ContextualHelp(ContextualHelpType.Url, "http://www.cmie.cn");
            pushButton.SetContextualHelp(contextHelp);
            // Set Icon
            pushButton.LargeImage = new BitmapImage(new Uri(Path.Combine(ProjectPath.Dlls, "ConduitLayout-32.png")));
        }

            /// <summary> 添加“打开面层面板”的按钮 </summary>
        private void AddPushButtonHideRange(RibbonPanel panel)
        {
            // Create a new push button
            PushButton pushButton =
                panel.AddItem(new PushButtonData("HideRange", "线管过滤", Path.Combine(ProjectPath.Dlls, Dll_Projects),
                    "ConduitLayout.cmd_HideRange")) as PushButton;
            pushButton.ToolTip = "将指定标高范围内的线管进行显示或者临时隐藏";
            // Set Contextual help
            ContextualHelp contextHelp = new ContextualHelp(ContextualHelpType.Url, "http://www.cmie.cn");
            pushButton.SetContextualHelp(contextHelp);
            // Set Icon
            pushButton.LargeImage = new BitmapImage(new Uri(Path.Combine(ProjectPath.Dlls, "Filter-32.png")));
        }

        #endregion

    }
}
