/*
Copyright (C) 2020 DeathCradle

This file is part of Open Terraria API v3 (OTAPI)

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program. If not, see <http://www.gnu.org/licenses/>.
*/
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
//let ticker = setInterval((...args) => {
//    // console.log('interval', lib.System.DateTime.Now, ...args);

//    if (NetManager.Instance) {
//        let message = `Automated JS client message ${lib.System.DateTime.Now.ToString()}`;
//        let msg = new ChatMessage(message);
//        let packet = NetTextModule.SerializeClientMessage(msg);
//        NetManager.Instance.SendToServer(packet);
//    }
//}, 5000, 'testing', 1, 2, 3);

export function Dispose() {
    console.log('JS Disposing');
    onUpdate.disconnect();

    //clearInterval(ticker);
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