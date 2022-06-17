using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Learning
{
	/// <summary>
	/// A learning scheme for learning constraints of size smaller than a given
	/// constant.
	/// </summary>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class FixedLengthLearning<D> : LimitedLearning<D>
		where D : DataStructureFactory
	{
		private const long serialVersionUID = 1L;

		private int maxlength;

		private int bound;

		public FixedLengthLearning()
			: this(3)
		{
		}

		public FixedLengthLearning(int maxlength)
		{
			this.maxlength = maxlength;
		}

		/*
		* (non-Javadoc)
		*
		* @see
		* org.sat4j.minisat.LimitedLearning#learningCondition(org.sat4j.minisat
		* .Constr)
		*/
		public override void Init()
		{
			SetBound(this.maxlength);
		}

		public void SetMaxLength(int v)
		{
			this.maxlength = v;
		}

		public int GetMaxLength()
		{
			return this.maxlength;
		}

		public override string ToString()
		{
			return "Limit learning to clauses of size smaller or equal to " + this.maxlength;
		}

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
