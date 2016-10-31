using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Color = System.Drawing.Color;
using ColorConverter = RevitStd.ColorConverter;
using Cursors = System.Windows.Input.Cursors;
using KeyEventArgs = System.Windows.Input.KeyEventArgs;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace eZRvt.FaceWall
{
    /// <summary>
    /// UC1.xaml 的交互逻辑
    /// </summary>
    public partial class MpFaceOptions : IDockablePaneProvider
    {
        #region ---   Properties

        public static readonly DockablePaneId DockablePaneId_FaceWall =
            new DockablePaneId(new Guid("F07A471F-6003-445C-8D51-3F2C63416891"));

        private static UIApplication _uiApp;
        private UIControlledApplication _uiControlledApp;


        private bool _isRegistered = false;

        // 外部事件
        private ExternalEvent m_exEvent;
        private ExEventHandler m_handler;

        #endregion

        #region ---   构造函数与初始化

        private static MpFaceOptions _faceOptionsPanel;

        /// <summary>
        /// Panel 的激活（全局中只有此一个Panel）
        /// </summary>
        /// <returns></returns>
        public static MpFaceOptions UniqueObject(UIControlledApplication uiControledApp)
        {
            //
            if (_faceOptionsPanel != null) return _faceOptionsPanel;
            //
            _faceOptionsPanel = new MpFaceOptions();
            return _faceOptionsPanel;
        }

        /// <summary>
        /// Panel 的激活（全局中只有此一个Panel）
        /// </summary>
        /// <returns></returns>
        public static MpFaceOptions UniqueObject(UIApplication uiApp)
        {
            _uiApp = uiApp;

            //
            if (_faceOptionsPanel != null) return _faceOptionsPanel;
            //
            _faceOptionsPanel = new MpFaceOptions();
            return _faceOptionsPanel;
        }

        /// <summary>
        /// 构造函数
        /// </summary>
        private MpFaceOptions()
        {
            InitializeComponent();

            // 
            m_handler = new ExEventHandler(this);
            m_exEvent = ExternalEvent.Create(m_handler);

            // 面层在Revit中的类别信息
            var itemSource = new FaceCategoryMapping[]
            {
                new FaceCategoryMapping("墙", FaceFilter.CategoryIds[0]),
                new FaceCategoryMapping("柱", FaceFilter.CategoryIds[1]),
                new FaceCategoryMapping("楼板", FaceFilter.CategoryIds[2]),
                new FaceCategoryMapping("结构框架", FaceFilter.CategoryIds[3]),
                new FaceCategoryMapping("屋顶", FaceFilter.CategoryIds[4]),
                new FaceCategoryMapping("天花板", FaceFilter.CategoryIds[5]),
            };
            ComboxCategory.ItemsSource = itemSource;
            ComboxCategory.DisplayMemberPath = "Text";
            ComboxCategory.SelectedValuePath = "CategoryId";
            ComboxCategory.SelectedIndex = 0;

            // 面层的类型
            FaceFilter ff = new FaceFilter(_uiApp.ActiveUIDocument.Document);
            IList<string> types = ff.GetFaceTypes(ff.GetAllInDoc());
            foreach (string t in types)
            {
                ComboxType.Items.Add(t);
            }
            ComboxType.SelectedIndex = 0;

            // Revit主窗口的句柄
            rvtHwnd = Process.GetCurrentProcess().MainWindowHandle;
                // 当 Revit中有其他的弹出窗口时，MainWindowHandle属性所指的可能就不是Revit程序的那个大窗口了。
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            // 设置窗口界面
            AsWindows();
        }

        /// <summary>
        /// Revit的主窗口的句柄。
        /// 当 Revit中有其他的弹出窗口时，MainWindowHandle属性所指的可能就不是Revit程序的那个大窗口了。
        /// </summary>
        IntPtr rvtHwnd;

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        /// <summary>
        /// 当此界面作为一个单独窗口时的设置
        /// </summary>
        private void AsWindows()
        {
            Window window = null;

            window = this as Window;

            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.ResizeMode = ResizeMode.NoResize;
            // window.Topmost = true;

            // 将窗口显示在Revit窗口之上

            WindowInteropHelper wndHelper = new WindowInteropHelper(this);
            IntPtr wpfHwnd = wndHelper.Handle;

            //string title = string.Format("Autodesk Revit 2016 - [三维视图: {三维} - 绘制面层.rvt]",); // Autodesk Revit 2016 - [三维视图: {三维} - 绘制面层.rvt]
            //rvtHwnd = Windows.FindWindow(null, "Autodesk Revit 2016 - [三维视图: {三维} - 绘制面层.rvt]");

            SetWindowLong(wpfHwnd, -8, rvtHwnd);
        }

        /// <summary>
        /// 当此界面作为Revit中的Dockable Pane时的设置
        /// </summary>
        private void AsPage()
        {
            Page page = null;
            // page = this as Page;
        }

        #endregion

        #region ---   界面的 禁用 与 启用、窗口的显示与隐藏

        public void DozeOff()
        {
            EnableCommands(false);
        }

        public void WakeUp()
        {
            EnableCommands(true);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"> True 表示启用，false 表示禁用 </param>
        private void EnableCommands(bool status)
        {
            if (status == false)
            {
                this.Cursor = Cursors.Wait;
                // 禁用子控件
                foreach (UIElement uiE in GridContent.Children)
                {
                    uiE.IsEnabled = false;
                }
            }
            else
            {
                this.Cursor = Cursors.Arrow;

                // 启用子控件
                foreach (UIElement uiE in GridContent.Children)
                {
                    uiE.IsEnabled = true;
                }
            }
        }

        private void Window_KeyDown_1(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void Window_Closing_1(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
        }

        private void Window_Closed_1(object sender, EventArgs e)
        {
            _faceOptionsPanel = null;
        }

        #endregion

        #region ---   DockablePane 注册 与 显示 隐藏

        public void RegisterPanel(UIApplication uiApp)
        {
            if (!_isRegistered) // if (!_isRegistered)
            {
                uiApp.RegisterDockablePane(DockablePaneId_FaceWall, "面层参数", this);
                _isRegistered = true;
            }
        }

        public void RegisterPanel(UIControlledApplication uiControledApp)
        {
            if (!_isRegistered) // if (!_isRegistered)
            {
                uiControledApp.RegisterDockablePane(DockablePaneId_FaceWall, "面层参数", this);
                _isRegistered = true;
            }
        }

        /// <summary>
        /// 此函数由uiApp.RegisterDockablePane时自动被调用。
        /// </summary>
        /// <param name="data"> 通过给此 data 实例的属性赋值，来确定要注册的 DockablePane 的样式 </param>
        void IDockablePaneProvider.SetupDockablePane(DockablePaneProviderData data)
        {
            data.FrameworkElement = (FrameworkElement) this;
            data.InitialState = new DockablePaneState();
            data.InitialState.SetFloatingRectangle(new Rectangle(100, 100, 200, 200));
            data.InitialState.DockPosition = DockPosition.Floating;
        }

        public void ShowPanel(UIControlledApplication uiControlledApp)
        {
            _uiControlledApp = uiControlledApp;

            Autodesk.Revit.UI.DockablePane dp = uiControlledApp.GetDockablePane(DockablePaneId_FaceWall);
            dp.Show();
        }

        public void ShowPanel(UIApplication uiApp)
        {
            _uiApp = uiApp;
            Autodesk.Revit.UI.DockablePane dp = uiApp.GetDockablePane(DockablePaneId_FaceWall);
            dp.Show();
        }

        public void HidePanel(UIApplication uiApp)
        {
            Autodesk.Revit.UI.DockablePane dp = uiApp.GetDockablePane(DockablePaneId_FaceWall);
            dp.Hide();
        }

        public void HidePanel(UIControlledApplication uiApp)
        {
            Autodesk.Revit.UI.DockablePane dp = uiApp.GetDockablePane(DockablePaneId_FaceWall);
            dp.Hide();
        }

        public Autodesk.Revit.UI.DockablePane GetPanel(UIControlledApplication uiControlledApp)
        {
            _uiControlledApp = uiControlledApp;

            Autodesk.Revit.UI.DockablePane dp = uiControlledApp.GetDockablePane(DockablePaneId_FaceWall);
            return dp;
        }

        public Autodesk.Revit.UI.DockablePane GetPanel(UIApplication uiApp)
        {
            _uiApp = uiApp;

            Autodesk.Revit.UI.DockablePane dp = uiApp.GetDockablePane(DockablePaneId_FaceWall);
            return dp;
        }

        #endregion

        #region ---   设置 颜色 、 厚度、面层类型

        private void ColorBoard_MouseDown(object sender, MouseButtonEventArgs e)
        {
            ColorDialog loColorForm = new ColorDialog
            {
                FullOpen = true,
            };

            Color color;
            ColorConverter.ConvertColor(((SolidColorBrush) ColorBoard.Background).Color, out color);
            loColorForm.Color = color;

            if (loColorForm.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                System.Windows.Media.Color clr = new System.Windows.Media.Color()
                {
                    R = loColorForm.Color.R,
                    G = loColorForm.Color.G,
                    B = loColorForm.Color.B,
                    A = loColorForm.Color.A
                };

                ColorBoard.Background = new SolidColorBrush(clr);
            }
        }

        private void ColorBoard_MouseEnter(object sender, MouseEventArgs e)
        {
            ColorBoard.ToolTip = ColorBoard.Background.ToString();
        }

        private void TextBlockThickness_TextChanged(object sender, TextChangedEventArgs e)
        {
            string str = TextBlockThickness.Text;
            if (string.IsNullOrEmpty(str))
            {
                return;
            }

            if (str == ".")
            {
                TextBlockThickness.Text = "0.";
                TextBlockThickness.SelectionStart = 2;
                return;
            }

            double thickness = 0;
            if (double.TryParse(str, out thickness) && thickness >= 0)
            {
            }
            else
            {
                TextBlockThickness.Text = "";
            }
        }

        // 面层类型设置

        private void ButtonDelType_Click(object sender, RoutedEventArgs e)
        {
            int index = ComboxType.SelectedIndex;
            if (index >= 0)
            {
                ComboxType.Items.RemoveAt(index);
                // 选择一项
                ComboxType.SelectedIndex = index < ComboxType.Items.Count ? index : index - 1;
            }
        }

        private void ButtonAddType_Click(object sender, RoutedEventArgs e)
        {
            FormAddFaceType addFaceType = new FormAddFaceType();
            if (addFaceType.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                if (!ComboxType.Items.Contains(addFaceType.FaceType))
                {
                    ComboxType.Items.Add(addFaceType.FaceType);
                    ComboxType.SelectedIndex = ComboxType.Items.Count - 1;
                }
            }
        }

        private void CheckBoxSameFaces_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxMultiFaces.IsChecked = false;
        }

        private void CheckBoxMultiFaces_Checked(object sender, RoutedEventArgs e)
        {
            CheckBoxSameNormalFaces.IsChecked = false;
        }

        #endregion

        #region ---   执行操作  m_exEvent.Raise()

        private void ButtonDrawFace_Click(object sender, RoutedEventArgs e)
        {
            m_handler.RequestId = ModelessCommandId.DrawFace;
            m_handler.DrawFaceOptions = GetFaceOptions();
            m_exEvent.Raise();
            DozeOff();
        }

        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            m_handler.RequestId = ModelessCommandId.Filter;
            m_handler.DrawFaceOptions = GetFaceOptions();
            m_exEvent.Raise();
        }

        private void ButtonSelectAll_Click(object sender, RoutedEventArgs e)
        {
            m_handler.RequestId = ModelessCommandId.SelectAll;
            m_handler.DrawFaceOptions = GetFaceOptions();
            m_exEvent.Raise();
        }

        #endregion

        #region ---   绘制面层选项 FaceOptions

        /// <summary>
        /// 从界面中获取面层的设置选项信息
        /// </summary>
        /// <returns></returns>
        public FaceOptions GetFaceOptions()
        {
            FaceOptions op = new FaceOptions(includeSameNormal: CheckBoxSameNormalFaces.IsChecked ?? false,
                excludeFace: CheckBoxExcludeFace.IsChecked ?? false,
                multiFaces: CheckBoxMultiFaces.IsEnabled && (CheckBoxMultiFaces.IsChecked ?? false),
                unionInnerSolids: CheckBoxUnionInnerSolids.IsChecked ?? false);

            // 颜色
            Autodesk.Revit.DB.Color c1;
            ColorConverter.ConvertColor(((SolidColorBrush) ColorBoard.Background).Color, out c1);
            op.Color = c1;

            // 厚度
            double thickNess;
            double.TryParse(TextBlockThickness.Text, out thickNess);
            op.SurfaceThickness = thickNess/1000;

            // 类别
            ElementId categoryId = ComboxCategory.SelectedValue as ElementId;
            op.CategoryId = categoryId ?? new ElementId(BuiltInCategory.OST_Walls);

            // 面层类型
            op.FaceType = ComboxType.Text;

            return op;
        }

        private class FaceCategoryMapping
        {
            public FaceCategoryMapping(string text, ElementId categoryId)
            {
                Text = text;
                CategoryId = categoryId;
            }

            public ElementId CategoryId { get; set; }
            public string Text { get; set; }
        }

        #endregion
    }
}