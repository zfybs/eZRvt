using System;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Events;
using eZstd.Miscellaneous;

namespace eZRvt.FaceWall
{
    class ExEventHandler : IExternalEventHandler
    {
        private MpFaceOptions _mainPanel;

        public FaceOptions DrawFaceOptions;

        public ModelessCommandId RequestId;


        public ExEventHandler(MpFaceOptions mainPanel)
        {
            _mainPanel = mainPanel;
        }

        public string GetName()
        {
            return "绘制面层";
        }

        /// <summary>
        /// Called to execute an API command and update the UI after the command is finished.
        /// </summary>
        public void Execute(UIApplication uiApp)
        {
            UIDocument uiDoc = uiApp.ActiveUIDocument;
            try
            {
                switch (RequestId)
                {

                    case ModelessCommandId.DrawFace:

                        // 绘制面层
                        FaceDrawer fd = new FaceDrawer();
                        fd.DrawFaces(uiDoc, DrawFaceOptions);

                        break;

                    case ModelessCommandId.Filter:

                        FaceFilter ff2 = new FaceFilter(uiDoc.Document);
                        var faces1 = ff2.FilterSelected(DrawFaceOptions);

                        //
                        uiDoc.Selection.SetElementIds(faces1);
                        break;

                    case ModelessCommandId.SelectAll:

                        FaceFilter ff3 = new FaceFilter(uiDoc.Document);
                        var faces2 = ff3.GetAllInSelected();
                        //
                        uiDoc.Selection.SetElementIds(faces2.Select(r => r.FaceElement.Id).ToList()); ;

                        break;
                }
            }
            catch (Exception ex)
            {
                DebugUtils.ShowDebugCatch(ex, RequestId.ToString(), "面层绘制或过滤");
            }
            finally
            {
                _mainPanel.WakeUp();
            }
        }

    }


    public enum ModelessCommandId : int
    {


        /// <summary>
        /// 根据设置信息绘制新的面层
        /// </summary>
        DrawFace,

        /// <summary>
        /// 过滤选择的单元集合或者整个文档中的面层单元
        /// </summary>
        SelectAll,

        /// <summary>
        /// 根据指定的类型等信息，来过滤选择的单元中的面层单元
        /// </summary>
        Filter
    }
}
