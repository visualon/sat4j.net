using System;
using System.Collections.Generic;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Utility class to sort the constraints according to their size.</summary>
	/// <remarks>
	/// Utility class to sort the constraints according to their size. Ties are
	/// broken according to the activity.
	/// </remarks>
	/// <author>daniel</author>
	[System.Serializable]
	public class SizeComparator : IComparer<Constr>
	{
		private const long serialVersionUID = 1L;

		/*
		* (non-Javadoc)
		*
		* @see java.util.Comparator#compare(java.lang.Object, java.lang.Object)
		*/
		public virtual int Compare(Constr c1, Constr c2)
		{
			int delta = c1.Size() - c2.Size();
			if (delta == 0)
			{
				return (int)Math.Round(c2.GetActivity() - c1.GetActivity());
			}
			return delta;
		}
	}
}
