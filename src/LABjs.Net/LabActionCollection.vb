<AspNetHostingPermission(SecurityAction.LinkDemand, Level:=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level:=AspNetHostingPermissionLevel.Minimal)> _
Public Class LabActionCollection
    Inherits System.Collections.ObjectModel.Collection(Of LabAction)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal actions As IEnumerable(Of LabAction))
        MyBase.New(actions)
    End Sub

End Class
