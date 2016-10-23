using System.Collections.Generic;
using System;
using System.Windows.Forms;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Data;
using System.Xml.Linq;
using Microsoft.VisualBasic;
using System.Collections;
using System.Linq;


namespace RevitStd.Curves
{

    /// <summary>
    /// 曲线集合的检查模式
    /// </summary>
    [Flags()]
    public enum CurveCheckMode
    {

        /// <summary>
        /// 所绘制的模型线并不要求相连，也就是不进行任何检测。
        /// </summary>
        Seperated = 0,

        /// <summary>
        /// 集合中的线条在整体上是连续的，但是线条之间的顺序可能是混乱的。
        /// </summary>
        Connected = 1,

        /// <summary>
        /// 绘制一个封闭的曲线集合。集合中不能包含多个封闭曲线链
        /// </summary>
        /// <remarks>可以通过连续曲线链的左端点与右端点是否重合来判断</remarks>
        Closed = 2,

        /// <summary>
        /// 集合中的曲线在水平面上，但是并不一定是连续的。
        /// </summary>
        HorizontalPlan = 8
    }

    /// <summary>
    /// 当前曲线集合在检测后的状态
    /// </summary>
    public enum CurveCheckState
    {

        /// <summary> 当前的曲线不满足检查要求，而且不能撤消，而应该直接退出绘制 </summary>
        Invalid_Exit = 0,

        /// <summary> 当前的曲线还未满足检查要求，但是还可以继续绘制。
        /// 比如要求绘制一个封闭的曲线链，则在未封闭的过程中，只要其是连续的，就还可以继续绘制。 </summary>
        Invalid_Continue = 2,

        /// <summary> 当前的曲线不满足检查要求，然后应该通过询问用户是否是撤消或者重做 </summary>
        Invalid_InquireForUndo = 3,

        /// <summary>
        /// 比如绘制一个封闭的曲线完成
        /// </summary>
        Valid_Exit = 3,

        /// <summary>
        /// 比如绘制连续曲线链时，在当前的连续链的基础上，还可以接着绘制
        /// </summary>
        Valid_Continue = 4,

        /// <summary>
        /// 比如在绘制有孔截面时，绘制好一个封闭曲线后，还可以继续绘制另一个封闭曲线，也可以不绘制了。
        /// </summary>
        Valid_InquireForContinue = 5

    }

}
