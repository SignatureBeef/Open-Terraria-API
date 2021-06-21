import Runtime from 'AddHostObject-OTAPI.Runtime';
import OTAPI from 'AddHostObject-OTAPI';
import lib from 'AddHostObject-mscorlib';
import tmr from 'AddHostObject-System.ComponentModel.TypeConverter';

const { Color } = OTAPI.Microsoft.Xna.Framework;
const { NetTextModule } = OTAPI.Terraria.GameContent.NetModules;
const { NetManager } = OTAPI.Terraria.Net;
const { NetworkText } = OTAPI.Terraria.Localization;

console.log('[JS] Hello server from javascript');

// [example] attach to runtime hooks
let onUpdate = Runtime.On.Terraria.Main.Update.connect((orig, instance, gameTime) => {
    orig(instance, gameTime);

    // console.log('UPDATE');
});

// [example] manually call terraria functions 
let ticker = setInterval((...args) => {
    console.log('interval', lib.System.DateTime.Now, ...args);

    if (NetManager.Instance) {
        var message = `Automated JS server message ${lib.System.DateTime.Now.ToString()}`;
        var colour = new Color(255, 0, 0);
        var msg = NetworkText.FromLiteral(message);
        var packet = NetTextModule.SerializeServerMessage(msg, colour, 255);
        NetManager.Instance.Broadcast(packet, -1);
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