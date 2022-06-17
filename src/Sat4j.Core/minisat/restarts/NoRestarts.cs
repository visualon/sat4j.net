using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Restarts
{
	/// <summary>Disable restarts in the solver.</summary>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class NoRestarts : RestartStrategy
	{
		private const long serialVersionUID = 1L;

		public void Init(SearchParams @params, SolverStats stats)
		{
		}

		public long NextRestartNumberOfConflict()
		{
			return long.MaxValue;
		}

		public void OnRestart()
		{
		}

		// do nothing
		public void Reset()
		{
		}

		// do nothing
		public void NewConflict()
		{
		}

		// do nothing
		public bool ShouldRestart()
		{
			return false;
		}

		public void OnBackjumpToRootLevel()
		{
		}

		// do nothing
		public override string ToString()
		{
			return "NoRestarts";
		}

		public void NewLearnedClause(Constr learned, int trailLevel)
		{
		}
	}
}
