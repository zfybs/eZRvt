using DllActivator;

namespace DllActivator
{

    /// <summary>
    /// 用于在IExternalCommand.Execute方法中，将整个项目的所有dll加载到进程中。
    /// 以避免出现在后面出现无法加载文件或者程序集的问题。
    /// 此接口是专门为AddinManager在调试时设计的，在最终软件发布之前，此接口以及所有与之相关的类以及调用方法都可以删除。
    /// </summary>
    /// <remarks>在每一次调用Execute方法的开关，都可以用如下代码来将对应项目的所有引用激活。
    /// DllActivator.DllActivator_Projects dat = new DllActivator.DllActivator_Projects();
    /// dat.ActivateReferences();
    /// </remarks>
    public interface IDllActivator_RevitStd
    {
        /// <summary> 激活本DLL所引用的那些DLLs </summary>
        void ActivateReferences();
    }

    public class DllActivator_RevitStd : IDllActivator_RevitStd
    {
        /// <summary>
        /// 激活本DLL所引用的那些DLLs
        /// </summary>
        public void ActivateReferences()
        {
            //
            var dat = new DllActivator_std();
            dat.ActivateReferences();
            //
        }
    }
}