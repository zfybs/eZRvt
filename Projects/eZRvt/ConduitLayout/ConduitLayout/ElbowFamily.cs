using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace eZRvt.ConduitLayout
{
    /// <summary>
    /// 弯头所对应的族文档
    /// </summary>
    public class ElbowFamily
    {
        public Document ElbowFamilyDoc;

        public ElbowFamily(Document elbowFamilyDoc)
        {
            if (elbowFamilyDoc == null || !elbowFamilyDoc.IsFamilyDocument) throw new ArgumentException("不是有效的弯头族。");
            ElbowFamilyDoc = elbowFamilyDoc;
        }

        public string GetFormula()
        {
            FamilyParameter pa = ElbowFamilyDoc.FamilyManager.get_Parameter(BuiltInParameter.RBS_CONDUIT_BENDRADIUS);   // 获取弯头族中“弯曲半径”参数
            return pa.Formula;  // 其值应该大致为 “管件外径 * 15”
        }

    }
}
