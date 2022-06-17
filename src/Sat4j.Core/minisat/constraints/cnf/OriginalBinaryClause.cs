using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Cnf
{
	/// <since>2.1</since>
	[System.Serializable]
	public class OriginalBinaryClause : BinaryClause
	{
		private const long serialVersionUID = 1L;

		public OriginalBinaryClause(IVecInt ps, ILits voc)
			: base(ps, voc)
		{
		}

		public override void SetLearnt()
		{
		}

		// do nothing
		public override bool Learnt()
		{
			return false;
		}

		/// <summary>Creates a brand new clause, presumably from external data.</summary>
		/// <param name="s">the object responsible for unit propagation</param>
		/// <param name="voc">the vocabulary</param>
		/// <param name="literals">the literals to store in the clause</param>
		/// <returns>
		/// the created clause or null if the clause should be ignored
		/// (tautology for example)
		/// </returns>
		public static Org.Sat4j.Minisat.Constraints.Cnf.OriginalBinaryClause BrandNewClause(UnitPropagationListener s, ILits voc, IVecInt literals)
		{
			Org.Sat4j.Minisat.Constraints.Cnf.OriginalBinaryClause c = new Org.Sat4j.Minisat.Constraints.Cnf.OriginalBinaryClause(literals, voc);
			c.Register();
			return c;
		}

		public override void ForwardActivity(double claInc)
		{
			this.activity += claInc;
		}

		/// <param name="claInc"/>
		public override void IncActivity(double claInc)
		{
		}

		public override void SetActivity(double claInc)
		{
		}
		// do nothing
	}
}
