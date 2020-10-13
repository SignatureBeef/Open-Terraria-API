using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace FullSerializer.Internal
{
	// Token: 0x0200002A RID: 42
	public class fsCyclicReferenceManager
	{
		// Token: 0x06000121 RID: 289 RVA: 0x00007442 File Offset: 0x00005642
		public void Enter()
		{
			this._depth++;
		}

		// Token: 0x06000122 RID: 290 RVA: 0x00007454 File Offset: 0x00005654
		public bool Exit()
		{
			this._depth--;
			if (this._depth == 0)
			{
				this._objectIds = new Dictionary<object, int>(fsCyclicReferenceManager.ObjectReferenceEqualityComparator.Instance);
				this._nextId = 0;
				this._marked = new Dictionary<int, object>();
			}
			if (this._depth < 0)
			{
				this._depth = 0;
				throw new InvalidOperationException("Internal Error - Mismatched Enter/Exit");
			}
			return this._depth == 0;
		}

		// Token: 0x06000123 RID: 291 RVA: 0x000074BD File Offset: 0x000056BD
		public object GetReferenceObject(int id)
		{
			if (!this._marked.ContainsKey(id))
			{
				throw new InvalidOperationException("Internal Deserialization Error - Object definition has not been encountered for object with id=" + id + "; have you reordered or modified the serialized data? If this is an issue with an unmodified Full Json implementation and unmodified serialization data, please report an issue with an included test case.");
			}
			return this._marked[id];
		}

		// Token: 0x06000124 RID: 292 RVA: 0x000074F4 File Offset: 0x000056F4
		public void AddReferenceWithId(int id, object reference)
		{
			this._marked[id] = reference;
		}

		// Token: 0x06000125 RID: 293 RVA: 0x00007504 File Offset: 0x00005704
		public int GetReferenceId(object item)
		{
			int num;
			if (!this._objectIds.TryGetValue(item, out num))
			{
				int nextId = this._nextId;
				this._nextId = nextId + 1;
				num = nextId;
				this._objectIds[item] = num;
			}
			return num;
		}

		// Token: 0x06000126 RID: 294 RVA: 0x00007541 File Offset: 0x00005741
		public bool IsReference(object item)
		{
			return this._marked.ContainsKey(this.GetReferenceId(item));
		}

		// Token: 0x06000127 RID: 295 RVA: 0x00007558 File Offset: 0x00005758
		public void MarkSerialized(object item)
		{
			int referenceId = this.GetReferenceId(item);
			if (this._marked.ContainsKey(referenceId))
			{
				throw new InvalidOperationException("Internal Error - " + item + " has already been marked as serialized");
			}
			this._marked[referenceId] = item;
		}

		// Token: 0x04000049 RID: 73
		private Dictionary<object, int> _objectIds = new Dictionary<object, int>(fsCyclicReferenceManager.ObjectReferenceEqualityComparator.Instance);

		// Token: 0x0400004A RID: 74
		private int _nextId;

		// Token: 0x0400004B RID: 75
		private Dictionary<int, object> _marked = new Dictionary<int, object>();

		// Token: 0x0400004C RID: 76
		private int _depth;

		// Token: 0x020000B9 RID: 185
		private class ObjectReferenceEqualityComparator : IEqualityComparer<object>
		{
			// Token: 0x060002A0 RID: 672 RVA: 0x00009C89 File Offset: 0x00007E89
			bool IEqualityComparer<object>.Equals(object x, object y)
			{
				return x == y;
			}

			// Token: 0x060002A1 RID: 673 RVA: 0x00009C8F File Offset: 0x00007E8F
			int IEqualityComparer<object>.GetHashCode(object obj)
			{
				return RuntimeHelpers.GetHashCode(obj);
			}

			// Token: 0x04000252 RID: 594
			public static readonly IEqualityComparer<object> Instance = new fsCyclicReferenceManager.ObjectReferenceEqualityComparator();
		}
	}
}
