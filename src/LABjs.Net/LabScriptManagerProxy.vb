Imports System.ComponentModel

''' <summary>
''' Adds additional actions to the LabScriptManager's chain
''' </summary>
''' <remarks>Must be included on the page after the LabScriptManager.  Primarily used by content pages and user controls.</remarks>
<ParseChildren(True, "Actions"), PersistChildren(False), NonVisualControl()> _
<AspNetHostingPermission(SecurityAction.LinkDemand, Level:=AspNetHostingPermissionLevel.Minimal), AspNetHostingPermission(SecurityAction.InheritanceDemand, Level:=AspNetHostingPermissionLevel.Minimal)> _
Public Class LabScriptManagerProxy
    Inherits Control

#Region "Constructors"

    Public Sub New()
    End Sub

    Public Sub New(ByVal action As LabAction)
        If action IsNot Nothing Then
            _actions = New LabActionCollection()
            _actions.Add(action)
        End If
    End Sub

    Public Sub New(ByVal actions As IEnumerable(Of LabAction))
        If actions IsNot Nothing Then
            _actions = New LabActionCollection(actions)
        End If
    End Sub

#End Region

#Region "Public Properties"

    Private _actions As LabActionCollection
    ''' <summary>
    ''' List of actions to be appended to the LabScriptManager's actions
    ''' </summary>
    ''' <remarks>You may also include LabActionGroups, which can be inserted at specific points in the LabScriptManager's action list</remarks>
    <MergableProperty(False), PersistenceMode(PersistenceMode.InnerProperty), Category("Behavior"), Editor("System.Web.UI.Design.CollectionEditorBase, System.Web.Extensions.Design, Version=3.5.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35", GetType(System.Drawing.Design.UITypeEditor)), DefaultValue(CStr(Nothing))> _
    Public ReadOnly Property Actions() As LabActionCollection
        Get
            If _actions Is Nothing Then
                _actions = New LabActionCollection()
            End If
            Return _actions
        End Get
    End Property

    <Browsable(False), DefaultValue(False)> _
    Public Overrides Property EnableViewState() As Boolean
        Get
            Return False
        End Get
        Set(ByVal value As Boolean)
        End Set
    End Property

    Private _priority As Integer = 0
    ''' <summary>
    ''' Controls the priority of this proxy relative to other proxies on the page
    ''' </summary>
    ''' <remarks>Defaults to zero.  Equal priorties will be in page order.</remarks>
    <Category("Behavior"), DefaultValue(0)> _
    Public Property Priority() As Integer
        Get
            Return _priority
        End Get
        Set(ByVal value As Integer)
            _priority = value
        End Set
    End Property


#End Region

#Region "Private Properties"

    Private _labScriptManager As LabScriptManager
    Private ReadOnly Property LabScriptManager() As LabScriptManager
        Get
            If _labScriptManager Is Nothing Then
                If Page Is Nothing Then
                    Throw New InvalidOperationException("Page Cannot Be Null")
                End If

                _labScriptManager = LabScriptManager.GetCurrent(Page)

                If _labScriptManager Is Nothing Then
                    Throw New InvalidOperationException(String.Format("There is no LabScriptManager defined for use by the LabScriptManagerProxy {0}.", ID))
                End If
            End If

            Return _labScriptManager
        End Get
    End Property

#End Region

#Region "Protected Methods"

    Protected Overrides Sub OnInit(ByVal e As System.EventArgs)
        MyBase.OnInit(e)

        If Not DesignMode Then
            LabScriptManager.RegisterProxy(Me)
        End If
    End Sub

#End Region

End Class
