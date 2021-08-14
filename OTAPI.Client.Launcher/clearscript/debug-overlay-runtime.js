import ImGuiNET from 'AddHostObject-ImGuiNET';
import OTAPI from 'AddHostObject-OTAPI';
import Runtime from 'AddHostObject-OTAPI.Runtime';
import tmr from 'AddHostObject-System.ComponentModel.TypeConverter';
import lib from 'AddHostObject-mscorlib,System.Core';

const { Main, NPC } = OTAPI.Terraria;
const { ImGui } = ImGuiNET.ImGuiNET;
const getGameInstance = () => host.cast(host.type(Main.instance.GetType()), Main.instance); // todo learn clearscript to see how to avoid needing to do this. without this, even though GetType is HostGame, it cannot find ImGuiDraw.

// var panel_active = host.newVar(true);

// var Filter = lib.System.Func(NPC, lib.System.Boolean);

// var oddFilter = new Filter(function (value) {

//     return value.active;

// });

// let npc_count = 0;

// // var npccounter = setInterval(() => {
// //     npc_count = Main.npc.Where(oddFilter).Count();
// // }, 1000)

// function cb_imgui_callback() {
//     if (panel_active.value) {
//         ImGui.Begin('JS Test', panel_active.ref);
//         // ImGui.Text(`NPCs: ${npc_count}`);
//         ImGui.End();
//     }
// }

// let cb_imgui = null;
// let onInit = null;
// if (!Main.instance) {
//     onInit = Runtime.On.Terraria.Main.Initialize.connect((orig, instance) => {
//         orig(instance);
//         cb_imgui = getGameInstance().ImGuiDraw.connect(cb_imgui_callback);
//     });
// } else {
//     cb_imgui = getGameInstance().ImGuiDraw.connect(cb_imgui_callback);
// }


// // let onUpdate = Runtime.On.Terraria.Main.Update.connect((orig, instance, gameTime) => {
// //     orig(instance, gameTime);
// //     // console.log('update', Main.myPlayer)

// //     // npc_count = Main.npc.Where(oddFilter).Count();
// //     npc_count = Main.player[Main.myPlayer].nearbyActiveNPCs;
// // });

export function Dispose() {
    // // if (onUpdate) {
    // //     onUpdate.disconnect();
    // //     onUpdate = null;
    // // }
    // if (cb_imgui) {
    //     cb_imgui.disconnect();
    //     cb_imgui = null;
    // }
    // if (onInit) {
    //     onInit.disconnect();
    //     onInit = null;
    // }
    // // clearInterval(npccounter);
};

function setTimeout(callback, time, ...args) {
    const handle = setInterval(() => {
        callback(...args);
        clearInterval(handle);
    }, time);
    handle.timer.AutoReset = false;
}

function setInterval(callback, time, ...args) {
    const timer = new tmr.System.Timers.Timer(time);
    const sub_elapsed = timer.Elapsed.connect(() => {
        callback(...args);
    });

    timer.AutoReset = true;
    timer.Enabled = true;

    return {
        timer,
        subs: {
            elapsed: sub_elapsed,
        },
    }
}

function clearInterval(handle) {
    if (handle.timer) {
        handle.timer.Stop();
        handle.subs.elapsed.disconnect();
        handle.timer.Dispose();
        handle.timer = null;
    }
}