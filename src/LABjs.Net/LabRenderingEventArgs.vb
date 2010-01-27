Public Class LabRenderingEventArgs
    Inherits EventArgs

    Private _actions As LabActionCollection
    ''' <summary>
    ''' Sorted list of actions to be included in the $LAB chain
    ''' </summary>
    ''' <remarks>This list may be modified, and the resulting list will be used instead</remarks>
    Public ReadOnly Property Actions() As LabActionCollection
        Get
            Return _actions
        End Get
    End Property

    Public Sub New(ByVal actions As LabActionCollection)
        If actions Is Nothing Then
            Throw New ArgumentNullException("actions")
        End If

        _actions = actions
    End Sub

End Class

Public Delegate Sub LabRenderingEventHandler(ByVal sender As Object, ByVal e As LabRenderingEventArgs)