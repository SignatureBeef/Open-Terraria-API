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
using System.Collections.Generic;

namespace OTAPI.Patcher.Targets
{
    public static class PatchTargets
    {
        public static string GetCliValue(string key)
        {
            string find = $"-{key}=";
            var match = Array.Find(Environment.GetCommandLineArgs(), x => x.StartsWith(find, StringComparison.CurrentCultureIgnoreCase));
            return match?.Substring(find.Length)?.ToLower();
        }

        static Dictionary<string, IPatchTarget> _targets = new Dictionary<string, IPatchTarget>()
        {
            //{"m", new TMLPatchTarget() },
            {"o", new OTAPIServerTarget() },
            {"c", new VanillaClientPatchTarget() },
        };

        public static IPatchTarget DeterminePatchTarget()
        {
            var cli = GetCliValue("patchTarget");

            if (!String.IsNullOrWhiteSpace(cli) && _targets.TryGetValue(cli, out IPatchTarget match))
                return match;

            int attempts = 5;
            do
            {
                Console.Write("Which target would you like?\n");

                foreach (var item in _targets.Keys)
                    Console.Write($"\t {item} - {_targets[item].DisplayText}\n");

                Console.Write(": ");

                var input = Console.ReadLine().ToLower();

                if (!String.IsNullOrWhiteSpace(input) && _targets.TryGetValue(input, out IPatchTarget inputMatch))
                    return inputMatch;

                if (String.IsNullOrWhiteSpace(input)) // no key entered
                    break;
            } while (attempts-- > 0);

            return new OTAPIServerTarget();
        }
    }
}
