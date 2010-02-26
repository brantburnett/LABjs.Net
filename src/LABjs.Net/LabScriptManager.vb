Imports System.ComponentModel
Imports System.Text
Imports System.Web.Configuration

Imports LABjs.Net.Internal

''' <summary>
''' Registers Javascript files using the LABjs parallel loading system
''' </summary>
''' <remarks>There can only be one LabScriptManager per page.  Must be included on the page before any LabScriptManagerProxy controls.</remarks>
<ParseChildren(True, "Actions"), PersistChildren(False), NonVisualControl()> _
<AspNetHostingPermission(SecurityAction.LinkDemand, Level:=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level:=AspNetHostingPermissionLevel.Minimal)> _
Public Class LabScriptManager
    Inherits Control

#Region "Shared"

    Private Const LABjsResourceName As String = "LABjs.Net.LAB.js"
    Private Const cdnLABjsResourceName As String = "LABjs.Net.cdnLAB.js"

    Private Shared ReadOnly _typedCaseInsensitiveComparer As New TypedCaseInsensitiveComparer()

    Private Shared _applicationDebug As Boolean?
    Private Shared ReadOnly Property ApplicationDebug() As Boolean
        Get
            If Not _applicationDebug.HasValue Then
                _applicationDebug = GetDebugFromConfig()
            End If

            Return _applicationDebug.Value
        End Get
    End Property

    ''' <summary>
    ''' Returns the LabScriptManager registered on the Page
    ''' </summary>
    ''' <param name="page">Page the retrieve the LabScriptManager from</param>
    ''' <returns>LabScriptManager for the Page</returns>
    Public Shared Function GetCurrent(ByVal page As Page) As LabScriptManager
        If page Is Nothing Then
            Throw New ArgumentNullException("page")
        End If

        Return TryCast(page.Items(GetType(LabScriptManager)), LabScriptManager)
    End Function

    Private Shared Function GetDebugFromConfig() As Boolean
        Dim webApplicationSection As CompilationSection = DirectCast(WebConfigurationManager.GetWebApplicationSection("system.web/compilation"), CompilationSection)
        If webApplicationSection IsNot Nothing Then
            Return webApplicationSection.Debug
        Else
            Return False
        End If
    End Function

#End Region

#Region "Constructors"

    Public Sub New()
    End Sub

    Public Sub New(ByVal action As LabAction)
        If action IsNot Nothing Then
            _actions = New LabActionCollection()
            _actions.Add(action)
        End If
    End Sub

    Public Sub New(ByVal actions As IEnumerable(Of LabAction))
        If actions IsNot Nothing Then
            _actions = New LabActionCollection(actions)
        End If
    End Sub

#End Region

#Region "Public Properties"

    Private _actions As LabActionCollection
    ''' <summary>
    ''' List of actions to be performed on $LAB
    ''' </summary>
    <MergableProperty(False), PersistenceMode(PersistenceMode.InnerProperty), Category("Behavior"), Editor("System.Web.UI.Design.CollectionEditorBase, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", GetType(System.Drawing.Design.UITypeEditor)), DefaultValue(CStr(Nothing))> _
    Public ReadOnly Property Actions() As LabActionCollection
        Get
            If _actions Is Nothing Then
                _actions = New LabActionCollection()
            End If
            Return _actions
        End Get
    End Property

    Private _cdnWaitTime As Integer = 5000
    ''' <summary>
    ''' Amount of time to wait, in milliseconds, before assuming that CDN script load failed
    ''' </summary>
    ''' <remarks>Used to get around Opera and Firefox not firing load events for failed script tags</remarks>
    <Category("Behavior"), DefaultValue(5000)> _
    Public Property CdnWaitTime() As Integer
        Get
            Return _cdnWaitTime
        End Get
        Set(ByVal value As Integer)
            _cdnWaitTime = value
        End Set
    End Property

    Private _debugNameStyle As LabDebugNameStyle = LabDebugNameStyle.AddDebug
    ''' <summary>
    ''' Specifies how the script name is altered if operating in debug mode
    ''' </summary>
    <Category("Behavior"), DefaultValue(LabDebugNameStyle.AddDebug)> _
    Public Property DebugNameStyle() As LabDebugNameStyle
        Get
            Return _debugNameStyle
        End Get
        Set(ByVal value As LabDebugNameStyle)
            _debugNameStyle = value
        End Set
    End Property

    Private _enableCdnFailover As Boolean = False
    ''' <summary>
    ''' If true, then cdnLABjs is loaded along with LABjs in order to support CDN failover
    ''' </summary>
    ''' <remarks>This has no effect if LabUrl is specified.  In that case, you must include cdnLABjs as part of the script loaded by LabUrl.</remarks>
    <Category("Behavior"), DefaultValue(False)> _
    Public Property EnableCdnFailover() As Boolean
        Get
            Return _enableCdnFailover
        End Get
        Set(ByVal value As Boolean)
            _enableCdnFailover = value
        End Set
    End Property

    <Browsable(False)> _
    Public Overrides Property EnableViewState() As Boolean
        Get
            Return False
        End Get
        Set(ByVal value As Boolean)
        End Set
    End Property

    ''' <summary>
    ''' Returns true if debug scripts should be used
    ''' </summary>
    Public ReadOnly Property IsDebuggingEnabled() As Boolean
        Get
            If ScriptMode <> ScriptMode.Auto AndAlso ScriptMode <> ScriptMode.Inherit Then
                Return ScriptMode = ScriptMode.Debug
            End If
            Return ApplicationDebug
        End Get
    End Property

    Private _labUrl As String = String.Empty
    ''' <summary>
    ''' Specifies a custom URL to use for the LABjs file.  By default, uses the resource included with the library.
    ''' </summary>
    <Category("Behavior"), DefaultValue(""), UrlProperty()> _
    Public Property LabUrl() As String
        Get
            If _labUrl Is Nothing Then
                Return String.Empty
            End If
            Return _labUrl
        End Get
        Set(ByVal value As String)
            _labUrl = value
        End Set
    End Property

    Private _proxies As LabScriptManagerProxyCollection
    ''' <summary>
    ''' List of the LabScriptManagerProxies registered on the page
    ''' </summary>
    Public ReadOnly Property Proxies() As LabScriptManagerProxyCollection
        Get
            If _proxies Is Nothing Then
                _proxies = New LabScriptManagerProxyCollection()
            End If

            Return _proxies
        End Get
    End Property

    Private _scriptMode As ScriptMode = UI.ScriptMode.Auto
    ''' <summary>
    ''' Specifies what mode should be used when rendering script calls
    ''' </summary>
    ''' <remarks>Auto or Inherit will choose the mode based upon the web.config file</remarks>
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

#Region "LABjs Options"

    Private _alwaysPreserveOrder As Boolean = False
    ''' <summary>
    ''' If true, an implicit wait() call is included after each script, causing all scripts to always be loaded in the order they are specified
    ''' </summary>
    ''' <remarks>Defaults to false</remarks>
    <Category("LABjs"), DefaultValue(False)> _
    Public Property AlwaysPreserveOrder() As Boolean
        Get
            Return _alwaysPreserveOrder
        End Get
        Set(ByVal value As Boolean)
            _alwaysPreserveOrder = value
        End Set
    End Property

    Private _usePreloading As Boolean = True
    ''' <summary>
    ''' If true, attempts to use preloading to load scripts in parallel
    ''' </summary>
    ''' <remarks>Defauls to true</remarks>
    <Category("LABjs"), DefaultValue(True)> _
    Public Property UsePreloading() As Boolean
        Get
            Return _usePreloading
        End Get
        Set(ByVal value As Boolean)
            _usePreloading = value
        End Set
    End Property

    Private _useLocalXHR As Boolean = True
    ''' <summary>
    ''' If true, an XHR AJAX call will be used to load scripts that are local to the domain of the page
    ''' </summary>
    ''' <remarks>Defaults to true</remarks>
    <Category("LABjs"), DefaultValue(True)> _
    Public Property UseLocalXHR() As Boolean
        Get
            Return _useLocalXHR
        End Get
        Set(ByVal value As Boolean)
            _useLocalXHR = value
        End Set
    End Property

    Private _useCachePreload As Boolean = True
    ''' <summary>
    ''' If true, will using text/html caching to allow scripts to load but delay execution until the proper point in the chain
    ''' </summary>
    ''' <remarks>Defaults to true</remarks>
    <Category("LABjs"), DefaultValue(True)> _
    Public Property UseCachePreload() As Boolean
        Get
            Return _useCachePreload
        End Get
        Set(ByVal value As Boolean)
            _useCachePreload = value
        End Set
    End Property

    Private _allowDuplicates As Boolean = True
    ''' <summary>
    ''' If false, LABjs will test for duplicate scripts and prevent them from loading.  Adds a slight client performance penalty.
    ''' </summary>
    ''' <remarks>Defaults to true</remarks>
    <Category("LABjs"), DefaultValue(True)> _
    Public Property AllowDuplicates() As Boolean
        Get
            Return _allowDuplicates
        End Get
        Set(ByVal value As Boolean)
            _allowDuplicates = value
        End Set
    End Property

    Private _appendTo As String = "head"
    ''' <summary>
    ''' Can be either "head" or "body".  Controls the DOM element the scripts will be appended to
    ''' </summary>
    ''' <remarks>Defaults to "head"</remarks>
    <Category("LABjs"), DefaultValue("head")> _
    Public Property AppendTo() As String
        Get
            Return _appendTo
        End Get
        Set(ByVal value As String)
            _appendTo = value
        End Set
    End Property

    Private _basePath As String = String.Empty
    ''' <summary>
    ''' Specifies a base path to be prepended to all relative URLs
    ''' </summary>
    <Category("LABjs"), DefaultValue("")> _
    Public Property BasePath() As String
        Get
            Return _basePath
        End Get
        Set(ByVal value As String)
            _basePath = value
        End Set
    End Property

#End Region

#End Region

#Region "Events"

    ''' <summary>
    ''' Allows modification of the list of actions being rendered
    ''' </summary>
    ''' <remarks></remarks>
    Public Event Rendering As LabRenderingEventHandler

    Protected Overridable Sub OnRendering(ByVal e As LabRenderingEventArgs)
        RaiseEvent Rendering(Me, e)
    End Sub

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Register a LabScriptManagerProxy to be included
    ''' </summary>
    ''' <param name="proxy">LabScriptManagerProxy to be registered</param>
    Public Overridable Sub RegisterProxy(ByVal proxy As LabScriptManagerProxy)
        If Not Proxies.Contains(proxy) Then
            Proxies.Add(proxy)
        End If
    End Sub

#End Region

#Region "Protected Methods"

    ''' <summary>
    ''' Generates the LabRenderContext to be used by the rendering process
    ''' </summary>
    ''' <returns>New LabRenderContext</returns>
    Protected Overridable Function CreateRenderContext() As LabRenderContext
        Return New LabRenderContext(Me, ScriptManager.GetCurrent(Page), IsDebuggingEnabled)
    End Function

    ''' <summary>
    ''' Returns the URL for LABjs, based upon the LabUrl property and the rendering context
    ''' </summary>
    ''' <param name="context">LabRenderContext to use when getting the URL</param>
    ''' <returns>Relative or absolute reference to LABjs</returns>
    Protected Overridable Function GetLabReferenceUrl(ByVal context As LabRenderContext) As String
        If context Is Nothing Then
            Throw New ArgumentNullException("context")
        End If

        If String.IsNullOrEmpty(LabUrl) Then
            Dim resourceName As String = IIf(EnableCdnFailover, cdnLABjsResourceName, LABjsResourceName)
            If context.IsDebuggingEnabled Then
                resourceName = LabHelper.GetDebugPath(resourceName, LabDebugNameStyle.AddDebug)
            End If

            If context.ScriptManager IsNot Nothing Then
                Return LabHelper.GetScriptResourceUrl(context.ScriptManager, resourceName, System.Reflection.Assembly.GetExecutingAssembly())
            Else
                Return context.Page.ClientScript.GetWebResourceUrl(GetType(LabScriptManager), resourceName)
            End If
        Else
            ' Only modify using debug name style if Debug is specifically set, not if we're using Auto or Inherit
            Return ResolveUrl(IIf(ScriptMode = UI.ScriptMode.Debug, LabHelper.GetDebugPath(LabUrl, DebugNameStyle), LabUrl))
        End If
    End Function

    ''' <summary>
    ''' Returns a list of actions to be performed on $LAB, merging in any LabScriptManagerProxy controls
    ''' </summary>
    ''' <param name="context">LabRenderContext to use</param>
    ''' <returns>List of actions to be performed on $LAB</returns>
    Protected Overridable Function GetActions(ByVal context As LabRenderContext) As LabActionCollection
        Dim res As New LabActionCollection(Actions)

        Dim proxies As LabScriptManagerProxyCollection = Me.Proxies
        If proxies.Count > 0 Then
            proxies.SortByPriority()

            ' Collect a list of the current named waits and replace them with merges
            Dim namedWaits As New Dictionary(Of String, LabMerge)(_typedCaseInsensitiveComparer)
            For i As Integer = 0 To res.Count - 1
                Dim wait As LabWait = TryCast(res(i), LabWait)
                If wait IsNot Nothing AndAlso Not String.IsNullOrEmpty(wait.Name) Then
                    Dim merge As New LabMerge(wait)
                    namedWaits(wait.Name) = merge
                    res(i) = merge
                End If
            Next

            ' Process the actions in each proxy
            For Each proxy As LabScriptManagerProxy In proxies

                For Each action As LabAction In proxy.Actions
                    Dim group As LabActionGroup = TryCast(action, LabActionGroup)

                    If group IsNot Nothing Then
                        'Process the items in the group

                        ' Find the LabMerge being referenced
                        Dim merge As LabMerge = Nothing
                        namedWaits.TryGetValue(group.InsertAt, merge)

                        Dim groupActions As LabActionCollection = group.Actions
                        For j As Integer = 0 To groupActions.Count - 1
                            ' Process each action in the group
                            Dim groupAction As LabAction = groupActions(j)

                            If TypeOf groupAction Is LabActionGroup Then
                                Throw New LabException("LabActionGroup cannot contain another LabActionGroup")
                            ElseIf TypeOf groupAction Is LabWait Then
                                If j <> groupActions.Count - 1 Then
                                    Throw New LabException("LabWait must be the last item in a LabActionGroup")
                                End If

                                If merge IsNot Nothing Then
                                    merge.Waits.Add(groupAction)
                                Else
                                    res.Add(groupAction)
                                End If
                            ElseIf merge IsNot Nothing Then
                                merge.Actions.Add(groupAction)
                            Else
                                res.Add(groupAction)
                            End If
                        Next
                    Else
                        ' Regular action, just add to the end of the list
                        res.Add(action)
                    End If
                Next

            Next
        End If

        MergeWaits(res)

        Return res
    End Function

    ''' <summary>
    ''' Merges adjacent LabMerges and LabWaits in the list of actions
    ''' </summary>
    ''' <param name="actions">List of actions to be merged.  Collection is modified in place.</param>
    Protected Overridable Sub MergeWaits(ByVal actions As LabActionCollection)
        'Store the previous Wait or Merge
        Dim previous As LabAction = Nothing

        Dim i As Integer = 0
        While i < actions.Count
            Dim action As LabAction = actions(i)

            If TypeOf action Is LabMerge Then
                If TypeOf previous Is LabWait AndAlso DirectCast(action, LabMerge).Actions.Count = 0 Then
                    DirectCast(action, LabMerge).Waits.Insert(0, previous)
                    previous = action
                    actions.RemoveAt(i - 1)
                ElseIf TypeOf previous Is LabMerge AndAlso DirectCast(action, LabMerge).Actions.Count = 0 Then
                    DirectCast(previous, LabMerge).Waits.AddRange(DirectCast(action, LabMerge).Waits)
                    actions.RemoveAt(i)
                Else
                    previous = action
                    i += 1
                End If
            ElseIf TypeOf action Is LabWait Then
                If TypeOf previous Is LabMerge Then
                    DirectCast(previous, LabMerge).Waits.Add(action)

                    actions.RemoveAt(i)
                ElseIf TypeOf previous Is LabWait Then
                    Dim merge As New LabMerge(previous)
                    merge.Waits.Add(action)
                    actions(i - 1) = merge

                    actions.RemoveAt(i)

                    previous = merge
                Else
                    previous = action
                    i += 1
                End If
            Else
                previous = Nothing
                i += 1
            End If
        End While

        ' Remove final action if it's an empty wait, there's no need
        If TypeOf previous Is LabMerge Then
            DirectCast(previous, LabMerge).IgnoreEmptyWait = True
        ElseIf TypeOf previous Is LabWait AndAlso DirectCast(previous, LabWait).IsEmpty Then
            actions.RemoveAt(actions.Count - 1)
        End If
    End Sub

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        MyBase.OnInit(e)

        If Not DesignMode Then
            Dim page As Page = Me.Page
            If page Is Nothing Then
                Throw New InvalidOperationException("Page Cannot Be Null")
            End If

            If GetCurrent(page) IsNot Nothing Then
                Throw New InvalidOperationException("Only One LabScriptManager Is Allowed Per Page")
            End If

            page.Items(GetType(LabScriptManager)) = Me

            AddHandler page.PreRenderComplete, AddressOf OnPagePreRenderComplete
        End If
    End Sub

    Protected Overrides Sub Render(ByVal writer As System.Web.UI.HtmlTextWriter)
    End Sub

    ''' <summary>
    ''' Renders the script block that includes LABjs
    ''' </summary>
    ''' <param name="writer">StringBuilder to render to</param>
    ''' <param name="context">LabRenderContext to use while rendering</param>
    Protected Overridable Sub RenderLabReference(ByVal writer As StringBuilder, ByVal context As LabRenderContext)
        If writer Is Nothing Then
            Throw New ArgumentNullException("writer")
        End If
        If context Is Nothing Then
            Throw New ArgumentNullException("context")
        End If

        writer.AppendFormat("<script src=""{0}"" type=""text/javascript""></script>" & vbCrLf, HttpUtility.HtmlAttributeEncode(GetLabReferenceUrl(context)))
    End Sub

    ''' <summary>
    ''' Renders the setOptions call on $LAB
    ''' </summary>
    ''' <param name="writer">StringBuilder to render to</param>
    ''' <param name="context">LabRenderContext to use while rendering</param>
    Protected Overridable Sub RenderOptions(ByVal writer As StringBuilder, ByVal context As LabRenderContext)
        If writer Is Nothing Then
            Throw New ArgumentNullException("writer")
        End If
        If context Is Nothing Then
            Throw New ArgumentNullException("context")
        End If

        Dim sb As New StringBuilder(".setOptions({")
        Dim hasValue As Boolean = False

        If AlwaysPreserveOrder Then
            If hasValue Then
                sb.Append(","c)
            End If

            sb.Append("AlwaysPreserveOrder:true")
            hasValue = True
        End If

        If Not UsePreloading Then
            If hasValue Then
                sb.Append(","c)
            End If

            sb.Append("UsePreloading:false")
            hasValue = True
        End If

        If Not UseLocalXHR Then
            If hasValue Then
                sb.Append(","c)
            End If

            sb.Append("UseLocalXHR:false")
            hasValue = True
        End If

        If Not UseCachePreload Then
            If hasValue Then
                sb.Append(","c)
            End If

            sb.Append("UseCachePreload:false")
            hasValue = True
        End If

        If Not AllowDuplicates Then
            If hasValue Then
                sb.Append(","c)
            End If

            sb.Append("AllowDuplicates:false")
            hasValue = True
        End If

        If AppendTo <> "head" Then
            If hasValue Then
                sb.Append(","c)
            End If

            sb.AppendFormat("AppendTo:""{0}""", LabHelper.JSStringEncode(AppendTo))
            hasValue = True
        End If

        If Not String.IsNullOrEmpty(BasePath) Then
            If hasValue Then
                sb.Append(","c)
            End If

            sb.AppendFormat("BasePath:""{0}""", LabHelper.JSStringEncode(BasePath))
            hasValue = True
        End If

        If CdnWaitTime <> 5000 Then
            If hasValue Then
                sb.Append(","c)
            End If

            sb.AppendFormat("CDNWaitTime:{0}", CdnWaitTime)
            hasValue = True
        End If

        If hasValue Then
            sb.Append("})")
            writer.AppendLine(sb.ToString())
        End If
    End Sub

    ''' <summary>
    ''' Renders the script block that calls LABjs with options and actions
    ''' </summary>
    ''' <param name="writer">StringBuilder to render to</param>
    ''' <param name="context">LabRenderContext to use while rendering</param>
    ''' <remarks></remarks>
    Protected Overridable Sub RenderScripts(ByVal writer As StringBuilder, ByVal context As LabRenderContext)
        If writer Is Nothing Then
            Throw New ArgumentNullException("writer")
        End If
        If context Is Nothing Then
            Throw New ArgumentNullException("context")
        End If

        Dim scripts As LabActionCollection = GetActions(context)
        OnRendering(New LabRenderingEventArgs(scripts))

        If scripts.Count > 0 Then
            writer.AppendLine("<script type=""text/javascript"">")
            writer.AppendLine("$LAB")

            RenderOptions(writer, context)

            Dim page As Page = Me.Page
            For Each action As LabAction In scripts
                action.Render(writer, context)
            Next

            writer.AppendLine("</script>")
        End If
    End Sub

#End Region

#Region "Private Methods"

    Private Sub OnPagePreRenderComplete(ByVal sender As Object, ByVal e As EventArgs)
        Dim context As LabRenderContext = CreateRenderContext()

        Dim writer As New StringBuilder()
        RenderLabReference(writer, context)
        RenderScripts(writer, context)

        Page.ClientScript.RegisterStartupScript(GetType(LabScriptManager), "LABjs", writer.ToString(), False)
    End Sub

#End Region

End Class
