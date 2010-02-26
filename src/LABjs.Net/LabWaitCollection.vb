<AspNetHostingPermission(SecurityAction.LinkDemand, Level:=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level:=AspNetHostingPermissionLevel.Minimal)> _
Public Class LabWaitCollection
    Inherits System.Collections.ObjectModel.Collection(Of LabWait)

    Public Sub AddRange(ByVal collection As IEnumerable(Of LabWait))
        For Each wait As LabWait In collection
            Add(wait)
        Next
    End Sub

End Class
