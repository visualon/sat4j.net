using System.Collections.Generic;
using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	/// <summary>
	/// This heuristic allows to order the selection of the variables using different
	/// levels.
	/// </summary>
	/// <author>leberre</author>
	[System.Serializable]
	public class LevelBasedVarOrderHeap : VarOrderHeap
	{
		private const long serialVersionUID = 1L;

		private int[] level;

		private readonly IList<IVecInt> varsByLevel = new AList<IVecInt>();

		public LevelBasedVarOrderHeap()
		{
		}

		public LevelBasedVarOrderHeap(IPhaseSelectionStrategy strategy)
			: base(strategy)
		{
		}

		protected internal override Heap CreateHeap(double[] activity)
		{
			return new Heap(new LevelAndActivityVariableComparator(activity, level));
		}

		/// <summary>Add a new level with vars</summary>
		/// <param name="vars"/>
		public virtual void AddLevel(IVecInt vars)
		{
			this.varsByLevel.Add(vars.Clone());
		}

		public virtual void AddLevel(int[] vars)
		{
			this.varsByLevel.Add(new VecInt(vars));
		}

		public override void Init()
		{
			// fill in level array
			int nlength = this.lits.NVars() + 1;
			if (level == null || level.Length < nlength)
			{
				this.level = new int[nlength];
				for (int i = 0; i < nlength; i++)
				{
					level[i] = int.MaxValue;
				}
			}
			IVecInt currentLevel;
			for (int i_1 = 1; i_1 <= varsByLevel.Count; i_1++)
			{
				currentLevel = varsByLevel[i_1 - 1];
				for (int j = 0; j < currentLevel.Size(); j++)
				{
					level[currentLevel.Get(j)] = i_1;
				}
			}
			base.Init();
		}

		public override string ToString()
		{
			return "Level and activity based heuristics using a heap " + this.phaseStrategy;
		}
		//$NON-NLS-1$
	}
}
