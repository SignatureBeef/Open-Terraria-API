import ('System');
import ('FNA', 'Microsoft.Xna.Framework.Input');
import ('OTAPI.Runtime', 'On.Terraria');

print 'Game script active';

local cb_initialize = Main.Initialize:Add(function (orig, instance)
    print '[LUA] Main.Initialize() called';
    orig(instance);
end);

local cb_update = Main.Update:Add(function (orig, instance, gameTime)
    local keyState = Keyboard.GetState();
    if keyState:IsKeyDown(Keys.Left) then
        print '[LUA] Left key down';
    end

    orig(instance, gameTime);
end);

Dispose = function()
    Main.Initialize:Remove(cb_initialize);
    Main.Update:Remove(cb_update);
end;