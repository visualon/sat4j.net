using System;
using System.Collections.Generic;
using System.Text;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Core
{
	/// <summary>A vector specific for primitive integers, widely used in the solver.</summary>
	/// <remarks>
	/// A vector specific for primitive integers, widely used in the solver. Note
	/// that if the vector has a sort method, the operations on the vector DO NOT
	/// preserve sorting.
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class VecInt : IVecInt
	{
		private const long serialVersionUID = 1L;

		public static readonly IVecInt Empty = new EmptyVecInt();

		public VecInt()
			: this(5)
		{
		}

		public VecInt(int size)
		{
			/*
			* Created on 9 oct. 2003
			*/
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
			this.myarray = new int[size];
		}

		/// <summary>Adapter method to translate an array of int into an IVecInt.</summary>
		/// <remarks>
		/// Adapter method to translate an array of int into an IVecInt.
		/// The array is used inside the VecInt, so the elements may be modified
		/// outside the VecInt. But it should not take much memory.The size of the
		/// created VecInt is the length of the array.
		/// </remarks>
		/// <param name="lits">a filled array of int.</param>
		public VecInt(int[] lits)
		{
			// NOPMD
			this.myarray = lits;
			this.nbelem = lits.Length;
		}

		/// <summary>Build a vector of a given initial size filled with an integer.</summary>
		/// <param name="size">the initial size of the vector</param>
		/// <param name="pad">the integer to fill the vector with</param>
		public VecInt(int size, int pad)
		{
			this.myarray = new int[size];
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

		/// <summary>Remove the latest nofelems elements from the vector</summary>
		/// <param name="nofelems">the number of elements to remove</param>
		public void Shrink(int nofelems)
		{
			// assert nofelems >= 0;
			// assert nofelems <= size();
			this.nbelem -= nofelems;
		}

		public void ShrinkTo(int newsize)
		{
			// assert newsize >= 0;
			// assert newsize < nbelem;
			this.nbelem = newsize;
		}

		/// <summary>depile le dernier element du vecteur.</summary>
		/// <remarks>
		/// depile le dernier element du vecteur. Si le vecteur est vide, ne fait
		/// rien.
		/// </remarks>
		public IVecInt Pop()
		{
			// assert size() != 0;
			--this.nbelem;
			return this;
		}

		public void GrowTo(int newsize, int pad)
		{
			// assert newsize > size();
			Ensure(newsize);
			while (--newsize >= 0)
			{
				this.myarray[this.nbelem++] = pad;
			}
		}

		public void Ensure(int nsize)
		{
			if (nsize >= this.myarray.Length)
			{
				int[] narray = new int[Math.Max(nsize, this.nbelem * 2)];
				System.Array.Copy(this.myarray, 0, narray, 0, this.nbelem);
				this.myarray = narray;
			}
		}

		public IVecInt Push(int elem)
		{
			Ensure(this.nbelem + 1);
			this.myarray[this.nbelem++] = elem;
			return this;
		}

		public void UnsafePush(int elem)
		{
			this.myarray[this.nbelem++] = elem;
		}

		public void Clear()
		{
			this.nbelem = 0;
		}

		public int Last()
		{
			// assert nbelem > 0;
			return this.myarray[this.nbelem - 1];
		}

		public int Get(int i)
		{
			// assert i >= 0 && i < nbelem;
			return this.myarray[i];
		}

		public int UnsafeGet(int i)
		{
			return this.myarray[i];
		}

		public void Set(int i, int o)
		{
			System.Diagnostics.Debug.Assert(i >= 0 && i < this.nbelem);
			this.myarray[i] = o;
		}

		public bool Contains(int e)
		{
			int[] workArray = this.myarray;
			// dvh, faster access
			for (int i = 0; i < this.nbelem; i++)
			{
				if (workArray[i] == e)
				{
					return true;
				}
			}
			return false;
		}

		/// <since>2.2</since>
		public int IndexOf(int e)
		{
			int[] workArray = this.myarray;
			// dvh, faster access
			for (int i = 0; i < this.nbelem; i++)
			{
				if (workArray[i] == e)
				{
					return i;
				}
			}
			return -1;
		}

		public int ContainsAt(int e)
		{
			return ContainsAt(e, -1);
		}

		public int ContainsAt(int e, int from)
		{
			int[] workArray = this.myarray;
			// dvh, faster access
			for (int i = from + 1; i < this.nbelem; i++)
			{
				if (workArray[i] == e)
				{
					return i;
				}
			}
			return -1;
		}

		/// <summary>Copy the content of this vector into another one.</summary>
		/// <remarks>
		/// Copy the content of this vector into another one. Uses Java
		/// <see cref="System.Array.Copy(object, int, object, int, int)"/>
		/// to make the copy.
		/// </remarks>
		/// <param name="copy">another VecInt vector</param>
		public void CopyTo(IVecInt copy)
		{
			Org.Sat4j.Core.VecInt ncopy = (Org.Sat4j.Core.VecInt)copy;
			int nsize = this.nbelem + ncopy.nbelem;
			ncopy.Ensure(nsize);
			System.Array.Copy(this.myarray, 0, ncopy.myarray, ncopy.nbelem, this.nbelem);
			ncopy.nbelem = nsize;
		}

		/// <summary>Copy the content of this vector into an array of integer.</summary>
		/// <remarks>
		/// Copy the content of this vector into an array of integer. Uses Java
		/// <see cref="System.Array.Copy(object, int, object, int, int)"/>
		/// to make the copy.
		/// </remarks>
		/// <param name="is">the target array.</param>
		public void CopyTo(int[] @is)
		{
			// assert is.length >= nbelem;
			System.Array.Copy(this.myarray, 0, @is, 0, this.nbelem);
		}

		public void MoveTo(IVecInt dest)
		{
			CopyTo(dest);
			this.nbelem = 0;
		}

		public void MoveTo2(IVecInt dest)
		{
			Org.Sat4j.Core.VecInt ndest = (Org.Sat4j.Core.VecInt)dest;
			int[] tmp = ndest.myarray;
			ndest.myarray = this.myarray;
			ndest.nbelem = this.nbelem;
			this.myarray = tmp;
			this.nbelem = 0;
		}

		public void MoveTo(int dest, int source)
		{
			this.myarray[dest] = this.myarray[source];
		}

		public void MoveTo(int[] dest)
		{
			System.Array.Copy(this.myarray, 0, dest, 0, this.nbelem);
			this.nbelem = 0;
		}

		public void MoveTo(int sourceStartingIndex, int[] dest)
		{
			System.Array.Copy(this.myarray, sourceStartingIndex, dest, 0, this.nbelem - sourceStartingIndex);
			this.nbelem = 0;
		}

		/// <summary>Insert an element at the very beginning of the vector.</summary>
		/// <remarks>
		/// Insert an element at the very beginning of the vector. The former first
		/// element is appended to the end of the vector in order to have a constant
		/// time operation.
		/// </remarks>
		/// <param name="elem">the element to put first in the vector.</param>
		public void InsertFirst(int elem)
		{
			if (this.nbelem > 0)
			{
				Push(this.myarray[0]);
				this.myarray[0] = elem;
				return;
			}
			Push(elem);
		}

		/// <summary>Remove an element that belongs to the Vector.</summary>
		/// <remarks>
		/// Remove an element that belongs to the Vector. The method will break if
		/// the element does not belong to the vector.
		/// </remarks>
		/// <param name="elem">an element from that VecInt</param>
		public void Remove(int elem)
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
			System.Array.Copy(this.myarray, j + 1, this.myarray, j, Size() - j - 1);
			Pop();
		}

		/// <summary>Delete the ith element of the vector.</summary>
		/// <remarks>
		/// Delete the ith element of the vector. The latest element of the vector
		/// replaces the removed element at the ith indexer.
		/// </remarks>
		/// <param name="i">the indexer of the element in the vector</param>
		/// <returns>
		/// the former ith element of the vector that is now removed from the
		/// vector
		/// </returns>
		public int Delete(int i)
		{
			// assert i >= 0 && i < nbelem;
			int ith = this.myarray[i];
			this.myarray[i] = this.myarray[--this.nbelem];
			return ith;
		}

		private int nbelem;

		private int[] myarray;

		/*
		* (non-Javadoc)
		*
		* @see java.lang.int#toString()
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

		internal void SelectionSort(int from, int to)
		{
			int i;
			int j;
			int besti;
			int tmp;
			for (i = from; i < to - 1; i++)
			{
				besti = i;
				for (j = i + 1; j < to; j++)
				{
					if (this.myarray[j] < this.myarray[besti])
					{
						besti = j;
					}
				}
				tmp = this.myarray[i];
				this.myarray[i] = this.myarray[besti];
				this.myarray[besti] = tmp;
			}
		}

		internal void Sort(int from, int to)
		{
			int width = to - from;
			if (width <= 15)
			{
				SelectionSort(from, to);
			}
			else
			{
				int[] locarray = this.myarray;
				int pivot = locarray[width / 2 + from];
				int tmp;
				int i = from - 1;
				int j = to;
				for (; ; )
				{
					do
					{
						i++;
					}
					while (locarray[i] < pivot);
					do
					{
						j--;
					}
					while (pivot < locarray[j]);
					if (i >= j)
					{
						break;
					}
					tmp = locarray[i];
					locarray[i] = locarray[j];
					locarray[j] = tmp;
				}
				Sort(from, i);
				Sort(i, to);
			}
		}

		/// <summary>sort the vector using a custom quicksort.</summary>
		public void Sort()
		{
			Sort(0, this.nbelem);
		}

		public void SortUnique()
		{
			int i;
			int j;
			int last;
			if (this.nbelem == 0)
			{
				return;
			}
			Sort(0, this.nbelem);
			i = 1;
			int[] locarray = this.myarray;
			last = locarray[0];
			for (j = 1; j < this.nbelem; j++)
			{
				if (last < locarray[j])
				{
					last = locarray[i] = locarray[j];
					i++;
				}
			}
			this.nbelem = i;
		}

		/// <summary>Two vectors are equals iff they have the very same elements in the order.</summary>
		/// <param name="obj">an object</param>
		/// <returns>
		/// true iff obj is a VecInt and has the same elements as this vector
		/// at each index.
		/// </returns>
		/// <seealso cref="object.Equals(object)"/>
		public override bool Equals(object obj)
		{
			if (obj is IVecInt)
			{
				IVecInt v = (IVecInt)obj;
				if (v.Size() != this.nbelem)
				{
					return false;
				}
				for (int i = 0; i < this.nbelem; i++)
				{
					if (v.Get(i) != this.myarray[i])
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
			long sum = 0;
			for (int i = 0; i < this.nbelem; i++)
			{
				sum += this.myarray[i];
			}
			return (int)sum / this.nbelem;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.specs.IVecInt2#pushAll(org.sat4j.specs.IVecInt2)
		*/
		public void PushAll(IVecInt vec)
		{
			Org.Sat4j.Core.VecInt nvec = (Org.Sat4j.Core.VecInt)vec;
			int nsize = this.nbelem + nvec.nbelem;
			Ensure(nsize);
			System.Array.Copy(nvec.myarray, 0, this.myarray, this.nbelem, nvec.nbelem);
			this.nbelem = nsize;
		}

		/// <summary>to detect that the vector is a subset of another one.</summary>
		/// <remarks>
		/// to detect that the vector is a subset of another one. Note that the
		/// method assumes that the two vectors are sorted!
		/// </remarks>
		/// <param name="vec">a vector</param>
		/// <returns>true iff the current vector is a subset of vec</returns>
		public bool IsSubsetOf(Org.Sat4j.Core.VecInt vec)
		{
			int i = 0;
			int j = 0;
			while (i < this.nbelem && j < vec.nbelem)
			{
				while (j < vec.nbelem && vec.myarray[j] < this.myarray[i])
				{
					j++;
				}
				if (j == vec.nbelem || this.myarray[i] != vec.myarray[j])
				{
					return false;
				}
				i++;
			}
			return true;
		}

		public IteratorInt Iterator()
		{
			return new _IteratorInt_521(this);
		}

		private sealed class _IteratorInt_521 : IteratorInt
		{
			public _IteratorInt_521(VecInt _enclosing)
			{
				this._enclosing = _enclosing;
				this.i = 0;
			}

			private int i;

			public bool HasNext()
			{
				return this.i < this._enclosing.nbelem;
			}

			public int Next()
			{
				if (this.i == this._enclosing.nbelem)
				{
					throw new NoSuchElementException();
				}
				return this._enclosing.myarray[this.i++];
			}

			private readonly VecInt _enclosing;
		}

		public bool IsEmpty()
		{
			return this.nbelem == 0;
		}

		/// <since>2.1</since>
		public int[] ToArray()
		{
			return this.myarray;
		}

		/// <since>2.3.1</since>
		/// <author>sroussel</author>
		public IVecInt[] Subset(int cardinal)
		{
			IList<IVecInt> liste = new AList<IVecInt>();
			IVecInt[] result;
			if (cardinal == 1)
			{
				result = new Org.Sat4j.Core.VecInt[this.Size()];
				for (int i = 0; i < this.Size(); i++)
				{
					result[i] = new Org.Sat4j.Core.VecInt(new int[] { this.Get(i) });
				}
				return result;
			}
			if (this.Size() == 0)
			{
				result = new Org.Sat4j.Core.VecInt[0];
				return result;
			}
			Org.Sat4j.Core.VecInt subVec = new Org.Sat4j.Core.VecInt();
			Org.Sat4j.Core.VecInt newVec;
			this.CopyTo(subVec);
			subVec.Remove(this.Get(0));
			foreach (IVecInt vecWithFirst in subVec.Subset(cardinal - 1))
			{
				newVec = new Org.Sat4j.Core.VecInt();
				vecWithFirst.CopyTo(newVec);
				newVec.InsertFirst(this.Get(0));
				liste.Add(newVec);
			}
			foreach (IVecInt vecWithoutFirst in subVec.Subset(cardinal))
			{
				liste.Add(vecWithoutFirst);
			}
			result = new Org.Sat4j.Core.VecInt[liste.Count];
			for (int i_1 = 0; i_1 < liste.Count; i_1++)
			{
				result[i_1] = liste[i_1];
			}
			return result;
		}

		internal void SelectionSort(int from, int to, IComparer<int> cmp)
		{
			int i;
			int j;
			int besti;
			int tmp;
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

		internal void Sort(int from, int to, IComparer<int> cmp)
		{
			int width = to - from;
			if (width <= 15)
			{
				SelectionSort(from, to, cmp);
			}
			else
			{
				int pivot = this.myarray[width / 2 + from];
				int tmp;
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

		/// <summary>Sort the vector according to a given order.</summary>
		/// <param name="comparator">a way to order the integers of that vector.</param>
		public void Sort(IComparer<int> comparator)
		{
			Sort(0, this.nbelem, comparator);
		}

		public IVecInt Clone()
		{
			IVecInt cloned = new Org.Sat4j.Core.VecInt(this.Size());
			this.CopyTo(cloned);
			return cloned;
		}
	}
}
