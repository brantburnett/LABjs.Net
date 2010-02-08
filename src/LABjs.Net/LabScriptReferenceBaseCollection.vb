<AspNetHostingPermission(SecurityAction.LinkDemand, Level:=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level:=AspNetHostingPermissionLevel.Minimal)> _
Public Class LabScriptReferenceBaseCollection
    Inherits List(Of LabScriptReferenceBase)

    Public Sub New()
        MyBase.New()
    End Sub

    Public Sub New(ByVal scripts As IEnumerable(Of LabScriptReferenceBase))
        MyBase.New(scripts)
    End Sub

End Class
