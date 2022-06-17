using System;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Restarts
{
	/// <summary>Minisat original restart strategy.</summary>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class MiniSATRestarts : RestartStrategy
	{
		private const long serialVersionUID = 1L;

		private double nofConflicts;

		private SearchParams @params;

		private int conflictcount;

		public void Init(SearchParams theParams, SolverStats stats)
		{
			this.@params = theParams;
			this.nofConflicts = theParams.GetInitConflictBound();
			this.conflictcount = 0;
		}

		public long NextRestartNumberOfConflict()
		{
			return Math.Round(this.nofConflicts);
		}

		public void OnRestart()
		{
			this.nofConflicts *= this.@params.GetConflictBoundIncFactor();
		}

		public override string ToString()
		{
			return "MiniSAT restarts strategy";
		}

		public bool ShouldRestart()
		{
			return this.conflictcount >= this.nofConflicts;
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
