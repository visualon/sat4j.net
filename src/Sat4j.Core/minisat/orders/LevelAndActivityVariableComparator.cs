using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	public class LevelAndActivityVariableComparator : VariableComparator
	{
		private readonly double[] activity;

		private readonly int[] level;

		public LevelAndActivityVariableComparator(double[] activity, int[] level)
		{
			this.activity = activity;
			this.level = level;
		}

		public virtual bool PreferredTo(int a, int b)
		{
			return level[a] < level[b] || (level[a] == level[b] && this.activity[a] > this.activity[b]);
		}

		public override string ToString()
		{
			return "Level and activity based variable heuristic";
		}
	}
}
