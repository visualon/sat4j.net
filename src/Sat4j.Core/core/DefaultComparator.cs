using System;
using System.Collections.Generic;
using Sharpen;

namespace Org.Sat4j.Core
{
	/// <summary>A simple comparator for comparable objects.</summary>
	/// <author>daniel</author>
	/// <?/>
	[System.Serializable]
	public sealed class DefaultComparator<A> : IComparer<A>
		where A : IComparable<A>
	{
		private const long serialVersionUID = 1L;

		public int Compare(A a, A b)
		{
			return a.CompareTo(b);
		}
	}
}
