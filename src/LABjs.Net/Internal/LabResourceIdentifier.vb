Imports System.ComponentModel
Imports System.Reflection

Namespace Internal

    <ImmutableObject(True)> _
    Public Class LabResourceIdentifier

        Private ReadOnly _assembly As Assembly
        Public ReadOnly Property Assembly() As Assembly
            Get
                Return _assembly
            End Get
        End Property

        Private ReadOnly _resourceName As String
        Public ReadOnly Property ResourceName() As String
            Get
                Return _resourceName
            End Get
        End Property

        Public Sub New(ByVal assembly As Assembly, ByVal resourceName As String)
            _assembly = assembly
            _resourceName = resourceName
        End Sub

        Public Overrides Function Equals(ByVal obj As Object) As Boolean
            If obj Is Me Then Return True

            Dim comp As LabResourceIdentifier = TryCast(obj, LabResourceIdentifier)
            Return comp IsNot Nothing AndAlso comp.Assembly Is Assembly AndAlso String.Equals(comp.ResourceName, ResourceName)
        End Function

        Public Overrides Function GetHashCode() As Integer
            Dim h1 As Integer = Assembly.GetHashCode()
            Dim h2 As Integer = ResourceName.GetHashCode()

            Return ((h1 << 5) + h1) Xor h2
        End Function

    End Class

End Namespace