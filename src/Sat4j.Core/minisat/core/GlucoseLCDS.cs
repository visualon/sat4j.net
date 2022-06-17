using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	[System.Serializable]
	internal class GlucoseLCDS<D> : LearnedConstraintsDeletionStrategy
		where D : DataStructureFactory
	{
		private readonly Solver<D> solver;

		private const long serialVersionUID = 1L;

		private int[] flags = new int[0];

		private int flag = 0;

		private readonly ConflictTimer timer;

		internal GlucoseLCDS(Solver<D> solver, ConflictTimer timer)
		{
			// private int wall = 0;
			this.solver = solver;
			this.timer = timer;
		}

		public virtual void Reduce(IVec<Constr> learnedConstrs)
		{
			this.solver.SortOnActivity();
			int i;
			int j;
			for (i = j = learnedConstrs.Size() / 2; i < learnedConstrs.Size(); i++)
			{
				Constr c = learnedConstrs.Get(i);
				if (c.Locked() || c.GetActivity() <= 2.0)
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
				solver.@out.Log(this.solver.GetLogPrefix() + "cleaning " + (learnedConstrs.Size() - j) + " clauses out of " + learnedConstrs.Size() + " with flag " + this.flag + "/" + solver.stats.GetConflicts());
			}
			//$NON-NLS-1$
			//$NON-NLS-1$ //$NON-NLS-2$
			// out.flush();
			solver.learnts.ShrinkTo(j);
		}

		public virtual ConflictTimer GetTimer()
		{
			return this.timer;
		}

		public override string ToString()
		{
			return "Glucose learned constraints deletion strategy with timer " + timer;
		}

		public virtual void Init()
		{
			int howmany = solver.voc.NVars();
			// wall = constrs.size() > 10000 ? constrs.size() : 10000;
			if (this.flags.Length <= howmany)
			{
				this.flags = new int[howmany + 1];
			}
			this.flag = 0;
			this.timer.Reset();
		}

		public virtual void OnClauseLearning(Constr constr)
		{
			int nblevel = ComputeLBD(constr);
			constr.IncActivity(nblevel);
		}

		protected internal virtual int ComputeLBD(Constr constr)
		{
			int nblevel = 1;
			this.flag++;
			int currentLevel;
			for (int i = 1; i < constr.Size(); i++)
			{
				currentLevel = solver.voc.GetLevel(constr.Get(i));
				if (this.flags[currentLevel] != this.flag)
				{
					this.flags[currentLevel] = this.flag;
					nblevel++;
				}
			}
			return nblevel;
		}

		public virtual void OnConflictAnalysis(Constr reason)
		{
		}

		public virtual void OnPropagation(Constr from)
		{
		}
	}
}
