using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	/// <author>
	/// leberre TODO To change the template for this generated type comment
	/// go to Window - Preferences - Java - Code Style - Code Templates
	/// </author>
	[System.Serializable]
	public sealed class PureOrder : VarOrderHeap
	{
		/// <summary>Comment for <code>serialVersionUID</code></summary>
		private const long serialVersionUID = 1L;

		private int period;

		private int cpt;

		public PureOrder()
			: this(20)
		{
		}

		public PureOrder(int p)
		{
			SetPeriod(p);
		}

		public void SetPeriod(int p)
		{
			this.period = p;
			this.cpt = this.period;
		}

		public int GetPeriod()
		{
			return this.period;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.VarOrder#select()
		*/
		public override int Select()
		{
			// wait period branching
			if (this.cpt < this.period)
			{
				this.cpt++;
			}
			else
			{
				// try to find a pure literal
				this.cpt = 0;
				int nblits = 2 * this.lits.NVars();
				for (int i = 2; i <= nblits; i++)
				{
					if (this.lits.IsUnassigned(i) && this.lits.Watches(i).Size() > 0 && this.lits.Watches(i ^ 1).Size() == 0)
					{
						return i;
					}
				}
			}
			// not found: using normal order
			return base.Select();
		}

		public override string ToString()
		{
			return "tries to first branch on a single phase watched unassigned variable (pure literal if using a CB data structure) else VSIDS from MiniSAT";
		}
		//$NON-NLS-1$
	}
}
