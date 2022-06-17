using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Utility class to be used to return the two results of a conflict analysis.</summary>
	/// <author>daniel</author>
	[System.Serializable]
	public sealed class Pair
	{
		private const long serialVersionUID = 1L;

		private int backtrackLevel;

		private Constr reason;

		public int GetBacktrackLevel()
		{
			return backtrackLevel;
		}

		public void SetBacktrackLevel(int backtrackLevel)
		{
			this.backtrackLevel = backtrackLevel;
		}

		public Constr GetReason()
		{
			return reason;
		}

		public void SetReason(Constr reason)
		{
			this.reason = reason;
		}
	}
}
