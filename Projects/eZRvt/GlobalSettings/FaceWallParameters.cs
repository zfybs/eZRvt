using System;
using System.IO;
using System.Reflection;

namespace eZRvt.GlobalSettings
{

    /// <summary> 与项目“面墙”相关的全局数据 </summary>
    internal static class FaceWallParameters
    {
        #region ---   共享参数文件

        /// <summary>
        /// 面层对象的共享参数的参数组名称
        /// </summary>
        public const string sp_Group_Face = "面层参数";

        public const string sp_FaceIdTag = "面层标识";
        /// <summary>
        /// 共享参数：面层对象的标识符
        /// </summary>
        public static readonly Guid sp_FaceIdTag_guid = new Guid("34789535-64b2-4cc8-8ea6-9c8958488042");

        public const string sp_Area = "面层面积";
        /// <summary>
        /// 共享参数：面层的面积
        /// </summary>
        public static readonly Guid sp_Area_guid = new Guid("6f78df73-155e-431f-b7dc-a35e3a92706b");


        public const string sp_Volumn = "面层体积";
        /// <summary>
        /// 共享参数：面层的体积
        /// </summary>
        public static readonly Guid sp_Volumn_guid = new Guid("e0662365-c1c2-4977-9de0-ef28e23eb3bc");

        public const string sp_FaceType = "面层类型";
        /// <summary>
        /// 共享参数：面层的类型，比如防水、抹灰等
        /// </summary>
        public static readonly Guid sp_FaceType_guid = new Guid("ecdd8c21-f531-48a1-a6ea-1c3d13653e5c");


        #endregion
        
        #region ---   UI面板参数

        public const string AddinTabName = "面层";
        public const string panelName_DrawFace = "绘制面层";
        public const string buttonName_DrawFace = "DrawFace";

        #endregion

        /// <summary> 面层对象区别于墙类别中其他对象的标识字段 </summary>
        public const string FaceIdentificaion = "CMIE_面层";
    }
}
