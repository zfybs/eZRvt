using System;
using System.Runtime.CompilerServices;

namespace RevitStd.Tests_Templates
{
    /// <summary>
    /// 一个基本的类的模板
    /// </summary>
    public class classTemplate

    {
        /// <summary>
        /// 字段模板
        /// </summary>
        private int Tfiled;
        /// <summary>
        /// 常数模板
        /// </summary>
        public const int Tconstant = 1;

        /// <summary>
        /// 事件模板
        /// </summary>
        public event EventHandler Tevent;

        /// <summary>
        /// 构造函数
        /// </summary>
        public classTemplate()
        {
            // throw new System.NotImplementedException();
        }

        /// <summary>
        /// 析构函数 Destructor
        /// </summary>
        /// <remarks>
        /// 析构函数(destructor) 与构造函数相反，当对象脱离其作用域时（例如对象所在的函数已调用完毕），系统自动执行析构函数。
        /// 析构函数往往用来做“清理善后” 的工作（例如在建立对象时用new开辟了一片内存空间，应在退出前在析构函数中用delete释放）。
        ///  程序员不能控制解构器何时将被执行因为这是由垃圾收集器决定的。解构器也在程序退出时被调用。
        /// </remarks>
        ~classTemplate()
        {
            // throw new System.NotImplementedException();
        }

        /// <summary>
        /// 可读可写属性模板
        /// </summary>
        /// <remarks>
        /// 也可以直接简写为：public int Tproperty{get;set;} 
        /// 当取消属性中的get或者set块的时候，此属性就成了只写或者只读属性。
        /// </remarks>
        public int Tproperty
        {
            get
            {
                throw new System.NotImplementedException();
            }
            set
            {
            }
        }

        /// <summary>
        /// 只写属性
        /// </summary>
        public int TwriteOnlyProperty
        {
            set { }
        }
        
        /// <summary>
        /// 方法模板
        /// </summary>
        /// <param name="paraParam">参考</param>
        public void Tmethod(params string[] paraParam)
        {
            // throw new System.NotImplementedException();
            foreach (var s in paraParam)
            {
                Console.WriteLine(s);
            }
            Console.Read();
        }

        /// <summary>
        /// 函数模板
        /// </summary>
        /// <param name="paraValue">一般的参数，默认为值传递</param>
        /// <param name="paraRef">地址传递，在调用时要在输入实参前面加上 ref 。</param>
        /// <param name="paraOut">用来输出的参数，在调用时要在输入实参前面加上 out 。</param>
        public short Tfunction(string paraValue, ref string paraRef, out string paraOut)
        {
            paraOut = 20.ToString();
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// 主程序入口
        /// </summary>
        static void Main(string[] args)
        {

        }
    }
}