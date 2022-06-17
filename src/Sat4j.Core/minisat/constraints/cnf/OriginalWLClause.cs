using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Cnf
{
	[System.Serializable]
	public sealed class OriginalWLClause : WLClause
	{
		public OriginalWLClause(IVecInt ps, ILits voc)
			: base(ps, voc)
		{
		}

		private const long serialVersionUID = 1L;

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.constraints.cnf.WLClause#register()
		*/
		public override void Register()
		{
			System.Diagnostics.Debug.Assert(this.lits.Length > 1);
			this.voc.Watch(this.lits[0] ^ 1, this);
			this.voc.Watch(this.lits[1] ^ 1, this);
		}

		public override bool Learnt()
		{
			return false;
		}

		public override void SetLearnt()
		{
		}

		// do nothing
		/// <summary>Creates a brand new clause, presumably from external data.</summary>
		/// <param name="s">the object responsible for unit propagation</param>
		/// <param name="voc">the vocabulary</param>
		/// <param name="literals">the literals to store in the clause</param>
		/// <returns>
		/// the created clause or null if the clause should be ignored
		/// (tautology for example)
		/// </returns>
		public static Org.Sat4j.Minisat.Constraints.Cnf.OriginalWLClause BrandNewClause(UnitPropagationListener s, ILits voc, IVecInt literals)
		{
			Org.Sat4j.Minisat.Constraints.Cnf.OriginalWLClause c = new Org.Sat4j.Minisat.Constraints.Cnf.OriginalWLClause(literals, voc);
			c.Register();
			return c;
		}

		/// <since>2.1</since>
		public override void ForwardActivity(double claInc)
		{
			this.activity += claInc;
		}

		/// <param name="claInc"/>
		public override void IncActivity(double claInc)
		{
		}

		private int savedindex = 2;

		public override bool PropagatePI(MandatoryLiteralListener s, int p)
		{
			int[] mylits = this.lits;
			// Lits[1] must contain a falsified literal
			if (mylits[0] == (p ^ 1))
			{
				mylits[0] = mylits[1];
				mylits[1] = p ^ 1;
			}
			// assert mylits[1] == (p ^ 1);
			int previous = p ^ 1;
			// look for a new satisfied literal to watch
			for (int i = savedindex; i < mylits.Length; i++)
			{
				if (this.voc.IsSatisfied(mylits[i]))
				{
					mylits[1] = mylits[i];
					mylits[i] = previous;
					this.voc.Watch(mylits[1] ^ 1, this);
					savedindex = i + 1;
					return true;
				}
			}
			// the clause is now either unit
			this.voc.Watch(p, this);
			// first literal is mandatory
			s.IsMandatory(mylits[0]);
			return true;
		}

		public override bool Propagate(UnitPropagationListener s, int p)
		{
			this.savedindex = 2;
			return base.Propagate(s, p);
		}
	}
}
