using System;
using System.Collections.Generic;
using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>An abstraction for the vector of int used on the library.</summary>
	/// <author>leberre</author>
	public interface IVecInt
	{
		int Size();

		/// <summary>Remove the latest nofelems elements from the vector.</summary>
		/// <param name="nofelems">the number of elements to remove.</param>
		void Shrink(int nofelems);

		/// <summary>Reduce the vector to a given number of elements.</summary>
		/// <remarks>
		/// Reduce the vector to a given number of elements.
		/// All the elements after the limit are remove from the vector.
		/// </remarks>
		/// <param name="newsize">the new size of the vector</param>
		void ShrinkTo(int newsize);

		/// <summary>Removes the last element of the vector.</summary>
		/// <remarks>
		/// Removes the last element of the vector.
		/// If the vector is empty, does nothing.
		/// </remarks>
		/// <returns>the vector itself, for method chaining.</returns>
		IVecInt Pop();

		void GrowTo(int newsize, int pad);

		void Ensure(int nsize);

		IVecInt Push(int elem);

		/// <summary>Push the element in the Vector without verifying if there is room for it.</summary>
		/// <remarks>
		/// Push the element in the Vector without verifying if there is room for it.
		/// USE WITH CAUTION!
		/// </remarks>
		/// <param name="elem">an integer</param>
		void UnsafePush(int elem);

		int UnsafeGet(int eleem);

		void Clear();

		int Last();

		int Get(int i);

		void Set(int i, int o);

		bool Contains(int e);

		/// <summary>Retrieve the index of an element.</summary>
		/// <param name="e">an element</param>
		/// <returns>the index of the element, -1 if not found</returns>
		/// <since>2.2</since>
		int IndexOf(int e);

		/// <summary>returns the index of the first occurrence of e, else -1.</summary>
		/// <param name="e">an integer</param>
		/// <returns>the index i such that <code>get(i)==e, else -1</code>.</returns>
		int ContainsAt(int e);

		/// <summary>
		/// returns the index of the first occurrence of e occurring after from
		/// (excluded), else -1.
		/// </summary>
		/// <param name="e">an integer</param>
		/// <param name="from">the index to start from (excluded).</param>
		/// <returns>
		/// the index i such that
		/// <code>i&gt;from and get(i)==e, else -1</code>
		/// </returns>
		int ContainsAt(int e, int from);

		/// <summary>Copy the content of the vector to another vector.</summary>
		/// <remarks>
		/// Copy the content of the vector to another vector.
		/// THIS METHOD IS NOT SPECIALLY EFFICIENT. USE WITH CAUTION.
		/// </remarks>
		/// <param name="copy">a non null vector of integers</param>
		void CopyTo(IVecInt copy);

		/// <summary>Copy the content of the vector to an array.</summary>
		/// <remarks>
		/// Copy the content of the vector to an array.
		/// THIS METHOD IS NOT SPECIALLY EFFICIENT. USE WITH CAUTION.
		/// </remarks>
		/// <param name="is">
		/// a non null array, containing sufficient space to copy the
		/// content of the current vector, i.e.
		/// <code>is.length &gt;= this.size()</code>.
		/// </param>
		void CopyTo(int[] @is);

		/// <summary>
		/// Move the content of the current vector to a destination one, in constant
		/// time.
		/// </summary>
		/// <param name="dest">a vector of integer.</param>
		void MoveTo(IVecInt dest);

		void MoveTo(int sourceStartingIndex, int[] dest);

		void MoveTo2(IVecInt dest);

		void MoveTo(int[] dest);

		/// <summary>Move elements inside the vector.</summary>
		/// <remarks>
		/// Move elements inside the vector. The content of the method is equivalent
		/// to: <code>vec[dest] = vec[source]</code>
		/// </remarks>
		/// <param name="dest">the index of the destination</param>
		/// <param name="source">the index of the source</param>
		void MoveTo(int dest, int source);

		/// <summary>Insert an element at the very begining of the vector.</summary>
		/// <remarks>
		/// Insert an element at the very begining of the vector. The former first
		/// element is appended to the end of the vector in order to have a constant
		/// time operation.
		/// </remarks>
		/// <param name="elem">the element to put first in the vector.</param>
		void InsertFirst(int elem);

		/// <summary>Remove an element from the vector.</summary>
		/// <param name="elem">an element of the vector</param>
		/// <exception cref="Sharpen.NoSuchElementException">if elem is not found in the vector.</exception>
		void Remove(int elem);

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
		int Delete(int i);

		void Sort();

		void Sort(IComparer<int> comparator);

		void SortUnique();

		/// <summary>To know if a vector is empty</summary>
		/// <returns>true iff the vector is empty.</returns>
		/// <since>1.6</since>
		bool IsEmpty();

		IteratorInt Iterator();

		/// <summary>Allow to access the internal representation of the vector as an array.</summary>
		/// <remarks>
		/// Allow to access the internal representation of the vector as an array.
		/// Note that only the content of index 0 to size() should be taken into
		/// account. USE WITH CAUTION
		/// </remarks>
		/// <returns>the internal representation of the Vector as an array.</returns>
		/// <since>2.1</since>
		int[] ToArray();

		/// <summary>Compute all subsets of cardinal k of the vector.</summary>
		/// <param name="k">a cardinal (<code>k&lt;= vec.size()</code>)</param>
		/// <returns>an array of IVectInt representing each a k-subset of this vector.</returns>
		/// <author>sroussel</author>
		/// <since>2.3.1</since>
		IVecInt[] Subset(int k);

		/// <summary>Clone the object.</summary>
		/// <returns>a copy of the object.</returns>
		/// <since>2.3.6</since>
		IVecInt Clone();
	}
}
