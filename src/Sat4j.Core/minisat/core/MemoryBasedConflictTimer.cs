using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	[System.Serializable]
	internal sealed class MemoryBasedConflictTimer : ConflictTimerAdapter
	{
		private const long NoBoundComputedYet = -1L;

		private const long serialVersionUID = 1L;

		private long memorybound = NoBoundComputedYet;

		internal MemoryBasedConflictTimer(Solver<DataStructureFactory> solver, int bound)
			: base(solver, bound)
		{
		}

		private long GetMemoryBound()
		{
			if (memorybound == NoBoundComputedYet)
			{
				memorybound = Runtime.GetRuntime().FreeMemory() / 10;
			}
			return memorybound;
		}

		public override void Run()
		{
			long freemem = Runtime.GetRuntime().FreeMemory();
			if (freemem < GetMemoryBound())
			{
				// Reduce the set of learnt clauses
				GetSolver().SetNeedToReduceDB(true);
			}
		}

		public override string ToString()
		{
			return "check every " + Bound() + " if the memory bound " + this.memorybound + " is reached";
		}
	}
}
