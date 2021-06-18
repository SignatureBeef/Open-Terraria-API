using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

// todo REWRITE
// have other ways in mind that should handle keys better (and handler subscriptions for clearscript)
// but its late and i just rather commit something buildable at this point.

namespace ModFramework.Modules.ClearScript.Typings
{
    public class TypingsGenerator : TypeTraverser
    {
        public TypingsGenerator()
        {

        }

        static string PatchKeyword(string word)
        {
            return word switch
            {
                "function" => "fn",
                _ => word
            };
        }

        private string GetJsType(Type type, Type parent)
        {
            string resolve()
            {
                if (this.Types.Contains(type)
                    && !type.IsGenericType
                )
                {
                    //return type.FullName;
                    return GetTypeName(type, parent);
                }

                return null;
            };

            return type.FullName switch
            {
                "System.Void" => "void",

                "System.Boolean" => "boolean",
                "System.Boolean[]" => "boolean[]",

                "System.String" => "string",
                "System.String[]" => "string[]",

                "System.Int16" => "number",
                "System.Int16[]" => "number[]",
                "System.Int32" => "number",
                "System.Int32[]" => "number[]",
                "System.Int64" => "number",
                "System.Int64[]" => "number[]",
                "System.Single" => "number",
                "System.Single[]" => "number[]",
                "System.Decimal" => "number",
                "System.Decimal[]" => "number[]",
                "System.Char" => "number",
                "System.Char[]" => "number[]",

                _ => resolve() ?? "any"
            };
        }

        static string GetAssemblyName(Type type) => type.Assembly.GetName().Name.Replace('.', '_');

        static string GetTypeName(Type type, Type parent)
        {
            //.Replace('.', '_')
            //var fullname = type.FullName ?? $"{type.Namespace}.{type.Name}";
            var fullname = type.Namespace != null ? $"{type.Namespace.Replace('.', '_')}.{type.Name}" : type.Name;

            if (type.Assembly.FullName == parent.Assembly.FullName && type.Namespace == parent.Namespace)
                return type.Name;

            var asm = GetAssemblyName(type);
            if (type.Namespace == null)
                return $"{asm}.{fullname}";

            //if (type.Assembly.FullName != parent.Assembly.FullName)
            {
                return $"{asm}_{fullname}";
            }
            //return fullname;
        }

        private void WriteMethodParameters(Type type, MethodBase method, StringBuilder sb, Type parent)
        {
            foreach (var arg in method.GetParameters())
            {
                if (arg.Position != 0)
                    sb.Append(", ");

                if (arg.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0)
                    sb.Append("...");

                //if (arg.Name == "function") sb.Append("_");

                if (!String.IsNullOrWhiteSpace(arg.Name))
                    sb.Append(PatchKeyword(arg.Name));
                else sb.Append($"arg_{arg.Position}");

                if (arg.HasDefaultValue)
                    sb.Append("?");

                sb.Append(": ");

                if (type.IsGenericType)
                {
                    if (type.GetGenericArguments().Any(t => t == arg.ParameterType))
                    {
                        sb.Append(String.IsNullOrWhiteSpace(arg.ParameterType.FullName)
                            ? arg.ParameterType.Name : GetTypeName(arg.ParameterType, parent)); //Vector2 on events needs to be fully qualified

                        continue;
                    }
                }
                var prmType = GetJsType(arg.ParameterType, parent);
                sb.Append(prmType);
            }
        }

        bool IsDuplicate(MemberInfo member, Type declaringType)
        {
            if (member.DeclaringType != declaringType)
            {
                // todo should use the members, but likely want to rewrite this entire thing anyway.
            }

            var dupe = declaringType;
            while (dupe != null)
            {
                var possible = dupe.GetMembers();
                var duplicates = possible.Count(x => x.Name == member.Name);
                if (duplicates > 1)
                {
                    return true;
                }

                dupe = dupe.BaseType;
            }

            return false;
        }

        void WriteGenericArgs(Type[] args, StringBuilder sb, Type parent)
        {
            var index = 0;
            sb.Append(args.Length);
            sb.Append("<");
            foreach (var ga in args)
            {
                if (index++ > 0) sb.Append(", ");
                if (ga.IsGenericType)
                {
                    var new_args = ga.GetGenericArguments();
                    //return false; // dont get support this
                    WriteGenericArgs(new_args, sb, parent);
                }

                if (ga.FullName != null)
                {
                    var n = GetJsType(ga, parent);
                    sb.Append(n);
                }
                else sb.Append(ga.Name);
            }
            sb.Append(">");
        }

        private bool WriteType(Type type, StringBuilder sb)
        {
            var typeIsStatic = type.IsAbstract && type.IsSealed;

            if (type.IsGenericType)
            {
                var ix = type.Name.LastIndexOf('`');
                var name = ix == -1 ? type.Name : type.Name.Substring(0, ix);
                sb.Append(name);

                var args = type.GetGenericArguments();
                WriteGenericArgs(args, sb, type);

                //sb.Append("<");
                //foreach (var ga in args)
                //{
                //    if (ga.IsGenericType)
                //    {
                //        return false; // dont get support this
                //    }
                //    sb.Append(ga.Name);
                //}
                //sb.Append(">");
            }
            else
            {
                sb.Append($"{type.Name}");
            }

            if (!type.IsEnum && type.BaseType != null)
            {
                sb.Append($" extends ");
                if (type.BaseType.IsGenericType)
                {
                    //var ix = type.BaseType.Name.IndexOf('`');
                    //var name = ix == -1 ? type.BaseType.Name : type.BaseType.Name.Substring(0, ix);

                    //sb.Append($"{type.BaseType.Namespace}.{name}");
                    var fullname = GetTypeName(type.BaseType, type);
                    var ix = fullname.IndexOf('`');
                    var name = ix == -1 ? fullname : fullname.Substring(0, ix);
                    sb.Append(name);
                    var ga = type.BaseType.GetGenericArguments();
                    WriteGenericArgs(ga, sb, type);
                }
                else
                {
                    sb.Append(GetTypeName(type.BaseType, type));
                }
            }

            sb.AppendLine(" {");

            foreach (var field in type.IsEnum ? type.GetFields() : type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
            {
                if (IsDuplicate(field, type)) continue;

                if (field.Name.Contains("k__BackingField"))
                    continue;

                if (type.IsEnum)
                {
                    if (!field.IsLiteral) continue;

                    sb.Append("\t\t");

                    sb.Append(field.Name);

                    sb.Append(" = ");
                    sb.Append(field.GetRawConstantValue());

                    sb.AppendLine(",");
                }
                else
                {
                    sb.Append("\t\t\t");
                    if (typeIsStatic || field.IsStatic) sb.Append("static ");

                    sb.Append(field.Name);

                    sb.Append(": ");
                    try
                    {
                        sb.Append(GetJsType(field.FieldType, type));
                    }
                    catch
                    {
                        sb.Append("any");
                    }

                    sb.AppendLine(";");
                }
            }

            foreach (var property in type.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
            {
                if (IsDuplicate(property, type)) continue;

                sb.Append("\t\t");
                if (typeIsStatic || property.GetMethod?.IsStatic == true || property.SetMethod?.IsStatic == true) sb.Append("static ");

                sb.Append(property.Name);

                sb.Append(": ");
                sb.Append(GetJsType(property.PropertyType, type));

                sb.AppendLine(";");
            }

            if (!type.IsEnum)
            {
                foreach (var method in type.GetConstructors(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
                {
                    if (type.GetProperties().Any(p => p.GetMethod == method || p.SetMethod == method)) continue;

                    sb.Append("\t\t");
                    if (typeIsStatic || method.IsStatic) sb.Append("static ");

                    sb.Append("constructor");
                    sb.Append("(");

                    //foreach (var arg in method.GetParameters())
                    //{
                    //    if (arg.Position != 0)
                    //        sb.Append(", ");

                    //    if (!String.IsNullOrWhiteSpace(arg.Name))
                    //        sb.Append(PatchKeyword(arg.Name));
                    //    else sb.Append($"arg_{arg.Position}");

                    //    sb.Append(": ");
                    //    sb.Append(GetJsType(arg.ParameterType, type));
                    //}

                    WriteMethodParameters(type, method, sb, type);

                    sb.AppendLine(");");
                }

                var methods = type.GetMethods();
                foreach (var method in methods)
                {
                    if (IsDuplicate(method, type)) continue;

                    if (type.GetProperties().Any(p => p.GetMethod == method || p.SetMethod == method)) continue;
                    if (type.GetProperties().Any(p => $"get_{p.Name}" == method.Name || $"set_{p.Name}" == method.Name)) continue; // TODO: use the correct implementation

                    if (method.Name.Contains("b__"))
                        continue;

                    var methodEvent = type.GetEvents().SingleOrDefault(e => e.AddMethod == method);

                    sb.Append("\t\t");
                    if (typeIsStatic || method.IsStatic) sb.Append("static ");

                    sb.Append(method.Name.Split('.').Last());

                    if (typeIsStatic || method.IsStatic)
                    {
                        var ga = type.GetGenericArguments();
                        if (ga.Length > 0)
                            WriteGenericArgs(ga, sb, type);
                    }

                    sb.Append("(");

                    if (methodEvent != null)
                    {
                        sb.Append("handler: (");
                        var invoke = methodEvent.EventHandlerType.GetMethod("Invoke");

                        WriteMethodParameters(methodEvent.EventHandlerType, invoke, sb, type);

                        sb.Append(") => " + GetJsType(invoke.ReturnType, type));

                        sb.AppendLine("): void;");
                    }
                    else
                    {
                        WriteMethodParameters(type, method, sb, type);

                        sb.Append("): " + GetJsType(method.ReturnType, type));

                        sb.AppendLine(";");
                    }
                }

                //foreach (var evt in type.GetEvents())
                //{
                //    sb.Append("\t\t");
                //    if (typeIsStatic || evt.AddMethod.IsStatic) sb.Append("static ");

                //    sb.Append(evt.Name);

                //    var invoke = evt.EventHandlerType.GetMethod("Invoke");

                //    sb.Append(": ");
                //    sb.Append("{ connect: (callback: (");
                //    WriteMethodParameters(type, invoke, sb);
                //    sb.Append(") => ");
                //    sb.Append(GetJsType(evt.EventHandlerType.GetMethod("Invoke").ReturnType));
                //    sb.Append(") => {disconnect: () => void} }");

                //    sb.AppendLine(";");
                //}
            }

            return true;
        }

        public void Write(string outputFolder)
        {
            if (Directory.Exists(outputFolder))
                Directory.Delete(outputFolder, true);

            Directory.CreateDirectory(outputFolder);

            //var sb = new StringBuilder();

            //sb.AppendLine("/** Auto generated by the ModFramework.Modules.ClearScript tool **/");
            //sb.AppendLine();
            //sb.AppendLine(@"declare module dotnet {");

            // [assembly][module][type data]
            var assemblies = new Dictionary<string, Dictionary<string, StringBuilder>>();

            var asd = this.Types[340].Assembly.GetTypes().Where((arg) => arg.FullName == null).ToArray();
            var modules = this.Types/*.Where(x => !String.IsNullOrWhiteSpace(x.Namespace))*/.Select(x => x.Namespace).Distinct();
            foreach (var module in modules)
            {
                //var outputFile = Path.Combine(outputFolder, $"{module}.d.ts");
                //var sb = new StringBuilder();

                //sb.AppendLine("/** Auto generated by the ModFramework.Modules.ClearScript tool **/");
                //sb.AppendLine();


                //if (!assemblies.TryGetValue(module, out Dictionary<string, StringBuilder> moduleInfo))
                //{
                //    moduleInfo = new Dictionary<string, StringBuilder>();
                //    assemblies.Add(module, moduleInfo);
                //}

                //sb.AppendLine($"module {module} {{");

                var types = this.Types
                    .Where(x => x.Namespace == module && x.Name.IndexOf("AnonymousType") == -1)
                    .Distinct()
                    .OrderBy(x => x.Namespace)
                    .ThenBy(y => y.FullName)
                    .ToArray();
                foreach (var type in types)
                {
                    //if (type.IsNested) continue;

                    StringBuilder sub = new StringBuilder();

                    if (type.IsInterface)
                    {
                        sub.Append($"\tinterface ");
                    }
                    else if (type.IsEnum)
                    {
                        if (type.GetCustomAttributes(typeof(FlagsAttribute), false).Length > 0)
                            sub.AppendLine($"\t/** Flags */");

                        sub.Append($"\tenum ");
                    }
                    else
                    {
                        sub.Append($"\t");
                        if (type.IsAbstract)
                            sub.Append("abstract ");
                        sub.Append("class ");
                    }

                    if (!WriteType(type, sub)) continue;


                    if (!assemblies.TryGetValue(type.Assembly.FullName, out Dictionary<string, StringBuilder> moduleInfo))
                    {
                        moduleInfo = new Dictionary<string, StringBuilder>();
                        assemblies.Add(type.Assembly.FullName, moduleInfo);
                    }

                    if (!moduleInfo.TryGetValue(module ?? "", out StringBuilder sb))
                    {
                        sb = new StringBuilder();
                        moduleInfo.Add(module ?? "", sb);
                    }

                    sb.Append(sub);

                    sb.AppendLine("\t}");
                }

                //sb.AppendLine("}");


                //File.WriteAllText(outputFile, sb.ToString());
            }

            // build files
            foreach (var assembly in assemblies)
            {
                var asm = new AssemblyName(assembly.Key);
                var outputFile = Path.Combine(outputFolder, $"{asm.Name}.d.ts");
                using var srm = File.OpenWrite(outputFile);
                using var sw = new StreamWriter(srm);

                //sw.WriteLine($"declare module {asm.Name.Replace('.', '_')} {{");

                foreach (var module in assembly.Value)
                {
                    var moduleName = (String.IsNullOrWhiteSpace(module.Key)
                        ? asm.Name : (asm.Name + '.' + module.Key)
                    ).Replace('.', '_');
                    //var moduleName = (asm.Name + '.' + module.Key).Replace('.', '_');

                    sw.WriteLine($"declare module {moduleName} {{");

                    //foreach (var typeInfo in module.)
                    sw.WriteLine(module.Value.ToString());

                    sw.WriteLine("}");
                }
                //sw.WriteLine("}");

                //var contents = String.Join("\n", assembly.Value.Values.Select(x => x.ToString()).ToArray());

                //var outputFile = Path.Combine(outputFolder, $"{asm.Name}.d.ts");
                //File.WriteAllText(outputFile, contents);
            }

            //sb.AppendLine(@"}");

            //File.WriteAllText(outputFile, sb.ToString());
        }
    }
}
