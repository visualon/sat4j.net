using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	[System.Serializable]
	public sealed class PositiveLiteralSelectionStrategy : IPhaseSelectionStrategy
	{
		private const long serialVersionUID = 1L;

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
			return LiteralsUtils.PosLit(var);
		}

		public void UpdateVar(int p)
		{
		}

		public override string ToString()
		{
			return "positive phase selection";
		}

		public void UpdateVarAtDecisionLevel(int q)
		{
		}
	}
}
