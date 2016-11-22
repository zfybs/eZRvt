Imports System.Xml
Imports System.IO
Imports RefreshRevitReference.RefProject
Public Class Form_ChangeReference

#Region "  ---  Properties"


    Private F_SolutionFile As String
    ''' <summary> .sln 文件的路径</summary>
    Public Property SolutionFile As String
        Get
            Return F_SolutionFile
        End Get
        Set(value As String)
            blnSolutionOpened = False
            F_SolutionFile = value
        End Set
    End Property

    ''' <summary> 要重新引用的Revit径</summary>
    Public Property NewRefPath As String

    Private F_Projects As New List(Of RefProject)
    Private WriteOnly Property Projects As List(Of RefProject)
        Set(value As List(Of RefProject))
            '
            With TextBox1
                .Clear()
                '
                Dim index As Integer
                index = (New FileInfo(SolutionFile)).Directory.FullName.Length

                '
                Dim c As Integer = value.Count
                Dim p As RefProject
                Dim ProjectFile As String
                For i = 0 To c - 1
                    p = value.Item(i)
                    ProjectFile = p.ProjectFilePath.Substring(index)
                    .AppendText("---------------------------  " & i + 1 & " ----------------------------" & vbCrLf &
                              ProjectFile & vbCrLf &
                            "RevitAPI.dll: " & p.RevitDllPath & vbCrLf & vbCrLf)
                Next
            End With
            ' 一定要最后再进行刷新
            F_Projects = value
        End Set
    End Property

#End Region

#Region "  ---  Fields"

    Private blnProjectsSaved As Boolean = True
    '
    Private F_blnSolutionOpened As Boolean
    Private Property blnSolutionOpened As Boolean
        Get
            Return Me.F_blnSolutionOpened
        End Get
        Set(value As Boolean)
            Me.F_blnSolutionOpened = value
            If value = True Then
                btnOpenSolution.Enabled = False
            Else
                btnOpenSolution.Enabled = True
            End If
        End Set
    End Property

    ''' <summary> 项目文件中引用的Revit的文件夹路径 </summary>
    Private OriginalRefPath As String

#End Region


    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.AllowDrop = True

        ' 绑定数据
        TextBox_SolutionPath.DataBindings.Add("Text", Me, "SolutionFile", False, DataSourceUpdateMode.OnPropertyChanged)
        TextBox_RevitPath.DataBindings.Add("Text", Me, "NewRefPath", False, DataSourceUpdateMode.OnPropertyChanged)
        '
        '设置初始路径
        TextBox_RevitPath.Text = My.Settings.LastDllPath
        TextBox_SolutionPath.Text = My.Settings.LastSolutionPath
    End Sub
    ''
    Private Function OpenSolution() As Boolean Handles btnOpenSolution.Click
        Dim blnSucceed As Boolean = True
        If Not blnSolutionOpened Then
            Dim pros As New List(Of RefProject)
            Dim strPro As String = Nothing
            Dim files As FileInfo()
            Dim ProsDir As New DirectoryInfo(Path.Combine(New FileInfo(SolutionFile).Directory.FullName, "Projects"))
            If Directory.Exists(ProsDir.FullName) Then
                For Each d As DirectoryInfo In ProsDir.GetDirectories
                    files = d.GetFiles("*.vbproj")
                    If files.Length > 0 Then
                        strPro = files(0).FullName
                    Else
                        files = d.GetFiles("*.csproj")
                        If files.Length > 0 Then
                            strPro = files(0).FullName
                        End If
                    End If
                    '
                    If strPro IsNot Nothing Then
                        Dim doc As New XmlDocument
                        doc.Load(strPro)
                        Dim p As New RefProject(doc, strPro)
                        pros.Add(p)
                        '
                        p.GetRevitPath()
                    End If
                Next
                blnSolutionOpened = True
                ' blnProjectsSaved = False  ' 在打开solution的操作中，并没有进行写入操作，所以并不需要保存
            Else
                MessageBox.Show("未找到对应的项目文件夹！", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
                blnSucceed = False
            End If
            Me.Projects = pros
        End If
        Return blnSucceed
    End Function

    Private Sub SaveAll()
        For Each p In F_Projects
            p.Save()
        Next
        Me.blnProjectsSaved = True
    End Sub

#Region "  ---  事件"

    ' 修改引用路径
    Private Sub btnOk_Click(sender As Object, e As EventArgs) Handles btnOk.Click
        '
        Dim blnSucceed As Boolean = True
        If Not blnSolutionOpened Then
            blnSucceed = OpenSolution()
        End If

        '
        For Each pro As RefProject In F_Projects
            pro.ChangeRef(Me.TextBox_RevitPath.Text)
        Next
        blnProjectsSaved = False
        '
        If blnSucceed Then
            Dim res As DialogResult = MessageBox.Show("引用路径修改成功，是否保存？", "Congratulations!", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
            If res = Windows.Forms.DialogResult.OK Then
                SaveAll()
                Me.btnOpenSolution.Enabled = True
            End If
        End If
        '
        blnSolutionOpened = False
        My.Settings.LastDllPath = TextBox_RevitPath.Text
        My.Settings.LastSolutionPath = TextBox_SolutionPath.Text
    End Sub

    ' 选择文件夹路径或者选择项目文件路径
    Private Sub btnChooseDirectory_Click(sender As Object, e As EventArgs) Handles btnChooseDirectory.Click
        With Me.FolderBrowserDialog1
            .ShowNewFolderButton = True
            .Description = "Revit.exe 所在的文件夹。"
            '
            If .ShowDialog() = Windows.Forms.DialogResult.OK Then
                TextBox_RevitPath.Text = .SelectedPath
            End If

        End With
    End Sub
    Private Sub btnChooseFile_Click(sender As Object, e As EventArgs) Handles btnChooseFile.Click
        With Me.OpenFileDialog1
            .Multiselect = False
            .Title = "Visual Studio 解决方案"
            .Filter = "项目(*.sln)|*.sln|所有文件(*.*)|*.*"
            '
            If .ShowDialog() = Windows.Forms.DialogResult.OK Then
                TextBox_SolutionPath.Text = .FileName
            End If

        End With
    End Sub

    ' 关闭窗口
    Private Sub Form_ChangeReference_FormClosing(sender As Object, e As FormClosingEventArgs) Handles Me.FormClosing
        If Not blnProjectsSaved Then
            Dim res As DialogResult = MessageBox.Show("是否保存对文档的修改？", "", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1)
            If res = Windows.Forms.DialogResult.OK Then
                SaveAll()
            End If
        End If
        '
        Me.btnOpenSolution.Enabled = True
    End Sub

#Region "  ---  拖拽项目文件"

    '拖拽操作
    Private Sub APPLICATION_MAINFORM_DragEnter(sender As Object, e As DragEventArgs) Handles Me.DragEnter
        ' See if the data includes text.
        If e.Data.GetDataPresent(DataFormats.FileDrop) Then
            ' There is text. Allow copy.
            e.Effect = DragDropEffects.Copy
        Else
            ' There is no text. Prohibit drop.
            e.Effect = DragDropEffects.None
        End If

    End Sub

    Private Sub APPLICATION_MAINFORM_DragDrop(sender As Object, e As DragEventArgs) Handles Me.DragDrop
        Dim FileDrop As String() = e.Data.GetData(DataFormats.FileDrop)
        ' DoSomething with the Files or Directories that are droped in.
        Dim filepath As String = FileDrop(0)
        Dim ext As String = Path.GetExtension(filepath)

        ' If String.Compare(ext, ".vbproj", True) = 0 OrElse (String.Compare(ext, ".csproj", True) = 0) Then
        If String.Compare(ext, ".sln", True) = 0 Then
            If String.Compare(filepath, Me.TextBox_SolutionPath.Text, True) <> 0 Then
                Me.TextBox_SolutionPath.Text = filepath
            End If

        Else
            MessageBox.Show("所选择的文件不是Visual Studio的 .sln 文件。", _
                             "Error", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If
    End Sub

#End Region

#End Region

End Class
