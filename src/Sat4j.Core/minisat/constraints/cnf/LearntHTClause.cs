using System;
using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Cnf
{
	/// <author>daniel</author>
	/// <since>2.1</since>
	[System.Serializable]
	public class LearntHTClause : HTClause
	{
		public LearntHTClause(IVecInt ps, ILits voc)
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
			// looking for the literal to put in tail
			if (this.middleLits.Length > 0)
			{
				int maxi = 0;
				int maxlevel = this.voc.GetLevel(this.middleLits[0]);
				for (int i = 1; i < this.middleLits.Length; i++)
				{
					int level = this.voc.GetLevel(this.middleLits[i]);
					if (level > maxlevel)
					{
						maxi = i;
						maxlevel = level;
					}
				}
				if (maxlevel > this.voc.GetLevel(this.tail))
				{
					int l = this.tail;
					this.tail = this.middleLits[maxi];
					this.middleLits[maxi] = l;
				}
			}
			// attach both head and tail literals.
			this.voc.Watch(LiteralsUtils.Neg(this.head), this);
			this.voc.Watch(LiteralsUtils.Neg(this.tail), this);
		}

		public override bool Learnt()
		{
			return true;
		}

		public override void SetLearnt()
		{
		}

		// do nothing
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

		public override bool PropagatePI(MandatoryLiteralListener l, int p)
		{
			throw new NotSupportedException("Not implemented yet!");
		}
	}
}
