using System;
using System.Collections.Generic;
using System.Text;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Core
{
	/// <summary>
	/// Simple but efficient vector implementation, based on the vector
	/// implementation available in MiniSAT.
	/// </summary>
	/// <remarks>
	/// Simple but efficient vector implementation, based on the vector
	/// implementation available in MiniSAT. Note that the elements are compared
	/// using their references, not using the equals method.
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class Vec<T> : IVec<T>
	{
		private const long serialVersionUID = 1L;

		/// <summary>Create a Vector with an initial capacity of 5 elements.</summary>
		public Vec()
			: this(5)
		{
		}

		/// <summary>Adapter method to translate an array of int into an IVec.</summary>
		/// <remarks>
		/// Adapter method to translate an array of int into an IVec.
		/// The array is used inside the Vec, so the elements may be modified outside
		/// the Vec. But it should not take much memory. The size of the created Vec
		/// is the length of the array.
		/// </remarks>
		/// <param name="elts">a filled array of T.</param>
		public Vec(T[] elts)
		{
			// MiniSat -- Copyright (c) 2003-2005, Niklas Een, Niklas Sorensson
			//
			// Permission is hereby granted, free of charge, to any person obtaining a
			// copy of this software and associated documentation files (the
			// "Software"), to deal in the Software without restriction, including
			// without limitation the rights to use, copy, modify, merge, publish,
			// distribute, sublicense, and/or sell copies of the Software, and to
			// permit persons to whom the Software is furnished to do so, subject to
			// the following conditions:
			//
			// The above copyright notice and this permission notice shall be included
			// in all copies or substantial portions of the Software.
			//
			// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
			// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
			// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
			// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
			// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
			// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
			// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
			// NOPMD
			this.myarray = elts;
			this.nbelem = elts.Length;
		}

		/// <summary>Create a Vector with a given capacity.</summary>
		/// <param name="size">the capacity of the vector.</param>
		public Vec(int size)
		{
			this.myarray = (T[])new object[size];
		}

		/// <summary>
		/// Construit un vecteur contenant de taille size rempli a l'aide de size
		/// pad.
		/// </summary>
		/// <param name="size">la taille du vecteur</param>
		/// <param name="pad">l'objet servant a remplir le vecteur</param>
		public Vec(int size, T pad)
		{
			this.myarray = (T[])new object[size];
			for (int i = 0; i < size; i++)
			{
				this.myarray[i] = pad;
			}
			this.nbelem = size;
		}

		public int Size()
		{
			return this.nbelem;
		}

		/// <summary>Remove nofelems from the Vector.</summary>
		/// <remarks>
		/// Remove nofelems from the Vector. It is assumed that the number of
		/// elements to remove is smaller or equals to the current number of elements
		/// in the vector
		/// </remarks>
		/// <param name="nofelems">the number of elements to remove.</param>
		public void Shrink(int nofelems)
		{
			// assert nofelems <= nbelem;
			while (nofelems-- > 0)
			{
				this.myarray[--this.nbelem] = null;
			}
		}

		/// <summary>reduce the Vector to exactly newsize elements</summary>
		/// <param name="newsize">the new size of the vector.</param>
		public void ShrinkTo(int newsize)
		{
			// assert newsize <= size();
			for (int i = this.nbelem; i > newsize; i--)
			{
				this.myarray[i - 1] = null;
			}
			this.nbelem = newsize;
		}

		// assert size() == newsize;
		/// <summary>Pop the last element on the stack.</summary>
		/// <remarks>
		/// Pop the last element on the stack. It is assumed that the stack is not
		/// empty!
		/// </remarks>
		public void Pop()
		{
			// assert size() > 0;
			this.myarray[--this.nbelem] = null;
		}

		public void GrowTo(int newsize, T pad)
		{
			// assert newsize >= size();
			Ensure(newsize);
			for (int i = this.nbelem; i < newsize; i++)
			{
				this.myarray[i] = pad;
			}
			this.nbelem = newsize;
		}

		public void Ensure(int nsize)
		{
			if (nsize >= this.myarray.Length)
			{
				T[] narray = (T[])new object[Math.Max(nsize, this.nbelem * 2)];
				System.Array.Copy(this.myarray, 0, narray, 0, this.nbelem);
				this.myarray = narray;
			}
		}

		public IVec<T> Push(T elem)
		{
			Ensure(this.nbelem + 1);
			this.myarray[this.nbelem++] = elem;
			return this;
		}

		public void UnsafePush(T elem)
		{
			this.myarray[this.nbelem++] = elem;
		}

		/// <summary>Insert an element at the very beginning of the vector.</summary>
		/// <remarks>
		/// Insert an element at the very beginning of the vector. The former first
		/// element is appended to the end of the vector in order to have a constant
		/// time operation.
		/// </remarks>
		/// <param name="elem">the element to put first in the vector.</param>
		public void InsertFirst(T elem)
		{
			if (this.nbelem > 0)
			{
				Push(this.myarray[0]);
				this.myarray[0] = elem;
				return;
			}
			Push(elem);
		}

		public void InsertFirstWithShifting(T elem)
		{
			if (this.nbelem > 0)
			{
				Ensure(this.nbelem + 1);
				for (int i = this.nbelem; i > 0; i--)
				{
					this.myarray[i] = this.myarray[i - 1];
				}
				this.myarray[0] = elem;
				this.nbelem++;
				return;
			}
			Push(elem);
		}

		public void Clear()
		{
			Arrays.Fill(this.myarray, 0, this.nbelem, null);
			this.nbelem = 0;
		}

		/// <summary>return the latest element on the stack.</summary>
		/// <remarks>
		/// return the latest element on the stack. It is assumed that the stack is
		/// not empty!
		/// </remarks>
		/// <returns>the last element on the stack (the one on the top)</returns>
		public T Last()
		{
			// assert size() != 0;
			return this.myarray[this.nbelem - 1];
		}

		public T Get(int index)
		{
			return this.myarray[index];
		}

		public void Set(int index, T elem)
		{
			this.myarray[index] = elem;
		}

		/// <summary>Remove an element that belongs to the Vector.</summary>
		/// <param name="elem">an element from the vector.</param>
		public void Remove(T elem)
		{
			// assert size() > 0;
			int j = 0;
			for (; this.myarray[j] != elem; j++)
			{
				if (j == Size())
				{
					throw new NoSuchElementException();
				}
			}
			// arraycopy is always faster than manual copy
			System.Array.Copy(this.myarray, j + 1, this.myarray, j, Size() - j - 1);
			this.myarray[--this.nbelem] = null;
		}

		/// <summary>Remove an element that belongs to the Vector.</summary>
		/// <remarks>
		/// Remove an element that belongs to the Vector. The method will break if
		/// the element does not belong to the vector. While
		/// <see cref="Vec{T}.Remove(object)"/>
		/// look
		/// from the beginning of the vector, this method starts from the end of the
		/// vector.
		/// </remarks>
		/// <param name="elem">an element from the vector.</param>
		public void RemoveFromLast(T elem)
		{
			int j = this.nbelem - 1;
			for (; this.myarray[j] != elem; j--)
			{
				if (j == -1)
				{
					throw new NoSuchElementException();
				}
			}
			// arraycopy is always faster than manual copy
			System.Array.Copy(this.myarray, j + 1, this.myarray, j, Size() - j - 1);
			this.myarray[--this.nbelem] = null;
		}

		/// <summary>Delete the ith element of the vector.</summary>
		/// <remarks>
		/// Delete the ith element of the vector. The latest element of the vector
		/// replaces the removed element at the ith indexer.
		/// </remarks>
		/// <param name="index">the indexer of the element in the vector</param>
		/// <returns>
		/// the former ith element of the vector that is now removed from the
		/// vector
		/// </returns>
		public T Delete(int index)
		{
			// assert index >= 0 && index < nbelem;
			T ith = this.myarray[index];
			this.myarray[index] = this.myarray[--this.nbelem];
			this.myarray[this.nbelem] = null;
			return ith;
		}

		/// <summary>Copy the content of the vector to another vector.</summary>
		/// <remarks>
		/// Copy the content of the vector to another vector.
		/// THIS METHOD IS NOT SPECIALLY EFFICIENT. USE WITH CAUTION.
		/// </remarks>
		/// <param name="copy">a non null vector</param>
		public void CopyTo(IVec<T> copy)
		{
			Org.Sat4j.Core.Vec<T> ncopy = (Org.Sat4j.Core.Vec<T>)copy;
			int nsize = this.nbelem + ncopy.nbelem;
			copy.Ensure(nsize);
			System.Array.Copy(this.myarray, 0, ncopy.myarray, ncopy.nbelem, this.nbelem);
			ncopy.nbelem = nsize;
		}

		/// <summary>Copy the content of the vector to an array.</summary>
		/// <remarks>
		/// Copy the content of the vector to an array.
		/// THIS METHOD IS NOT SPECIALLY EFFICIENT. USE WITH CAUTION.
		/// </remarks>
		/// <param name="dest">
		/// a non null array, containing sufficient space to copy the
		/// content of the current vector, i.e.
		/// <code>dest.length &gt;= this.size()</code>.
		/// </param>
		public void CopyTo<E>(E[] dest)
		{
			// assert dest.length >= nbelem;
			System.Array.Copy(this.myarray, 0, dest, 0, this.nbelem);
		}

		/*
		* Copy one vector to another (cleaning the first), in constant time.
		*/
		public void MoveTo(IVec<T> dest)
		{
			CopyTo(dest);
			Clear();
		}

		public void MoveTo(int dest, int source)
		{
			if (dest != source)
			{
				this.myarray[dest] = this.myarray[source];
				this.myarray[source] = null;
			}
		}

		public T[] ToArray()
		{
			// DLB findbugs ok
			return this.myarray;
		}

		private int nbelem;

		private T[] myarray;

		/*
		* (non-Javadoc)
		*
		* @see java.lang.Object#toString()
		*/
		public override string ToString()
		{
			StringBuilder stb = new StringBuilder();
			for (int i = 0; i < this.nbelem - 1; i++)
			{
				stb.Append(this.myarray[i]);
				stb.Append(",");
			}
			//$NON-NLS-1$
			if (this.nbelem > 0)
			{
				stb.Append(this.myarray[this.nbelem - 1]);
			}
			return stb.ToString();
		}

		internal void SelectionSort(int from, int to, IComparer<T> cmp)
		{
			int i;
			int j;
			int besti;
			T tmp;
			for (i = from; i < to - 1; i++)
			{
				besti = i;
				for (j = i + 1; j < to; j++)
				{
					if (cmp.Compare(this.myarray[j], this.myarray[besti]) < 0)
					{
						besti = j;
					}
				}
				tmp = this.myarray[i];
				this.myarray[i] = this.myarray[besti];
				this.myarray[besti] = tmp;
			}
		}

		internal void Sort(int from, int to, IComparer<T> cmp)
		{
			int width = to - from;
			if (width <= 15)
			{
				SelectionSort(from, to, cmp);
			}
			else
			{
				T pivot = this.myarray[width / 2 + from];
				T tmp;
				int i = from - 1;
				int j = to;
				for (; ; )
				{
					do
					{
						i++;
					}
					while (cmp.Compare(this.myarray[i], pivot) < 0);
					do
					{
						j--;
					}
					while (cmp.Compare(pivot, this.myarray[j]) < 0);
					if (i >= j)
					{
						break;
					}
					tmp = this.myarray[i];
					this.myarray[i] = this.myarray[j];
					this.myarray[j] = tmp;
				}
				Sort(from, i, cmp);
				Sort(i, to, cmp);
			}
		}

		/// <summary>Sort the vector according to a given order on the elements.</summary>
		/// <param name="comparator">a way to order the elements of the vector</param>
		public void Sort(IComparer<T> comparator)
		{
			Sort(0, this.nbelem, comparator);
		}

		public void SortUnique(IComparer<T> cmp)
		{
			int i;
			int j;
			T last;
			if (this.nbelem == 0)
			{
				return;
			}
			Sort(0, this.nbelem, cmp);
			i = 1;
			last = this.myarray[0];
			for (j = 1; j < this.nbelem; j++)
			{
				if (cmp.Compare(last, this.myarray[j]) < 0)
				{
					last = this.myarray[i] = this.myarray[j];
					i++;
				}
			}
			this.nbelem = i;
		}

		/*
		* (non-Javadoc)
		*
		* @see java.lang.Object#equals(java.lang.Object)
		*/
		public override bool Equals(object obj)
		{
			if (obj is IVec<object>)
			{
				IVec<object> v = (IVec<object>)obj;
				if (v.Size() != Size())
				{
					return false;
				}
				for (int i = 0; i < Size(); i++)
				{
					if (!v.Get(i).Equals(Get(i)))
					{
						return false;
					}
				}
				return true;
			}
			return false;
		}

		/*
		* (non-Javadoc)
		*
		* @see java.lang.Object#hashCode()
		*/
		public override int GetHashCode()
		{
			int sum = 0;
			for (int i = 0; i < this.nbelem; i++)
			{
				sum += this.myarray[i].GetHashCode() / this.nbelem;
			}
			return sum;
		}

		public IEnumerator<T> Iterator()
		{
			return new _IEnumerator_493(this);
		}

		private sealed class _IEnumerator_493 : IEnumerator<T>
		{
			public _IEnumerator_493(Vec<T> _enclosing)
			{
				this._enclosing = _enclosing;
				this.i = 0;
			}

			private int i;

			public bool HasNext()
			{
				return this.i < this._enclosing._enclosing.nbelem;
			}

			public T Next()
			{
				if (this.i == this._enclosing._enclosing.nbelem)
				{
					throw new NoSuchElementException();
				}
				return this._enclosing._enclosing.myarray[this.i++];
			}

			public void Remove()
			{
				throw new NotSupportedException();
			}

			private readonly Vec<T> _enclosing;
		}

		public bool IsEmpty()
		{
			return this.nbelem == 0;
		}

		/// <since>2.1</since>
		public bool Contains(T e)
		{
			for (int i = 0; i < this.nbelem; i++)
			{
				if (this.myarray[i].Equals(e))
				{
					return true;
				}
			}
			return false;
		}

		/// <since>2.2</since>
		public int IndexOf(T element)
		{
			for (int i = 0; i < this.nbelem; i++)
			{
				if (this.myarray[i].Equals(element))
				{
					return i;
				}
			}
			return -1;
		}

		public IVec<T> Clone()
		{
			IVec<T> cloned = new Org.Sat4j.Core.Vec<T>(this.Size());
			this.CopyTo(cloned);
			return cloned;
		}
	}
}
