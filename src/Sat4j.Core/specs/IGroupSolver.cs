using System.Collections.Generic;
using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>Represents a CNF in which clauses are grouped into levels.</summary>
	/// <remarks>
	/// Represents a CNF in which clauses are grouped into levels. It was first used
	/// to build a high level MUS solver for SAT 2011 competition.
	/// </remarks>
	/// <author>leberre</author>
	/// <since>2.3.3</since>
	public interface IGroupSolver : ISolver
	{
		/// <param name="literals">a clause</param>
		/// <param name="groupId">
		/// the level of the clause set. The specific level 0 is used to
		/// denote hard clauses.
		/// </param>
		/// <returns>on object representing that clause in the solver.</returns>
		/// <exception cref="ContradictionException"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		IConstr AddClause(IVecInt literals, int groupId);

		/// <returns>the list of Dimacs variables created for the group solver.</returns>
		/// <since>2.3.6</since>
		ICollection<int> GetAddedVars();
	}
}
