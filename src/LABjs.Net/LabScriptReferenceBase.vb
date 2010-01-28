Imports System.Collections.Specialized
Imports System.ComponentModel

Imports LABjs.Net.Internal

Public MustInherit Class LabScriptReferenceBase
    Inherits LabAction

#Region "Public Properties"

    Private _type As String
    ''' <summary>
    ''' MIME type of the script
    ''' </summary>
    ''' <remarks>Defaults to "text/javascript"</remarks>
    <Category("Behavior"), DefaultValue("text/javascript")> _
    Public Property Type() As String
        Get
            If _type Is Nothing Then
                Return "text/javascript"
            End If
            Return _type
        End Get
        Set(ByVal value As String)
            _type = value
        End Set
    End Property

    Private _charSet As String
    ''' <summary>
    ''' Character set of the script, if any should be specified
    ''' </summary>
    <Category("Behavior"), DefaultValue("")> _
    Public Property CharSet() As String
        Get
            If _charSet Is Nothing Then
                Return String.Empty
            End If
            Return _charSet
        End Get
        Set(ByVal value As String)
            _charSet = value
        End Set
    End Property

    Private _allowDuplicates As LabAllowDuplicates
    ''' <summary>
    ''' Can override the AllowDuplicates setting from the LabScriptManager for this reference only
    ''' </summary>
    <Category("Behavior"), DefaultValue(LabAllowDuplicates.Default)> _
    Public Property AllowDuplicates() As LabAllowDuplicates
        Get
            Return _allowDuplicates
        End Get
        Set(ByVal value As LabAllowDuplicates)
            _allowDuplicates = value
        End Set
    End Property

    Private _wait As Boolean
    ''' <summary>
    ''' If enabled, LABjs should always wait for this script to load before continuing
    ''' </summary>
    ''' <remarks>Defaults to false</remarks>
    <Category("Behavior"), DefaultValue(False)> _
    Public Property Wait() As Boolean
        Get
            Return _wait
        End Get
        Set(ByVal value As Boolean)
            _wait = value
        End Set
    End Property

#End Region

#Region "Public Methods"

    ''' <summary>
    ''' Returns the list of options to be included in the call to script()
    ''' </summary>
    ''' <remarks>Be sure to wrap string options in quotes</remarks>
    Public Overridable Function GetOptions(ByVal context As LabRenderContext) As NameValueCollection
        Dim result As New NameValueCollection()

        If Not String.IsNullOrEmpty(Type) Then
            result.Add("type", """" & LabHelper.JSStringEncode(Type) & """")
        End If
        If Not String.IsNullOrEmpty(CharSet) Then
            result.Add("charset", """" & LabHelper.JSStringEncode(CharSet) & """")
        End If
        If AllowDuplicates = LabAllowDuplicates.Yes Then
            result.Add("allowDup", "true")
        ElseIf AllowDuplicates = LabAllowDuplicates.No Then
            result.Add("allowDup", "false")
        End If

        Return result
    End Function

#End Region

End Class
