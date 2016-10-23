using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace RevitStd
{
    public static class FilterTools
    {
        #region "------从 Family 获得  FamilyInstance"

        /// <summary> 返回项目文档中某族Family的所有实例 </summary>
        /// <param name="Category">此族所属的 BuiltInCategory 类别，如果不确定，就不填。</param>
        /// <param name="Family"></param>
        public static FilteredElementCollector Instances(this Family Family,
            BuiltInCategory Category = BuiltInCategory.INVALID)
        {
            Document doc = Family.Document;
            List<ElementId> SymbolsId = Family.GetFamilySymbolIds().ToList();
            FilteredElementCollector Collector1 = new FilteredElementCollector(doc);

            if (SymbolsId.Count > 0)
            {
                // 创建过滤器
                FamilyInstanceFilter Filter = new FamilyInstanceFilter(doc, SymbolsId[0]);
                // 执行过滤条件
                if (Category != BuiltInCategory.INVALID)
                {
                    Collector1 = Collector1.OfCategory(Category);
                }
                Collector1.WherePasses(Filter);
            }
            // 当族类型多于一个时，才进行相交
            if (SymbolsId.Count > 1)
            {
                for (int index = 1; index <= SymbolsId.Count - 1; index++)
                {
                    // 创建过滤器
                    FamilyInstanceFilter Filter = new FamilyInstanceFilter(doc, SymbolsId[index]);
                    FilteredElementCollector Collector2 = new FilteredElementCollector(doc);
                    // 执行过滤条件
                    if (Category != BuiltInCategory.INVALID)
                    {
                        Collector2 = Collector2.OfCategory(Category);
                    }
                    Collector2.WherePasses(Filter);

                    // 将此FamilySymbol的实例添加到集合中
                    Collector1.UnionWith(Collector2);
                }
            }
            return Collector1;
        }

        #endregion

        #region "------从 Family 获得 FamilySymbol"

        /// <summary>
        /// 通过族名称获得该族的类型
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static ISet<ElementId> getFamilySymbolsByFamilyName(Document doc, String name)
        {
            //得到族
            Family fam = doc.FindFamily(name);
            ISet<ElementId> famSymbolIds = fam.GetFamilySymbolIds();
            return famSymbolIds;
        }

        #endregion

        #region "------从 FamilySymbol 获得 FamilyInstance"

        /// <summary> 返回项目文档中某族类型FamilySymbol的所有实例 </summary>
        /// <param name="FamilySymbol"></param>
        public static FilteredElementCollector Instances(this FamilySymbol FamilySymbol)
        {
            Document doc = FamilySymbol.Document;
            ElementId FamilySymbolId = FamilySymbol.Id;
            FilteredElementCollector InsancesColl = new FilteredElementCollector(doc);
            FamilyInstanceFilter FIFilter = new FamilyInstanceFilter(doc, FamilySymbolId);
            InsancesColl.WherePasses(FIFilter);
            return InsancesColl;
        }

        #endregion

        #region "------交"

        /// <summary>
        /// 返回两个集合交集
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="col1"></param>
        /// <param name="col2"></param>
        /// <returns></returns>
        public static ICollection<ElementId> intersect(Document doc, ICollection<ElementId> col1,
            ICollection<ElementId> col2)
        {
            //过滤器
            FilteredElementCollector colFilter1 = new FilteredElementCollector(doc, col1);
            FilteredElementCollector colFilter2 = new FilteredElementCollector(doc, col2);
            FilteredElementCollector colResult = colFilter1.IntersectWith(colFilter2);
            return colResult.ToElementIds();
        }

        #endregion

        #region "------并"

        /// <summary>
        /// 返回两个集合并集
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="col1"></param>
        /// <param name="col2"></param>
        /// <returns></returns>
        public static ICollection<ElementId> union(Document doc, ICollection<ElementId> col1,
            ICollection<ElementId> col2)
        {
            //过滤器
            FilteredElementCollector colFilter1 = new FilteredElementCollector(doc, col1);
            FilteredElementCollector colFilter2 = new FilteredElementCollector(doc, col2);
            FilteredElementCollector colResult = colFilter1.UnionWith(colFilter2);
            return colResult.ToElementIds();
        }

        #endregion

        #region "------从 Document 中获得 Element：搜索文档中的元素"

        /// <summary>
        /// 根据指定的类型、类别与名称来搜索Revit文档所有元素中的第一个有效对象
        /// </summary>
        /// <param name="rvtDoc"></param>
        /// <param name="targetType"></param>
        /// <param name="targetCategory"> INVALID 表示不进行类别限制 </param>
        /// <param name="targetName"></param>
        /// <returns></returns>
        public static IList<Element> FindElements(this Document rvtDoc, Type
            targetType, BuiltInCategory targetCategory = BuiltInCategory.INVALID, string targetName = null)
        {
            //'  first, narrow down to the elements of the given type and category
            var collector = new FilteredElementCollector(rvtDoc).OfClass(targetType);

            // 是否要按类别搜索
            if (!(targetCategory == BuiltInCategory.INVALID))
            {
                collector.OfCategory(targetCategory);
            }

            // 是否要按名称搜索
            if (targetName != null)
            {
                //'  using LINQ query here.
                var elems = from element in collector
                    where element.Name.Equals(targetName)
                    select element;

                //'  put the result as a list of element for accessibility.
                return elems.ToList();
            }
            return collector.ToElements();
        }

        /// <summary>
        ///  根据指定的类型、类别与名称来搜索Revit文档指定元素集合中的第一个有效对象
        /// </summary>
        /// <param name="rvtDoc">要进行搜索的Revit文档</param>
        /// <param name="SourceElements">要从文档中的哪个集合中来进行搜索</param>
        /// <param name="targetType"></param>
        /// <param name="targetCategory"> INVALID 表示不进行类别限制 </param>
        /// <param name="targetName"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static IList<Element> FindElements(this Document rvtDoc, ICollection<ElementId> SourceElements, Type
            targetType, BuiltInCategory targetCategory = BuiltInCategory.INVALID, string targetName = null)
        {
            var collector = new FilteredElementCollector(rvtDoc, SourceElements);

            // 搜索类型
            collector.OfClass(targetType);

            // 是否要搜索类别
            if (!(targetCategory == BuiltInCategory.INVALID))
            {
                collector.OfCategory(targetCategory);
            }

            // 是否要搜索名称
            if (targetName != null)
            {
                IEnumerable<Element> elems = default(IEnumerable<Element>);
                //'  parse the collection for the given names
                //'  using LINQ query here.
                elems = from element in collector
                    where element.Name.Equals(targetName)
                    select element;
                return elems.ToList();
            }

            return collector.ToElements();
        }

        /// <summary>
        /// 根据指定的类型、类别与名称来搜索Revit文档的所有元素中的第一个有效对象
        /// </summary>
        /// <param name="rvtDoc"></param>
        /// <param name="targetType"></param>
        /// <param name="targetCategory"> INVALID 表示不进行类别限制 </param>
        /// <param name="targetName"></param>
        /// <returns></returns>
        public static Element FindElement(this Document rvtDoc, Type
            targetType, BuiltInCategory targetCategory = BuiltInCategory.INVALID, string targetName = null)
        {
            //'  find a list of elements using the overloaded method.
            IList<Element> elems = FindElements(rvtDoc, targetType, targetCategory, targetName);

            //'  return the first one from the result.
            if (elems.Count > 0)
            {
                return elems[0];
            }
            return null;
        }

        /// <summary>
        /// 根据指定的类型、类别与名称来搜索Revit文档指定元素集合中的第一个有效对象
        /// </summary>
        /// <param name="rvtDoc"></param>
        /// <param name="SourceElements"></param>
        /// <param name="targetType"></param>
        /// <param name="targetCategory"> INVALID 表示不进行类别限制 </param>
        /// <param name="targetName"></param>
        /// <returns></returns>
        public static Element FindElement(this Document rvtDoc, ICollection<ElementId> SourceElements, Type
            targetType, BuiltInCategory targetCategory = BuiltInCategory.INVALID, string targetName = null)
        {
            //'  find a list of elements using the overloaded method.
            IList<Element> elems = FindElements(rvtDoc, targetType, targetCategory, targetName);

            //'  return the first one from the result.
            if (elems.Count > 0)
            {
                return elems[0];
            }
            return null;
        }

        #endregion

        #region "------从 Document 中获得 Family"

        /// <summary> 返回项目文档中指定名称的族Family对象 </summary>
        /// <param name="FamilyName">在此文档中，所要搜索的族对象的名称</param>
        /// <param name="Category">此族所属的 BuiltInCategory 类别，如果不确定，就不填。</param>
        /// <returns>如果没有找到指定的 Family，则返回 null。</returns>
        public static Family FindFamily(this Document Doc, string FamilyName,
            BuiltInCategory Category = BuiltInCategory.INVALID)
        {
            // 文档中所有的族对象
            FilteredElementCollector cols = new FilteredElementCollector(Doc);
            IList<Element> Familys = cols.OfClass(typeof (Family)).ToElements();
            // 按名称搜索族（Linq语句）
            IEnumerable<Family> Q = default(IEnumerable<Family>);
            if (Category == BuiltInCategory.INVALID) // 只搜索族的名称
            {
                Q = from Family ff in Familys
                    where ff.Name == FamilyName
                    select ff;
            }
            else // 同时搜索族对象的类别，注意，族的类别信息保存在属性中。
            {
                Q = from Family ff in Familys
                    where (ff.Name == FamilyName) && (ff.FamilyCategory.Id == new ElementId(BuiltInCategory.OST_Site))
                    select ff;
            }
            //
            Family fam = null;
            if (Q.Any())
            {
                fam = Q.First();
            }
            return fam;
        }

        /// <summary> 返回项目文档中指定类别的族对象。在函数中会对所有族对象的FamilyCategory进行判断。</summary>
        /// <param name="Category">此族所属的 BuiltInCategory 类别，即FamilyCategory属性所对应的类别。</param>
        public static List<Family> FindFamilies(this Document Doc, BuiltInCategory Category)
        {
            List<Family> fams = new List<Family>();
            // 文档中所有的族对象
            FilteredElementCollector cols = new FilteredElementCollector(Doc);
            IList<Element> Familys = cols.OfClass(typeof (Family)).ToElements();
            // 按类别搜索族（Linq语句）
            if (Category != BuiltInCategory.INVALID) // 只搜索族类别
            {
                IEnumerable<Family> Q = from Family ff in Familys
                    where ff.FamilyCategory.Id == new ElementId(BuiltInCategory.OST_Site)
                    select ff;
                fams = Q.ToList();
            }
            return fams;
        }

        #endregion
    }
}