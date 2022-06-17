using System.Collections.Generic;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	[System.Serializable]
	internal sealed class SizeLCDS : LearnedConstraintsDeletionStrategy
	{
		private const long serialVersionUID = 1L;

		private readonly ConflictTimer timer;

		private readonly Solver<DataStructureFactory> solver;

		private static readonly IComparer<Constr> comparator = new SizeComparator();

		internal SizeLCDS(Solver<DataStructureFactory> solver, ConflictTimer timer)
		{
			this.timer = timer;
			this.solver = solver;
		}

		public void Reduce(IVec<Constr> learnedConstrs)
		{
			solver.learnts.Sort(comparator);
			int i;
			int j;
			for (i = j = learnedConstrs.Size() / 2; i < learnedConstrs.Size(); i++)
			{
				Constr c = learnedConstrs.Get(i);
				if (c.Locked() || c.Size() == 2)
				{
					learnedConstrs.Set(j++, solver.learnts.Get(i));
				}
				else
				{
					c.Remove(solver);
					solver.slistener.Delete(c);
				}
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
			return "Sized based learned constraints deletion strategy with timer " + timer;
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
