using System;
using System.Collections.Generic;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Core
{
	[System.Serializable]
	internal sealed class EmptyVecInt : IVecInt
	{
		private const long serialVersionUID = 1L;

		public int Size()
		{
			return 0;
		}

		public void Shrink(int nofelems)
		{
		}

		public void ShrinkTo(int newsize)
		{
		}

		public IVecInt Pop()
		{
			throw new NotSupportedException();
		}

		public void GrowTo(int newsize, int pad)
		{
		}

		public void Ensure(int nsize)
		{
		}

		public IVecInt Push(int elem)
		{
			throw new NotSupportedException();
		}

		public void UnsafePush(int elem)
		{
			throw new NotSupportedException();
		}

		public void Clear()
		{
		}

		public int Last()
		{
			throw new NotSupportedException();
		}

		public int Get(int i)
		{
			throw new NotSupportedException();
		}

		public void Set(int i, int o)
		{
			throw new NotSupportedException();
		}

		public bool Contains(int e)
		{
			return false;
		}

		public void CopyTo(IVecInt copy)
		{
		}

		public void CopyTo(int[] @is)
		{
		}

		public void MoveTo(IVecInt dest)
		{
		}

		public void MoveTo2(IVecInt dest)
		{
		}

		public void MoveTo(int[] dest)
		{
		}

		public void InsertFirst(int elem)
		{
			throw new NotSupportedException();
		}

		public void Remove(int elem)
		{
			throw new NotSupportedException();
		}

		public int Delete(int i)
		{
			throw new NotSupportedException();
		}

		public void Sort()
		{
		}

		public void SortUnique()
		{
		}

		public int UnsafeGet(int eleem)
		{
			throw new NotSupportedException();
		}

		public int ContainsAt(int e)
		{
			throw new NotSupportedException();
		}

		public int ContainsAt(int e, int from)
		{
			throw new NotSupportedException();
		}

		public void MoveTo(int dest, int source)
		{
			throw new NotSupportedException();
		}

		public bool IsEmpty()
		{
			return true;
		}

		public IteratorInt Iterator()
		{
			return new _IteratorInt_144();
		}

		private sealed class _IteratorInt_144 : IteratorInt
		{
			public _IteratorInt_144()
			{
			}

			public bool HasNext()
			{
				return false;
			}

			public int Next()
			{
				throw new NotSupportedException();
			}
		}

		public int[] ToArray()
		{
			throw new NotSupportedException();
		}

		public int IndexOf(int e)
		{
			return -1;
		}

		public override string ToString()
		{
			return "[]";
		}

		public void MoveTo(int sourceStartingIndex, int[] dest)
		{
			throw new NotSupportedException();
		}

		public IVecInt[] Subset(int cardinal)
		{
			return new IVecInt[0];
		}

		public override bool Equals(object o)
		{
			if (o is IVecInt)
			{
				return ((IVecInt)o).IsEmpty();
			}
			return false;
		}

		public override int GetHashCode()
		{
			return 0;
		}

		public void Sort(IComparer<int> comparator)
		{
		}

		public IVecInt Clone()
		{
			return new EmptyVecInt();
		}
	}
}
