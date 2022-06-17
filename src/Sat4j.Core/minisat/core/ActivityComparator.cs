using System;
using System.Collections.Generic;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Utility class to sort the constraints according to their activity.</summary>
	/// <author>daniel</author>
	[System.Serializable]
	public class ActivityComparator : IComparer<Constr>
	{
		private const long serialVersionUID = 1L;

		/*
		* (non-Javadoc)
		*
		* @see java.util.Comparator#compare(java.lang.Object, java.lang.Object)
		*/
		public virtual int Compare(Constr c1, Constr c2)
		{
			long delta = Math.Round(c1.GetActivity() - c2.GetActivity());
			if (delta == 0)
			{
				return c1.Size() - c2.Size();
			}
			return (int)delta;
		}
	}
}
