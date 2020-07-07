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

using System;

namespace OTAPI
{
    /// <summary>
    /// Defines when the mod is ran
    /// </summary>
    public enum ModType
    {
        /// <summary>
        /// Occurs when the binary is about to be read and patched
        /// </summary>
        Read,

        /// <summary>
        /// Occurs before MonoMod starts merging binaries together
        /// </summary>
        PreMerge,

        /// <summary>
        /// Occurs before MonoMod applies patches
        /// </summary>
        PrePatch,

        /// <summary>
        /// Occurs after MonoMod has completed processing all patches
        /// </summary>
        PostPatch,

        /// <summary>
        /// Occurs when the patched binary has started
        /// </summary>
        Runtime,
    }

    /// <summary>
    /// Defines a generalised order in which the mod is applied
    /// </summary>
    public enum ModPriority : int
    {
        /// <summary>
        /// May run slightly earlier than other mods 
        /// </summary>
        Early = -100,

        /// <summary>
        /// Default priority, no preference or requirements
        /// </summary>
        Default = 0,

        /// <summary>
        /// May run later than most mods
        /// </summary>
        Late = 50,

        /// <summary>
        /// May be one of the last mods to be ran
        /// </summary>
        Last = 100,
    }

    /// <summary>
    /// Describes an OTAPI modification instance
    /// </summary>
    public class ModificationAttribute : Attribute
    {
        /// <summary>
        /// Description of what the mod is doing
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// What type of modifiction
        /// </summary>
        public ModType Type { get; set; }

        /// <summary>
        /// A positive or negative value defining the priority of this mod against the others in it's type.
        /// </summary>
        public ModPriority Priority { get; set; }

        /// <summary>
        /// What this modification needs to wait for in order to be executed
        /// </summary>
        public Type[] Dependencies { get; set; }

        public ModificationAttribute(ModType type, string description,
            ModPriority priority = ModPriority.Default,
            Type[] dependencies = null
        )
        {
            this.Description = description;
            this.Type = type;
            this.Priority = priority;
        }

        public Type InstanceType { get; set; }
    }
}
