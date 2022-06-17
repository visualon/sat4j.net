using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Learning
{
	/// <summary>
	/// Selects the constraints to learn according to its length as a percentage of
	/// the total number of variables in the solver universe.
	/// </summary>
	/// <author>daniel</author>
	[System.Serializable]
	public sealed class PercentLengthLearning<D> : LimitedLearning<D>
		where D : DataStructureFactory
	{
		private const long serialVersionUID = 1L;

		private int maxpercent;

		private int bound;

		public PercentLengthLearning()
			: this(10)
		{
		}

		public PercentLengthLearning(int percent)
		{
			this.maxpercent = percent;
		}

		public void SetLimit(int percent)
		{
			this.maxpercent = percent;
		}

		public int GetLimit()
		{
			return this.maxpercent;
		}

		public override void Init()
		{
			base.Init();
			SetBound(this.lits.RealnVars() * this.maxpercent / 100);
		}

		public override string ToString()
		{
			return "Limit learning to clauses of size smaller or equal to " + this.maxpercent + "% of the number of variables";
		}

		//$NON-NLS-1$
		//$NON-NLS-1$
		protected internal void SetBound(int newbound)
		{
			this.bound = newbound;
		}

		protected internal override bool LearningCondition(Constr constr)
		{
			return constr.Size() <= this.bound;
		}
	}
}
