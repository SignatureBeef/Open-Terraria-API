import ('Terraria');
import ('OTAPI');
import ('System');

--Main.versionNumber = Main.versionNumber .. ' Hellow from NLua';

-- Console.WriteLine('Net script : ' .. Main.versionNumber);

print 'Net script active'

--local send_callback = Hooks.NetMessage.SendData:Add(function(sender, args)
--    Console.WriteLine('[LUA] Send Callback: ' .. args.BufferId)
--end);

--local recv_callback = Hooks.MessageBuffer.GetData:Add(function(sender, args)
--    Console.WriteLine('[LUA] Recv Callback: ' .. args.instance.whoAmI)
--end);

Dispose = function ()
    --Hooks.NetMessage.SendData:Remove(send_callback);
    --Hooks.MessageBuffer.GetData:Remove(recv_callback);
end;