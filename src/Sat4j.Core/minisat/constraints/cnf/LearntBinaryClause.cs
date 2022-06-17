using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Cnf
{
	/// <author>daniel</author>
	/// <since>2.1</since>
	[System.Serializable]
	public class LearntBinaryClause : BinaryClause
	{
		private const long serialVersionUID = 1L;

		public LearntBinaryClause(IVecInt ps, ILits voc)
			: base(ps, voc)
		{
		}

		public override void SetLearnt()
		{
		}

		// do nothing
		public override bool Learnt()
		{
			return true;
		}

		public override void ForwardActivity(double claInc)
		{
		}

		/// <param name="claInc"/>
		public override void IncActivity(double claInc)
		{
			this.activity += claInc;
		}

		public override void SetActivity(double d)
		{
			this.activity = d;
		}
	}
}
