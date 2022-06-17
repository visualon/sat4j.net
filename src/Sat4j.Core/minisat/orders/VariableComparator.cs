using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	public interface VariableComparator
	{
		/// <summary>Compare two variables according to a heuristic.</summary>
		/// <param name="a">a variable id</param>
		/// <param name="b">a variable id</param>
		/// <returns>true iff a &lt; b (a is preferred to b)</returns>
		bool PreferredTo(int a, int b);
	}
}
