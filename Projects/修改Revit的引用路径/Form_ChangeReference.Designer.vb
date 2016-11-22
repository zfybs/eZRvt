<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form_ChangeReference
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(Form_ChangeReference))
        Me.Label1 = New System.Windows.Forms.Label()
        Me.TextBox_RevitPath = New System.Windows.Forms.TextBox()
        Me.btnChooseDirectory = New System.Windows.Forms.Button()
        Me.btnOk = New System.Windows.Forms.Button()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.FolderBrowserDialog1 = New System.Windows.Forms.FolderBrowserDialog()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.TextBox_SolutionPath = New System.Windows.Forms.TextBox()
        Me.btnChooseFile = New System.Windows.Forms.Button()
        Me.OpenFileDialog1 = New System.Windows.Forms.OpenFileDialog()
        Me.btnOpenSolution = New System.Windows.Forms.Button()
        Me.TextBox1 = New System.Windows.Forms.TextBox()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.SuspendLayout()
        '
        'Label1
        '
        Me.Label1.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.Label1.AutoSize = True
        Me.Label1.Location = New System.Drawing.Point(14, 333)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(125, 12)
        Me.Label1.TabIndex = 0
        Me.Label1.Text = "Revit.dll 文件夹路径"
        '
        'TextBox_RevitPath
        '
        Me.TextBox_RevitPath.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.TextBox_RevitPath.Location = New System.Drawing.Point(14, 349)
        Me.TextBox_RevitPath.Name = "TextBox_RevitPath"
        Me.TextBox_RevitPath.Size = New System.Drawing.Size(514, 21)
        Me.TextBox_RevitPath.TabIndex = 1
        '
        'btnChooseDirectory
        '
        Me.btnChooseDirectory.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnChooseDirectory.Location = New System.Drawing.Point(532, 349)
        Me.btnChooseDirectory.Name = "btnChooseDirectory"
        Me.btnChooseDirectory.Size = New System.Drawing.Size(75, 23)
        Me.btnChooseDirectory.TabIndex = 2
        Me.btnChooseDirectory.Text = "选择"
        Me.btnChooseDirectory.UseVisualStyleBackColor = True
        '
        'btnOk
        '
        Me.btnOk.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.btnOk.Location = New System.Drawing.Point(532, 384)
        Me.btnOk.Name = "btnOk"
        Me.btnOk.Size = New System.Drawing.Size(75, 23)
        Me.btnOk.TabIndex = 3
        Me.btnOk.Text = "修改"
        Me.btnOk.UseVisualStyleBackColor = True
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Location = New System.Drawing.Point(14, 71)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(179, 12)
        Me.Label2.TabIndex = 4
        Me.Label2.Text = "项目文件及引用的Revit.dll路径"
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Location = New System.Drawing.Point(12, 9)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(59, 12)
        Me.Label3.TabIndex = 0
        Me.Label3.Text = ".sln 文件"
        '
        'TextBox_SolutionPath
        '
        Me.TextBox_SolutionPath.Location = New System.Drawing.Point(12, 25)
        Me.TextBox_SolutionPath.Name = "TextBox_SolutionPath"
        Me.TextBox_SolutionPath.Size = New System.Drawing.Size(514, 21)
        Me.TextBox_SolutionPath.TabIndex = 1
        '
        'btnChooseFile
        '
        Me.btnChooseFile.Location = New System.Drawing.Point(532, 25)
        Me.btnChooseFile.Name = "btnChooseFile"
        Me.btnChooseFile.Size = New System.Drawing.Size(75, 23)
        Me.btnChooseFile.TabIndex = 2
        Me.btnChooseFile.Text = "选择"
        Me.btnChooseFile.UseVisualStyleBackColor = True
        '
        'OpenFileDialog1
        '
        Me.OpenFileDialog1.FileName = "OpenFileDialog1"
        '
        'btnOpenSolution
        '
        Me.btnOpenSolution.Location = New System.Drawing.Point(532, 66)
        Me.btnOpenSolution.Name = "btnOpenSolution"
        Me.btnOpenSolution.Size = New System.Drawing.Size(75, 23)
        Me.btnOpenSolution.TabIndex = 6
        Me.btnOpenSolution.Text = "打开"
        Me.ToolTip1.SetToolTip(Me.btnOpenSolution, "打开此.sln文件所在的文件夹中所有的Project，并搜索其中的VB或C#项目文件")
        Me.btnOpenSolution.UseVisualStyleBackColor = True

        '
        'TextBox1
        '
        Me.TextBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TextBox1.Location = New System.Drawing.Point(16, 95)
        Me.TextBox1.Multiline = True
        Me.TextBox1.Name = "TextBox1"
        Me.TextBox1.ScrollBars = System.Windows.Forms.ScrollBars.Both
        Me.TextBox1.Size = New System.Drawing.Size(512, 212)
        Me.TextBox1.TabIndex = 8
        Me.TextBox1.WordWrap = False
        '
        'Form_ChangeReference
        '
        Me.AcceptButton = Me.btnOk
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 12.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(619, 419)
        Me.Controls.Add(Me.TextBox1)
        Me.Controls.Add(Me.btnOpenSolution)
        Me.Controls.Add(Me.Label2)
        Me.Controls.Add(Me.btnOk)
        Me.Controls.Add(Me.btnChooseFile)
        Me.Controls.Add(Me.btnChooseDirectory)
        Me.Controls.Add(Me.TextBox_SolutionPath)
        Me.Controls.Add(Me.TextBox_RevitPath)
        Me.Controls.Add(Me.Label3)
        Me.Controls.Add(Me.Label1)
        Me.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.Name = "Form_ChangeReference"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "修改引用"
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label1 As System.Windows.Forms.Label
    Friend WithEvents TextBox_RevitPath As System.Windows.Forms.TextBox
    Friend WithEvents btnChooseDirectory As System.Windows.Forms.Button
    Friend WithEvents btnOk As System.Windows.Forms.Button
    Friend WithEvents Label2 As System.Windows.Forms.Label
    Friend WithEvents FolderBrowserDialog1 As System.Windows.Forms.FolderBrowserDialog
    Friend WithEvents Label3 As System.Windows.Forms.Label
    Friend WithEvents TextBox_SolutionPath As System.Windows.Forms.TextBox
    Friend WithEvents btnChooseFile As System.Windows.Forms.Button
    Friend WithEvents OpenFileDialog1 As System.Windows.Forms.OpenFileDialog
    Friend WithEvents btnOpenSolution As System.Windows.Forms.Button
    Friend WithEvents TextBox1 As System.Windows.Forms.TextBox
    Friend WithEvents ToolTip1 As System.Windows.Forms.ToolTip

End Class
