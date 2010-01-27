Imports System.Reflection
Imports System.Text.RegularExpressions

Namespace Internal

    Public Class LabHelper

        Private Shared ReadOnly _getScriptResourceUrl As MethodInfo
        Private Shared ReadOnly _stringEncodeRegex As Regex

        ''' <summary>
        ''' Encodes a string for inclusion in Javascript, replacing all double quotes with escaped double quotes
        ''' </summary>
        Public Shared Function JSStringEncode(ByVal str As String) As String
            If String.IsNullOrEmpty(str) Then Return str

            Return _stringEncodeRegex.Replace(str, "\$&")
        End Function

        ''' <summary>
        ''' Calls the protected GetScriptResourceUrl method on a ScriptManager
        ''' </summary>
        ''' <param name="scriptManager">ScriptManager to be used</param>
        ''' <param name="name">Name of the resource</param>
        ''' <param name="assembly">Assembly the resource is located in</param>
        ''' <returns>ScriptResource.axd URL for the resource</returns>
        ''' <remarks>We use ScriptResource.axd instead of WebResource.axd where possible because it supports gzip compression on older versions of IIS</remarks>
        Public Shared Function GetScriptResourceUrl(ByVal scriptManager As ScriptManager, ByVal name As String, ByVal assembly As Assembly)
            If scriptManager Is Nothing Then
                Throw New ArgumentNullException("scriptManager")
            End If

            Return DirectCast(_getScriptResourceUrl.Invoke(scriptManager, New Object() {name, assembly}), String)
        End Function

        ''' <summary>
        ''' Given a path to a release .js file, returns the path to a debug version
        ''' </summary>
        ''' <param name="releasePath">Path to a .js file to be converted to a debug path</param>
        ''' <param name="debugNameStyle">Type of debug file naming convention to use</param>
        ''' <remarks>Replaces the trailing .js with .debug.js, accounting for query parameters</remarks>
        Public Shared Function GetDebugPath(ByVal releasePath As String, ByVal debugNameStyle As LabDebugNameStyle) As String
            Dim path As String
            Dim queryString As String

            If (releasePath.IndexOf("?"c) >= 0) Then
                Dim index As Integer = releasePath.IndexOf("?"c)
                path = releasePath.Substring(0, index)
                queryString = releasePath.Substring(index)
            Else
                path = releasePath
                queryString = Nothing
            End If

            If debugNameStyle = LabDebugNameStyle.AddDebug OrElse debugNameStyle = LabDebugNameStyle.Default Then
                If path.EndsWith(".js", StringComparison.Ordinal) Then
                    Return path.Substring(0, path.Length - 3) & ".debug.js" & queryString
                Else
                    Return releasePath
                End If
            Else
                If path.EndsWith(".min.js", StringComparison.Ordinal) Then
                    Return path.Substring(0, path.Length - 7) & ".js" & queryString
                Else
                    Return releasePath
                End If
            End If
        End Function

        Shared Sub New()
            _getScriptResourceUrl = GetType(ScriptManager).GetMethod("GetScriptResourceUrl", BindingFlags.Instance Or BindingFlags.NonPublic, Nothing, CallingConventions.Any, New Type() {GetType(String), GetType(Assembly)}, Nothing)
            _stringEncodeRegex = New Regex("[\\""]", RegexOptions.Compiled)
        End Sub

    End Class

End Namespace