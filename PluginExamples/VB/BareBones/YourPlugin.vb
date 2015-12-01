Imports OTA.Command
Imports OTA.Plugin

<OTAVersion(1, 0)>
Public Class YourPlugin : Inherits BasePlugin
    Public Sub New()
        MyBase.Version = "1"
        MyBase.Author = "TDSM"
        MyBase.Name = "Simple name"
        MyBase.Description = "This plugin does these awesome things!"
    End Sub

    Protected Overrides Sub Initialized(state As Object)
    	ProgramLog.Plugin.Log ("Your plugin is initialising")
    End Sub

    <Hook(HookOrder.NORMAL)> _
    Sub MyFunctionNameThatDoesntMatter(ByRef ctx As HookContext, ByRef args As HookArgs.PlayerEnteredGame)
        'Your implementation
    End Sub

End Class
