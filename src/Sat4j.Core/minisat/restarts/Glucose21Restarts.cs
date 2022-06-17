using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Restarts
{
	/// <summary>
	/// Dynamic restart strategy of Glucose 2.1 as presented in Refining restarts
	/// strategies for SAT and UNSAT formulae.
	/// </summary>
	/// <remarks>
	/// Dynamic restart strategy of Glucose 2.1 as presented in Refining restarts
	/// strategies for SAT and UNSAT formulae. Gilles Audemard and Laurent Simon, in
	/// CP'2012.
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public class Glucose21Restarts : RestartStrategy
	{
		private const long serialVersionUID = 1L;

		private readonly CircularBuffer bufferLBD = new CircularBuffer(50);

		private readonly CircularBuffer bufferTrail = new CircularBuffer(5000);

		private long sumOfAllLBD = 0;

		private SolverStats stats;

		public virtual void Reset()
		{
			sumOfAllLBD = 0;
			bufferLBD.Clear();
			bufferTrail.Clear();
		}

		public virtual void NewConflict()
		{
		}

		public virtual void NewLearnedClause(Constr learned, int trailLevel)
		{
			// on conflict
			int lbd = (int)learned.GetActivity();
			bufferLBD.Push(lbd);
			sumOfAllLBD += lbd;
			bufferTrail.Push(trailLevel);
			// was
			// ... trailLevel > 1.4 * bufferTrail.average()
			// uses now only integers to avoid rounding issues
			if (stats.GetConflicts() > 10000 && bufferTrail.IsFull() && trailLevel * 5L > 7L * bufferTrail.Average())
			{
				bufferLBD.Clear();
			}
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
			// was
			// ... && bufferLBD.average() * 0.8 > sumOfAllLBD / stats.conflicts
			// uses now only integers to avoid rounding issues
			return bufferLBD.IsFull() && bufferLBD.Average() * stats.GetConflicts() * 4L > sumOfAllLBD * 5L;
		}

		public virtual void OnRestart()
		{
			bufferLBD.Clear();
		}

		public virtual void OnBackjumpToRootLevel()
		{
		}

		public override string ToString()
		{
			return "Glucose 2.1 dynamic restart strategy";
		}
	}
}
