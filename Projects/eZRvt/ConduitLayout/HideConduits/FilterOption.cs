using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace eZRvt.ConduitLayout
{
    public class FilterOption
    {
        /// <summary>
        /// 顶部标高位置，单位为m
        /// </summary>
        public double Top;
        /// <summary>
        /// 底部标高位置，单位为m
        /// </summary>
        public double Bottom;

        /// <summary>
        /// 如果为true，表示线管中只要有一个 Connector 不在视图范围内，则将其隐藏；
        /// 如果为false，表示线管中只要有一个 Connector 在视图范围内，则不将其隐藏
        /// </summary>
        public bool CheckAllConnector;

        /// <summary>
        /// 如果为true，表示将标高区间内的线管对象进行显示；
        /// 如果为false，表示将标高区间内的线管对象进行隐藏；
        /// </summary>
        public bool ShowElementsInRange;

        public FilterOption(double top, double bottom, bool checkAllConnector, bool showElementsInRange)
        {
            Top = top;
            Bottom = bottom;
            CheckAllConnector = checkAllConnector;
            ShowElementsInRange = showElementsInRange;
        }
    }
}
