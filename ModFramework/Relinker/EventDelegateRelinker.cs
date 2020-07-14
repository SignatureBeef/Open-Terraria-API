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
using Mono.Cecil;
using System.Linq;

namespace ModFramework.Relinker
{
    /// <summary>
    /// Correct event delegates. MonoMod will public everything, including the delegate handler that is compiler generated and shares the same name.
    /// This problem causes build exceptions as it cannot figure out which member to use.
    /// </summary>
    [MonoMod.MonoModIgnore]
    public class EventDelegateRelinker : RelinkTask
    {
        public override void Relink(EventDefinition typeEvent)
        {
            var delegateMember = typeEvent.DeclaringType.Fields.SingleOrDefault(f => f.Name == typeEvent.Name);
            if(delegateMember?.IsPublic == true)
            {
                delegateMember.IsPublic = false;
                delegateMember.IsPrivate = true;
            }
        }
    }
}
