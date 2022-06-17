using Org.Sat4j.Minisat.Core;
using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	[System.Serializable]
	public class SubsetVarOrder : VarOrderHeap
	{
		private readonly int[] varsToTest;

		private bool[] inSubset;

		public SubsetVarOrder(int[] varsToTest)
		{
			this.varsToTest = new int[varsToTest.Length];
			System.Array.Copy(varsToTest, 0, this.varsToTest, 0, varsToTest.Length);
		}

		private const long serialVersionUID = 1L;

		public override void Init()
		{
			int nlength = this.lits.NVars() + 1;
			if (this.activity == null || this.activity.Length < nlength)
			{
				this.activity = new double[nlength];
			}
			this.inSubset = new bool[nlength];
			this.phaseStrategy.Init(nlength);
			this.activity[0] = -1;
			this.heap = new Heap(new ActivityBasedVariableComparator(this.activity));
			this.heap.SetBounds(nlength);
			foreach (int var in this.varsToTest)
			{
				System.Diagnostics.Debug.Assert(var > 0);
				System.Diagnostics.Debug.Assert(var <= this.lits.NVars(), string.Empty + this.lits.NVars() + "/" + var);
				//$NON-NLS-1$ //$NON-NLS-2$
				this.inSubset[var] = true;
				this.activity[var] = 0.0;
				if (this.lits.BelongsToPool(var))
				{
					this.heap.Insert(var);
				}
			}
		}

		public override void Undo(int x)
		{
			if (this.inSubset[x] && !this.heap.InHeap(x))
			{
				this.heap.Insert(x);
			}
		}
	}
}
