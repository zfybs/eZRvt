using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;

namespace RevitStd
{
    /// <summary>
    /// Revit中对象的扩展方法
    /// </summary>
    /// <remarks></remarks>
    public static class ExtensionMethods
    {
        #region   ---  elementId

        /// <summary> 从ElementId返回其所在的Document中的Element对象 </summary>
        /// <param name="elementId"></param>
        /// <param name="Doc">此elementId所位于的文档</param>
        public static Element Element(this ElementId elementId, Document Doc)
        {
            return Doc.GetElement(elementId);
        }

        #endregion

        #region   ---  Family

        /// <summary> 对Document中加载的族进行重命名 </summary>
        /// <param name="Family"></param>
        /// <param name="NewName">要重新命名的新名称</param>
        public static void ReName(this Family Family, string NewName)
        {
            Document doc = Family.Document;
            using (Transaction tran = new Transaction(doc, "Rename family"))
            {
                tran.Start();
                Family.Name = NewName;
                tran.Commit();
            }
        }

        #endregion

        #region   ---  Transform

        /// <summary> 以矩阵的形式返回变换矩阵，仅作显示之用 </summary>
        /// <param name="Trans"></param>
        public static string ToString_Matrix(this Transform Trans)
        {
            string str = "";
            Transform with_1 = Trans;
            str = "(" + with_1.BasisX.X.ToString("0.000") + "  ,  " + with_1.BasisY.X.ToString("0.000") + "  ,  " +
                  with_1.BasisZ.X.ToString("0.000") + "  ,  " + with_1.Origin.X.ToString("0.000") + ")" + "\r\n" +
                  "(" + with_1.BasisX.Y.ToString("0.000") + "  ,  " + with_1.BasisY.Y.ToString("0.000") + "  ,  " +
                  with_1.BasisZ.Y.ToString("0.000") + "  ,  " + with_1.Origin.Y.ToString("0.000") + ")" + "\r\n" +
                  "(" + with_1.BasisX.Z.ToString("0.000") + "  ,  " + with_1.BasisY.Z.ToString("0.000") + "  ,  " +
                  with_1.BasisZ.Z.ToString("0.000") + "  ,  " + with_1.Origin.Z.ToString("0.000") + ")";
            return str;
        }

        #endregion

        #region   ---  Double

        /// <summary> 长度单位转换：将英尺转换为毫米 1英尺=304.8mm </summary>
        /// <param name="value_foot"></param>
        /// <remarks> 1 foot = 12 inches = 304.8 mm</remarks>
        public static double Foot2mm(this double value_foot)
        {
            // 1 foot = 12 inches = 304.8 mm
            return value_foot * 304.8;
        }

        #endregion
    }
}