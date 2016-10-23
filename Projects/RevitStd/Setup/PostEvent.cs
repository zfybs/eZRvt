using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace RevitStd
{
    static class PostEvent
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            //Application.Run(new Form1());


            string assPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            DirectoryInfo programmDir = new FileInfo(assPath).Directory;

            string addinFilePath = Path.Combine(programmDir.FullName, "FaceWall.addin");
            string externalApplicationDll = Path.Combine(programmDir.GetDirectories("bin")[0].FullName, "FaceWall.dll");
            string externalApplicationGUID = "dbb30c8f-65c9-4b9b-8e77-1fd252dc377b";
            string revitAddinPath = @"C:\ProgramData\Autodesk\Revit\Addins\2016";


            // 修改 addin 文件中的内容，并将其复制到Revit插件目录中
            ChangeAndMoveAddinFile(addinFilePath, externalApplicationDll, revitAddinPath, externalApplicationGUID);

            // 自杀
            KillFileByBat(assPath);
        }

        /// <summary>
        /// 修改addin文件中的Assembly字段为对应插件的dll的绝对路径，再将此addin文件添加到Revit的插件安装目录中。
        /// </summary>
        /// <param name="addinFilePath">安装包中的插件注册的 .addin 文件的绝对路径</param>
        /// <param name="externalApplicationDllPath">安装包中插件的 .dll 文件的绝对路径，用来作为修改后的 .addin文件中的Assembly字段的值</param>
        /// <param name="revitAddinPath"> Revit的插件目录，比如 "C:\ProgramData\Autodesk\Revit\Addins\2016" </param>
        /// <param name="externalApplicationGuid">对 .addin 文件中的 匹配的 ClientId 所对应的子插件进行路径的修改。
        /// 如果不指定此参数，则会将addin文件中所有的子插件的Assembly字段都修改为这一个绝对路径。</param>
        private static void ChangeAndMoveAddinFile(string addinFilePath, string externalApplicationDllPath, string revitAddinPath, string externalApplicationGuid = null)
        {

            // 将 addin 文件中的Assembly字段修改为对应的绝对路径
            AddinFile addinFile = new AddinFile(addinFilePath);
            addinFile.ExtendAssemblyToAbsolutePath(externalApplicationDllPath, externalApplicationGuid);

            // 将修改完后的 addin 文件保存到Revit中的插件目录中
            string addinName = Path.GetFileName(addinFilePath);
            addinFile.SaveTo(Path.Combine(revitAddinPath, addinName));
        }

        [DllImport("shell32.dll")]
        public static extern IntPtr ShellExecute(IntPtr hwnd, string lpOperation, string lpFile, string lpParameters, string lpDirectory, int nShowCmd);

        private static void KillFileByBat(string executableFile)
        {
            FileInfo assemblyFile = new FileInfo(executableFile);

            var directory = assemblyFile.Directory;
            string tm = DateTime.Now.ToString("yyyyMMddhhmmss");
            string batFileName = "Kill-" + tm + ".bat";
            string batFilePath = Path.Combine(directory.FullName, batFileName);

            FileStream fs = new FileStream(batFilePath, mode: FileMode.CreateNew);
            StreamWriter sw = new StreamWriter(fs, encoding: Encoding.ASCII);
            sw.WriteLine("echo off");
            sw.WriteLine("del " + assemblyFile.Name);
            sw.WriteLine("del " + batFileName);
            sw.Close();
            fs.Close();

            // 执行 .bat 文件，以删除当前运行的.exe。
            ShellExecute(IntPtr.Zero, "open", batFileName, "", "", 0);
        }
    }
}
