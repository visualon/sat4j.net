using System;
using System.Collections.Generic;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Core
{
	/// <summary>
	/// Utility class to allow Read Only access to an
	/// <see cref="Org.Sat4j.Specs.IVec{T}"/>
	/// .
	/// </summary>
	/// <author>daniel</author>
	/// <?/>
	[System.Serializable]
	public sealed class ReadOnlyVec<T> : IVec<T>
	{
		private const long serialVersionUID = 1L;

		private readonly IVec<T> vec;

		public ReadOnlyVec(IVec<T> vec)
		{
			this.vec = vec;
		}

		public void Clear()
		{
			throw new NotSupportedException();
		}

		public void CopyTo(IVec<T> copy)
		{
			this.vec.CopyTo(copy);
		}

		public void CopyTo<E>(E[] dest)
		{
			this.vec.CopyTo(dest);
		}

		public T Delete(int i)
		{
			throw new NotSupportedException();
		}

		public void Ensure(int nsize)
		{
			throw new NotSupportedException();
		}

		public T Get(int i)
		{
			return this.vec.Get(i);
		}

		public void GrowTo(int newsize, T pad)
		{
			throw new NotSupportedException();
		}

		public void InsertFirst(T elem)
		{
			throw new NotSupportedException();
		}

		public void InsertFirstWithShifting(T elem)
		{
			throw new NotSupportedException();
		}

		public bool IsEmpty()
		{
			return this.vec.IsEmpty();
		}

		public IEnumerator<T> Iterator()
		{
			return this.vec.Iterator();
		}

		public T Last()
		{
			return this.vec.Last();
		}

		public void MoveTo(IVec<T> dest)
		{
			throw new NotSupportedException();
		}

		public void MoveTo(int dest, int source)
		{
			throw new NotSupportedException();
		}

		public void Pop()
		{
			throw new NotSupportedException();
		}

		public IVec<T> Push(T elem)
		{
			throw new NotSupportedException();
		}

		public void Remove(T elem)
		{
			throw new NotSupportedException();
		}

		public void RemoveFromLast(T elem)
		{
			throw new NotSupportedException();
		}

		public void Set(int i, T o)
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

		public void Sort(IComparer<T> comparator)
		{
			throw new NotSupportedException();
		}

		public void SortUnique(IComparer<T> comparator)
		{
			throw new NotSupportedException();
		}

		public T[] ToArray()
		{
			T[] array = new T[this.vec.Size()];
			this.vec.CopyTo(array);
			return array;
		}

		public void UnsafePush(T elem)
		{
			throw new NotSupportedException();
		}

		/// <since>2.1</since>
		public bool Contains(T element)
		{
			return this.vec.Contains(element);
		}

		/// <since>2.2</since>
		public int IndexOf(T element)
		{
			return this.vec.IndexOf(element);
		}

		public override string ToString()
		{
			return this.vec.ToString();
		}

		public override int GetHashCode()
		{
			return this.vec.GetHashCode();
		}

		public override bool Equals(object obj)
		{
			return this.vec.Equals(obj);
		}

		public IVec<T> Clone()
		{
			IVec<T> cloned = new Vec<T>(this.Size());
			this.CopyTo(cloned);
			return new Org.Sat4j.Core.ReadOnlyVec<T>(cloned);
		}
	}
}
