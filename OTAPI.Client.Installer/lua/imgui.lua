import ('Terraria');
import ('Terraria.Chat')
import ('Terraria.Net')
import ('Terraria.GameContent.NetModules')
import ('Terraria.Localization')
import ('OTAPI');
import ('System');
import ('ImGuiNET');
import ('System.Numerics', 'System.Numerics')
import ('FNA', 'Microsoft.Xna.Framework');
local Runtime = import ('OTAPI.Runtime', 'On.Terraria');
-- ImGui.Text("Hello, world!");

Main.versionNumber = 'Last script load: ' .. DateTime.Now:ToString();

print ('[LUA] ImGUI script active: ' .. Main.versionNumber);
local show_test_window = false;
local my_tool_active = true;

local data = '';
local data_history = '';
-- local data_history = [];

local cb_imgui_callback = function()
    -- print 'cb_imgui';

    -- ImGui.Text('Test3');

    -- -- print data;
    -- ImGui.InputText("Test", data, 256);

    -- if ImGui.Button("Test Window") then
    --     -- show_test_window = show_test_window
    --     if show_test_window then
    --         show_test_window = false
    --     else
    --         show_test_window = true
    --     end
    -- end

    -- if show_test_window then
    --     -- ImGui.SetNextWindowPos(new Num.Vector2(650, 20), ImGuiCond.FirstUseEver);
    --     ImGui.ShowDemoWindow(show_test_window);
    --     -- show_test_window = false;
    -- end

    ImGui.Begin('Lua Chat', my_tool_active);
    
    ImGui.Text("Message");

    -- local tmp = data;
    returnValue, tmp = ImGui.InputText("", data, 512, luanet.enum (ImGuiInputTextFlags, 'EnterReturnsTrue,AutoSelectAll'))

    if returnValue then
        print ('Test' .. tmp);
        -- print ('' .. data);
        data = tmp;
        data_history = DateTime.Now:ToString('HH:mm:ss') .. '> ' .. tmp .. '\n' .. data_history;

        msg = ChatMessage(tmp);
        packet = NetTextModule.SerializeClientMessage(msg);
        print ('Chat:' .. msg.Text)
        Console.WriteLine (msg.CommandId.name)
        NetManager.Instance:SendToServer(packet);
    end

    ImGui.Text('History');
    ImGui.BeginChild("Scrolling");
  
    ImGui.Text(data_history);

    ImGui.EndChild();
    ImGui.End();

end;

local cb_imgui = nil;
local cb_init = nil;

if Main.instance == nil then
    cb_init = Runtime.Main.Initialize:Add(function (orig, instance)
        orig(instance);
    
        if cb_imgui == nil then
            cb_imgui = Main.instance.ImGuiDraw:Add(cb_imgui_callback);
        end
    end);
else
    cb_imgui = Main.instance.ImGuiDraw:Add(cb_imgui_callback);
end

Dispose = function()
    if cb_imgui then
        Main.instance.ImGuiDraw:Remove(cb_imgui);
    end
    if cb_init then
        Runtime.Main.Initialize:Remove(cb_init);
    end
end;

