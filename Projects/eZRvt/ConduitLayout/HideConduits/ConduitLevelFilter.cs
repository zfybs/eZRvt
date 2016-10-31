using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Form = System.Windows.Forms.Form;
using eZstd.Windows;
using RevitStd;

namespace eZRvt.ConduitLayout
{
    public partial class ConduitLevelFilter : ShowDialogForm
    {
        public FilterOption FilterOptions;

        private UIDocument _uiDoc;

        #region ---   构造函数与初始化

        private static ConduitLevelFilter _uniqueConduitLevelFilter;
        public static ConduitLevelFilter GetUniqueConduitLevelFilter(UIDocument uiDoc)
        {
            ConduitLevelFilter cdlf = null;
            cdlf = _uniqueConduitLevelFilter ?? new ConduitLevelFilter();
            //
            cdlf._uiDoc = uiDoc;
            cdlf.RefreshLevels(uiDoc);
            return cdlf;
        }

        /// <summary> 构造函数 </summary>
        private ConduitLevelFilter()
        {
            InitializeComponent();

            //
            textBoxTopOffset.Text = "0";
            textBoxBottomOffset.Text = "0";

            //
            textBoxTopOffset.LostFocus += TextBoxTopOffsetOnLostFocus;
            textBoxBottomOffset.LostFocus += TextBoxBottomOffsetOnLostFocus;
            textBoxTopElevation.LostFocus += TextBoxTopElevationOnLostFocus;
            textBoxBottomElevation.LostFocus += TextBoxBottomElevationOnLostFocus;

            //
            textBoxTopOffset.Focus();
            textBoxBottomOffset.Focus();
            Focus();
        }

        /// <summary>
        /// 将新的文档中的标高对象刷新到窗口界面中
        /// </summary>
        /// <param name="uiDoc"></param>
        private void RefreshLevels(UIDocument uiDoc)
        {
            List<LevelElevation> lll = GetLevels(uiDoc.Document);
            //
            comboBoxTopLevel.DisplayMember = "LevelName";
            comboBoxTopLevel.ValueMember = "Elevation";
            comboBoxTopLevel.DataSource = lll;
            //
            comboBoxBottomLevel.DisplayMember = "LevelName";
            comboBoxBottomLevel.ValueMember = "Elevation";
            comboBoxBottomLevel.DataSource = lll.ToList();
        }
        #endregion

        #region ---   获取标高对象

        private List<LevelElevation> GetLevels(Document doc)
        {
            var ll = doc.FindElements(typeof(Level), BuiltInCategory.OST_Levels);

            Level level;
            List<LevelElevation> lll = new List<LevelElevation>();
            foreach (Element l in ll)
            {
                level = (Level)l;
                lll.Add(new LevelElevation(level.Name, UnitUtils.ConvertFromInternalUnits(level.Elevation, DisplayUnitType.DUT_METERS)));
            }
            return lll;
        }

        private class LevelElevation
        {
            public string LevelName { get; set; }
            public double Elevation { get; set; }

            public LevelElevation(string levelName, double elevation)
            {
                LevelName = levelName;
                Elevation = elevation;
            }
        }

        #endregion

        #region ---   界面操作

        #region ---   选择标高

        private double topLevelElevation;
        private double bottomLevelElevation;

        private void comboBoxTopLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            topLevelElevation = (double)comboBoxTopLevel.SelectedValue;
            TextBoxTopOffsetOnLostFocus(sender, e);
            TextBoxTopElevationOnLostFocus(sender, e);

        }

        private void comboBoxBottomLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            bottomLevelElevation = (double)comboBoxBottomLevel.SelectedValue;
            TextBoxBottomOffsetOnLostFocus(sender, e);
            TextBoxBottomElevationOnLostFocus(sender, e);

        }
        #endregion

        #region ---   修改偏移或总标高的文字

        private void TextBoxBottomElevationOnLostFocus(object sender, EventArgs eventArgs)
        {
            double elev;
            if (!double.TryParse(textBoxBottomElevation.Text, out elev))
            {
                MessageBox.Show("底部标高不是有效的数值", "出错");
                textBoxBottomElevation.Text = textBoxBottomElevation.Tag.ToString();
            }
            else
            {
                textBoxBottomOffset.Text = (elev - bottomLevelElevation).ToString(CultureInfo.InvariantCulture);
                textBoxBottomElevation.Tag = elev.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void TextBoxTopElevationOnLostFocus(object sender, EventArgs eventArgs)
        {
            double elev;
            if (!double.TryParse(textBoxTopElevation.Text, out elev))
            {
                MessageBox.Show("顶部标高不是有效的数值", "出错");
                textBoxTopElevation.Text = textBoxTopElevation.Tag.ToString();
            }
            else
            {
                textBoxTopOffset.Text = (elev - topLevelElevation).ToString(CultureInfo.InvariantCulture);
                textBoxTopElevation.Tag = elev.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void TextBoxBottomOffsetOnLostFocus(object sender, EventArgs eventArgs)
        {
            double elev;
            if (!double.TryParse(textBoxBottomOffset.Text, out elev))
            {
                MessageBox.Show("底部偏移不是有效的数值", "出错");
                textBoxBottomOffset.Text = textBoxBottomOffset.Tag.ToString();
            }
            else
            {
                textBoxBottomElevation.Text = (elev + bottomLevelElevation).ToString(CultureInfo.InvariantCulture);
                textBoxBottomOffset.Tag = elev.ToString(CultureInfo.InvariantCulture);
            }
        }

        private void TextBoxTopOffsetOnLostFocus(object sender, EventArgs eventArgs)
        {
            double elev;
            if (!double.TryParse(textBoxTopOffset.Text, out elev))
            {
                MessageBox.Show("顶部偏移不是有效的数值", "出错");
                textBoxTopOffset.Text = textBoxTopOffset.Tag.ToString();
            }
            else
            {
                textBoxTopElevation.Text = (elev + topLevelElevation).ToString(CultureInfo.InvariantCulture);
                textBoxTopOffset.Tag = elev.ToString(CultureInfo.InvariantCulture);
            }
        }

        #endregion

        #region ---   通过界面拾取指定对象的标高

        private bool _pickTopLevel;

        private void buttonChooseTopLevel_Click(object sender, EventArgs e)
        {
            _pickTopLevel = true;
            HideAndOperate(new PickLevelProc(PickLevel), HideMethodReturned);
        }

        private void buttonChoosebottomLevel_Click(object sender, EventArgs e)
        {
            _pickTopLevel = false;
            HideAndOperate(new PickLevelProc(PickLevel), HideMethodReturned);
        }

        delegate double? PickLevelProc();

        /// <summary>
        /// 选择的对象的标高，单位为m。
        /// </summary>
        /// <returns></returns>
        private double? PickLevel()
        {
            try
            {
                Reference refe = _uiDoc.Selection.PickObject(ObjectType.Element, "选择任一单元以拾取其标高");
                Element elem = _uiDoc.Document.GetElement(refe);

                var loc = elem.Location;
                if (loc is LocationPoint)
                {
                    return UnitUtils.ConvertFromInternalUnits(((LocationPoint)loc).Point.Z, DisplayUnitType.DUT_METERS);
                }
                else if (loc is LocationCurve)
                {
                    return UnitUtils.ConvertFromInternalUnits(((LocationCurve)loc).Curve.GetEndPoint(0).Z, DisplayUnitType.DUT_METERS);
                }
            }
            catch (Exception)
            {
                return null;
            }
            return null;
        }

        /// <summary>
        /// 用在 ShowDialogWin 或者 ShowDialogForm 类中。
        /// 当 DialogForm 被隐藏并执行完与Revit交互的hideProc方法后被触发。此事件响应完会即会立即执行  System.Windows.Window.ShowDialog();
        /// </summary>
        /// <param name="returnedValue"> hideProc 方法执行完成后的返回值，如果 hideProc 方法没有返回值，则 returnedValue 为 null。</param>
        private void HideMethodReturned(object returnedValue)
        {
            double? level = (double?)returnedValue;
            if (level != null)
            {
                if (_pickTopLevel)
                {
                    textBoxTopElevation.Text = level.ToString();
                    TextBoxTopElevationOnLostFocus(null, null);
                }
                else
                {
                    textBoxBottomElevation.Text = level.ToString();
                    TextBoxBottomElevationOnLostFocus(null, null);
                }

            }
        }

        #endregion

        #endregion

        #region ---   获取结果

        private void buttonOk_Click(object sender, EventArgs e)
        {
            //
            Focus();
            //
            FilterOptions = GetFilterOption();
            if (FilterOptions != null)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void buttonCancel_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            FilterOptions = null;
            Close();
        }

        private FilterOption GetFilterOption()
        {
            double top, bottom;


            if (!double.TryParse(textBoxTopElevation.Text, out top))
            {
                MessageBox.Show("顶部标高不是有效的数值", "出错");
                return null;
            }

            if (!double.TryParse(textBoxBottomElevation.Text, out bottom))
            {
                MessageBox.Show("底部标高不是有效的数值", "出错");
                return null;
            }

            //
            FilterOption fo = new FilterOption(top, bottom,
                checkAllConnector: checkBoxcheckAllConnector.Checked, showElementsInRange: radioButtonShow.Checked);

            //
            return fo;
        }
        #endregion
    }
}
