<AspNetHostingPermission(SecurityAction.LinkDemand, Level:=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level:=AspNetHostingPermissionLevel.Minimal)> _
Public Class LabScriptManagerProxyCollection
    Inherits List(Of LabScriptManagerProxy)

    Private Shared _comparer As New Internal.LabScriptManagerProxyComparer()

    Public Sub SortByPriority()
        Sort(_comparer)
    End Sub

End Class
