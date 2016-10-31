using System.IO;
using System.Reflection;

namespace eZRvt.GlobalSettings
{
    /// <summary> 整个项目中所有的相关文件或文件夹的绝对路径 </summary>
    internal static class ProjectPath
    {
        private static readonly string DllName = "eZRvt.dll";

        /// <summary> Application的Dll所对应的路径，也就是“eZRvt.dll”文件的路径。 </summary>
        public static string DllFile
        {
            get
            {
                // 在用 addinManager 调试时，每次的 dll 的文件路径会不一样，对应为 AddinManager 复制出的那个新 dll 的路径。
                string path = @"F:\ProgrammingCases\GitHubProjects\eZRvt\bin\" + DllName;
                return File.Exists(path) ? path : Assembly.GetExecutingAssembly().FullName;
            }
        }

        /// <summary> Application的Dll所对应的路径，也就是“bin”文件夹的目录。 </summary>
        public static readonly string Dlls = new FileInfo(DllFile).DirectoryName;

        /// <summary> Application的Dll所对应的路径，也就是“bin”文件夹的目录。 </summary>
        public static readonly string Solution = new DirectoryInfo(Dlls).Parent.FullName;
        
        /// <summary> “Resources”文件夹的目录。 </summary>
        public static readonly string Resources = Path.Combine(Solution, @"Resources");
        
        /// <summary> “Resources/Data”文件夹的目录。 </summary>
        public static readonly string Data = Path.Combine(Resources, @"Data");

        /// <summary> “Resources/Data”文件夹的目录。 </summary>
        public static readonly string icons = Path.Combine(Resources, @"icons");

        /// <summary> 共享参数文本文件的绝对路径 </summary>
        public static string SharedParameters = Path.Combine(Data, @"GlobalSharedParameters.txt");
    }
}