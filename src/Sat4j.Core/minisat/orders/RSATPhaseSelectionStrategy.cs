using Org.Sat4j.Core;
using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	/// <summary>Keeps track of the phase of the latest assignment.</summary>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class RSATPhaseSelectionStrategy : AbstractPhaserecordingSelectionStrategy
	{
		private const long serialVersionUID = 1L;

		public override void AssignLiteral(int p)
		{
			this.phase[LiteralsUtils.Var(p)] = p;
		}

		public override string ToString()
		{
			return "lightweight component caching from RSAT";
		}

		public override void UpdateVar(int p)
		{
		}

		public override void UpdateVarAtDecisionLevel(int p)
		{
		}
	}
}
