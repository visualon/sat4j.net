using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Constraints;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Minisat.Learning;
using Org.Sat4j.Minisat.Orders;
using Org.Sat4j.Minisat.Restarts;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j
{
	/// <summary>
	/// That class is the entry point to the default, best performing configuration
	/// of SAT4J.
	/// </summary>
	/// <remarks>
	/// That class is the entry point to the default, best performing configuration
	/// of SAT4J. Since there is only one concrete reference per strategy used inside
	/// the solver, it is a good start for people willing to reduce the library to
	/// the minimal number of classes needed to run a SAT solver. The main method
	/// allow to run the solver from the command line.
	/// </remarks>
	/// <author>leberre</author>
	/// <since>2.2</since>
	[System.Serializable]
	public class LightFactory : ASolverFactory<ISolver>
	{
		private const long serialVersionUID = 1460304168178023681L;

		private static LightFactory instance;

		private static void CreateInstance()
		{
			lock (typeof(LightFactory))
			{
				if (instance == null)
				{
					instance = new LightFactory();
				}
			}
		}

		/// <summary>Access to the single instance of the factory.</summary>
		/// <returns>the singleton of that class.</returns>
		public static LightFactory Instance()
		{
			if (instance == null)
			{
				CreateInstance();
			}
			return instance;
		}

		public override ISolver DefaultSolver()
		{
			MiniSATLearning<DataStructureFactory> learning = new MiniSATLearning<DataStructureFactory>();
			Solver<DataStructureFactory> solver = new Solver<DataStructureFactory>(learning, new MixedDataStructureDanielWL(), new VarOrderHeap(new RSATPhaseSelectionStrategy()), new ArminRestarts());
			learning.SetSolver(solver);
			solver.SetSimplifier(solver.ExpensiveSimplification);
			solver.SetSearchParams(new SearchParams(1.1, 100));
			return solver;
		}

		public override ISolver LightSolver()
		{
			return DefaultSolver();
		}

		public static void Main(string[] args)
		{
			AbstractLauncher lanceur = new BasicLauncher<ISolver>(LightFactory.Instance());
			if (args.Length != 1)
			{
				lanceur.Usage();
				return;
			}
			lanceur.Run(args);
			System.Environment.Exit(lanceur.GetExitCode().Value());
		}
	}
}
