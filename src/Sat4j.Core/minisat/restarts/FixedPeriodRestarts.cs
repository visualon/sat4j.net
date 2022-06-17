using System;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Restarts
{
	[System.Serializable]
	public class FixedPeriodRestarts : RestartStrategy
	{
		private const long serialVersionUID = 1L;

		private long conflictCount;

		private long period;

		public virtual void Reset()
		{
			conflictCount = 0;
		}

		public virtual void NewConflict()
		{
			conflictCount++;
		}

		public virtual void Init(SearchParams @params, SolverStats stats)
		{
			this.conflictCount = 0;
		}

		[Obsolete]
		public virtual long NextRestartNumberOfConflict()
		{
			return period;
		}

		public virtual bool ShouldRestart()
		{
			return conflictCount >= period;
		}

		public virtual void OnRestart()
		{
			this.conflictCount = 0;
		}

		public virtual void OnBackjumpToRootLevel()
		{
		}

		public virtual void NewLearnedClause(Constr learned, int trailLevel)
		{
		}

		public virtual long GetPeriod()
		{
			return period;
		}

		public virtual void SetPeriod(long period)
		{
			this.period = period;
		}

		public override string ToString()
		{
			return "constant restarts strategy every " + this.period + " conflicts";
		}
	}
}
