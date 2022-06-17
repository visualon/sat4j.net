using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>The most general abstraction for handling a constraint.</summary>
	/// <author>leberre</author>
	public interface IConstr
	{
		/// <returns>true iff the clause was learnt during the search</returns>
		bool Learnt();

		/// <returns>the number of literals in the constraint.</returns>
		int Size();

		/// <summary>returns the ith literal in the constraint</summary>
		/// <param name="i">the index of the literal</param>
		/// <returns>a literal</returns>
		int Get(int i);

		/// <summary>To obtain the activity of the constraint.</summary>
		/// <returns>the activity of the clause.</returns>
		/// <since>2.1</since>
		double GetActivity();

		/// <summary>Partition constraints into the ones that only propagate once (e.g.</summary>
		/// <remarks>
		/// Partition constraints into the ones that only propagate once (e.g.
		/// clauses and cardinality constraints) and the ones that can be propagated
		/// several times (e.g. pseudo-boolean constraints).
		/// </remarks>
		/// <returns>
		/// true if the constraint can propagate literals at different
		/// decision levels.
		/// </returns>
		/// <since>2.3.1</since>
		bool CanBePropagatedMultipleTimes();

		/// <summary>
		/// Produces a human readable representation of the constraint, using a
		/// specific mapping.
		/// </summary>
		/// <param name="mapper">
		/// a textual (potentially partial) representation of the Dimacs
		/// variables.
		/// </param>
		/// <returns>a textual representation of the constraint.</returns>
		/// <since>2.3.6</since>
		string ToString(VarMapper mapper);
	}
}
