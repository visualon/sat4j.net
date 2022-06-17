using System;
using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	/// <summary>
	/// The variable selection strategy randomly picks one phase, either positive or
	/// negative.
	/// </summary>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class RandomLiteralSelectionStrategy : IPhaseSelectionStrategy
	{
		private const long serialVersionUID = 1L;

		/// <since>2.2</since>
		public static readonly Random Rand = new Random(123456789);

		public void AssignLiteral(int p)
		{
		}

		public void Init(int nlength)
		{
		}

		public void Init(int var, int p)
		{
		}

		public int Select(int var)
		{
			if (Rand.NextDouble() > 0.5)
			{
				return LiteralsUtils.PosLit(var);
			}
			return LiteralsUtils.NegLit(var);
		}

		public void UpdateVar(int p)
		{
		}

		public void UpdateVarAtDecisionLevel(int q)
		{
		}

		public override string ToString()
		{
			return "random phase selection";
		}
	}
}
