using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Mimics the checks found in Glucose.</summary>
	/// <author>daniel</author>
	[System.Serializable]
	internal sealed class LBDConflictTimer : ConflictTimerAdapter
	{
		private const long serialVersionUID = 1L;

		private int nbconflict = 0;

		private const int MaxClause = 5000;

		private const int IncClause = 1000;

		private int nextbound = MaxClause;

		internal LBDConflictTimer(Solver<DataStructureFactory> solver, int bound)
			: base(solver, bound)
		{
		}

		public override void Run()
		{
			this.nbconflict += Bound();
			if (this.nbconflict >= this.nextbound)
			{
				this.nextbound += IncClause;
				// if (nextbound > wall) {
				// nextbound = wall;
				// }
				this.nbconflict = 0;
				GetSolver().SetNeedToReduceDB(true);
			}
		}

		public override void Reset()
		{
			base.Reset();
			this.nextbound = MaxClause;
			if (this.nbconflict >= this.nextbound)
			{
				this.nbconflict = 0;
				GetSolver().SetNeedToReduceDB(true);
			}
		}

		public override string ToString()
		{
			return "check every " + Bound() + " if the learned constraints reach increasing bounds: " + MaxClause + " step " + IncClause;
		}
	}
}
