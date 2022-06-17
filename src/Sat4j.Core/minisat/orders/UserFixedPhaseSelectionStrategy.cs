using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	/// <summary>
	/// Selection strategy where the phase selection is decided at init time and is
	/// not updated during the search.
	/// </summary>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class UserFixedPhaseSelectionStrategy : AbstractPhaserecordingSelectionStrategy
	{
		private const long serialVersionUID = 1L;

		public override void AssignLiteral(int p)
		{
		}

		public override void UpdateVar(int p)
		{
		}

		public override string ToString()
		{
			return "Fixed selection strategy.";
		}

		public override void UpdateVarAtDecisionLevel(int q)
		{
		}
	}
}
