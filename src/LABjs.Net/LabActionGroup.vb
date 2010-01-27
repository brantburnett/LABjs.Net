Imports System.ComponentModel

''' <summary>
''' Used to insert a set of actions at a specific point in the $LAB chain
''' </summary>
''' <remarks>Can only be used with a LabScriptManagerProxy</remarks>
<DefaultProperty("Scripts"), ParseChildren(True, "Actions"), PersistChildren(False)> _
<AspNetHostingPermission(SecurityAction.LinkDemand, Level:=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level:=AspNetHostingPermissionLevel.Minimal)> _
Public Class LabActionGroup
    Inherits LabAction

#Region "Public Properties"

    Private _insertAt As String = String.Empty
    ''' <summary>
    ''' Name of the LabWait in the LabScriptManager where the actions should be inserted at
    ''' </summary>
    ''' <remarks>
    ''' Actions are inserted in the chain before the targeted LabWait.
    ''' If the name is not found, the actions are inserted at the end of the chain.
    ''' </remarks>
    <Category("Behavior"), DefaultValue("")> _
    Public Property InsertAt() As String
        Get
            Return _insertAt
        End Get
        Set(ByVal value As String)
            _insertAt = value
        End Set
    End Property

    Private _actions As LabActionCollection
    ''' <summary>
    ''' List of actions to be inserted
    ''' </summary>
    ''' <remarks>This list can only include a single LabWait, and this must be the last action in the list</remarks>
    <MergableProperty(False), PersistenceMode(PersistenceMode.InnerProperty), Category("Behavior"), Editor("System.Web.UI.Design.CollectionEditorBase, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", GetType(System.Drawing.Design.UITypeEditor)), DefaultValue(CStr(Nothing))> _
    Public ReadOnly Property Actions() As LabActionCollection
        Get
            If _actions Is Nothing Then
                _actions = New LabActionCollection()
            End If
            Return _actions
        End Get
    End Property

#End Region

    Public Overrides Sub Render(ByVal writer As System.Text.StringBuilder, ByVal context As LabRenderContext)
        Throw New LabException("LabActionGroup can only be used with a LabScriptManagerProxy")
    End Sub

End Class
