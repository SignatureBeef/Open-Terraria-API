using System;
using System.Collections.Generic;
using System.Reflection;

namespace FullSerializer.Internal
{
	// Token: 0x0200002F RID: 47
	public static class fsVersionManager
	{
		// Token: 0x0600014B RID: 331 RVA: 0x00007A04 File Offset: 0x00005C04
		public static fsResult GetVersionImportPath(string currentVersion, fsVersionedType targetVersion, out List<fsVersionedType> path)
		{
			path = new List<fsVersionedType>();
			if (!fsVersionManager.GetVersionImportPathRecursive(path, currentVersion, targetVersion))
			{
				return fsResult.Fail(string.Concat(new string[]
				{
					"There is no migration path from \"",
					currentVersion,
					"\" to \"",
					targetVersion.VersionString,
					"\""
				}));
			}
			path.Add(targetVersion);
			return fsResult.Success;
		}

		// Token: 0x0600014C RID: 332 RVA: 0x00007A68 File Offset: 0x00005C68
		private static bool GetVersionImportPathRecursive(List<fsVersionedType> path, string currentVersion, fsVersionedType current)
		{
			for (int i = 0; i < current.Ancestors.Length; i++)
			{
				fsVersionedType fsVersionedType = current.Ancestors[i];
				if (fsVersionedType.VersionString == currentVersion || fsVersionManager.GetVersionImportPathRecursive(path, currentVersion, fsVersionedType))
				{
					path.Add(fsVersionedType);
					return true;
				}
			}
			return false;
		}

		// Token: 0x0600014D RID: 333 RVA: 0x00007AB8 File Offset: 0x00005CB8
		public static fsOption<fsVersionedType> GetVersionedType(Type type)
		{
			return default(fsOption<fsVersionedType>);
		}

		// Token: 0x0600014E RID: 334 RVA: 0x00007BD8 File Offset: 0x00005DD8
		private static void VerifyConstructors(fsVersionedType type)
		{
			ConstructorInfo[] declaredConstructors = type.ModelType.GetDeclaredConstructors();
			for (int i = 0; i < type.Ancestors.Length; i++)
			{
				Type modelType = type.Ancestors[i].ModelType;
				bool flag = false;
				for (int j = 0; j < declaredConstructors.Length; j++)
				{
					ParameterInfo[] parameters = declaredConstructors[j].GetParameters();
					if (parameters.Length == 1 && parameters[0].ParameterType == modelType)
					{
						flag = true;
						break;
					}
				}
				if (!flag)
				{
					throw new fsMissingVersionConstructorException(type.ModelType, modelType);
				}
			}
		}

		// Token: 0x0600014F RID: 335 RVA: 0x00007C64 File Offset: 0x00005E64
		private static void VerifyUniqueVersionStrings(fsVersionedType type)
		{
			Dictionary<string, Type> dictionary = new Dictionary<string, Type>();
			Queue<fsVersionedType> queue = new Queue<fsVersionedType>();
			queue.Enqueue(type);
			while (queue.Count > 0)
			{
				fsVersionedType fsVersionedType = queue.Dequeue();
				if (dictionary.ContainsKey(fsVersionedType.VersionString) && dictionary[fsVersionedType.VersionString] != fsVersionedType.ModelType)
				{
					throw new fsDuplicateVersionNameException(dictionary[fsVersionedType.VersionString], fsVersionedType.ModelType, fsVersionedType.VersionString);
				}
				dictionary[fsVersionedType.VersionString] = fsVersionedType.ModelType;
				foreach (fsVersionedType item in fsVersionedType.Ancestors)
				{
					queue.Enqueue(item);
				}
			}
		}

		// Token: 0x04000056 RID: 86
		private static readonly Dictionary<Type, fsOption<fsVersionedType>> _cache = new Dictionary<Type, fsOption<fsVersionedType>>();
	}
}
