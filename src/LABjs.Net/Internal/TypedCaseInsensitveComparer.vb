Namespace Internal

    Public Class TypedCaseInsensitiveComparer
        Implements IComparer(Of String)
        Implements IEqualityComparer(Of String)

        Private Function Compare(ByVal x As String, ByVal y As String) As Integer Implements System.Collections.Generic.IComparer(Of String).Compare
            Return String.Compare(x, y, True)
        End Function

        Private Function IEqualityComparerEquals(ByVal x As String, ByVal y As String) As Boolean Implements System.Collections.Generic.IEqualityComparer(Of String).Equals
            Return String.Equals(x, y, StringComparison.OrdinalIgnoreCase)
        End Function

        Private Function IEqualityComparerGetHashCode(ByVal obj As String) As Integer Implements System.Collections.Generic.IEqualityComparer(Of String).GetHashCode
            Return obj.GetHashCode()
        End Function

    End Class

End Namespace