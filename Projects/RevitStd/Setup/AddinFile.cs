using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RevitStd
{

    //<?xml version="1.0" encoding="utf-8"?>
    //<RevitAddIns>
    //<AddIn Type="Application">
    //  <Name>ExternalApplication</Name>
    //  <Assembly>C:\ProgramData\Autodesk\Revit\Addins\2016\ConduitLayout\ConduitLayout.dll</Assembly>
    //  <ClientId>19A9FC15-6D44-4117-8A9D-3694C9654698</ClientId>
    //  <FullClassName>ConduitLayout.ExternalApplication</FullClassName>
    //  <VendorId>ADSK</VendorId>
    //  <VendorDescription>Autodesk, www.autodesk.com</VendorDescription>
    //</AddIn>
    //</RevitAddIns>

    /// <summary>
    /// Revit 插件的 .Addin 文件
    /// </summary>
    public class AddinFile
    {
        private string _addinFilePath;

        private XmlDocument _xmlDoc;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="filePath"></param>
        public AddinFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                string ext = Path.GetExtension(filePath);
                if (ext != null && ext.Equals(".addin", StringComparison.OrdinalIgnoreCase))
                {
                    _addinFilePath = filePath;
                    _xmlDoc = new XmlDocument();
                    _xmlDoc.Load(filePath);
                    //
                    return;
                }
            }
            throw new ArgumentException("指定的路径不是有效的 Revit 的 Addin 文件");
        }

        /// <summary>
        /// 将 Addin 文件所记录的AddIn中，指定 clientId 的插件的 Assembly 字段的值修改为某指定的dll的绝对地址。
        /// </summary>
        /// <param name="absolutePath"></param>
        /// <param name="clientId"> 表示 GUID 值的字段串，指将匹配 ClientId 的addin项的Assembly字段进行修改；
        /// 如果不指定此参数，则为此Addin文件中所有的Assembly字段的值都修改为此指定的 absolutePath。 </param>
        public void ExtendAssemblyToAbsolutePath(string absolutePath, string clientId = null)
        {
            if (clientId == null)
            {
                var assemblyNodes = _xmlDoc.SelectNodes("/RevitAddIns/AddIn/Assembly");
                if (assemblyNodes == null) return;

                foreach (XmlNode assemblyNode in assemblyNodes)
                {
                    assemblyNode.InnerText = absolutePath;
                }
            }
            else
            {
                var assemblyNodes = _xmlDoc.SelectNodes("/RevitAddIns/AddIn/Assembly");
                if (assemblyNodes == null) return;

                foreach (XmlNode assemblyNode in assemblyNodes)
                {
                    XmlNode clientIdNode = assemblyNode.ParentNode.SelectSingleNode("ClientId");
                    if (clientIdNode != null && clientIdNode.InnerText.Equals(clientId, StringComparison.OrdinalIgnoreCase))
                    {
                        assemblyNode.InnerText = absolutePath;
                    }
                }
            }
        }

        /// <summary>
        /// 将内存中的 XMLDocument 文件保存到硬盘文件中。
        /// </summary>
        /// <param name="destinationPath"></param>
        public void SaveTo(string destinationPath)
        {
            _xmlDoc.Save(destinationPath);
        }
    }
}
