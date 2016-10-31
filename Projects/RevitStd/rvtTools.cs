using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Autodesk.Revit.DB;
using eZstd.Windows;

namespace RevitStd
{
    /// <summary> Revit中的一些常规性操作工具 </summary>
    public static class RvtTools
    {
        /// <summary> 撤消 Revit 的操作 </summary>
        public static void Undo()
        {
            UIntPtr ptr0 = new UIntPtr(0);
            // 第一步，先取消当前的所有操作
            // 在Revit UI界面中退出绘制，即按下ESCAPE键
            WindowsUtil.keybd_event((byte)27, (byte)0, 0, ptr0); // 按下 ESCAPE键
            WindowsUtil.keybd_event((byte)27, (byte)0, 0x2, ptr0); // 按键弹起

            // 第二步，按下 Ctrl + Z
            // 在Revit UI界面中退出绘制
            WindowsUtil.keybd_event((byte)17, (byte)0, 0, ptr0); // 按下 Control 键
            WindowsUtil.keybd_event((byte)90, (byte)0, 0, ptr0); // 按下 Z 键

            WindowsUtil.keybd_event((byte)90, (byte)0, 2, ptr0);
            WindowsUtil.keybd_event((byte)17, (byte)0, 2, ptr0); // 按键弹起

        }

        /// <summary> 将时间数值转换为字符串，其显示精度为分钟 </summary>
        public static string FormatTimeToMinite(DateTime time)
        {
            return time.ToString("yy/MM/dd hh:mm");
        }

    }
}
