using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitStd.Curves;

namespace RevitStd.Selector
{
    /// <summary>
    /// 在Revit界面中选择出多个封闭的模型线
    /// </summary>
    public class ClosedCurveSelector
    {
        private readonly UIDocument _uiDoc;

        /// <summary>
        /// 是否要分次选择多个封闭的模型曲线链
        /// </summary>
        private readonly bool _multipleClosed;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="uiDoc">进行模型线选择的那个文档</param>
        /// <param name="multiple"> 是否要分次选择多个封闭的模型曲线链</param>
        public ClosedCurveSelector(UIDocument uiDoc, bool multiple)
        {
            _uiDoc = uiDoc;
            this._multipleClosed = multiple;
        }

        /// <summary>
        /// 开启同步操作：在Revit UI 界面中选择封闭的模型曲线链
        /// </summary>
        /// <returns></returns>
        public CurveArrArray SendSelect(out List<List<ModelCurve>> modelCurvess)
        {
            CurveArrArray profiles = new CurveArrArray(); // 每一次创建开挖土体时，在NewExtrusion方法中，要创建的实体的轮廓
            modelCurvess = new List<List<ModelCurve>>();
            //
            bool blnStop = false;
            do
            {
                blnStop = true;
                List<ModelCurve> modelCurves;
                CurveLoop cvLoop = GetLoopedCurve(out modelCurves);
                // 检验并添加
                if (cvLoop != null && cvLoop.Any())
                {
                    CurveArray cvArr = new CurveArray();
                    foreach (Curve c in cvLoop)
                    {
                        cvArr.Append(c);
                    }

                    //
                    profiles.Append(cvArr);
                    modelCurvess.Add(modelCurves);

                    // 是否要继续添加
                    if (this._multipleClosed)
                    {
                        DialogResult res = MessageBox.Show("曲线添加成功，是否还要继续添加？", "提示", MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question);
                        if (res == DialogResult.Yes)
                        {
                            blnStop = false;
                        }
                    }
                }
            } while (!blnStop);
            return profiles;
        }

        /// <summary>
        /// 获取一组连续封闭的模型线
        /// </summary>
        /// <returns></returns>
        /// <remarks></remarks>
        private CurveLoop GetLoopedCurve(out List<ModelCurve> modelCurves)
        {
            Document Doc = this._uiDoc.Document;
            //

            IList<Reference> boundaries = Selector.SelectGeneralElements<ModelCurve>(_uiDoc, out modelCurves, "选择一组模型线");
            //
            if (boundaries == null || !boundaries.Any())
            {
                return null;
            }

            CurveLoop cvLoop = new CurveLoop();
            try
            {
                if (boundaries.Count == 1) // 要么是封闭的圆或圆弧，要么就不封闭
                {
                    Curve c = ((ModelCurve)Doc.GetElement(boundaries[0])).GeometryCurve;
                    if ((c is Arc || c is Ellipse) && !c.IsBound)
                    {
                        cvLoop.Append(c);
                    }
                    else
                    {
                        throw new InvalidOperationException("选择的一条圆弧线或者椭圆线并不封闭。");
                    }
                }
                else
                {
                    // 对于选择了多条曲线的情况
                    IList<Curve> cs = CurvesFormator.GetContiguousCurvesFromCurves(Doc, boundaries);

                    if (cs != null)
                    {
                        foreach (Curve c in cs)
                        {
                            cvLoop.Append(c);
                        }
                    }
                    else
                    {
                        throw new InvalidOperationException("所选择的曲线不连续。");
                    }

                    if (cvLoop.IsOpen())
                    {
                        throw new InvalidOperationException("所选择的曲线不能形成封闭的曲线。");
                    }
                    else if (!cvLoop.HasPlane())
                    {
                        throw new InvalidOperationException("所选择的曲线不在同一个平面上。");
                    }
                    else
                    {
                        return cvLoop;
                    }
                }
            }
            catch (Exception ex)
            {
                DialogResult res =
                    MessageBox.Show(
                        ex.Message + " 点击是以重新选择，点击否以退出绘制。" + "\r\n" + "当前选择的曲线条数为：" + Convert.ToString(boundaries.Count) +
                        "条。" +
                        "\r\n" + ex.StackTrace, "Warnning", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    cvLoop = GetLoopedCurve(out modelCurves);
                }
                else
                {
                    cvLoop = new CurveLoop();
                    return cvLoop;
                }
            }
            return cvLoop;
        }

    }
}