Imports System.Reflection
Imports System.Text.RegularExpressions

Namespace Internal

    Public Class LabHelper

        Private Shared ReadOnly _getScriptResourceUrl As MethodInfo
        Private Shared ReadOnly _stringEncodeRegex As Regex

        Private Shared _getCombinedScriptResourceUrl As MethodInfo

        Private Shared _pairOfAssemblyList As Type
        Private Shared _pairOfAssemblyListConstrutor As ConstructorInfo

        Private Shared _pairOfStringCultureInfo As Type
        Private Shared _pairOfStringCultureInfoConstrutor As ConstructorInfo

        Private Shared _listOfPairOfAssemblyList As Type
        Private Shared _listOfPairOfAssemblyListConstrutor As ConstructorInfo
        Private Shared _listOfPairOfAssemblyListAdd As MethodInfo

        Private Shared _listOfPairOfStringCultureInfo As Type
        Private Shared _listOfPairOfStringCultureInfoConstrutor As ConstructorInfo
        Private Shared _listOfPairOfStringCultureInfoAdd As MethodInfo

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
        ''' Returns a valid URL for a combined set of scripts
        ''' </summary>
        ''' <remarks>Requires ASP.Net AJAX be operational on your site.  For URLs, use a null Assembly.</remarks>
        Public Shared Function GetCombinedScriptResourceUrl(ByVal scripts As List(Of Pair(Of Assembly, List(Of String)))) As String
            If scripts Is Nothing Then
                Throw New ArgumentNullException("scripts")
            End If

            EnsureGetCombinedScriptResourceUrl()

            Dim culture As System.Globalization.CultureInfo = System.Globalization.CultureInfo.CurrentCulture

            Dim newList As Object = _listOfPairOfAssemblyListConstrutor.Invoke(Nothing)
            For Each assembly As Pair(Of Assembly, List(Of String)) In scripts
                Dim subList As Object = _listOfPairOfStringCultureInfoConstrutor.Invoke(Nothing)
                For Each name As String In assembly.Second
                    Dim namePair As Object = _pairOfStringCultureInfoConstrutor.Invoke(New Object() {name, culture})
                    _listOfPairOfStringCultureInfoAdd.Invoke(subList, New Object() {namePair})
                Next

                Dim assemblyPair As Object = _pairOfAssemblyListConstrutor.Invoke(New Object() {assembly.First, subList})
                _listOfPairOfAssemblyListAdd.Invoke(newList, New Object() {assemblyPair})
            Next

            Return _getCombinedScriptResourceUrl.Invoke(Nothing, New Object() {newList, True, False})
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

        Private Shared Sub EnsureGetCombinedScriptResourceUrl()
            'Dim perm As New ReflectionPermission(PermissionState.Unrestricted)
            'perm.Assert()

            If _getCombinedScriptResourceUrl Is Nothing Then
                Dim scriptResourceHandler As Type = GetType(System.Web.Handlers.ScriptResourceHandler)
                Dim list As Type = Type.GetType("System.Collections.Generic.List`1", True)
                Dim pair As Type = Type.GetType("System.Web.Util.Pair`2, System.Web.Extensions, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", True)

                _pairOfStringCultureInfo = pair.MakeGenericType(GetType(String), GetType(System.Globalization.CultureInfo))
                _pairOfStringCultureInfoConstrutor = _pairOfStringCultureInfo.GetConstructor(New Type() {GetType(String), GetType(System.Globalization.CultureInfo)})

                _listOfPairOfStringCultureInfo = list.MakeGenericType(_pairOfStringCultureInfo)
                _listOfPairOfStringCultureInfoConstrutor = _listOfPairOfStringCultureInfo.GetConstructor(Type.EmptyTypes)
                _listOfPairOfStringCultureInfoAdd = _listOfPairOfStringCultureInfo.GetMethod("Add", New Type() {_pairOfStringCultureInfo})

                _pairOfAssemblyList = pair.MakeGenericType(GetType(Assembly), _listOfPairOfStringCultureInfo)
                _pairOfAssemblyListConstrutor = _pairOfAssemblyList.GetConstructor(New Type() {GetType(Assembly), _listOfPairOfStringCultureInfo})

                _listOfPairOfAssemblyList = list.MakeGenericType(_pairOfAssemblyList)
                _listOfPairOfAssemblyListConstrutor = _listOfPairOfAssemblyList.GetConstructor(Type.EmptyTypes)
                _listOfPairOfAssemblyListAdd = _listOfPairOfAssemblyList.GetMethod("Add", New Type() {_pairOfAssemblyList})

                _getCombinedScriptResourceUrl = scriptResourceHandler.GetMethod("GetScriptResourceUrl", BindingFlags.Static Or BindingFlags.NonPublic, Nothing, CallingConventions.Any, _
                                                                                New Type() {_listOfPairOfAssemblyList, GetType(Boolean), GetType(Boolean)}, Nothing)
            End If
        End Sub

        Shared Sub New()
            _getScriptResourceUrl = GetType(ScriptManager).GetMethod("GetScriptResourceUrl", BindingFlags.Instance Or BindingFlags.NonPublic, Nothing, CallingConventions.Any, New Type() {GetType(String), GetType(Assembly)}, Nothing)
            _stringEncodeRegex = New Regex("[\\""]", RegexOptions.Compiled)
        End Sub

    End Class

End Namespace