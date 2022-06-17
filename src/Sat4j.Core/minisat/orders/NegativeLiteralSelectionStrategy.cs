using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	[System.Serializable]
	public sealed class NegativeLiteralSelectionStrategy : IPhaseSelectionStrategy
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
			return LiteralsUtils.NegLit(var);
		}

		public void UpdateVar(int p)
		{
		}

		public override string ToString()
		{
			return "negative phase selection";
		}

		public void UpdateVarAtDecisionLevel(int q)
		{
		}
	}
}
