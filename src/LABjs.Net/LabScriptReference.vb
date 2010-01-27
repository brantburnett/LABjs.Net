Imports System.ComponentModel
Imports System.Reflection
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web.UI

Imports LABjs.Net.Internal

''' <summary>
''' Adds a script() call to the $LAB chain
''' </summary>
''' <remarks>
''' Options can be specified using the provided properties.
''' You can refer to a URL using the Path property, or to an assembly resource using the Assembly and Name properties.
''' </remarks>
<AspNetHostingPermission(SecurityAction.LinkDemand, Level:=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level:=AspNetHostingPermissionLevel.Minimal)> _
Public Class LabScriptReference
    Inherits LabAction

#Region "Shared"

    Private Shared _assemblyCache As SortedList
    Private Shared ReadOnly Property AssemblyCache() As SortedList
        Get
            If _assemblyCache Is Nothing Then
                _assemblyCache = SortedList.Synchronized(New SortedList())
            End If

            Return _assemblyCache
        End Get
    End Property

    Private Shared _assemblyContainsWebResourceCache As Hashtable
    Private Shared ReadOnly Property AssemblyContainsWebResourceCache() As Hashtable
        Get
            If _assemblyContainsWebResourceCache Is Nothing Then
                _assemblyContainsWebResourceCache = Hashtable.Synchronized(New Hashtable())
            End If

            Return _assemblyContainsWebResourceCache
        End Get
    End Property

    ' Keeps a cache of the first type in an assembly, for use when calling GetWebResourceUrl
    Private Shared Function GetAssemblyType(ByVal assembly As Assembly) As Type
        Dim obj As Object = AssemblyCache(assembly)
        If obj Is Nothing Then
            Dim types As Type() = assembly.GetTypes()
            If types.Length > 0 Then
                Dim type As Type = types(0)
                AssemblyCache.Add(assembly, type)
                Return type
            End If

            AssemblyCache.Add(assembly, New Object())
            Return Nothing
        ElseIf TypeOf obj Is Type Then
            Return DirectCast(obj, Type)
        Else
            Return Nothing
        End If
    End Function

    ' Keeps a cache remembering if assemblies contain specific resources, for use in debug mode only
    Private Shared Function AssemblyContainsWebResource(ByVal assembly As Assembly, ByVal resourceName As String) As Boolean
        Dim pair As New Internal.LabResourceIdentifier(assembly, resourceName)
        Dim exists As Object = AssemblyContainsWebResourceCache.Item(pair)

        If exists Is Nothing Then
            exists = False

            Dim attribute As WebResourceAttribute
            For Each attribute In assembly.GetCustomAttributes(GetType(WebResourceAttribute), False)
                If String.Equals(attribute.WebResource, resourceName, StringComparison.Ordinal) Then
                    If assembly.GetManifestResourceStream(resourceName) IsNot Nothing Then
                        exists = True
                    End If
                    Exit For
                End If
            Next

            AssemblyContainsWebResourceCache.Item(pair) = exists
        End If

        Return CBool(exists)
    End Function

#End Region

#Region "Constructor"

    Public Sub New()
    End Sub

    Public Sub New(ByVal path As String)
        _path = path
    End Sub

    Public Sub New(ByVal name As String, ByVal assembly As String)
        _name = name
        _assembly = assembly
    End Sub

#End Region

#Region "Public Properties"

    Private _alternate As String
    ''' <summary>
    ''' Specifies an alternate URL to load if the CDN load fails
    ''' </summary>
    ''' <remarks>Has no effect unless a Test is also specified</remarks>
    <Category("Behavior"), DefaultValue(""), UrlProperty("*.js")> _
    Public Property Alternate() As String
        Get
            If _alternate Is Nothing Then
                Return String.Empty
            End If
            Return _alternate
        End Get
        Set(ByVal value As String)
            _alternate = value
        End Set
    End Property

    Private _assembly As String
    ''' <summary>
    ''' Assembly name where the script resource is located
    ''' </summary>
    ''' <remarks>Name must also be specified</remarks>
    <Category("Behavior"), DefaultValue("")> _
    Public Property Assembly() As String
        Get
            If _assembly Is Nothing Then
                Return String.Empty
            End If
            Return _assembly
        End Get
        Set(ByVal value As String)
            _assembly = value
        End Set
    End Property

    Private _debugNameStyle As LabDebugNameStyle = LabDebugNameStyle.Default
    ''' <summary>
    ''' Specifies how the script name is altered if operating in debug mode
    ''' </summary>
    <Category("Behavior"), DefaultValue(LabDebugNameStyle.Default)> _
    Public Property DebugNameStyle() As LabDebugNameStyle
        Get
            Return _debugNameStyle
        End Get
        Set(ByVal value As LabDebugNameStyle)
            _debugNameStyle = value
        End Set
    End Property

    Private _path As String
    ''' <summary>
    ''' Path to the script file.  Can be either absolute or application relative.
    ''' </summary>
    <Category("Behavior"), DefaultValue(""), UrlProperty("*.js")> _
    Public Property Path() As String
        Get
            If _path Is Nothing Then
                Return String.Empty
            End If
            Return _path
        End Get
        Set(ByVal value As String)
            _path = value
        End Set
    End Property

    Private _name As String
    ''' <summary>
    ''' Name of the script resource
    ''' </summary>
    <Category("Behavior"), DefaultValue("")> _
    Public Property Name() As String
        Get
            If _name Is Nothing Then
                Return String.Empty
            End If
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    Private _scriptMode As ScriptMode = ScriptMode.Auto
    ''' <summary>
    ''' Specifies the script mode to use when loading this script
    ''' </summary>
    <Category("Behavior"), DefaultValue(ScriptMode.Auto)> _
    Public Property ScriptMode() As ScriptMode
        Get
            Return _scriptMode
        End Get
        Set(ByVal value As ScriptMode)
            If value < ScriptMode.Auto OrElse value > ScriptMode.Release Then
                Throw New ArgumentOutOfRangeException("value")
            End If

            _scriptMode = value
        End Set
    End Property

    Private _test As String
    ''' <summary>
    ''' Specifies the test used to determine if the CDN load succeeded
    ''' </summary>
    ''' <value></value>
    ''' <returns></returns>
    ''' <remarks>
    ''' Has no effect unless an Alternate is also specified.
    ''' Can be a period-delimited property chain to test for existence (i.e "jQuery" or "jQuery.fn.metadata")
    ''' Can also be a function which is called, which returns true if the library was loaded successfully
    ''' </remarks>
    <Category("Behavior"), DefaultValue("")> _
    Public Property Test() As String
        Get
            If _test Is Nothing Then
                Return String.Empty
            End If
            Return _test
        End Get
        Set(ByVal value As String)
            _test = value
        End Set
    End Property

    Private _type As String
    ''' <summary>
    ''' MIME type of the script
    ''' </summary>
    ''' <remarks>Defaults to "text/javascript"</remarks>
    <Category("Behavior"), DefaultValue("text/javascript")> _
    Public Property Type() As String
        Get
            If _type Is Nothing Then
                Return "text/javascript"
            End If
            Return _type
        End Get
        Set(ByVal value As String)
            _type = value
        End Set
    End Property

    Private _charSet As String
    ''' <summary>
    ''' Character set of the script, if any should be specified
    ''' </summary>
    <Category("Behavior"), DefaultValue("")> _
    Public Property CharSet() As String
        Get
            If _charSet Is Nothing Then
                Return String.Empty
            End If
            Return _charSet
        End Get
        Set(ByVal value As String)
            _charSet = value
        End Set
    End Property

    Private _allowDuplicates As LabAllowDuplicates
    ''' <summary>
    ''' Can override the AllowDuplicates setting from the LabScriptManager for this reference only
    ''' </summary>
    <Category("Behavior"), DefaultValue(LabAllowDuplicates.Default)> _
    Public Property AllowDuplicates() As LabAllowDuplicates
        Get
            Return _allowDuplicates
        End Get
        Set(ByVal value As LabAllowDuplicates)
            _allowDuplicates = value
        End Set
    End Property

    Private _wait As Boolean
    ''' <summary>
    ''' If enabled, LABjs should always wait for this script to load before continuing
    ''' </summary>
    ''' <remarks>Defaults to false</remarks>
    <Category("Behavior"), DefaultValue(False)> _
    Public Property Wait() As Boolean
        Get
            Return _wait
        End Get
        Set(ByVal value As Boolean)
            _wait = value
        End Set
    End Property

    Private _autoSecure As Boolean = True
    ''' <summary>
    ''' If enabled, automatically changes http: urls to https: if a secure connection is being used for the page
    ''' </summary>
    ''' <remarks>Defaults to true</remarks>
    <Category("Behavior"), DefaultValue(True)> _
    Public Property AutoSecure() As Boolean
        Get
            Return _autoSecure
        End Get
        Set(ByVal value As Boolean)
            _autoSecure = value
        End Set
    End Property

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Returns the URL to use to reference this script
    ''' </summary>
    ''' <param name="context">LabRenderContext to use</param>
    Public Overridable Function GetUrl(ByVal context As LabRenderContext) As String
        Dim hasName As Boolean = Not String.IsNullOrEmpty(Name)
        Dim hasPath As Boolean = Not String.IsNullOrEmpty(Path)
        Dim hasAssembly As Boolean = Not String.IsNullOrEmpty(Assembly)

        If Not hasName AndAlso Not hasPath Then
            Throw New InvalidOperationException("Name And Path Cannot Be Empty")
        End If
        If hasAssembly AndAlso Not hasName Then
            Throw New InvalidOperationException("Assembly Requires Name")
        End If

        Dim isDebugMode As Boolean = False
        If ScriptMode = ScriptMode.Inherit Then
            isDebugMode = context.IsDebuggingEnabled
        ElseIf ScriptMode = ScriptMode.Auto Then
            If Not String.IsNullOrEmpty(Name) Then
                ' Auto inherits if using a resource
                isDebugMode = context.IsDebuggingEnabled
            End If
        Else
            isDebugMode = ScriptMode = UI.ScriptMode.Debug
        End If

        If hasPath Then
            Dim url As String = context.Page.ResolveUrl(IIf(isDebugMode, LabHelper.GetDebugPath(Path, GetEffectiveDebugNameStyle(context)), Path))
            If AutoSecure AndAlso context.Page.Request.IsSecureConnection AndAlso url.StartsWith("http:", StringComparison.OrdinalIgnoreCase) Then
                url = "https:" & url.Substring(5)
            End If
            Return url
        Else
            Dim name As String = Me.Name
            Dim assemblyInfo As Assembly = System.Reflection.Assembly.Load(Assembly)
            If assemblyInfo Is Nothing Then
                Throw New InvalidOperationException("Assembly Not Found")
            End If

            If isDebugMode Then
                Dim debugName As String = LabHelper.GetDebugPath(Me.Name, GetEffectiveDebugNameStyle(context))
                If AssemblyContainsWebResource(assemblyInfo, debugName) Then
                    name = debugName
                End If
            End If

            If context.ScriptManager IsNot Nothing Then
                Return LabHelper.GetScriptResourceUrl(context.ScriptManager, name, assemblyInfo)
            Else
                Dim type As Type = GetAssemblyType(assemblyInfo)
                Return context.Page.ClientScript.GetWebResourceUrl(type, name)
            End If
        End If
    End Function

    ''' <summary>
    ''' Gets the parameters to pass to the script() method on $LAB
    ''' </summary>
    ''' <param name="context">LabRenderContext to use</param>
    Public Overridable Function GetParameter(ByVal context As LabRenderContext) As String
        Dim url As String = GetUrl(context)

        Dim hasType As Boolean = Type <> "text/javascript"
        Dim hasCharSet As Boolean = Not String.IsNullOrEmpty(CharSet)
        Dim hasAlternate As Boolean = Not String.IsNullOrEmpty(Alternate) AndAlso Not String.IsNullOrEmpty(Test)

        If Not hasType AndAlso Not hasCharSet AndAlso AllowDuplicates = LabAllowDuplicates.Default AndAlso Not hasAlternate Then
            ' No options specified, just return the URL as a string
            Return """" & LabHelper.JSStringEncode(url) & """"
        Else
            Dim sb As New StringBuilder()
            sb.Append("{src: """)
            sb.Append(LabHelper.JSStringEncode(url))
            sb.Append("""")

            If hasType Then
                sb.Append(", type: """)
                sb.Append(LabHelper.JSStringEncode(Type))
                sb.Append("""")
            End If
            If hasCharSet Then
                sb.Append(", charset: """)
                sb.Append(LabHelper.JSStringEncode(CharSet))
                sb.Append("""")
            End If
            If AllowDuplicates = LabAllowDuplicates.Yes Then
                sb.Append(", allowDup: true")
            ElseIf AllowDuplicates = LabAllowDuplicates.No Then
                sb.Append(", allowDup: false")
            End If
            If hasAlternate Then
                sb.Append(", alt: """)
                sb.Append(context.Page.ResolveUrl(LabHelper.JSStringEncode(Alternate)))
                sb.Append(""", test: ")
                If Test.StartsWith("function ", StringComparison.Ordinal) Then
                    sb.Append(Test)
                Else
                    sb.Append("""")
                    sb.Append(Test)
                    sb.Append("""")
                End If
            End If

            sb.Append("}")
            Return sb.ToString()
        End If
    End Function

    Public Overrides Sub Render(ByVal writer As StringBuilder, ByVal context As LabRenderContext)
        writer.AppendLine(String.Format(vbTab & ".script({0})", GetParameter(context)))
    End Sub

#End Region

#Region "Protected Methods"

    Protected Overridable Function GetEffectiveDebugNameStyle(ByVal context As LabRenderContext) As LabDebugNameStyle
        If DebugNameStyle = LabDebugNameStyle.Default Then
            Return context.Manager.DebugNameStyle
        Else
            Return DebugNameStyle
        End If
    End Function

#End Region

End Class
