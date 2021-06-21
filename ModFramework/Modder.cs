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
using ModFramework.Relinker;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MonoMod;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using System.IO;

namespace ModFramework
{
    [MonoMod.MonoModIgnore]
    public class ModFwModder : MonoMod.MonoModder, IRelinkProvider
    {
        public bool AllowInterreferenceReplacements { get; set; } = true;
        protected List<RelinkTask> TaskList { get; set; } = new List<RelinkTask>();
        public IEnumerable<RelinkTask> Tasks => TaskList;

        public event MonoMod.MethodRewriter OnRewritingMethod;
        public event MonoMod.MethodBodyRewriter OnRewritingMethodBody;

        public virtual void AddTask(RelinkTask task)
        {
            task.Modder = this;
            task.RelinkProvider = this;
            TaskList.Add(task);
            TaskList.Sort((a, b) => a.Order - b.Order);
            task.Registered();
        }

        public virtual void RunTasks(Action<RelinkTask> callback)
        {
            foreach (var task in TaskList)
                callback(task);
        }

        public ModFwModder()
        {
            MethodParser = (MonoModder modder, MethodBody body, Instruction instr, ref int instri) => true;
            MethodRewriter = (MonoModder modder, MethodDefinition method) =>
            {
                if (method.Body?.HasVariables == true)
                    foreach (var variable in method.Body.Variables)
                        RunTasks(t => t.Relink(method, variable));

                if (method.HasParameters)
                    foreach (var parameter in method.Parameters)
                        RunTasks(t => t.Relink(method, parameter));

                OnRewritingMethod?.Invoke(modder, method);
            };
            MethodBodyRewriter = (MonoModder modder, MethodBody body, Instruction instr, int instri) =>
            {
                RunTasks(t => t.Relink(body, instr));
                OnRewritingMethodBody?.Invoke(modder, body, instr, instri);
            };

            AddTask(new EventDelegateRelinker());
        }

        public override void Read()
        {
            base.Read();

            Modifier.Apply(ModType.Read, this);
        }

        public override void PatchRefs()
        {
            Modifier.Apply(ModType.PreMerge, this);
            base.PatchRefs();
        }

        public override void PatchRefsInType(TypeDefinition type)
        {
            base.PatchRefsInType(type);

            RunTasks(t => t.Relink(type));

            if (type.HasEvents)
                foreach (var typeEvent in type.Events)
                    RunTasks(t => t.Relink(typeEvent));

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

        public override void AutoPatch()
        {
            Modifier.Apply(ModType.PrePatch, this);

            base.AutoPatch();

            Modifier.Apply(ModType.PostPatch, this);

            foreach (var relinked in RelinkModuleMap)
            {
                // remove the references
                foreach (var asmref in Module.AssemblyReferences.ToArray())
                    if (asmref.Name.Equals(relinked.Key))
                        Module.AssemblyReferences.Remove(asmref);
            }
        }

        public void RelinkAssembly(string fromAssemblyName, ModuleDefinition toModule = null)
        {
            this.RelinkModuleMap[fromAssemblyName] = toModule ?? this.Module;
        }
    }
}
