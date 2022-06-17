using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Perform a task when a given number of conflicts is reached.</summary>
	/// <author>daniel</author>
	[System.Serializable]
	public abstract class ConflictTimerAdapter : ConflictTimer
	{
		private const long serialVersionUID = 1L;

		private int counter;

		private readonly int bound;

		private readonly Solver<DataStructureFactory> solver;

		public ConflictTimerAdapter(Solver<DataStructureFactory> solver, int bound)
		{
			this.bound = bound;
			this.counter = 0;
			this.solver = solver;
		}

		public virtual void Reset()
		{
			this.counter = 0;
		}

		public virtual void NewConflict()
		{
			this.counter++;
			if (this.counter == this.bound)
			{
				Run();
				this.counter = 0;
			}
		}

		public abstract void Run();

		public virtual Solver<DataStructureFactory> GetSolver()
		{
			return this.solver;
		}

		public virtual int Bound()
		{
			return this.bound;
		}
	}
}
