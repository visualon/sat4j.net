using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	public class ActivityBasedVariableComparator : VariableComparator
	{
		private readonly double[] activity;

		public ActivityBasedVariableComparator(double[] activity)
		{
			this.activity = activity;
		}

		public virtual bool PreferredTo(int a, int b)
		{
			return this.activity[a] > this.activity[b];
		}

		public override string ToString()
		{
			return "Activity-based variable heuristic";
		}
	}
}
