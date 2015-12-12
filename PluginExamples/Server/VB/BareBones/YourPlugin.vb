Imports OTA.Command
Imports OTA.Plugin
Imports OTA.Logging

<OTAVersion(1, 0)> _
Public Class YourPlugin : Inherits BasePlugin
    Public Sub New()
        Me.Version = "1"
        Me.Author = "TDSM"
        Me.Name = "Simple name"
        Me.Description = "This plugin does these awesome things!"
    End Sub

    Protected Overrides Sub Initialized(state As Object)
    	ProgramLog.Debug.Log ("Your plugin is initialising", False)
    End Sub

    <Hook(HookOrder.NORMAL)> _
    Sub MyFunctionNameThatDoesntMatter(ByRef ctx As HookContext, ByRef args As HookArgs.PlayerEnteredGame)
        'Your implementation
    End Sub

End Class
