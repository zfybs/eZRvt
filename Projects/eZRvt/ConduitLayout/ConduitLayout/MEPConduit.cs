using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Electrical;


namespace eZRvt.ConduitLayout
{
    /// <summary>
    /// 弯头
    /// </summary>
    public class MEPConduit
    {
        /// <summary> 线管对象 </summary>
        public Conduit ConduitIns;

        public MEPConduit(Conduit conduit)
        {
            ConduitIns = conduit;
        }

        #region ---   获取各种参数


        /// <summary>
        /// 获取线管的直径。
        /// </summary>
        /// <param name="cd"> 线管对象 </param>
        /// <returns>线管的直径，单位为 inch </returns>
        public double? GetConduitDiameter()
        {
            Parameter pa = ConduitIns.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM);
            if (pa != null)
            {
                return pa.AsDouble();
            }
            return null;
        }

        /// <summary>
        /// 设置线管的直径。
        /// </summary>
        /// <param name="cd"> 线管对象 </param>
        /// <param name="diameter"> 直径，单位为 inch </param>
        public void SetConduitDiameter(double diameter)
        {
            Parameter pa = ConduitIns.get_Parameter(BuiltInParameter.RBS_CONDUIT_DIAMETER_PARAM);
            if (pa != null && !pa.IsReadOnly)
            {
                pa.Set(diameter);
            }
        }

        /// <summary>
        /// 设置线管的外径
        /// </summary>
        /// <param name="cd"> 线管对象 </param>
        /// <param name="diameter"> 直径，单位为 inch </param>
        public double? GetConduitOuterDiameter()
        {
            Parameter pa = ConduitIns.get_Parameter(BuiltInParameter.RBS_CONDUIT_OUTER_DIAM_PARAM);
            if (pa != null)
            {
                return pa.AsDouble();
            }
            return null;
        }

        #endregion
    }
}
