using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Learning
{
	/// <summary>
	/// Learn only clauses which size is smaller than a percentage of the number of
	/// variables.
	/// </summary>
	/// <author>leberre</author>
	[System.Serializable]
	public abstract class LimitedLearning<D> : LearningStrategy<D>
		where D : DataStructureFactory
	{
		private const long serialVersionUID = 1L;

		private readonly NoLearningButHeuristics<D> none;

		private readonly MiniSATLearning<D> all;

		protected internal ILits lits;

		private SolverStats stats;

		public LimitedLearning()
		{
			this.none = new NoLearningButHeuristics<D>();
			this.all = new MiniSATLearning<D>();
		}

		public virtual void SetSolver(Solver<D> s)
		{
			if (s != null)
			{
				this.lits = s.GetVocabulary();
				SetVarActivityListener(s);
				this.all.SetDataStructureFactory(s.GetDSFactory());
				this.stats = s.GetStats();
			}
		}

		public virtual void Learns(Constr constr)
		{
			if (LearningCondition(constr))
			{
				this.all.Learns(constr);
			}
			else
			{
				this.none.Learns(constr);
				this.stats.IncIgnoredclauses();
			}
		}

		protected internal abstract bool LearningCondition(Constr constr);

		public virtual void Init()
		{
			this.all.Init();
			this.none.Init();
		}

		public virtual void SetVarActivityListener(VarActivityListener s)
		{
			this.none.SetVarActivityListener(s);
			this.all.SetVarActivityListener(s);
		}
	}
}
