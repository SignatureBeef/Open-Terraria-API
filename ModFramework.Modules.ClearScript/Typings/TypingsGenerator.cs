using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace ModFramework.Modules.ClearScript.Typings
{
    public class TypingsGenerator : TypeTraverser
    {
        public TypingsGenerator()
        {

        }

        private string GetJsType(Type type)
        {
            string resolve()
            {
                if (this.Types.Contains(type)
                    && !type.IsGenericType
                )
                {
                    return type.FullName;
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

        private void WriteMethodParameters(Type type, MethodBase method, StringBuilder sb)
        {
            foreach (var arg in method.GetParameters())
            {
                if (arg.Position != 0)
                    sb.Append(", ");

                if (arg.GetCustomAttributes(typeof(ParamArrayAttribute), false).Length > 0)
                    sb.Append("...");

                if (arg.Name == "function") sb.Append("_");

                if (!String.IsNullOrWhiteSpace(arg.Name))
                    sb.Append(arg.Name);
                else sb.Append($"arg_{arg.Position}");

                if (arg.HasDefaultValue)
                    sb.Append("?");

                sb.Append(": ");

                if (type.IsGenericType)
                {
                    if (type.GetGenericArguments().Any(t => t == arg.ParameterType))
                    {
                        sb.Append(arg.ParameterType.Name);

                        continue;
                    }
                }
                sb.Append(GetJsType(arg.ParameterType));
            }
        }

        private bool WriteType(Type type, StringBuilder sb)
        {
            var typeIsStatic = type.IsAbstract && type.IsSealed;

            if (type.IsGenericType)
            {
                sb.Append($"{type.Name.Substring(0, type.Name.LastIndexOf('`'))}");

                var args = type.GetGenericArguments();

                sb.Append("<");
                foreach (var ga in args)
                {
                    if (ga.IsGenericType)
                    {
                        return false; // dont get support this
                    }
                    sb.Append(ga.Name);
                }
                sb.Append(">");
            }
            else
            {
                sb.Append($"{type.Name}");

                if (!type.IsEnum && type.BaseType != null && type.BaseType.FullName != "System.Object")
                {
                    sb.Append($" extends {type.BaseType.FullName}");
                }
            }

            sb.AppendLine(" {");

            foreach (var field in type.IsEnum ? type.GetFields() : type.GetFields(BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance))
            {
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
                        sb.Append(GetJsType(field.FieldType));
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
                sb.Append("\t\t");
                if (typeIsStatic || property.GetMethod?.IsStatic == true || property.SetMethod?.IsStatic == true) sb.Append("static ");

                sb.Append(property.Name);

                sb.Append(": ");
                sb.Append(GetJsType(property.PropertyType));

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

                    foreach (var arg in method.GetParameters())
                    {
                        if (arg.Position != 0)
                            sb.Append(", ");

                        if (!String.IsNullOrWhiteSpace(arg.Name))
                            sb.Append(arg.Name);
                        else sb.Append($"arg_{arg.Position}");

                        sb.Append(": ");
                        sb.Append(GetJsType(arg.ParameterType));
                    }

                    sb.AppendLine(");");
                }

                foreach (var method in type.GetMethods())
                {
                    if (type.GetProperties().Any(p => p.GetMethod == method || p.SetMethod == method)) continue;
                    if (type.GetProperties().Any(p => $"get_{p.Name}" == method.Name || $"set_{p.Name}" == method.Name)) continue; // TODO: use the correct implementation

                    if (method.Name.Contains("b__"))
                        continue;

                    var methodEvent = type.GetEvents().SingleOrDefault(e => e.AddMethod == method);

                    sb.Append("\t\t");
                    if (typeIsStatic || method.IsStatic) sb.Append("static ");

                    sb.Append(method.Name);
                    sb.Append("(");

                    if (methodEvent != null)
                    {
                        sb.Append("handler: (");
                        var invoke = methodEvent.EventHandlerType.GetMethod("Invoke");
                        WriteMethodParameters(methodEvent.EventHandlerType, invoke, sb);

                        sb.Append(") => " + GetJsType(invoke.ReturnType));

                        sb.AppendLine("): void;");
                    }
                    else
                    {
                        WriteMethodParameters(type, method, sb);

                        sb.Append("): " + GetJsType(method.ReturnType));

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

            var modules = this.Types.Select(x => x.Namespace).Distinct();
            foreach (var module in modules)
            {
                var outputFile = Path.Combine(outputFolder, $"{module}.d.ts");
                var sb = new StringBuilder();

                sb.AppendLine("/** Auto generated by the ModFramework.Modules.ClearScript tool **/");
                sb.AppendLine();

                sb.AppendLine($"declare module {module} {{");

                var types = this.Types.Where(x => x.Namespace == module).Distinct().OrderBy(x => x.Namespace).ThenBy(y => y.FullName).ToArray();
                foreach (var type in types)
                {
                    if (type.IsNested) continue;

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

                    sb.Append(sub);

                    sb.AppendLine("\t}");
                }

                sb.AppendLine("}");
                File.WriteAllText(outputFile, sb.ToString());
            }

            //sb.AppendLine(@"}");

            //File.WriteAllText(outputFile, sb.ToString());
        }
    }
}
