namespace BareBones

open System
open OTA.Plugin
open OTA.Command
open OTA.Logging
open Terraria

[<OTAVersion(1, 0)>]
type Class1() =
    inherit BasePlugin()
    
    do
        base.Version <- "1"
        base.Author <- "TDSM"
        base.Name <- "Simple name"
        base.Description <- "This plugin does these awesome things!"
        
    override this.Initialized(state) =
        do
            ProgramLog.Plugin.Log("Your plugin is initialising")
            |> ignore
        |> ignore

    
    [<Hook(HookOrder.NORMAL)>]
    member this.MyFunctionNameThatDoesntMatter(ctx: HookContext byref, args: HookArgs.PlayerKilled byref) =
        do
            //Your implementation
            ctx.Player.SendMessage("Hello from F#", Microsoft.Xna.Framework.Color.Green)
        |> ignore
