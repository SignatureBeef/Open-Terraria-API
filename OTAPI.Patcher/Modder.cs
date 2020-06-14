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
using Mono.Cecil.Cil;
using MonoMod;
using OTAPI.Mods.Relinker;
using System;
using System.Collections.Generic;

namespace OTAPI.Patcher
{
    public class OTAPIModder : MonoMod.MonoModder, IRelinkProvider
    {
        public bool AllowInterreferenceReplacements { get; set; } = true;
        protected List<RelinkTask> _tasks { get; set; } = new List<RelinkTask>();
        public IEnumerable<RelinkTask> Tasks => _tasks;

        public virtual void AddTask(RelinkTask task)
        {
            task.Modder = this;
            task.RelinkProvider = this;
            _tasks.Add(task);
            _tasks.Sort((a, b) => a.Order - b.Order);
        }

        public virtual void RunTasks(Action<RelinkTask> callback)
        {
            foreach (var task in _tasks)
                callback(task);
        }

        public OTAPIModder()
        {
            MethodParser = (MonoModder modder, MethodBody body, Instruction instr, ref int instri) =>
            {
                return true;
            };
            MethodRewriter = (MonoModder modder, MethodDefinition method) =>
            {
                if (method.Body?.HasVariables == true)
                    foreach (var variable in method.Body.Variables)
                        RunTasks(t => t.Relink(method, variable));

                if (method.HasParameters)
                    foreach (var parameter in method.Parameters)
                        RunTasks(t => t.Relink(method, parameter));
            };
            MethodBodyRewriter = (MonoModder modder, MethodBody body, Instruction instr, int instri) =>
            {
                RunTasks(t => t.Relink(body, instr));
            };
        }

        public override void PatchRefs()
        {
            OTAPI.Modifier.Apply(OTAPI.ModType.PreMerge, this);
            base.PatchRefs();
        }

        public override void PatchRefsInType(TypeDefinition type)
        {
            base.PatchRefsInType(type);

            if (type.HasFields)
                foreach (var field in type.Fields)
                    RunTasks(t => t.Relink(field));

            if (type.HasProperties)
                foreach (var property in type.Properties)
                    RunTasks(t => t.Relink(property));
        }

        public override void PatchRefsInMethod(MethodDefinition method)
        {
            base.PatchRefsInMethod(method);

            RunTasks(t => t.Relink(method));
        }
    }
}
