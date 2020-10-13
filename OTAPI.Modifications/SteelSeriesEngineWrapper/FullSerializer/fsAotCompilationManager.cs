using System;
using System.Collections.Generic;
using System.Text;
using FullSerializer.Internal;

namespace FullSerializer
{
	// Token: 0x02000003 RID: 3
	public class fsAotCompilationManager
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000002 RID: 2 RVA: 0x00002060 File Offset: 0x00000260
		public static Dictionary<Type, string> AvailableAotCompilations
		{
			get
			{
				for (int i = 0; i < fsAotCompilationManager._uncomputedAotCompilations.Count; i++)
				{
					fsAotCompilationManager.AotCompilation aotCompilation = fsAotCompilationManager._uncomputedAotCompilations[i];
					fsAotCompilationManager._computedAotCompilations[aotCompilation.Type] = fsAotCompilationManager.GenerateDirectConverterForTypeInCSharp(aotCompilation.Type, aotCompilation.Members, aotCompilation.IsConstructorPublic);
				}
				fsAotCompilationManager._uncomputedAotCompilations.Clear();
				return fsAotCompilationManager._computedAotCompilations;
			}
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020C4 File Offset: 0x000002C4
		public static bool TryToPerformAotCompilation(fsConfig config, Type type, out string aotCompiledClassInCSharp)
		{
			if (fsMetaType.Get(config, type).EmitAotData())
			{
				aotCompiledClassInCSharp = fsAotCompilationManager.AvailableAotCompilations[type];
				return true;
			}
			aotCompiledClassInCSharp = null;
			return false;
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020E8 File Offset: 0x000002E8
		public static void AddAotCompilation(Type type, fsMetaProperty[] members, bool isConstructorPublic)
		{
			fsAotCompilationManager._uncomputedAotCompilations.Add(new fsAotCompilationManager.AotCompilation
			{
				Type = type,
				Members = members,
				IsConstructorPublic = isConstructorPublic
			});
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002120 File Offset: 0x00000320
		private static string GetConverterString(fsMetaProperty member)
		{
			if (member.OverrideConverterType == null)
			{
				return "null";
			}
			return string.Format("typeof({0})", member.OverrideConverterType.CSharpName(true));
		}

		// Token: 0x06000006 RID: 6 RVA: 0x0000214C File Offset: 0x0000034C
		private static string GenerateDirectConverterForTypeInCSharp(Type type, fsMetaProperty[] members, bool isConstructorPublic)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = type.CSharpName(true);
			string text2 = type.CSharpName(true, true);
			stringBuilder.AppendLine("using System;");
			stringBuilder.AppendLine("using System.Collections.Generic;");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("namespace FullSerializer {");
			stringBuilder.AppendLine("    partial class fsConverterRegistrar {");
			stringBuilder.AppendLine(string.Concat(new string[]
			{
				"        public static Speedup.",
				text2,
				"_DirectConverter Register_",
				text2,
				";"
			}));
			stringBuilder.AppendLine("    }");
			stringBuilder.AppendLine("}");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("namespace FullSerializer.Speedup {");
			stringBuilder.AppendLine(string.Concat(new string[]
			{
				"    public class ",
				text2,
				"_DirectConverter : fsDirectConverter<",
				text,
				"> {"
			}));
			stringBuilder.AppendLine("        protected override fsResult DoSerialize(" + text + " model, Dictionary<string, fsData> serialized) {");
			stringBuilder.AppendLine("            var result = fsResult.Success;");
			stringBuilder.AppendLine();
			foreach (fsMetaProperty fsMetaProperty in members)
			{
				stringBuilder.AppendLine(string.Concat(new string[]
				{
					"            result += SerializeMember(serialized, ",
					fsAotCompilationManager.GetConverterString(fsMetaProperty),
					", \"",
					fsMetaProperty.JsonName,
					"\", model.",
					fsMetaProperty.MemberName,
					");"
				}));
			}
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("            return result;");
			stringBuilder.AppendLine("        }");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("        protected override fsResult DoDeserialize(Dictionary<string, fsData> data, ref " + text + " model) {");
			stringBuilder.AppendLine("            var result = fsResult.Success;");
			stringBuilder.AppendLine();
			for (int j = 0; j < members.Length; j++)
			{
				fsMetaProperty fsMetaProperty2 = members[j];
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"            var t",
					j,
					" = model.",
					fsMetaProperty2.MemberName,
					";"
				}));
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"            result += DeserializeMember(data, ",
					fsAotCompilationManager.GetConverterString(fsMetaProperty2),
					", \"",
					fsMetaProperty2.JsonName,
					"\", out t",
					j,
					");"
				}));
				stringBuilder.AppendLine(string.Concat(new object[]
				{
					"            model.",
					fsMetaProperty2.MemberName,
					" = t",
					j,
					";"
				}));
				stringBuilder.AppendLine();
			}
			stringBuilder.AppendLine("            return result;");
			stringBuilder.AppendLine("        }");
			stringBuilder.AppendLine();
			stringBuilder.AppendLine("        public override object CreateInstance(fsData data, Type storageType) {");
			if (isConstructorPublic)
			{
				stringBuilder.AppendLine("            return new " + text + "();");
			}
			else
			{
				stringBuilder.AppendLine("            return Activator.CreateInstance(typeof(" + text + "), /*nonPublic:*/true);");
			}
			stringBuilder.AppendLine("        }");
			stringBuilder.AppendLine("    }");
			stringBuilder.AppendLine("}");
			return stringBuilder.ToString();
		}

		// Token: 0x04000002 RID: 2
		private static Dictionary<Type, string> _computedAotCompilations = new Dictionary<Type, string>();

		// Token: 0x04000003 RID: 3
		private static List<fsAotCompilationManager.AotCompilation> _uncomputedAotCompilations = new List<fsAotCompilationManager.AotCompilation>();

		// Token: 0x020000B1 RID: 177
		private struct AotCompilation
		{
			// Token: 0x04000245 RID: 581
			public Type Type;

			// Token: 0x04000246 RID: 582
			public fsMetaProperty[] Members;

			// Token: 0x04000247 RID: 583
			public bool IsConstructorPublic;
		}
	}
}
