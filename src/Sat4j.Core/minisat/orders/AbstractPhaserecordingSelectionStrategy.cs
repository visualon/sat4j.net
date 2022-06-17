using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	[System.Serializable]
	public abstract class AbstractPhaserecordingSelectionStrategy : IPhaseSelectionStrategy
	{
		private const long serialVersionUID = 1L;

		protected internal int[] phase;

		public virtual void Init(int nlength)
		{
			if (this.phase == null || this.phase.Length < nlength)
			{
				this.phase = new int[nlength];
			}
			for (int i = 1; i < nlength; i++)
			{
				this.phase[i] = LiteralsUtils.NegLit(i);
			}
		}

		public virtual void Init(int var, int p)
		{
			this.phase[var] = p;
		}

		public virtual int Select(int var)
		{
			return this.phase[var];
		}

		public abstract void AssignLiteral(int arg1);

		public abstract void UpdateVar(int arg1);

		public abstract void UpdateVarAtDecisionLevel(int arg1);
	}
}
