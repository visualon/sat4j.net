using System;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Restarts
{
	/// <summary>
	/// Rapid restart strategy presented by Armin Biere during it's SAT 07 invited
	/// talk.
	/// </summary>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class ArminRestarts : RestartStrategy
	{
		private const long serialVersionUID = 1L;

		private double inner;

		private double outer;

		private long conflicts;

		private SearchParams @params;

		private long conflictcount = 0;

		public void Init(SearchParams theParams, SolverStats stats)
		{
			this.@params = theParams;
			this.inner = theParams.GetInitConflictBound();
			this.outer = theParams.GetInitConflictBound();
			this.conflicts = Math.Round(this.inner);
		}

		public long NextRestartNumberOfConflict()
		{
			return this.conflicts;
		}

		public void OnRestart()
		{
			if (this.inner >= this.outer)
			{
				this.outer *= this.@params.GetConflictBoundIncFactor();
				this.inner = this.@params.GetInitConflictBound();
			}
			else
			{
				this.inner *= this.@params.GetConflictBoundIncFactor();
			}
			this.conflicts = Math.Round(this.inner);
			this.conflictcount = 0;
		}

		public override string ToString()
		{
			return "Armin Biere (Picosat) restarts strategy";
		}

		public bool ShouldRestart()
		{
			return this.conflictcount >= this.conflicts;
		}

		public void OnBackjumpToRootLevel()
		{
			this.conflictcount = 0;
		}

		public void Reset()
		{
			this.conflictcount = 0;
		}

		public void NewConflict()
		{
			this.conflictcount++;
		}

		public void NewLearnedClause(Constr learned, int trailLevel)
		{
		}
	}
}
