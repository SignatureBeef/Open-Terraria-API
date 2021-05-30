import ('System');
import ('OTAPI.Runtime', 'On.Terraria');

print '[LUA] Hello server from lua';

local cb_initialize = Main.Initialize:Add(function (orig, instance)
    print '[LUA] Main.Initialize() called';

    orig:Invoke(instance);
end);

Dispose = function()
    Main.Initialize:Remove(cb_initialize);
end;