using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Cnf
{
	[System.Serializable]
	public sealed class LearntWLClause : WLClause
	{
		public LearntWLClause(IVecInt ps, ILits voc)
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
			if (this.lits.Length == 0)
			{
				return;
			}
			System.Diagnostics.Debug.Assert(this.lits.Length > 1);
			// prendre un deuxieme litt???ral ??? surveiller
			int maxi = 1;
			int maxlevel = this.voc.GetLevel(this.lits[1]);
			for (int i = 2; i < this.lits.Length; i++)
			{
				int level = this.voc.GetLevel(this.lits[i]);
				if (level > maxlevel)
				{
					maxi = i;
					maxlevel = level;
				}
			}
			int l = this.lits[1];
			this.lits[1] = this.lits[maxi];
			this.lits[maxi] = l;
			// add really the clause inside the solver
			this.voc.Watch(this.lits[0] ^ 1, this);
			this.voc.Watch(this.lits[1] ^ 1, this);
		}

		public override bool Learnt()
		{
			return true;
		}

		public override void SetLearnt()
		{
		}

		// do nothing
		/// <since>2.1</since>
		public override void ForwardActivity(double claInc)
		{
		}

		/// <param name="claInc"/>
		public override void IncActivity(double claInc)
		{
			this.activity += claInc;
		}

		public override bool PropagatePI(MandatoryLiteralListener s, int p)
		{
			this.voc.Watch(p, this);
			return true;
		}
	}
}
