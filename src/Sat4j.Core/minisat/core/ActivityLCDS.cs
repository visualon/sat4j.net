using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	[System.Serializable]
	internal sealed class ActivityLCDS : LearnedConstraintsDeletionStrategy
	{
		private const long serialVersionUID = 1L;

		private readonly ConflictTimer timer;

		private readonly Solver<DataStructureFactory> solver;

		internal ActivityLCDS(Solver<DataStructureFactory> solver, ConflictTimer timer)
		{
			this.timer = timer;
			this.solver = solver;
		}

		public void Reduce(IVec<Constr> learnedConstrs)
		{
			solver.SortOnActivity();
			int i;
			int j;
			for (i = j = 0; i < solver.learnts.Size() / 2; i++)
			{
				Constr c = solver.learnts.Get(i);
				if (c.Locked() || c.Size() == 2)
				{
					solver.learnts.Set(j++, solver.learnts.Get(i));
				}
				else
				{
					c.Remove(solver);
					solver.slistener.Delete(c);
				}
			}
			for (; i < solver.learnts.Size(); i++)
			{
				solver.learnts.Set(j++, solver.learnts.Get(i));
			}
			if (solver.IsVerbose())
			{
				solver.@out.Log(solver.GetLogPrefix() + "cleaning " + (solver.learnts.Size() - j) + " clauses out of " + solver.learnts.Size());
			}
			//$NON-NLS-1$
			//$NON-NLS-1$ 
			// out.flush();
			solver.learnts.ShrinkTo(j);
		}

		public ConflictTimer GetTimer()
		{
			return this.timer;
		}

		public override string ToString()
		{
			return "Activity based learned constraints deletion strategy with timer " + timer;
		}

		public void Init()
		{
		}

		// do nothing
		public void OnClauseLearning(Constr constr)
		{
		}

		// do nothing
		public void OnConflictAnalysis(Constr reason)
		{
			if (reason.Learnt())
			{
				solver.ClaBumpActivity(reason);
			}
		}

		public void OnPropagation(Constr from)
		{
		}
		// do nothing
	}
}
