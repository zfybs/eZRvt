using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;

namespace eZRvt.FaceWall
{

    /// <summary>
    /// 进行面层的绘制或者过滤时的设置参数
    /// </summary>
    public class FaceOptions
    {
        #region ---   Properties

        private double _surfaceThickness;
        /// <summary>
        /// 面层的厚度，单位为米
        /// </summary>
        public double SurfaceThickness
        {
            get { return _surfaceThickness; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(@"厚度值必须大于零");
                }
                _surfaceThickness = value;
            }
        }

        /// <summary> 面层对象在Revit中要显示的颜色 </summary>
        public Color Color { get; set; }

        /// <summary> 面层对象在Revit中所属的类别 </summary>
        public ElementId CategoryId { get; set; }

        /// <summary> 面层的类型，比如防水、抹灰等 </summary>
        public string FaceType { get; set; }

        /// <summary>
        /// 如果为true，则在绘制面层时，要将选择的单元中，所有与选定的面同法向的面上均绘制出面层
        /// </summary>
        public readonly bool IncludeSameNormal;

        /// <summary>
        /// 如果为true，则在选择表面时排除面层对象
        /// </summary>
        public readonly bool ExcludeFaceElement;

        /// <summary>
        /// 如果为true，则先选择多个面，再将对应的面层Solid存储在一个directShape对象中
        /// </summary>
        public readonly bool MultiFaces;

        /// <summary>
        /// 当一个面层对象中有多个实体时，是否将将这些实体进行可能的组合。
        /// </summary>
        public readonly bool UnionInnerSolids;

        #endregion

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="doc">面层的厚度，单位为米</param>
        /// <param name="includeSameNormal"> 如果为true，则在绘制面层时，要将选择的单元中，所有与选定的面同法向的面上均绘制出面层 </param>
        /// <param name="excludeFace"> 如果为true，则在选择表面时排除面层对象 </param>
        /// <param name="multiFaces"> 如果为true，则先选择多个面，再将对应的面层Solid存储在一个directShape对象中 </param>
        /// <param name="surfaceThickness">面层的厚度，单位为米</param>
        /// <param name="color"> 面层对象在Revit中要显示的颜色 </param>
        /// <param name="categoryId"> 面层对象在Revit中所属的类别 </param>
        /// <param name="faceType"> 面层的类型，比如防水、抹灰等 </param>
        internal FaceOptions(bool includeSameNormal, bool excludeFace, bool multiFaces, bool unionInnerSolids,
            double surfaceThickness = 0.01, Color color = null,
            ElementId categoryId = null, string faceType = null)
        {
            SurfaceThickness = surfaceThickness;
            Color = color ?? new Color(255, 0, 0);
            CategoryId = categoryId ?? new ElementId(BuiltInCategory.OST_Walls);
            FaceType = faceType ?? "防水";
            IncludeSameNormal = includeSameNormal;
            ExcludeFaceElement = excludeFace;
            MultiFaces = multiFaces;
            UnionInnerSolids = unionInnerSolids;
        }
    }
}

