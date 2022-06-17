using Org.Sat4j.Core;
using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	/// <summary>Keeps record of the phase of a variable in the lastest recorded clause.</summary>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class PhaseInLastLearnedClauseSelectionStrategy : AbstractPhaserecordingSelectionStrategy
	{
		private const long serialVersionUID = 1L;

		public override void UpdateVar(int p)
		{
			this.phase[LiteralsUtils.Var(p)] = p;
		}

		public override string ToString()
		{
			return "phase appearing in latest learned clause";
		}

		public override void AssignLiteral(int p)
		{
		}

		public override void UpdateVarAtDecisionLevel(int q)
		{
		}
	}
}
