Imports System.IO
Imports System.Xml


Public Class RefProject

#Region "  ---  Fields"

    ''' <summary> 项目文件中引用的RevitAPI.dll的节点 </summary>
    Private RevitAPI As XmlElement

    ''' <summary> 项目文件中引用的RevitAPIUI.dll的节点 </summary>
    Private RevitAPIUI As XmlElement

    ''' <summary> 项目文件中引用的RevitAPIUI.dll的的文件夹，
    ''' 虽然一台电脑中只有唯一的一个路径，但是可能proj文件中引用出错，而引用了多个不同的路径。 </summary>
    Public RevitDllPath As String
    '
    Public Property ProjectFilePath As String
    '
    Private F_xmlDoc As XmlDocument

    Private Property xmlDoc As XmlDocument
        Get
            Return F_xmlDoc
        End Get
        Set(value As XmlDocument)
            Me.F_xmlDoc = value
            If value Is Nothing Then
                RevitAPI = Nothing
                RevitAPIUI = Nothing
            End If
        End Set
    End Property

#End Region

    Public Sub New(ByVal doc As XmlDocument, FilePath As String)
        With Me
            .xmlDoc = doc
            .ProjectFilePath = FilePath
        End With
    End Sub

    ''' <summary>
    ''' 利用GetElementsByTagName方法。
    ''' 打开项目文件，并从项目文件的xml文件中，提取出对应的RevitAPI.dll与RevitAPIUI.dll所在的文件夹的路径
    ''' </summary>
    ''' <returns>此项目文件所引用的Revit.dll所在的文件夹路径</returns>
    ''' <remarks></remarks>
    Public Function GetRevitPath() As String
        Dim strRevitDllPath As String = Nothing
        With xmlDoc
            Dim root As XmlElement = .DocumentElement
            Dim ItemGroups As XmlNodeList = root.GetElementsByTagName("ItemGroup")
            For Each el As XmlElement In ItemGroups
                Dim Refs As XmlNodeList = el.GetElementsByTagName("Reference")
                If (Refs IsNot Nothing) AndAlso Refs.Count > 0 Then
                    For Each ref As XmlElement In Refs

                        Dim HintPath As XmlElement = ref.GetElementsByTagName("HintPath").Item(0)
                        ' 终于找到了
                        If HintPath IsNot Nothing Then
                            Dim strPath As String = HintPath.InnerText
                            If strPath.Contains("RevitAPI.dll") Then
                                RevitAPI = HintPath
                            End If
                            If strPath.Contains("RevitAPIUI.dll") Then
                                RevitAPIUI = HintPath
                            End If
                        End If
                    Next
                End If
            Next
        End With

        If Me.RevitAPI IsNot Nothing Then
            strRevitDllPath = Path.GetDirectoryName(RevitAPI.InnerText)
            Me.RevitDllPath = strRevitDllPath
        End If
        Return strRevitDllPath
    End Function

    ''' <summary>
    ''' 修改引用的路径
    ''' </summary>
    ''' <param name="NewRefPath"></param>
    ''' <remarks></remarks>
    Public Sub ChangeRef(ByVal NewRefPath As String)
        '
        If RevitAPI IsNot Nothing Then
            RevitAPI.InnerText = Path.Combine(NewRefPath, "RevitAPI.dll")
        End If
        If RevitAPIUI IsNot Nothing Then
            RevitAPIUI.InnerText = Path.Combine(NewRefPath, "RevitAPIUI.dll")
        End If
        '
    End Sub

    Public Sub Save()
        xmlDoc.Save(ProjectFilePath)
    End Sub
End Class
