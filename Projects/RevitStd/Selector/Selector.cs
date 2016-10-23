using System;
using System.Collections.Generic;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using RevitStd.SelectionFilter;

namespace RevitStd.Selector
{
    /// <summary> Revit界面的选择器，会触发 uidoc.Selection.PickObject()等方法 </summary>
    public static class Selector
    {
        #region ---   选择一般情况下的一个对象或者一组对象

        /// <summary> 选择一个对象 </summary>
        /// <param name="uidoc"></param>
        /// <typeparam name="TElement">要过滤的 Element 类型，此类型必须要继承自<see cref="Autodesk.Revit.DB.Element"/>类</typeparam>
        /// <param name="selectedElement">最后选择的 Element 对象，如果未正确地选择到平面，则此值为 null</param>
        /// <param name="statusPrompt">在UI中选择时要在状态栏中显示的提示文字</param>
        /// <returns>选择到的几何对象，如果未正常选择，则返回 null </returns>
        public static Reference SelectGeneralElement<TElement>(UIDocument uidoc, out TElement selectedElement,
            string statusPrompt = "选择一个单元") where TElement : Element
        {
            Reference pickedRef = null;
            GeneralElementFilter<TElement> selector = new GeneralElementFilter<TElement>(uidoc);
            try
            {
                pickedRef = uidoc.Selection.PickObject(ObjectType.Element, selector, statusPrompt);
            }
            catch (Exception ex)
            {
                selectedElement = null;
                return null;
            }

            if (pickedRef != null)
            {
                selectedElement = uidoc.Document.GetElement(pickedRef) as TElement;
                return pickedRef;
            }
            else
            {
                selectedElement = null;
                return null;
            }
        }

        /// <summary> 选择一组对象 </summary>
        /// <param name="uidoc"></param>
        /// <typeparam name="TElement">要过滤的 Element 类型，此类型必须要继承自<see cref="Autodesk.Revit.DB.Element"/>类</typeparam>
        /// <param name="selectedElements">最后选择的 Element 对象，如果未正确地选择到平面，则此集合中的元素个数为0</param>
        /// <param name="statusPrompt">在UI中选择时要在状态栏中显示的提示文字</param>
        /// <returns>选择到的几何对象，如果未正常选择，则此集合中的元素个数为0 </returns>
        public static IList<Reference> SelectGeneralElements<TElement>(UIDocument uidoc,
            out List<TElement> selectedElements,
            string statusPrompt = "选择一组单元") where TElement : Element
        {
            IList<Reference> pickedRefs = null;
            //
            GeneralElementFilter<TElement> selector = new GeneralElementFilter<TElement>(uidoc);
            try
            {
                pickedRefs = uidoc.Selection.PickObjects(ObjectType.Element, selector, statusPrompt);
            }
            catch (Exception ex)
            {
                selectedElements = new List<TElement>();
                return new List<Reference>();
            }

            if (pickedRefs != null && pickedRefs.Count > 0)
            {
                selectedElements = new List<TElement>();
                foreach (Reference r in pickedRefs)
                {
                    selectedElements.Add(uidoc.Document.GetElement(r) as TElement);
                }
                return pickedRefs;
            }
            else
            {
                selectedElements = new List<TElement>();
                return new List<Reference>();
            }
        }

        #endregion
        
        #region ---   选择各种不同类型的几何体——面 Face

        /// <summary> 选择一个任意形状的面 </summary>
        /// <param name="uidoc"></param>
        /// <typeparam name="TElement">要过滤的 Element 类型，此类型必须要继承自<see cref="Autodesk.Revit.DB.Element"/>类</typeparam>
        /// <param name="selectedElement">最后选择的 Element 对象，如果未正确地选择到平面，则此值为 null</param>
        /// <param name="selectedFace">最后选择的 平面 对象，如果未正确地选择到平面，则此值为 null</param>
        /// <param name="statusPrompt">在UI中选择时要在状态栏中显示的提示文字</param>
        /// <returns>选择到的几何对象，如果未正常选择，则返回 null </returns>
        /// <remarks>
        /// 	如果此FamilyInstance. HasModifiedGeometry() 返回False，
        /// 则Element .GetGeometryObjectFromReference(reference对象) 所返回的 Face或Edge对象是
        /// 此 FamilyInstance 所对应的族类型中的基准对象，后期必须要通过FamilyInstance.GetTransform() 
        /// 提取到此 FamilyInstance 的Transform后，再将此 Transform 应用到返回的 Face或Edge对象上，
        /// 才能得到位于项目坐标系中的实际对象。</remarks>
        public static Reference SelectFace<TElement>(UIDocument uidoc,
            out TElement selectedElement, out Face selectedFace,
            string statusPrompt = "选择一个任意形状的面") where TElement : Element
        {
            Reference pickedRef = null;
            GeneralGeometryFilter<TElement, Face> selector =
                new GeneralGeometryFilter<TElement, Face>(uidoc);
            try
            {
                pickedRef = uidoc.Selection.PickObject(ObjectType.Face, selector, statusPrompt);
            }
            catch (Exception ex)
            {
                selectedElement = null;
                selectedFace = null;
                return null;
            }

            if (pickedRef != null)
            {
                selectedElement = uidoc.Document.GetElement(pickedRef) as TElement;
                selectedFace = selectedElement.GetGeometryObjectFromReference(pickedRef) as Face;

                return pickedRef;
            }
            else
            {
                selectedElement = null;
                selectedFace = null;
                return null;
            }
        }

        /// <summary> 选择一个平面 </summary>
        /// <param name="uidoc"></param>
        /// <typeparam name="TElement">要过滤的 Element 类型，此类型必须要继承自<see cref="Autodesk.Revit.DB.Element"/>类</typeparam>
        /// <param name="selectedElement">最后选择的 Element 对象，如果未正确地选择到平面，则此值为 null</param>
        /// <param name="selectedFace">最后选择的 平面 对象，如果未正确地选择到平面，则此值为 null</param>
        /// <param name="statusPrompt">在UI中选择时要在状态栏中显示的提示文字</param>
        /// <returns>选择到的几何对象，如果未正常选择，则返回 null </returns>
        /// <remarks>
        /// 	如果此FamilyInstance. HasModifiedGeometry() 返回False，
        /// 则Element .GetGeometryObjectFromReference(reference对象) 所返回的 Face或Edge对象是
        /// 此 FamilyInstance 所对应的族类型中的基准对象，后期必须要通过FamilyInstance.GetTransform() 
        /// 提取到此 FamilyInstance 的Transform后，再将此 Transform 应用到返回的 Face或Edge对象上，
        /// 才能得到位于项目坐标系中的实际对象。</remarks>
        public static Reference SelectPlanarFace<TElement>(UIDocument uidoc,
            out TElement selectedElement, out PlanarFace selectedFace,
            string statusPrompt = "选择实体的表面（平面对象）。") where TElement : Element
        {
            Reference pickedRef = null;
            GeneralGeometryFilter<TElement, PlanarFace> selector =
                new GeneralGeometryFilter<TElement, PlanarFace>(uidoc);
            try
            {
                pickedRef = uidoc.Selection.PickObject(ObjectType.Face, selector, statusPrompt);
            }
            catch (Exception ex)
            {
                selectedElement = null;
                selectedFace = null;
                return null;
            }

            if (pickedRef != null)
            {
                selectedElement = uidoc.Document.GetElement(pickedRef) as TElement;
                selectedFace = selectedElement.GetGeometryObjectFromReference(pickedRef) as PlanarFace;

                return pickedRef;
            }
            else
            {
                selectedElement = null;
                selectedFace = null;
                return null;
            }
        }

        /// <summary> 选择一个法向为竖直方向平面 </summary>
        /// <typeparam name="TElement">要过滤的 Element 类型，此类型必须要继承自<see cref="Autodesk.Revit.DB.Element"/>类</typeparam>
        /// <param name="uidoc"></param>
        /// <param name="selectedElement">最后选择的 Element 对象，如果未正确地选择到平面，则此值为 null</param>
        /// <param name="selectedFace">最后选择的 平面 对象，如果未正确地选择到平面，则此值为 null</param>
        /// <param name="statusPrompt">在UI中选择时要在状态栏中显示的提示文字</param>
        /// <returns>选择到的几何对象，如果未正常选择，则返回 null </returns>
        /// <remarks>
        /// 	如果此FamilyInstance. HasModifiedGeometry() 返回False，
        /// 则Element .GetGeometryObjectFromReference(reference对象) 所返回的 Face或Edge对象是
        /// 此 FamilyInstance 所对应的族类型中的基准对象，后期必须要通过FamilyInstance.GetTransform() 
        /// 提取到此 FamilyInstance 的Transform后，再将此 Transform 应用到返回的 Face或Edge对象上，
        /// 才能得到位于项目坐标系中的实际对象。</remarks>
        public static Reference SelectHorizontalPlanFace<TElement>(UIDocument uidoc,
            out TElement selectedElement, out PlanarFace selectedFace,
            string statusPrompt = "选择实体的表面（水平面对象）。") where TElement : Element
        {
            Reference pickedRef = null;
            HorizontalPlanarFaceFilter<TElement> selector = new HorizontalPlanarFaceFilter<TElement>(uidoc);
            try
            {
                pickedRef = uidoc.Selection.PickObject(ObjectType.Face, selector, statusPrompt);
            }
            catch (Exception ex)
            {
                selectedElement = null;
                selectedFace = null;
                return null;
            }

            if (pickedRef != null)
            {
                selectedElement = uidoc.Document.GetElement(pickedRef) as TElement;
                selectedFace = selectedElement.GetGeometryObjectFromReference(pickedRef) as PlanarFace;

                return pickedRef;
            }
            else
            {
                selectedElement = null;
                selectedFace = null;
                return null;
            }
        }

        #endregion


    }
}