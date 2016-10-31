using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using eZstd.Windows;

namespace RevitStd
{
    /// <summary> 与 Revit中的颜色相关的操作 </summary>
    public static class ColorConverter
    {
       
        #region ---   Color 颜色的格式转换

        public static void ConvertColor(System.Windows.Media.Color mediaColor, out Autodesk.Revit.DB.Color color2)
        {
            color2 = new Autodesk.Revit.DB.Color(mediaColor.R, mediaColor.G, mediaColor.B);
        }


        public static void ConvertColor(Autodesk.Revit.DB.Color revitcColor, out System.Windows.Media.Color color2)
        {
            color2 = new System.Windows.Media.Color()
            {
                R = revitcColor.Red,
                G = revitcColor.Green,
                B = revitcColor.Blue,
            };
        }

        public static void ConvertColor(System.Drawing.Color drawingColor, out System.Windows.Media.Color color2)
        {
            color2 = new System.Windows.Media.Color()
            {
                R = drawingColor.R,
                G = drawingColor.G,
                B = drawingColor.B,
            };
        }

        public static void ConvertColor(System.Drawing.Color drawingColor, out Autodesk.Revit.DB.Color color2)
        {
            color2 = new Autodesk.Revit.DB.Color(drawingColor.R, drawingColor.G, drawingColor.B);
        }

        public static void ConvertColor(System.Windows.Media.Color mediaColor, out System.Drawing.Color color2)
        {
            color2 = System.Drawing.Color.FromArgb(255, mediaColor.R, mediaColor.G, mediaColor.B);

        }


        #endregion
    }
}
