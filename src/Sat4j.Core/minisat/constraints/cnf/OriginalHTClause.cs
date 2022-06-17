using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Cnf
{
	/// <since>2.1</since>
	[System.Serializable]
	public class OriginalHTClause : HTClause
	{
		public OriginalHTClause(IVecInt ps, ILits voc)
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
			this.voc.Watch(LiteralsUtils.Neg(this.head), this);
			this.voc.Watch(LiteralsUtils.Neg(this.tail), this);
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
		public static Org.Sat4j.Minisat.Constraints.Cnf.OriginalHTClause BrandNewClause(UnitPropagationListener s, ILits voc, IVecInt literals)
		{
			Org.Sat4j.Minisat.Constraints.Cnf.OriginalHTClause c = new Org.Sat4j.Minisat.Constraints.Cnf.OriginalHTClause(literals, voc);
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
		public override bool PropagatePI(MandatoryLiteralListener l, int p)
		{
			if (this.head == LiteralsUtils.Neg(p))
			{
				int[] mylits = this.middleLits;
				// moving head on the right
				while (savedindexhead < mylits.Length && this.voc.IsFalsified(mylits[savedindexhead]))
				{
					savedindexhead++;
				}
				System.Diagnostics.Debug.Assert(savedindexhead <= mylits.Length);
				if (savedindexhead == mylits.Length)
				{
					l.IsMandatory(this.tail);
					return true;
				}
				this.head = mylits[savedindexhead];
				mylits[savedindexhead] = LiteralsUtils.Neg(p);
				this.voc.Watch(LiteralsUtils.Neg(this.head), this);
				return true;
			}
			System.Diagnostics.Debug.Assert(this.tail == LiteralsUtils.Neg(p));
			int[] mylits_1 = this.middleLits;
			// moving tail on the left
			while (savedindextail >= 0 && this.voc.IsFalsified(mylits_1[savedindextail]))
			{
				savedindextail--;
			}
			System.Diagnostics.Debug.Assert(-1 <= savedindextail);
			if (-1 == savedindextail)
			{
				l.IsMandatory(this.head);
				return true;
			}
			this.tail = mylits_1[savedindextail];
			mylits_1[savedindextail] = LiteralsUtils.Neg(p);
			this.voc.Watch(LiteralsUtils.Neg(this.tail), this);
			return true;
		}

		private int savedindexhead;

		private int savedindextail;

		public override bool Propagate(UnitPropagationListener s, int p)
		{
			this.savedindexhead = 0;
			this.savedindextail = middleLits.Length - 1;
			return base.Propagate(s, p);
		}
	}
}
