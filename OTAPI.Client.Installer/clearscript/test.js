import Runtime from 'AddHostObject-OTAPI.Runtime';
import OTAPI from 'AddHostObject-OTAPI';
import lib from 'AddHostObject-mscorlib';
import tmr from 'AddHostObject-System.ComponentModel.TypeConverter';
import FNA from 'AddHostObject-FNA';

const Keyboard = FNA.Microsoft.Xna.Framework.Input.Keyboard;
const Keys = FNA.Microsoft.Xna.Framework.Input.Keys;

// [example] attach to runtime hooks
let onUpdate = Runtime.On.Terraria.Main.Update.connect((orig, instance, gameTime) => {
    orig(instance, gameTime);

    // [example] use some xna 
    let keyState = Keyboard.GetState();
    if (keyState.IsKeyDown(Keys.Left)) {
        console.log('LEFT DOWN');
    }
});

// [example] manually call terraria functions 
let ticker = setInterval((...args) => {
    // console.log('interval', lib.System.DateTime.Now, ...args); 
    if (OTAPI.Terraria.Net.NetManager.Instance) {
        var message = `Automated JS client message ${lib.System.DateTime.Now.ToString()}`;
        var msg = new OTAPI.Terraria.Chat.ChatMessage(message);
        var packet = OTAPI.Terraria.GameContent.NetModules.NetTextModule.SerializeClientMessage(msg);
        OTAPI.Terraria.Net.NetManager.Instance.SendToServer(packet);
    }
}, 5000, 'testing', 1, 2, 3);

export function Dispose() {
    console.log('JS Disposing');
    onUpdate.disconnect();

    clearInterval(ticker);
};

function setInterval(callback, time, ...args) {
    let timer = new tmr.System.Timers.Timer(time);
    let sub_elapsed = timer.Elapsed.connect(() => {
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