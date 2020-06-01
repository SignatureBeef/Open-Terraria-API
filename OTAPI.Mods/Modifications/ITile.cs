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
using MonoMod;
using MonoMod.Utils;
using System.Linq;
using Mono.Cecil;

namespace OTAPI.Modifications
{
    [Modification(ModificationType.Patchtime, "Implementing ITile")]
    [MonoMod.MonoModIgnore]
    class ITile
    {
        public ITile(MonoModder modder)
        {
            var tile = modder.GetReference<Terraria.Tile>();



            var itile = Emitters.InterfaceEmitter.Emit(tile);
            modder.Module.Types.Add(itile);

            // var collection = Emitters.CollectionEmitter.Emit(tile);
            // modder.Module.Types.Add(collection);


            // foreach (var field in tile.Fields.Where(x => !x.HasConstant))
            // {
            // 	var property = field.AsProperty().AsVirtual();
            //     remapper.RemapFieldToProperty(field, property);
            // 	// field.ReplaceWith(property); 
            // }
        }

        // [RemapHook]
        // public void Remap(FieldDefinition field)
        // {
        //     if (field.FieldType.FullName == "Terraria.Tile[,]")
        //     {

        //     }
        // }
    }
}
