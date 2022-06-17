using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Orders;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Heap implementation used to maintain the variables order in some heuristics.</summary>
	/// <author>daniel</author>
	[System.Serializable]
	public sealed class Heap
	{
		private const long serialVersionUID = 1L;

		/*
		* default serial version id
		*/
		private static int Left(int i)
		{
			return i << 1;
		}

		private static int Right(int i)
		{
			return i << 1 ^ 1;
		}

		private static int Parent(int i)
		{
			return i >> 1;
		}

		private readonly IVecInt heap = new VecInt();

		private readonly IVecInt indices = new VecInt();

		private readonly VariableComparator comparator;

		// heap of ints
		// int -> index in heap
		internal void PercolateUp(int i)
		{
			int x = this.heap.Get(i);
			int p = Parent(i);
			while (i != 1 && comparator.PreferredTo(x, this.heap.Get(p)))
			{
				this.heap.Set(i, this.heap.Get(p));
				this.indices.Set(this.heap.Get(p), i);
				i = p;
				p = Parent(p);
			}
			this.heap.Set(i, x);
			this.indices.Set(x, i);
		}

		internal void PercolateDown(int i)
		{
			int x = this.heap.Get(i);
			while (Left(i) < this.heap.Size())
			{
				int child = Right(i) < this.heap.Size() && comparator.PreferredTo(this.heap.Get(Right(i)), this.heap.Get(Left(i))) ? Right(i) : Left(i);
				if (!comparator.PreferredTo(this.heap.Get(child), x))
				{
					break;
				}
				this.heap.Set(i, this.heap.Get(child));
				this.indices.Set(this.heap.Get(i), i);
				i = child;
			}
			this.heap.Set(i, x);
			this.indices.Set(x, i);
		}

		internal bool Ok(int n)
		{
			return n >= 0 && n < this.indices.Size();
		}

		public Heap(VariableComparator comparator)
		{
			// NOPMD
			this.comparator = comparator;
			this.heap.Push(-1);
		}

		public void SetBounds(int size)
		{
			System.Diagnostics.Debug.Assert(size >= 0);
			this.indices.GrowTo(size, 0);
		}

		public bool InHeap(int n)
		{
			System.Diagnostics.Debug.Assert(Ok(n));
			return this.indices.Get(n) != 0;
		}

		public void Increase(int n)
		{
			System.Diagnostics.Debug.Assert(Ok(n));
			System.Diagnostics.Debug.Assert(InHeap(n));
			PercolateUp(this.indices.Get(n));
		}

		public bool Empty()
		{
			return this.heap.Size() == 1;
		}

		public int Size()
		{
			return this.heap.Size() - 1;
		}

		public int Get(int i)
		{
			int r = this.heap.Get(i);
			this.heap.Set(i, this.heap.Last());
			this.indices.Set(this.heap.Get(i), i);
			this.indices.Set(r, 0);
			this.heap.Pop();
			if (this.heap.Size() > 1)
			{
				PercolateDown(1);
			}
			return r;
		}

		public void Insert(int n)
		{
			System.Diagnostics.Debug.Assert(Ok(n));
			this.indices.Set(n, this.heap.Size());
			this.heap.Push(n);
			PercolateUp(this.indices.Get(n));
		}

		public int Getmin()
		{
			return Get(1);
		}

		public bool HeapProperty()
		{
			return HeapProperty(1);
		}

		public bool HeapProperty(int i)
		{
			return i >= this.heap.Size() || (Parent(i) == 0 || !comparator.PreferredTo(this.heap.Get(i), this.heap.Get(Parent(i)))) && HeapProperty(Left(i)) && HeapProperty(Right(i));
		}
	}
}
