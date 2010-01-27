''' <summary>
''' Used to pass settings between methods during the rendering process
''' </summary>
''' <remarks></remarks>
Public Class LabRenderContext

    Private _manager As LabScriptManager
    Public ReadOnly Property Manager() As LabScriptManager
        Get
            Return _manager
        End Get
    End Property

    Private _page As Page
    Public ReadOnly Property Page() As Page
        Get
            Return _page
        End Get
    End Property

    Private _scriptManager As ScriptManager
    Public ReadOnly Property ScriptManager() As ScriptManager
        Get
            Return _scriptManager
        End Get
    End Property

    Private _isDebuggingEnabled As Boolean
    Public ReadOnly Property IsDebuggingEnabled() As Boolean
        Get
            Return _isDebuggingEnabled
        End Get
    End Property

    Public Sub New(ByVal manager As LabScriptManager, ByVal scriptManager As ScriptManager, ByVal isDebuggingEnabled As Boolean)
        If manager Is Nothing Then
            Throw New ArgumentNullException("manager")
        End If

        _manager = manager
        _page = manager.Page
        If _page Is Nothing Then
            Throw New ArgumentNullException("manager.Page")
        End If

        _scriptManager = scriptManager
        _isDebuggingEnabled = isDebuggingEnabled
    End Sub

End Class
