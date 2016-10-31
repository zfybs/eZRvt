using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using eZRvt.GlobalSettings;

namespace eZRvt.FaceWall
{
    public class WallFace
    {
        public readonly DirectShape FaceElement;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="faceElement">请自行确保 faceElement 所对应的单元是一个面层对象</param>
        private WallFace(DirectShape faceElement)
        {
            FaceElement = faceElement;
        }

        #region ---   提取参数

        /// <summary>
        /// 获取面层对象的标识符。正常情况下，此参数的值应该是一个常数“CMIE_面层”
        /// </summary>
        /// <param name="ds">  </param>
        /// <param name="wallFace"> 如果此函数返回 true，返回面层对象，如果此函数返回False，则此参数返回 null </param>
        /// <returns>如果没有找到此参数，则返回false。</returns>
        public static bool IsWallFace(Element ds, out WallFace wallFace)
        {
            if (!(ds is DirectShape))
            {
                wallFace = null;
                return false;
            }

            return IsWallFace((DirectShape)ds, out wallFace);
        }

        /// <summary>
        /// 获取面层对象的标识符。正常情况下，此参数的值应该是一个常数“CMIE_面层”
        /// </summary>
        /// <param name="ds">  </param>
        /// <param name="wallFace"> 如果此函数返回 true，返回面层对象，如果此函数返回False，则此参数返回 null </param>
        /// <returns>如果没有找到此参数，则返回false。</returns>
        public static bool IsWallFace(DirectShape ds, out WallFace wallFace)
        {
            if (ds == null)
            {
                wallFace = null;
                return false;
            }
            Parameter pa = ds.get_Parameter(FaceWallParameters.sp_FaceIdTag_guid);
            if (pa == null)
            {
                wallFace = null;
                return false;
            }

            string idTag = pa.AsString();
            if (idTag == null || !pa.AsString().Equals(FaceWallParameters.FaceIdentificaion))
            {
                wallFace = null;
                return false;
            }
            wallFace = new WallFace(ds);
            return true;
        }


        /// <summary>
        /// 获取面层类型，如“防水”
        /// </summary>
        /// <param name="faceType">提取到的面层类型信息</param>
        /// <returns>如果没有找到此参数，则返回false。</returns>
        public bool GetFaceType(out string faceType)
        {
            Parameter pa = FaceElement.get_Parameter(FaceWallParameters.sp_FaceType_guid);
            if (pa == null)
            {
                faceType = null;
                return false;
            }
            faceType = pa.AsString();
            return true;
        }

        /// <summary>
        /// 获取面层的面积
        /// </summary>
        /// <param name="area">提取到的面层面积信息</param>
        /// <returns>如果没有找到此参数，则返回false。</returns>
        public bool GetArea(out double area)
        {
            Parameter pa = FaceElement.get_Parameter(FaceWallParameters.sp_Area_guid);
            if (pa == null)
            {
                area = -1;
                return false;
            }
            area = pa.AsDouble();
            return true;
        }

        /// <summary>
        /// 获取面层的体积
        /// </summary>
        /// <param name="volumn">提取到的面层体积信息</param>
        /// <returns>如果没有找到此参数，则返回false。</returns>
        public bool GetVolumn(out double volumn)
        {
            Parameter pa = FaceElement.get_Parameter(FaceWallParameters.sp_Volumn_guid);
            if (pa == null)
            {
                volumn = -1;
                return false;
            }
            volumn = pa.AsDouble();
            return true;
        }

        #endregion
    }
}
