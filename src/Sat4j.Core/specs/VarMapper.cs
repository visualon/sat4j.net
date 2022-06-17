using Sharpen;

namespace Org.Sat4j.Specs
{
	public interface VarMapper
	{
		/// <summary>Map a Dimacs boolean variable to a specific textual representation.</summary>
		/// <remarks>
		/// Map a Dimacs boolean variable to a specific textual representation. If
		/// none is found, the value of var will be returned as text.
		/// </remarks>
		/// <param name="var">a Dimacs variable</param>
		/// <returns>a textual representation of that var</returns>
		/// <since>2.3.6</since>
		string Map(int var);
	}
}
