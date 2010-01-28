Public Class Pair(Of TFirst, TSecond)

    Private _first As TFirst
    Public ReadOnly Property First() As TFirst
        Get
            Return _first
        End Get
    End Property

    Private _second As TSecond
    Public ReadOnly Property Second() As TSecond
        Get
            Return _second
        End Get
    End Property

    Public Sub New(ByVal first As TFirst, ByVal second As TSecond)
        _first = first
        _second = second
    End Sub

    Public Overrides Function Equals(ByVal obj As Object) As Boolean
        If obj Is Me Then Return True

        Dim pair As Pair(Of TFirst, TSecond) = TryCast(obj, Pair(Of TFirst, TSecond))
        If pair Is Nothing _
            OrElse (pair._first IsNot Nothing AndAlso _first Is Nothing) _
            OrElse (_first IsNot Nothing AndAlso Not _first.Equals(pair._first)) Then
            Return False
        End If

        Return (pair._second Is Nothing AndAlso _second Is Nothing) _
                OrElse (_second IsNot Nothing AndAlso _second.Equals(pair._second))
    End Function

    Public Overrides Function GetHashCode() As Integer
        Dim h1 As Integer = IIf(First IsNot Nothing, First.GetHashCode(), 0)
        Dim h2 As Integer = IIf(Second IsNot Nothing, Second.GetHashCode(), 0)

        Return ((h1 << 5) + h1) Xor h2
    End Function

End Class
