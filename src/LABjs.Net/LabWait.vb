Imports System.ComponentModel
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Web.Caching

''' <summary>
''' Represents a wait() action in the $LAB chain, with an optional inline script to be executed when the wait completes
''' </summary>
<ParseChildren(True, "InlineScript"), PersistChildren(False)> _
<AspNetHostingPermission(SecurityAction.LinkDemand, Level:=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level:=AspNetHostingPermissionLevel.Minimal)> _
Public Class LabWait
    Inherits LabAction

#Region "Constants"

    Private Shared ReadOnly _parseRegex As New Regex("^\s*<script[^>]*>\s*(?<script>.*?)\s*</script\s*>\s*$", RegexOptions.Singleline Or RegexOptions.Compiled)
    Private Shared ReadOnly _slidingExpiration As TimeSpan = TimeSpan.FromMinutes(30)
    Private Const _cachePrefix As String = "LABjs.Net.LabScriptWait:"

#End Region

#Region "Constructor"

    Public Sub New()
    End Sub

    Public Sub New(ByVal inlineScript As String)
        _inlineScript = inlineScript
        _detectScriptTags = False
    End Sub

#End Region

#Region "Public Properties"

    Private _inlineScript As String
    ''' <summary>
    ''' Inline script that is executed when the wait is completed.
    ''' </summary>
    ''' <remarks>By default, you can include an outer pair &lt;script&gt;&lt;/script&gt; tags in your inline script so that Visual Studio syntax highlighting will work.</remarks>
    <Category("Behavior"), DefaultValue(""), PersistenceMode(PersistenceMode.EncodedInnerDefaultProperty)> _
    Public Property InlineScript() As String
        Get
            If _inlineScript Is Nothing Then
                Return String.Empty
            End If
            Return _inlineScript
        End Get
        Set(ByVal value As String)
            _inlineScript = value
        End Set
    End Property

    Private _cacheKey As String = String.Empty
    ''' <summary>
    ''' Set to an application wide unique key to cache the parsed inline script in the ASP.Net cache
    ''' </summary>
    ''' <remarks>This can improve efficiency when you're using DetectScriptTags, and has no effect otherwise.  Be certain that this key is unique to your entire application.  Having two different inline scripts with the same cache key anywhere on your site will cause unexpected results.</remarks>
    <DefaultValue("")> _
    Public Property CacheKey() As String
        Get
            Return _cacheKey
        End Get
        Set(ByVal value As String)
            _cacheKey = value
        End Set
    End Property

    Private _detectScriptTags As Boolean = True
    ''' <summary>
    ''' If enabled, you can include an outer pair &lt;script&gt;&lt;/script&gt; tags in your inline script.
    ''' </summary>
    ''' <remarks>Defaults to true.  Setting to false can provide a marginal performance improvement if you're not using script tags.</remarks>
    <DefaultValue(True)> _
    Public Property DetectScriptTags() As Boolean
        Get
            Return _detectScriptTags
        End Get
        Set(ByVal value As Boolean)
            _detectScriptTags = value
        End Set
    End Property

    Private _name As String
    ''' <summary>
    ''' Specifies a name for the LabWait 
    ''' </summary>
    ''' <remarks>
    ''' This is used in combination with LabScriptManagerProxy controls and LabActionGroups.  LabActionGroups specify an InsertAt
    ''' property, and all of their actions are added to the chain in front of the LabWait with that name.  They can also contain
    ''' a LabWait at the end of their action list, which will merge the inline scripts with the named LabWait's script.
    ''' </remarks>
    Public Property Name() As String
        Get
            Return _name
        End Get
        Set(ByVal value As String)
            _name = value
        End Set
    End Property

    ''' <summary>
    ''' Returns true if this wait doesn't contain an inline script
    ''' </summary>
    Public ReadOnly Property IsEmpty() As Boolean
        Get
            Return String.IsNullOrEmpty(InlineScript)
        End Get
    End Property

#End Region

#Region "Public Methods"

    Public Overrides Sub Render(ByVal writer As StringBuilder, ByVal context As LabRenderContext)
        Dim script As String = ParseInlineScript(context.Page.Cache)

        If String.IsNullOrEmpty(script) Then
            writer.AppendLine(vbTab & ".wait()")
        Else
            writer.AppendLine(vbTab & ".wait(function() {")
            writer.Append(vbTab & vbTab)
            writer.AppendLine(script)
            writer.AppendLine(vbTab & "})")
        End If
    End Sub

    ''' <summary>
    ''' Returns the parsed inline script, removing any script tags if DetectScriptTags is set to true
    ''' </summary>
    ''' <param name="cache">Application-wide Cache to be used to store the parsed result, if CacheKey has been set</param>
    Public Overridable Function ParseInlineScript(ByVal cache As Cache) As String
        Dim script As String = InlineScript
        If script.Length = 0 Then Return script

        Dim result As String
        If Not String.IsNullOrEmpty(CacheKey) AndAlso DetectScriptTags Then
            Dim key As String = _cachePrefix & CacheKey

            result = cache(key)
            If result Is Nothing Then
                result = RemoveScriptTags(script)
                cache.Add(key, result, Nothing, cache.NoAbsoluteExpiration, _slidingExpiration, CacheItemPriority.Normal, Nothing)
            End If
        ElseIf DetectScriptTags Then
            result = RemoveScriptTags(script)
        Else
            result = script
        End If

        Return result
    End Function

    Private Shared Function RemoveScriptTags(ByVal script As String) As String
        Dim match As Match = _parseRegex.Match(script)
        If match.Success Then
            Return match.Groups("script").Value
        Else
            Return script
        End If
    End Function

#End Region

End Class
