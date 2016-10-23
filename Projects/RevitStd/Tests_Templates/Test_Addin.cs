// VBConversions Note: VB project level imports
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
// End of VB project level imports

using Autodesk.Revit.DB;
using Autodesk.Revit.UI;


namespace RevitStd.Tests_Templates
{
	
	[Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]public class ExternalCommand : IExternalCommand
		{
		
		public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
		{
			//TaskDialog.Show("Revit", "ExternalCommand1")
			return Result.Succeeded;
		}
	}
	
	
	public class ExternalApplication : IExternalApplication
	{
		
		public Result OnShutdown(UIControlledApplication application)
		{
			//TaskDialog.Show("Revit", "ExternalApplication")
			return Result.Succeeded;
		}
		
		public Result OnStartup(UIControlledApplication application)
		{
			//Call NewRibbon(application)
			return Result.Succeeded;
		}
		
		/// <summary>
		/// 创建一个新的 Ribbon
		/// </summary>
		public void NewRibbon(UIControlledApplication application)
		{
			//添加一个新的Ribbon面板
			//Dim ribbonPanel As RibbonPanel = application.CreateRibbonPanel("NewRibbonPanel")
			
			// ''在新的Ribbon面板上添加一个按钮
			// ''点击这个按钮，前一个例子“HelloRevit”这个插件将被运行。
			//Dim pushButton As PushButton = ribbonPanel.AddItem(New PushButtonData("HelloRevit",
			//        "HelloRevit", "F:\Software\Revit\RevitDevelop\eZRevtiTools\eZrvt_ExApp\eZrvt_ExApp\bin\Debug\eZrvt_ExApp.dll", "eZrvt_ExApp.test.ExternalCommand"))
			
			//给按钮添加一个图片
			//Dim uriImage As Uri = New Uri("C:\Users\tt\Desktop\11.png")
			//Dim largeImage As BitmapImage = New BitmapImage(uriImage)
			//pushButton.LargeImage = largeImage
			
		}
		
	}
	
	public class ExternalDBApplication : IExternalDBApplication
	{
		
		
		public ExternalDBApplicationResult OnShutdown(Autodesk.Revit.ApplicationServices.ControlledApplication application)
		{
			//TaskDialog.Show("Revit", "ExternalDBApplication")
			return ExternalDBApplicationResult.Succeeded;
		}
		
		public ExternalDBApplicationResult OnStartup(Autodesk.Revit.ApplicationServices.ControlledApplication application)
		{
			//TaskDialog.Show("Revit", "ExternalDBApplication")
			return ExternalDBApplicationResult.Succeeded;
		}
		
	}
	
	
	
}

//三种不同类型的插件的 XML 调用方式：
//<?xml version="1.0" encoding="utf-8"?>
//<RevitAddIns>
//  <AddIn Type="Application">
//    <Name>ExternalApplication</Name>
//    <Assembly>F:\Software\Revit\RevitDevelop\eZrvt\eZrvt_ExApp\eZrvt_ExApp\bin\Debug\eZrvt_ExApp.dll</Assembly>
//    <ClientId>1a706721-ae9e-41ba-a783-cb4092401317</ClientId>
//    <FullClassName>eZrvt_ExApp.test.ExternalApplication</FullClassName>
//    <VendorId>ADSK</VendorId>
//    <VendorDescription>Autodesk, www.autodesk.com</VendorDescription>
//  </AddIn>
//  <AddIn Type="Command">
//    <Assembly>F:\Software\Revit\RevitDevelop\eZrvt\eZrvt_ExApp\eZrvt_ExApp\bin\Debug\eZrvt_ExApp.dll</Assembly>
//    <ClientId>089df578-a98e-492c-a7e1-299f15572a42</ClientId>
//    <FullClassName>eZrvt_ExApp.test.ExternalCommand</FullClassName>
//    <Text>ExternalCommand</Text>
//    <Description>""</Description>
//    <VisibilityMode>AlwaysVisible</VisibilityMode>
//    <VendorId>ADSK</VendorId>
//    <VendorDescription>Autodesk, www.autodesk.com</VendorDescription>
//  </AddIn>
//
//   <AddIn Type="DBApplication">
//      <Assembly>F:\Software\Revit\RevitDevelop\eZrvt\eZrvt_ExApp\eZrvt_ExApp\bin\Debug\eZrvt_ExApp.dll</Assembly>
//      <ClientId>bae83b6a-36e4-47e5-99b3-666a1060716b</ClientId>
//      <FullClassName>eZrvt_ExApp.test.ExternalDBApplication</FullClassName>
//      <Name>Revit LookupA</Name>
//      <VendorId>ADSK</VendorId>
//      <VendorDescription>Autodesk, www.autodesk.com</VendorDescription>
//   </AddIn>
//
//</RevitAddIns>
