using System;
using System.Collections.Generic;
using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>An abstraction on the type of vector used in the library.</summary>
	/// <author>leberre</author>
	public interface IVec<T>
	{
		/// <returns>the number of elements contained in the vector</returns>
		int Size();

		/// <summary>Remove nofelems from the Vector.</summary>
		/// <remarks>
		/// Remove nofelems from the Vector. It is assumed that the number of
		/// elements to remove is smaller or equals to the current number of elements
		/// in the vector
		/// </remarks>
		/// <param name="nofelems">the number of elements to remove.</param>
		void Shrink(int nofelems);

		/// <summary>reduce the Vector to exactly newsize elements</summary>
		/// <param name="newsize">the new size of the vector.</param>
		void ShrinkTo(int newsize);

		/// <summary>Pop the last element on the stack.</summary>
		/// <remarks>
		/// Pop the last element on the stack. It is assumed that the stack is not
		/// empty!
		/// </remarks>
		void Pop();

		void GrowTo(int newsize, T pad);

		void Ensure(int nsize);

		IVec<T> Push(T elem);

		/// <summary>To push an element in the vector when you know you have space for it.</summary>
		/// <param name="elem">an element</param>
		void UnsafePush(T elem);

		/// <summary>Insert an element at the very begining of the vector.</summary>
		/// <remarks>
		/// Insert an element at the very begining of the vector. The former first
		/// element is appended to the end of the vector in order to have a constant
		/// time operation.
		/// </remarks>
		/// <param name="elem">the element to put first in the vector.</param>
		void InsertFirst(T elem);

		void InsertFirstWithShifting(T elem);

		void Clear();

		/// <summary>return the latest element on the stack.</summary>
		/// <remarks>
		/// return the latest element on the stack. It is assumed that the stack is
		/// not empty!
		/// </remarks>
		/// <returns>the last (top) element on the stack</returns>
		T Last();

		T Get(int i);

		void Set(int i, T o);

		/// <summary>Remove an element from the vector.</summary>
		/// <param name="elem">an element of the vector</param>
		/// <exception cref="Sharpen.NoSuchElementException">if elem is not found in the vector.</exception>
		void Remove(T elem);

		/// <summary>Remove an element from the vector, starting with the last element.</summary>
		/// <param name="elem">an element of the vector</param>
		/// <exception cref="Sharpen.NoSuchElementException">if elem is not found in the vector.</exception>
		/// <since>2.3.6</since>
		void RemoveFromLast(T elem);

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
		T Delete(int i);

		/// <summary>Copy the content of the vector to another vector.</summary>
		/// <remarks>
		/// Copy the content of the vector to another vector.
		/// THIS METHOD IS NOT SPECIALLY EFFICIENT. USE WITH CAUTION.
		/// </remarks>
		/// <param name="copy">a non null vector</param>
		void CopyTo(IVec<T> copy);

		/// <summary>Copy the content of the vector to an array.</summary>
		/// <remarks>
		/// Copy the content of the vector to an array.
		/// THIS METHOD IS NOT SPECIALLY EFFICIENT. USE WITH CAUTION.
		/// </remarks>
		/// <param name="dest">
		/// a non null array, containing sufficient space to copy the
		/// content of the current vector, i.e.
		/// <code>dest.length &lt;= this.size()</code>.
		/// </param>
		/// <?/>
		void CopyTo<E>(E[] dest);

		/// <summary>Allow to access the internal representation of the vector as an array.</summary>
		/// <remarks>
		/// Allow to access the internal representation of the vector as an array.
		/// Note that only the content of index 0 to size() should be taken into
		/// account. USE WITH CAUTION
		/// </remarks>
		/// <returns>the internal representation of the Vector as an array.</returns>
		T[] ToArray();

		/// <summary>Move the content of the vector into dest.</summary>
		/// <remarks>
		/// Move the content of the vector into dest. Note that the vector become
		/// empty. The content of the vector is appended to dest.
		/// </remarks>
		/// <param name="dest">the vector where top put the content of this vector</param>
		void MoveTo(IVec<T> dest);

		/// <summary>Move elements inside the vector.</summary>
		/// <remarks>
		/// Move elements inside the vector. The content of the method is equivalent
		/// to: <code>vec[dest] = vec[source]</code>
		/// </remarks>
		/// <param name="dest">the index of the destination</param>
		/// <param name="source">the index of the source</param>
		void MoveTo(int dest, int source);

		/*
		* @param comparator
		*/
		void Sort(IComparer<T> comparator);

		void SortUnique(IComparer<T> comparator);

		/// <summary>To know if a vector is empty</summary>
		/// <returns>true iff the vector is empty.</returns>
		/// <since>1.6</since>
		bool IsEmpty();

		IEnumerator<T> Iterator();

		/// <param name="element">an object</param>
		/// <returns>true iff element is found in the vector.</returns>
		/// <since>2.1</since>
		bool Contains(T element);

		/// <param name="element">an object</param>
		/// <returns>the index of the element if it is found in the vector, else -1.</returns>
		/// <since>2.2</since>
		int IndexOf(T element);

		/// <summary>Clone the object.</summary>
		/// <returns>a copy of the object.</returns>
		/// <since>2.3.6</since>
		IVec<T> Clone();
	}
}
