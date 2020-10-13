using System;
using System.Collections.Generic;

namespace FullSerializer
{
	// Token: 0x02000006 RID: 6
	public sealed class fsContext
	{
		// Token: 0x06000016 RID: 22 RVA: 0x000027CD File Offset: 0x000009CD
		public void Reset()
		{
			this._contextObjects.Clear();
		}

		// Token: 0x06000017 RID: 23 RVA: 0x000027DA File Offset: 0x000009DA
		public void Set<T>(T obj)
		{
			this._contextObjects[typeof(T)] = obj;
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000027F7 File Offset: 0x000009F7
		public bool Has<T>()
		{
			return this._contextObjects.ContainsKey(typeof(T));
		}

		// Token: 0x06000019 RID: 25 RVA: 0x00002810 File Offset: 0x00000A10
		public T Get<T>()
		{
			object obj;
			if (this._contextObjects.TryGetValue(typeof(T), out obj))
			{
				return (T)((object)obj);
			}
			throw new InvalidOperationException("There is no context object of type " + typeof(T));
		}

		// Token: 0x0400000E RID: 14
		private readonly Dictionary<Type, object> _contextObjects = new Dictionary<Type, object>();
	}
}
