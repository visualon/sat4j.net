using Org.Sat4j.Core;
using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	/// <since>2.2</since>
	[System.Serializable]
	public sealed class PhaseCachingAutoEraseStrategy : AbstractPhaserecordingSelectionStrategy
	{
		private const long serialVersionUID = 1L;

		public override void AssignLiteral(int p)
		{
			this.phase[LiteralsUtils.Var(p)] = p;
		}

		public override void UpdateVar(int p)
		{
			this.phase[LiteralsUtils.Var(p)] = p;
		}

		public override string ToString()
		{
			return "Phase caching with auto forget feature";
		}

		public override void UpdateVarAtDecisionLevel(int q)
		{
		}
	}
}
