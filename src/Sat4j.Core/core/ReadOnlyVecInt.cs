using System;
using System.Collections.Generic;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Core
{
	/// <summary>Utility class to allow Read Only access only to an IVecInt.</summary>
	/// <author>daniel</author>
	[System.Serializable]
	public sealed class ReadOnlyVecInt : IVecInt
	{
		private const long serialVersionUID = 1L;

		private readonly IVecInt vec;

		public ReadOnlyVecInt(IVecInt vec)
		{
			this.vec = vec;
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public bool Contains(int e)
		{
			return this.vec.Contains(e);
		}

		public int ContainsAt(int e)
		{
			return this.vec.ContainsAt(e);
		}

		public int ContainsAt(int e, int from)
		{
			return this.vec.ContainsAt(e, from);
		}

		public void CopyTo(IVecInt copy)
		{
			this.vec.CopyTo(copy);
		}

		public void CopyTo(int[] @is)
		{
			this.vec.CopyTo(@is);
		}

		public int Delete(int i)
		{
			throw new NotSupportedException();
		}

		public void Ensure(int nsize)
		{
			throw new NotSupportedException();
		}

		public int Get(int i)
		{
			return this.vec.Get(i);
		}

		public void GrowTo(int newsize, int pad)
		{
			throw new NotSupportedException();
		}

		public void InsertFirst(int elem)
		{
			throw new NotSupportedException();
		}

		public bool IsEmpty()
		{
			return this.vec.IsEmpty();
		}

		public IteratorInt Iterator()
		{
			return this.vec.Iterator();
		}

		public int Last()
		{
			return this.vec.Last();
		}

		public void MoveTo(IVecInt dest)
		{
			throw new NotSupportedException();
		}

		public void MoveTo(int[] dest)
		{
			throw new NotSupportedException();
		}

		public void MoveTo(int dest, int source)
		{
			throw new NotSupportedException();
		}

		public void MoveTo2(IVecInt dest)
		{
			throw new NotSupportedException();
		}

		public IVecInt Pop()
		{
			throw new NotSupportedException();
		}

		public IVecInt Push(int elem)
		{
			throw new NotSupportedException();
		}

		public void Remove(int elem)
		{
			throw new NotSupportedException();
		}

		public void Set(int i, int o)
		{
			throw new NotSupportedException();
		}

		public void Shrink(int nofelems)
		{
			throw new NotSupportedException();
		}

		public void ShrinkTo(int newsize)
		{
			throw new NotSupportedException();
		}

		public int Size()
		{
			return this.vec.Size();
		}

		public void Sort()
		{
			throw new NotSupportedException();
		}

		public void SortUnique()
		{
			throw new NotSupportedException();
		}

		public int UnsafeGet(int eleem)
		{
			return this.vec.UnsafeGet(eleem);
		}

		public void UnsafePush(int elem)
		{
			throw new NotSupportedException();
		}

		/// <since>2.1</since>
		public int[] ToArray()
		{
			int[] copy = new int[this.vec.Size()];
			this.vec.CopyTo(copy);
			return copy;
		}

		/// <since>2.2</since>
		public int IndexOf(int e)
		{
			return this.vec.IndexOf(e);
		}

		public override string ToString()
		{
			return this.vec.ToString();
		}

		public void MoveTo(int sourceStartingIndex, int[] dest)
		{
			throw new NotSupportedException();
		}

		/// <author>sroussel</author>
		/// <since>2.3.1</since>
		public IVecInt[] Subset(int cardinal)
		{
			return null;
		}

		public override int GetHashCode()
		{
			return this.vec.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.vec.Equals(obj);
		}

		public void Sort(IComparer<int> comparator)
		{
			throw new NotSupportedException();
		}

		public IVecInt Clone()
		{
			IVecInt cloned = new VecInt(this.Size());
			this.CopyTo(cloned);
			return cloned;
		}
	}
}
