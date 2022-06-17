using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Restarts
{
	/// <summary>
	/// Implementation of Exponential Moving Average to decide when to restart as
	/// presented by Armin Biere at PoS15, using Donald Knuth implementation avoiding
	/// floating point numbers.
	/// </summary>
	/// <remarks>
	/// Implementation of Exponential Moving Average to decide when to restart as
	/// presented by Armin Biere at PoS15, using Donald Knuth implementation avoiding
	/// floating point numbers.
	/// http://fmv.jku.at/biere/talks/Biere-POS15-talk.pdf
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public class EMARestarts : RestartStrategy
	{
		private const long serialVersionUID = 1L;

		private long fast;

		private long slow;

		private SolverStats stats;

		private long limit;

		public virtual void Reset()
		{
			fast = 0;
			slow = 0;
			limit = 50;
		}

		public virtual void NewConflict()
		{
		}

		public virtual void Init(SearchParams @params, SolverStats stats)
		{
			this.stats = stats;
			Reset();
		}

		public virtual long NextRestartNumberOfConflict()
		{
			return 0;
		}

		public virtual bool ShouldRestart()
		{
			return this.stats.GetConflicts() > limit && fast / 125 > slow / 100;
		}

		public virtual void OnRestart()
		{
			limit = this.stats.GetConflicts() + 50;
		}

		public virtual void OnBackjumpToRootLevel()
		{
		}

		public virtual void NewLearnedClause(Constr learned, int trailLevel)
		{
			int lbd = (int)learned.GetActivity();
			fast -= fast >> 5;
			fast += lbd << (32 - 5);
			slow -= slow >> 14;
			slow += lbd << (32 - 14);
		}

		public override string ToString()
		{
			return "Exponential Moving Average (EMA, Biere) restarts strategy";
		}
	}
}
