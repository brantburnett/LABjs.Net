''' <summary>
''' Represents an action in the $LAB chain
''' </summary>
<AspNetHostingPermission(SecurityAction.LinkDemand, Level:=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level:=AspNetHostingPermissionLevel.Minimal)> _
Public MustInherit Class LabAction

    ''' <summary>
    ''' Renders the action to the $LAB chain
    ''' </summary>
    ''' <param name="writer">StringBuilder to render to</param>
    ''' <param name="context">LabRenderContext to use</param>
    Public MustOverride Sub Render(ByVal writer As System.Text.StringBuilder, ByVal context As LabRenderContext)

End Class
