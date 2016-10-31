using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

namespace eZRvt.FaceWall
{

    public class FaceFilter
    {
        /// <summary>
        /// 所有的面层对象可能的类别的集合
        /// </summary>
        public static ElementId[] CategoryIds = new ElementId[]
        {
            new ElementId(BuiltInCategory.OST_Walls),
            new ElementId(BuiltInCategory.OST_Columns),
            new ElementId(BuiltInCategory.OST_Floors),
            new ElementId(BuiltInCategory.OST_StructuralFraming),
            new ElementId(BuiltInCategory.OST_Roofs),
            new ElementId(BuiltInCategory.OST_Ceilings),
        };

        private Document _doc;
        UIDocument _uiDoc;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="doc"></param>
        public FaceFilter(Document doc)
        {
            _doc = doc;
            _uiDoc = new UIDocument(doc);
        }

        #region ---   过滤出集合中的面层对象
        /// <summary>
        /// 从整个文档中的单元集合中过滤出面层对象
        /// </summary>
        public ICollection<WallFace> GetAllInDoc()
        {
            ICollection<WallFace> faces = faceLookup(_doc, null);

            return faces;
        }

        /// <summary>
        /// 从选择的单元集合或者整个文档中的单元集合中过滤出面层对象，并将其在界面中选中
        /// </summary>
        public ICollection<WallFace> GetAllInSelected()
        {

            ICollection<WallFace> faces;
            //
            ICollection<ElementId> eleIds = _uiDoc.Selection.GetElementIds();
            //
            // 如果没有选择任何单元，则从整个文档中进行搜索 
            faces = eleIds.Count == 0
                ? faceLookup(_doc, null)
                : faceLookup(_doc, eleIds);

            //
            return faces;
        }

        /// <summary>
        /// 从选择的单元集合或者整个文档中的单元集合中，根据指定的面层选项来进行过滤
        /// </summary>
        public ICollection<ElementId> FilterSelected(FaceOptions filter)
        {
            ICollection<ElementId> faces;
            //
            ICollection<ElementId> eleIds = _uiDoc.Selection.GetElementIds();
            //
            // 如果没有选择任何单元，则从整个文档中进行搜索 
            faces = eleIds.Count == 0
                ? Filterfaces(_doc, null, filter)
                : Filterfaces(_doc, eleIds, filter);

            //
            return faces;
        }

        //   面层对象的选择
        /// <summary>
        ///  从整个文档中的指定集合中搜索面层对象
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elementIds"> 如果其值为null，则表示搜索集合为整个文档中的所有单元 </param>
        /// <returns></returns>
        private IList<WallFace> faceLookup(Document doc, ICollection<ElementId> elementIds)
        {
            List<WallFace> faces = new List<WallFace>();

            FilteredElementCollector coll;
            coll = (elementIds == null) ? new FilteredElementCollector(doc) : new FilteredElementCollector(doc, elementIds);

            // 首先判断类型
            coll.OfClass(typeof(DirectShape));

            // 判断单元的类别是否是指定的类别集合中的一个
            List<Element> faceCategoryElems = coll.Where(ele => CategoryIds.Contains(ele.Category.Id)).ToList();

            // 再判断参数中是否有标识参数
            Parameter pa;
            string tag;
            foreach (Element ele in faceCategoryElems)
            {
                WallFace wf;
                if (!WallFace.IsWallFace(ele, out wf))
                {
                    continue;
                }
                // 满足所有过滤条件
                faces.Add(wf);
            }
            return faces;
        }

        //   根据指定的类别与类型来进行面层对象的过滤
        /// <summary>
        ///  从整个文档中的指定集合中 按指定的过滤选项 搜索面层对象
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elementIds"> 如果其值为null，则表示过滤集合为整个文档中的所有单元 </param>
        /// <returns></returns>
        private IList<ElementId> Filterfaces(Document doc, ICollection<ElementId> elementIds, FaceOptions opt)
        {
            List<ElementId> faces = new List<ElementId>();

            FilteredElementCollector coll;
            coll = (elementIds == null) ? new FilteredElementCollector(doc) : new FilteredElementCollector(doc, elementIds);

            // 首先判断类型 与 过滤指定的类别
            coll.OfClass(typeof(DirectShape)).OfCategoryId(opt.CategoryId);

            // 跟据参数值进行过滤
            string tag;
            foreach (Element ele in coll)
            {
                WallFace wallface;

                if (!WallFace.IsWallFace(ele as DirectShape, out wallface))
                {
                    continue;
                }

                // 2. 过滤面层类型，如“防水”
                bool hasType = wallface.GetFaceType(out tag);
                if (!hasType || tag == null || !tag.Equals(opt.FaceType, StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                // 满足所有过滤条件
                faces.Add(ele.Id);
            }

            return faces;
        }

        #endregion

        #region ---   根据参数值进行进一步过滤

        /// <summary>
        /// 从给出的面层对象集合中，获得所有的面层类型（比如涂料、防水等）
        /// </summary>
        /// <param name="faces"></param>
        /// <returns> 返回的面层类型集合中，没有相同的项。 </returns>
        public IList<string> GetFaceTypes(IEnumerable<WallFace> faces)
        {
            IList<string> types = new List<string>();

            string type;
            foreach (WallFace f in faces)
            {
                if (f.GetFaceType(out type) && !types.Contains(type))
                {
                    types.Add(type);
                }
            }
            return types;
        }

        #endregion

    }

}
