using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Learning
{
	/// <summary>Learn clauses with a great number of active variables.</summary>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class ActiveLearning<D> : LimitedLearning<D>
		where D : DataStructureFactory
	{
		private const long serialVersionUID = 1L;

		private double percent;

		private IOrder order;

		private int maxpercent;

		public ActiveLearning()
			: this(0.95)
		{
		}

		public ActiveLearning(double d)
		{
			this.percent = d;
		}

		public void SetOrder(IOrder order)
		{
			this.order = order;
		}

		public override void SetSolver(Solver<D> s)
		{
			base.SetSolver(s);
			this.order = s.GetOrder();
		}

		public void SetActivityPercent(double d)
		{
			this.percent = d;
		}

		public double GetActivityPercent()
		{
			return this.percent;
		}

		/*
		* (non-Javadoc)
		*
		* @see
		* org.sat4j.minisat.LimitedLearning#learningCondition(org.sat4j.minisat
		* .Constr)
		*/
		protected internal override bool LearningCondition(Constr clause)
		{
			int nbactivevars = 0;
			for (int i = 0; i < clause.Size(); i++)
			{
				if (this.order.VarActivity(clause.Get(i)) > 1)
				{
					nbactivevars++;
				}
			}
			return nbactivevars > clause.Size() * this.percent;
		}

		public override string ToString()
		{
			return "Limit learning to clauses containing active literals (" + this.percent * 100 + "%)";
		}

		//$NON-NLS-1$
		public void SetLimit(int percent)
		{
			this.maxpercent = percent;
		}

		public int GetLimit()
		{
			return this.maxpercent;
		}
	}
}
