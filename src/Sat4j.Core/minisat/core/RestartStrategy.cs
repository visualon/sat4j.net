using System;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Abstraction allowing to choose various restarts strategies.</summary>
	/// <author>leberre</author>
	public interface RestartStrategy : ConflictTimer
	{
		/// <summary>Hook method called just before the search starts.</summary>
		/// <param name="params">the user's search parameters.</param>
		/// <param name="stats">
		/// some statistics about the search (number of conflicts,
		/// restarts, etc).
		/// </param>
		void Init(SearchParams @params, SolverStats stats);

		/// <summary>Ask for the next restart in number of conflicts.</summary>
		/// <remarks>Ask for the next restart in number of conflicts. Deprecated since 2.3.2</remarks>
		/// <returns>the delay in conflicts before the next restart.</returns>
		[Obsolete]
		long NextRestartNumberOfConflict();

		/// <summary>Ask the strategy if the solver should restart.</summary>
		/// <returns>true if the solver should restart, else false.</returns>
		bool ShouldRestart();

		/// <summary>
		/// Hook method called when a restart occurs (once the solver has backtracked
		/// to top decision level).
		/// </summary>
		void OnRestart();

		/// <summary>Called when the solver backjumps to the root level.</summary>
		/// <since>2.3.2</since>
		void OnBackjumpToRootLevel();

		/// <summary>
		/// Callback method called when a new clause is learned by the solver, after
		/// conflict analysis.
		/// </summary>
		/// <param name="learned">the new clause</param>
		/// <param name="trailLevel">the number of literals assigned when the conflict occurred.</param>
		/// <since>2.3.3</since>
		void NewLearnedClause(Constr learned, int trailLevel);
	}
}
