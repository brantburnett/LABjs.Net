Imports System.Text

Namespace Internal

    ''' <summary>
    ''' Used internally to merge multiple LabWaits together into a single wait() call
    ''' </summary>
    Public Class LabMerge
        Inherits LabAction

#Region "Constructor"

        Public Sub New()
            _actions = New LabActionCollection()
            _waits = New LabWaitCollection()
        End Sub

        Public Sub New(ByVal wait As LabWait)
            MyClass.New()

            If wait IsNot Nothing Then
                _waits.Add(wait)
            End If
        End Sub

#End Region

#Region "Public Properties"

        Private _actions As LabActionCollection
        ''' <summary>
        ''' List of actions to be included before the final wait() call
        ''' </summary>
        Public ReadOnly Property Actions() As LabActionCollection
            Get
                Return _actions
            End Get
        End Property

        Private _waits As LabWaitCollection
        ''' <summary>
        ''' List of LabWaits to be merged together
        ''' </summary>
        Public ReadOnly Property Waits() As LabWaitCollection
            Get
                Return _waits
            End Get
        End Property

#End Region

#Region "Public Methods"

        Public Overrides Sub Render(ByVal writer As System.Text.StringBuilder, ByVal context As LabRenderContext)
            ' First include any actions
            For Each action As LabAction In Actions
                action.Render(writer, context)
            Next

            Dim script As New StringBuilder()
            Dim hasScript As Boolean = False

            For Each wait As LabWait In Waits
                Dim str As String = wait.ParseInlineScript(context.Page.Cache)

                If Not String.IsNullOrEmpty(str) Then
                    script.Append(vbTab & vbTab)
                    script.AppendLine(str)
                    hasScript = True
                End If
            Next

            If Not hasScript Then
                writer.AppendLine(vbTab & ".wait()")
            Else
                writer.AppendLine(vbTab & ".wait(function() {")
                writer.Append(script.ToString())
                writer.AppendLine(vbTab & "})")
            End If
        End Sub

#End Region

    End Class

End Namespace