using System.Collections.Generic;
using Autodesk.Revit.DB;

namespace RevitStd.Curves
{
    /// <summary>
    /// 对曲线集合的形式进行修改，但是不检查曲线集合是否连续或者共面等限制条件，如果出错，自行负责。
    /// </summary>
    public static class CurvesConverter
    {
        #region   ---   IEnumerable<Curve>

        public static void Convert(IEnumerable<Curve> sourceCurves, out CurveArray targetCurves)
        {
            targetCurves = new CurveArray();
            foreach (Curve c in sourceCurves)
            {
                targetCurves.Append(c);
            }
        }

        #endregion

        #region   ---   List<List<Curve>>

        /// <summary>  </summary>
        public static void Convert(List<List<Curve>> sourceCurves, out CurveArrArray targetCurves)
        {
            targetCurves = new CurveArrArray();
            foreach (List<Curve> curves in sourceCurves)
            {
                CurveArray ca = new CurveArray();
                foreach (Curve c in curves)
                {
                    ca.Append(c);
                }
                targetCurves.Append(ca);
            }
        }

        /// <summary>  </summary>
        public static void Convert(List<List<Curve>> sourceCurves, out List<Curve> targetCurves)
        {
            targetCurves = new List<Curve>();
            foreach (List<Curve> curves in sourceCurves)
            {
                foreach (Curve c in curves)
                {
                    targetCurves.Add(c);
                }
            }
        }

        #endregion

        #region   ---   CurveArray

        public static void Convert(CurveArray sourceCurves, out IList<Curve> targetCurves)
        {
            targetCurves = new List<Curve>();
            foreach (Curve c in sourceCurves)
            {
                targetCurves.Add(c);
            }
        }

        #endregion

        #region   ---   CurveLoop

        public static void Convert(CurveLoop sourceCurves, out CurveArray targetCurves)
        {
            targetCurves = new CurveArray();
            foreach (Curve c in sourceCurves)
            {
                targetCurves.Append(c);
            }
        }

        #endregion

        #region   ---   IEnumerable<CurveLoop>

        public static void Convert(IEnumerable<CurveLoop> sourceCurves, out CurveArray targetCurves)
        {
            targetCurves = new CurveArray();
            foreach (CurveLoop cl in sourceCurves)
            {
                foreach (var c in cl)
                {
                    targetCurves.Append(c);
                }
            }
        }

        #endregion

        #region   ---   EdgeArray

        public static void Convert(EdgeArray sourceCurves, out IList<Curve> targetCurves)
        {
            targetCurves = new List<Curve>();
            foreach (Edge ed in sourceCurves)
            {
                targetCurves.Add(ed.AsCurve());
            }
        }

        public static void Convert(EdgeArray sourceCurves, out CurveArray targetCurves)
        {
            targetCurves = new CurveArray();
            foreach (Edge ed in sourceCurves)
            {
                targetCurves.Append(ed.AsCurve());
            }
        }

        #endregion

        #region   ---   EdgeArrayArray

        public static void Convert(EdgeArrayArray sourceCurves, out List<Curve> targetCurves)
        {
            targetCurves = new List<Curve>();
            foreach (EdgeArray cl in sourceCurves)
            {
                foreach (Edge c in cl)
                {
                    targetCurves.Add(c.AsCurve());
                }
            }
        }

        /// <summary> 转换曲线集合的格式，并进行空间变换 </summary>
        public static void Convert(EdgeArrayArray sourceCurves, Transform transf, out List<List<Curve>> targetCurves)
        {
            targetCurves = new List<List<Curve>>();

            foreach (EdgeArray cl in sourceCurves)
            {
                var curveloop = new List<Curve>();
                foreach (Edge c in cl)
                {
                    curveloop.Add(c.AsCurve().CreateTransformed(transf));
                }
                targetCurves.Add(curveloop);
            }
        }

        /// <summary> 转换曲线集合的格式，并进行空间变换 </summary>
        /// <param name="transf"></param>
        public static void Convert(EdgeArrayArray sourceCurves, Transform transf, out List<Curve> targetCurves)
        {
            List<List<Curve>> llc;
            Convert(sourceCurves, transf, out llc);
            //
            Convert(llc, out targetCurves);
        }

        /// <summary> 转换曲线集合的格式，并进行空间变换 </summary>
        public static void Convert(EdgeArrayArray sourceCurves, Transform transf, out CurveArrArray targetCurves)
        {
            List<List<Curve>> llc;
            Convert(sourceCurves, transf, out llc);
            //
            Convert(llc, out targetCurves);
        }

        #endregion
    }
}