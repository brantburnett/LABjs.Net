Public Enum LabAllowDuplicates
    ''' <summary>
    ''' On LabScriptManager, defaults to Yes.  On LabScriptReference, uses the setting on LabScriptManager
    ''' </summary>
    [Default] = 0

    ''' <summary>
    ''' LABjs will not perform duplicate checking
    ''' </summary>
    Yes = 1

    ''' <summary>
    ''' LABjs will perform duplicate checking
    ''' </summary>
    No = 2
End Enum

Public Enum LabDebugNameStyle
    ''' <summary>
    ''' Inherits the DebugNameStyle from the LabScriptManager
    ''' </summary>
    ''' <remarks></remarks>
    [Default] = 0

    ''' <summary>
    ''' Changes script.js to script.debug.js
    ''' </summary>
    ''' <remarks></remarks>
    AddDebug = 1

    ''' <summary>
    ''' Changes script.min.js to script.js
    ''' </summary>
    ''' <remarks></remarks>
    RemoveMin = 2
End Enum