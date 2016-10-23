using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace RevitStd.SelectionFilter
{

    #region ---   通用选择器 GeneralElementSelector 与 GeneralGeometrySelector

    /// <summary>
    /// 一般化的 Element 选择器：限定了 Element 的类型
    /// </summary>
    /// <typeparam name="TElement">要过滤的 Element 类型，此类型必须要继承自<see cref="Autodesk.Revit.DB.Element"/>类</typeparam>
    public class GeneralElementFilter<TElement> : ISelectionFilter where TElement : Element
    {
        protected readonly UIDocument _uiDoc;

        #region ---   Property
        //
        protected TElement _elem;

        #endregion

        /// <summary> 构造函数 </summary>
        /// <param name="uiDoc"></param>
        public GeneralElementFilter(UIDocument uiDoc)
        {
            _uiDoc = uiDoc;
        }

        public virtual bool AllowElement(Element element)
        {
            //
            if (element is TElement)
            {
                _elem = element as TElement;
                return true;
            }
            else
            {
                return false;
            }
        }

        public virtual bool AllowReference(Reference refer, XYZ point)
        {
            return true;
        }
    }

    /// <summary>
    /// 一般化的 Geometry 选择器：限定了 Element 与 Reference 的类型
    /// </summary>
    /// <typeparam name="TElement">要过滤的 Element 类型，此类型必须要继承自<see cref="Autodesk.Revit.DB.Element"/>类</typeparam>
    /// <typeparam name="TGeometry">要过滤的几何类型，此类型必须要继承自<see cref="GeometryObject"/>类</typeparam>
    public class GeneralGeometryFilter<TElement, TGeometry> : GeneralElementFilter<TElement>
        where TElement : Element where TGeometry : GeometryObject
    {
        #region ---   Property

        protected TGeometry _geometryObject;

        #endregion

        /// <summary> 构造函数 </summary>
        /// <param name="uiDoc"></param>
        public GeneralGeometryFilter(UIDocument uiDoc) : base(uiDoc)
        { }

        public override bool AllowElement(Element element)
        {
            return base.AllowElement(element);
        }

        public override bool AllowReference(Reference refer, XYZ point)
        {
            GeometryObject geo = _elem.GetGeometryObjectFromReference(refer);

            //
            if (geo is TGeometry)
            {
                _geometryObject = geo as TGeometry;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    #endregion

    // 其他各种选择过滤器 ISelectionFilter
    /// <summary>
    /// 一般化的选择器：限定了 Element 与 Reference 的类型
    /// </summary>
    /// <typeparam name="TElement">要过滤的 Element 类型，此类型必须要继承自<see cref="Autodesk.Revit.DB.Element"/>类</typeparam>
    public class HorizontalPlanarFaceFilter<TElement> : GeneralGeometryFilter<TElement, PlanarFace> where TElement : Element
    {
        /// <summary> 构造函数 </summary>
        /// <param name="uiDoc"></param>
        public HorizontalPlanarFaceFilter(UIDocument uiDoc) : base(uiDoc)
        {
        }

        public override bool AllowElement(Element element)
        {
            bool v = base.AllowElement(element);
            // MessageBox.Show(SelectedElement.Id.IntegerValue.ToString());
            return v;
        }

        public override bool AllowReference(Reference refer, XYZ point)
        {
            if (base.AllowReference(refer, point))
            {
                // 判断平面的法向是否竖直
                if (GeoHelper.IsSameDirection(_geometryObject.FaceNormal, new XYZ(0, 0, 1)) ||
                    GeoHelper.IsSameDirection(_geometryObject.FaceNormal, new XYZ(0, 0, -1)))
                {
                    return true;
                }
            }
            return false;
        }
    }
}
