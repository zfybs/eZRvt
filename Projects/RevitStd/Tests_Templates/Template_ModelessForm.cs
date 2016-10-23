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

using Autodesk.Revit.UI;
using Autodesk.Revit.DB;
using Forms = System.Windows.Forms;
using Autodesk.Revit.UI.Selection;

namespace RevitStd.Tests_Templates
{
	
	/// <summary>
	/// 无模态窗口的模板
	/// 此窗口可以直接通过Form.Show来进行调用
	/// </summary>
	/// <remarks></remarks>
	public partial class Template_ModelessForm : IExternalEventHandler
	{
		
#region    ---   Declarations
		
#region    ---   Types
		
		/// <summary>
		/// 每一个外部事件调用时所提出的需求，为了在Execute方法中充分获取窗口的需求，
		/// 所以将调用外部事件的窗口控件以及对应的触发事件参数也传入Execute方法中。
		/// </summary>
		/// <remarks></remarks>
		private class RequestParameter
		{
			
			private object F_sender;
			/// <summary> 引发Form事件控件对象 </summary>
public dynamic sender
			{
				get
				{
					return F_sender;
				}
			}
			
			private EventArgs F_e;
			/// <summary> Form中的事件所对应的事件参数 </summary>
public EventArgs e
			{
				get
				{
					return F_e;
				}
			}
			
			private Request F_Id;
			/// <summary> 具体的需求 </summary>
public Request Id
			{
				get
				{
					return F_Id;
				}
			}
			
			
			/// <summary>
			/// 定义事件需求与窗口中引发此事件的控件对象及对应的事件参数
			/// </summary>
			/// <param name="RequestId">具体的需求</param>
			/// <param name="e">Form中的事件所对应的事件参数</param>
			/// <param name="sender">引发Form事件控件对象</param>
			/// <remarks></remarks>
			public RequestParameter(Request RequestId, EventArgs e = null, object sender = null)
			{
				RequestParameter with_1 = this;
				with_1.F_sender = sender;
				with_1.F_e = e;
				with_1.F_Id = RequestId;
			}
		}
		
		/// <summary>
		/// ModelessForm的操作需求，用来从窗口向IExternalEventHandler对象传递需求。
		/// </summary>
		/// <remarks></remarks>
		private enum Request
		{
			/// <summary>
			/// 与Revit用户界面进行交互。弥补了Form.ShowDialog不能进行Selection.PickObjects等操作的缺陷。
			/// </summary>
			/// <remarks></remarks>
			Pick,
			/// <summary>
			/// 开启Revit事务以修改Revit文档。弥补了Form.Show后开启事务时给出报错：
			/// “Starting a transaction from an external application running outside of APIcontext is not allowed.”的问题。
			/// </summary>
			/// <remarks></remarks>
			Delete
		}
		
#endregion
		
#region    ---   Fields
		
		/// <summary>用来触发外部事件（通过其Raise方法） </summary>
		/// <remarks>ExEvent属性是必须有的，它用来执行Raise方法以触发事件。</remarks>
		private ExternalEvent ExEvent;
		
		/// <summary> Execute方法所要执行的需求 </summary>
		/// <remarks>在Form中要执行某一个操作时，先将对应的操作需求信息赋值为一个RequestId枚举值，然后再执行ExternalEvent.Raise()方法。
		/// 然后Revit会在会在下个闲置时间（idling time cycle）到来时调用IExternalEventHandler.Excute方法，在这个Execute方法中，
		/// 再通过RequestId来提取对应的操作需求，</remarks>
		private RequestParameter RequestPara;
		
		private Document Doc;
		
#endregion
		
#region    ---   Properties
		
#endregion
		
#endregion
		
#region    ---   构造函数与窗口的打开关闭
		
		public Template_ModelessForm(Document Doc)
		{
			// This call is required by the designer.
			InitializeComponent();
			// Add any initialization after the InitializeComponent() call.
			//' ----------------------
			
			this.StartPosition = FormStartPosition.CenterScreen;
			this.Doc = Doc;
			
			//' ------ 将所有的初始化工作完成后，执行外部事件的绑定 ----------------
			// 新建一个外部事件实例
			this.ExEvent = ExternalEvent.Create(this);
		}
		
		protected override void OnClosed(EventArgs e)
		{
			// 保存的实例需要进行释放
			this.ExEvent.Dispose();
			this.ExEvent = null;
			//
			base.OnClosed(e);
		}
		
		public string GetName()
		{
			return "Revit External Event & ModelessForm";
		}
		
#endregion
		
#region    ---   界面效果与事件响应
		
		/// <summary> 在Revit执行相关操作时，禁用窗口中的控件 </summary>
		private void DozeOff()
		{
			foreach (Forms.Control c in this.Controls)
			{
				c.Enabled = false;
			}
		}
		
		/// <summary> 在外部事件RequestHandler中的Execute方法执行完成后，用来激活窗口中的控件 </summary>
		private void WarmUp()
		{
			foreach (Forms.Control c in this.Controls)
			{
				c.Enabled = true;
			}
		}
		
#endregion
		
#region    ---   执行操作 ExternalEvent.Raise 与 IExternalEventHandler.Execute
		
		public void Button1_Click(object sender, EventArgs e)
		{
			this.RequestPara = new RequestParameter(Request.Pick, e, sender);
			this.ExEvent.Raise();
			this.DozeOff();
		}
		
		public void Button2_Click(object sender, EventArgs e)
		{
			this.RequestPara = new RequestParameter(Request.Delete, e, sender);
			this.ExEvent.Raise();
			this.DozeOff();
		}
		
		//'为每一项操作执行具体的实现
		/// <summary>
		/// 在执行ExternalEvent.Raise()方法之前，请先将操作需求信息赋值给其RequestHandler对象的RequestId属性。
		/// 当ExternalEvent.Raise后，Revit会在下个闲置时间（idling time cycle）到来时调用IExternalEventHandler.Execute方法的实现。
		/// </summary>
		/// <param name="app">此属性由Revit自动提供，其值不是Nothing，而是一个真实的UIApplication对象</param>
		/// <remarks>由于在通过外部程序所引发的操作中，如果出现异常，Revit并不会给出任何提示或者报错，
		/// 而是直接退出函数。所以要将整个操作放在一个Try代码块中，以处理可能出现的任何报错。</remarks>
		public void Execute(UIApplication app)
		{
			try // 由于在通过外部程序所引发的操作中，如果出现异常，Revit并不会给出任何提示或者报错，而是直接退出函数。所以这里要将整个操作放在一个Try代码块中，以处理可能出现的任何报错。
			{
				
				UIDocument uiDoc = new UIDocument(Doc);
				
				// 开始执行具体的操作
				switch (RequestPara.Id) // 判断具体要干什么
				{
					case Request.Pick:
						
						var a = uiDoc.Selection.PickObject(ObjectType.Element);
						MessageBox.Show(System.Convert.ToString(a.ElementId.IntegerValue.ToString()));
						break;
						
					case Request.Delete:
						
						ICollection<ElementId> ids = uiDoc.Selection.GetElementIds();
						if (ids.Count == 0)
						{
							MessageBox.Show("请先选择一个元素");
						}
						else
						{
							ElementId id = ids.First();
							
							using (Transaction tr = new Transaction(Doc, "删除对象"))
							{
								if (tr.Start() == TransactionStatus.Started)
								{
									Doc.Delete(id);
									tr.Commit();
								}
							}
							
						}
						break;
					default:
						break;
				}
			}
			catch (Exception ex)
			{
				MessageBox.Show("出错" + "\r\n" + ex.Message + "\r\n" + ex.TargetSite.Name,
					"外部事件执行出错", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
			finally
			{
				// 刷新Form，将Form中的Controls的Enable属性设置为True
				this.WarmUp();
			}
		}
		
#endregion
		
	}
}
