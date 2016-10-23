using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.Revit.DB;
using RevitStd.Curves;


namespace RevitStd.Curves
{
    /// <summary>
    /// 判断曲线集合是否满足指定的格式，或者将集合中的曲线进行标准化，比如形成连续曲线，投影到同一个平面中等。
    /// </summary>
    public static partial class CurvesFormator
    {
        #region  曲线连续性 的 实现方法

        /// <summary> 将 CurveArray 中的曲线进行重新排列，以组成连续的曲线链 </summary>
        /// <param name="curveArr"> 要进行重组的曲线集合 </param>
        /// <param name="contigeousCurves"> 如果不能形成连续的曲线链，则返回 null </param>
        public static void GetContiguousCurvesFromCurves(CurveArray curveArr, out IList<Curve> contigeousCurves)
        {
            IList<Curve> curves = curveArr.Cast<Curve>().ToList();
            // Build a list of curves from the curve elements
            contigeousCurves = CurvesFormator.GetContiguousCurvesFromCurves(curves);
        }

        /// <summary> 将 CurveArray 中的曲线进行重新排列，以组成连续的曲线链 </summary>
        /// <param name="curveArr"> 要进行重组的曲线集合 </param>
        /// <param name="contigeousCurves"> 如果不能形成连续的曲线链，则返回 null，从外部调用来看，此参数可以与第一个参数curveArr赋同一个实参。 </param>
        public static void GetContiguousCurvesFromCurves(CurveArray curveArr, out CurveArray contigeousCurves)
        {
            IList<Curve> curves;

            // Build a list of curves from the curve elements
            CurvesFormator.GetContiguousCurvesFromCurves(curveArr, out curves);

            CurvesConverter.Convert(curves, out contigeousCurves);
        }

        /// <summary> 将 CurveArray 中的曲线进行重新排列，以组成连续的曲线链 </summary>
        /// <param name="curveArr"> 要进行重组的曲线集合 </param>
        /// <param name="contigeousCurves"> 如果不能形成连续的曲线链，则返回 null </param>
        public static void GetContiguousCurvesFromCurves(CurveArrArray curveArr, out CurveArrArray contigeousCurves)
        {
            CurveArrArray curveOut = new CurveArrArray();
            foreach (CurveArray cArr in curveArr)
            {
                CurveArray conti;
                GetContiguousCurvesFromCurves(cArr, out conti);
                if (conti == null)
                {
                    contigeousCurves = null;
                    return;
                }
                curveOut.Append(conti);
            }
            contigeousCurves = curveOut;
            return;
        }

        /// <summary>
        /// 从选择的Curve Elements中，获得连续排列的多段曲线（不一定要封闭）。
        /// </summary>
        /// <param name="doc">曲线所在文档</param>
        /// <param name="SelectedCurves">多条曲线元素所对应的Reference，可以通过Selection.PickObjects返回。
        /// 注意，SelectedCurves中每一条曲线都必须是有界的（IsBound），否则，其GetEndPoint会报错。</param>
        /// <returns>如果输入的曲线可以形成连续的多段线，则返回重新排序后的多段线集合；
        /// 如果输入的曲线不能形成连续的多段线，则返回Nothing！</returns>
        public static IList<Curve> GetContiguousCurvesFromCurves(Document doc, IList<Reference> SelectedCurves)
        {
            IList<Curve> curves = new List<Curve>();

            // Build a list of curves from the curve elements
            foreach (Reference reference in SelectedCurves)
            {
                CurveElement curveElement = doc.GetElement(reference) as CurveElement;
                curves.Add(curveElement.GeometryCurve.Clone());
            }
            //
            curves = CurvesFormator.GetContiguousCurvesFromCurves(curves);
            return curves;
        }

        /// <summary>
        /// 从指定的Curve集合中中，获得连续排列的多段曲线（不一定要封闭）。如果不连续，则返回Nothing。
        /// </summary>
        /// <param name="curves">多条曲线元素所对应的集合
        /// 注意，curves 集合中每一条曲线都必须是有界的（IsBound），否则，其 GetEndPoint 会报错。</param>
        /// <returns>如果输入的曲线可以形成连续的多段线，则返回重新排序后的多段线集合；
        /// 如果输入的曲线不能形成连续的多段线，则返回Nothing！</returns>
        public static IList<Curve> GetContiguousCurvesFromCurves(IList<Curve> curves)
        {
            return ContiguousCurveChain.FormatChain(curves);
        }

        #endregion

        #region  曲线共平面

        /// <summary>
        /// 曲线集合在同一个平面上，而不限定是哪一个法向的平面
        /// </summary>
        /// <param name="curves"></param>
        /// <returns></returns>
        public static bool IsInOnePlan(IList<Curve> curves)
        {
            bool bln = true;
            if (curves.Count < 2)
            {
                bln = true;
            }
            else
            {
                // 先找到一个基准平面法向
                XYZ v1 = default(XYZ); // 通过两个方向向量确定一个平面
                XYZ v2 = default(XYZ);
                XYZ norm = null; // 两个方向向量所确定的平面的法向
                //
                int CurveIndex = 0;
                float curveIndex2;
                //
                Curve c = default(Curve); // 进行搜索的曲线
                double start = 0; // 从曲线的起始处还是中间开始截取那半条曲线
                c = curves[0];
                v1 = c.GetEndPoint(0) - c.Evaluate(start + 0.5, true); // 第一条曲线的前半段

                // 以半条曲线作为递进，来搜索出基准平面
                for (curveIndex2 = 0.5f; curveIndex2 <= curves.Count - 1; curveIndex2 += 0.5f)
                {
                    if (curveIndex2 / 0.5 % 2 == 0)
                    {
                        start = 0;
                    }
                    else
                    {
                        start = 0.5;
                    }
                    CurveIndex = (int)Math.Floor(curveIndex2);
                    //
                    c = curves[CurveIndex];
                    if (!c.IsBound)
                    {
                        c.MakeBound(0, 1);
                    }
                    v2 = c.Evaluate(start, true) - c.Evaluate(start + 0.5, true);
                    if (v1.AngleTo(v2) < GeoHelper.AngleTolerance)
                    {
                        continue;
                    }
                    else
                    {
                        norm = v1.CrossProduct(v2);
                        break;
                    }
                }
                // 找到了基准法向后，再对后面的曲线进行比较
                if (norm == null)
                {
                    bln = true; // 说明所有的曲线都是共线的，那自然也就是共面的
                }
                else
                {
                    for (int i = CurveIndex + 1; i <= curves.Count - 1; i++)
                    {
                        c = curves[i];
                        if (!InPlan(c, norm))
                        {
                            bln = false;
                            break;
                        }
                    }
                }
            }
            return bln;
        }

        /// <summary>
        /// 曲线集合中的所有曲线是否在指定法向的平面上
        /// </summary>
        /// <param name="curves"></param>
        /// <param name="planNormal">指定平面的法向向量</param>
        /// <returns></returns>
        public static dynamic IsInOnePlan(ICollection<Curve> curves, XYZ planNormal)
        {
            bool bln = true;
            foreach (Curve c in curves)
            {
                if (!InPlan(c, planNormal))
                {
                    return false;
                }
            }
            return bln;
        }

        /// <summary>
        /// 指定的曲线是否位于指定的平面内。
        /// </summary>
        /// <param name="c"></param>
        /// <param name="planNormal"></param>
        /// <returns>空间三维曲线可能并不会在任何一个平面内，此时其自然是返回False。</returns>
        private static bool InPlan(Curve c, XYZ planNormal)
        {
            XYZ vec1 = default(XYZ);
            XYZ vec2 = default(XYZ);
            if (!c.IsBound)
            {
                c.MakeBound(0, 1);
            }
            //
            if (c is Line)
            {
                vec1 = c.GetEndPoint(0) - c.GetEndPoint(1);
                if (Math.Abs(vec1.AngleTo(planNormal) - 0.5 * Math.PI) < GeoHelper.AngleTolerance)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (!(c is Arc) || c is Ellipse || c is NurbSpline)
            {
                vec1 = c.GetEndPoint(0) - c.Evaluate(0.5, true);
                vec2 = c.Evaluate(0.5, true) - c.Evaluate(1, true);
                if ((Math.Abs(vec1.AngleTo(planNormal) - 0.5 * Math.PI) < GeoHelper.AngleTolerance) &&
                    (Math.Abs(vec2.AngleTo(planNormal) - 0.5 * Math.PI) < GeoHelper.AngleTolerance))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (c is CylindricalHelix) // 圆柱螺旋线
            {
                return false;
            }
            //
            return false;
        }

        #endregion
    }
}