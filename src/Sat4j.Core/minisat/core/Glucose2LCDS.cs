using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	[System.Serializable]
	internal class Glucose2LCDS<D> : GlucoseLCDS<D>
		where D : DataStructureFactory
	{
		private readonly Solver<D> solver;

		private const long serialVersionUID = 1L;

		internal Glucose2LCDS(Solver<D> solver, ConflictTimer timer)
			: base(solver, timer)
		{
			this.solver = solver;
		}

		public override string ToString()
		{
			return "Glucose 2 learned constraints deletion strategy (LBD updated on propagation) with timer " + GetTimer();
		}

		public override void OnPropagation(Constr from)
		{
			if (from.GetActivity() > 2.0)
			{
				int nblevel = ComputeLBD(from);
				if (nblevel < from.GetActivity())
				{
					solver.stats.IncUpdateLBD();
					from.SetActivity(nblevel);
				}
			}
		}
	}
}
