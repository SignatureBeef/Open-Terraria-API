using System;
using System.Collections.Generic;
using System.Reflection;

namespace FullSerializer.Internal
{
	// Token: 0x02000032 RID: 50
	public static class fsTypeCache
	{
		// Token: 0x06000165 RID: 357 RVA: 0x00008014 File Offset: 0x00006214
		static fsTypeCache()
		{
			foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
			{
				fsTypeCache._assembliesByName[assembly.FullName] = assembly;
				fsTypeCache._assembliesByIndex.Add(assembly);
			}
			fsTypeCache._cachedTypes = new Dictionary<string, Type>();
			AppDomain.CurrentDomain.AssemblyLoad += fsTypeCache.OnAssemblyLoaded;
		}

		// Token: 0x06000166 RID: 358 RVA: 0x00008098 File Offset: 0x00006298
		private static void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
		{
			fsTypeCache._assembliesByName[args.LoadedAssembly.FullName] = args.LoadedAssembly;
			fsTypeCache._assembliesByIndex.Add(args.LoadedAssembly);
			fsTypeCache._cachedTypes = new Dictionary<string, Type>();
		}

		// Token: 0x06000167 RID: 359 RVA: 0x000080D0 File Offset: 0x000062D0
		private static bool TryDirectTypeLookup(string assemblyName, string typeName, out Type type)
		{
			Assembly assembly;
			if (assemblyName != null && fsTypeCache._assembliesByName.TryGetValue(assemblyName, out assembly))
			{
				type = assembly.GetType(typeName, false);
				return type != null;
			}
			type = null;
			return false;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x00008108 File Offset: 0x00006308
		private static bool TryIndirectTypeLookup(string typeName, out Type type)
		{
			for (int i = 0; i < fsTypeCache._assembliesByIndex.Count; i++)
			{
				Assembly assembly = fsTypeCache._assembliesByIndex[i];
				type = assembly.GetType(typeName);
				if (type != null)
				{
					return true;
				}
				foreach (Type type2 in assembly.GetTypes())
				{
					if (type2.FullName == typeName)
					{
						type = type2;
						return true;
					}
				}
			}
			type = null;
			return false;
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0000817E File Offset: 0x0000637E
		public static void Reset()
		{
			fsTypeCache._cachedTypes = new Dictionary<string, Type>();
		}

		// Token: 0x0600016A RID: 362 RVA: 0x0000818A File Offset: 0x0000638A
		public static Type GetType(string name)
		{
			return fsTypeCache.GetType(name, null);
		}

		// Token: 0x0600016B RID: 363 RVA: 0x00008194 File Offset: 0x00006394
		public static Type GetType(string name, string assemblyHint)
		{
			if (string.IsNullOrEmpty(name))
			{
				return null;
			}
			Type type;
			if (!fsTypeCache._cachedTypes.TryGetValue(name, out type))
			{
				if (!fsTypeCache.TryDirectTypeLookup(assemblyHint, name, out type))
				{
					fsTypeCache.TryIndirectTypeLookup(name, out type);
				}
				fsTypeCache._cachedTypes[name] = type;
			}
			return type;
		}

		// Token: 0x0400005F RID: 95
		private static Dictionary<string, Type> _cachedTypes = new Dictionary<string, Type>();

		// Token: 0x04000060 RID: 96
		private static Dictionary<string, Assembly> _assembliesByName = new Dictionary<string, Assembly>();

		// Token: 0x04000061 RID: 97
		private static List<Assembly> _assembliesByIndex = new List<Assembly>();
	}
}
