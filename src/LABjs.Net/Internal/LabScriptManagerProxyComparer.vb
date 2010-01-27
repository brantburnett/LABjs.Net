Namespace Internal

    Public Class LabScriptManagerProxyComparer
        Implements IComparer(Of LabScriptManagerProxy)

        Public Function Compare(ByVal x As LabScriptManagerProxy, ByVal y As LabScriptManagerProxy) As Integer Implements System.Collections.Generic.IComparer(Of LabScriptManagerProxy).Compare
            If x Is Nothing Then
                Throw New ArgumentNullException("x")
            End If
            If y Is Nothing Then
                Throw New ArgumentNullException("y")
            End If

            Return x.Priority.CompareTo(y.Priority)
        End Function

    End Class

End Namespace
