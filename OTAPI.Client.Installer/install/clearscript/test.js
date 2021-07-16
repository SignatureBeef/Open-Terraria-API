import Runtime from 'AddHostObject-OTAPI.Runtime';
import OTAPI from 'AddHostObject-OTAPI';
import lib from 'AddHostObject-mscorlib';
import tmr from 'AddHostObject-System.ComponentModel.TypeConverter';
import FNA from 'AddHostObject-FNA';

const { Keyboard, Keys } = FNA.Microsoft.Xna.Framework.Input;
const { ChatMessage } = OTAPI.Terraria.Chat;
const { NetTextModule } = OTAPI.Terraria.GameContent.NetModules;
const { NetManager } = OTAPI.Terraria.Net;

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

    if (NetManager.Instance) {
        let message = `Automated JS client message ${lib.System.DateTime.Now.ToString()}`;
        let msg = new ChatMessage(message);
        let packet = NetTextModule.SerializeClientMessage(msg);
        NetManager.Instance.SendToServer(packet);
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