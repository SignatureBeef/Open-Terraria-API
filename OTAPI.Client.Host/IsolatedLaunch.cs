// Copyright (C) 2020-2021 DeathCradle
//
// This file is part of Open Terraria API v3 (OTAPI)
//
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.

using System;

namespace OTAPI.Client.Host
{
    public class IsolatedLaunch
    {
        static NLua.Lua Script;

        public static void Launch(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            Terraria.Program.OnLaunched += (_, _) =>
            {
                Console.WriteLine("Launched");

                //Terraria.Main.versionNumber += " [OTAPI.Client]";
                //Terraria.Main.versionNumber2 += " [OTAPI.Client]";

                Script = new NLua.Lua();
                Script.LoadCLRPackage();

                Script.DoString(@"
                    import ('Terraria');
                    import ('OTAPI');
                    import ('System');

                    Main.versionNumber = Main.versionNumber .. ' Hellow from NLua';

                    Console.WriteLine('Ran nlua, vers: ' .. Main.versionNumber);

                    local send_callback = function(sender, args)
                        Console.WriteLine('[LUA] Send Callback: ' .. args.bufferId)
                    end;
                    Hooks.NetMessage.SendData:Add(send_callback);

                    local recv_callback = function(sender, args)
                        Console.WriteLine('[LUA] Recv Callback: ' .. args.instance.whoAmI)
                    end;
                    Hooks.MessageBuffer.GetData:Add(recv_callback);
                ");

                var s2 = new NLua.Lua();
                s2.LoadCLRPackage();

                s2.DoString(@"
                    import ('System');
                    import ('FNA', 'Microsoft.Xna.Framework.Input');
                    import ('OTAPI.Runtime', 'On.Terraria');

                    Main.Initialize:Add(function (orig, self)
                        Console.WriteLine('[LUA] Main.Initialize() called');
                        orig(self);
                    end);

                    Main.Update:Add(function (orig, self, gameTime)
                        local keyState = Keyboard.GetState();
                        if keyState:IsKeyDown(Keys.Left) then
                            Console.WriteLine('[LUA] Left key down!?');
                        end

                        orig(self, gameTime);
                    end);
                ");
            };

            Terraria.MacLaunch.Main(args);
        }

        private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            System.IO.File.AppendAllText("errors.txt", e.ExceptionObject.ToString());
        }
    }
}
