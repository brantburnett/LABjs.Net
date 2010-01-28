Imports System.Collections.Specialized
Imports System.ComponentModel
Imports System.Reflection

Imports LABjs.Net.Internal

''' <summary>
''' Allows the combining of multiple scripts into a single HTTP request
''' </summary>
''' <remarks>
''' Requires ASP.Net AJAX be operational on your site.
''' Note that LABjs options on the individual script references are ignored, only the options on the LabScriptCombine are used.
''' </remarks>
<ParseChildren(True, "Scripts"), PersistChildren(False)> _
<AspNetHostingPermission(SecurityAction.LinkDemand, Level:=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level:=AspNetHostingPermissionLevel.Minimal)> _
Public Class LabScriptCombine
    Inherits LabScriptReferenceBase

#Region "Public Properties"

    Private _scripts As LabScriptReferenceCollection
    ''' <summary>
    ''' List of scripts to be combined
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks></remarks>
    <MergableProperty(False), PersistenceMode(PersistenceMode.InnerProperty), Category("Behavior"), Editor("System.Web.UI.Design.CollectionEditorBase, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", GetType(System.Drawing.Design.UITypeEditor)), DefaultValue(CStr(Nothing))> _
    Public ReadOnly Property Scripts() As LabScriptReferenceCollection
        Get
            If _scripts Is Nothing Then
                _scripts = New LabScriptReferenceCollection()
            End If
            Return _scripts
        End Get
    End Property

#End Region

#Region "Public Methods"

    Public Overrides Sub Render(ByVal writer As System.Text.StringBuilder, ByVal context As LabRenderContext)
        If Scripts.Count > 0 Then
            Dim list As New List(Of Pair(Of Assembly, List(Of String)))
            For Each script As LabScriptReference In Scripts
                Dim info As Pair(Of Assembly, String) = script.GetScriptInfo(context)

                Dim found As Boolean = False
                For Each pair As Pair(Of Assembly, List(Of String)) In list
                    If pair.First Is info.First Then
                        pair.Second.Add(info.Second)
                        found = True
                        Exit For
                    End If
                Next

                If Not found Then
                    Dim nameList As New List(Of String)()
                    nameList.Add(info.Second)
                    list.Add(New Pair(Of Assembly, List(Of String))(info.First, nameList))
                End If
            Next

            Dim options As NameValueCollection = GetOptions(context)

            If options.Count = 0 Then
                writer.Append(vbTab & ".script(""")
                writer.Append(LabHelper.GetCombinedScriptResourceUrl(list))
                writer.Append(""")")
            Else
                writer.Append(vbTab & ".script({src:""")
                writer.Append(LabHelper.GetCombinedScriptResourceUrl(list))
                writer.Append("""")

                For Each key As String In options.Keys
                    writer.Append(","c)
                    writer.Append(key)
                    writer.Append(":"c)
                    writer.Append(options(key))
                Next

                writer.Append("})")
            End If
        End If
    End Sub

#End Region

End Class
