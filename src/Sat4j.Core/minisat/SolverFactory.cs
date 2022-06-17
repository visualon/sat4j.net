using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Constraints;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Minisat.Learning;
using Org.Sat4j.Minisat.Orders;
using Org.Sat4j.Minisat.Restarts;
using Org.Sat4j.Opt;
using Org.Sat4j.Specs;
using Org.Sat4j.Tools;
using Sharpen;

namespace Org.Sat4j.Minisat
{
	/// <summary>User friendly access to pre-constructed solvers.</summary>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class SolverFactory : ASolverFactory<ISolver>
	{
		private const long serialVersionUID = 1L;

		private static Org.Sat4j.Minisat.SolverFactory instance;

		/// <summary>Private constructor.</summary>
		/// <remarks>Private constructor. Use singleton method instance() instead.</remarks>
		/// <seealso cref="Instance()"/>
		private SolverFactory()
			: base()
		{
		}

		// thread safe implementation of the singleton design pattern
		private static void CreateInstance()
		{
			lock (typeof(SolverFactory))
			{
				if (instance == null)
				{
					instance = new Org.Sat4j.Minisat.SolverFactory();
				}
			}
		}

		/// <summary>Access to the single instance of the factory.</summary>
		/// <returns>the singleton of that class.</returns>
		public static Org.Sat4j.Minisat.SolverFactory Instance()
		{
			if (instance == null)
			{
				CreateInstance();
			}
			return instance;
		}

		/// <returns>
		/// a "default" "minilearning" solver learning clauses of size
		/// smaller than 10 % of the total number of variables with a heap
		/// based var order.
		/// </returns>
		public static Solver<DataStructureFactory> NewMiniLearningHeap()
		{
			return NewMiniLearningHeap(new MixedDataStructureDanielWL());
		}

		public static ICDCL<DataStructureFactory> NewMiniLearningHeapEZSimp()
		{
			Solver<DataStructureFactory> solver = NewMiniLearningHeap();
			solver.SetSimplifier(solver.SimpleSimplification);
			return solver;
		}

		public static Solver<DataStructureFactory> NewMiniLearningHeapExpSimp()
		{
			Solver<DataStructureFactory> solver = NewMiniLearningHeap();
			solver.SetSimplifier(solver.ExpensiveSimplification);
			return solver;
		}

		public static Solver<DataStructureFactory> NewMiniLearningHeapRsatExpSimp()
		{
			Solver<DataStructureFactory> solver = NewMiniLearningHeapExpSimp();
			solver.SetOrder(new VarOrderHeap(new RSATPhaseSelectionStrategy()));
			return solver;
		}

		public static Solver<DataStructureFactory> NewMiniLearningHeapRsatExpSimpBiere()
		{
			Solver<DataStructureFactory> solver = NewMiniLearningHeapRsatExpSimp();
			solver.SetRestartStrategy(new ArminRestarts());
			solver.SetSearchParams(new SearchParams(1.1, 100));
			return solver;
		}

		public static ICDCL<DataStructureFactory> NewMiniLearningHeapRsatExpSimpLuby()
		{
			ICDCL<DataStructureFactory> solver = NewMiniLearningHeapRsatExpSimp();
			solver.SetRestartStrategy(new LubyRestarts(512));
			return solver;
		}

		public static ICDCL<DataStructureFactory> NewGlucose21()
		{
			Solver<DataStructureFactory> solver = NewMiniLearningHeapRsatExpSimp();
			solver.SetRestartStrategy(new Glucose21Restarts());
			solver.SetLearnedConstraintsDeletionStrategy(solver.lbd_based);
			return solver;
		}

		private static Solver<DataStructureFactory> NewBestCurrentSolverConfiguration(DataStructureFactory dsf)
		{
			MiniSATLearning<DataStructureFactory> learning = new MiniSATLearning<DataStructureFactory>();
			Solver<DataStructureFactory> solver = new Solver<DataStructureFactory>(learning, dsf, new VarOrderHeap(new RSATPhaseSelectionStrategy()), new ArminRestarts());
			solver.SetSearchParams(new SearchParams(1.1, 100));
			solver.SetSimplifier(solver.ExpensiveSimplification);
			return solver;
		}

		/// <since>2.2</since>
		public static ICDCL<DataStructureFactory> NewGreedySolver()
		{
			MiniSATLearning<DataStructureFactory> learning = new MiniSATLearning<DataStructureFactory>();
			Solver<DataStructureFactory> solver = new Solver<DataStructureFactory>(learning, new MixedDataStructureDanielWL(), new RandomWalkDecorator(new VarOrderHeap(new RSATPhaseSelectionStrategy())), new NoRestarts());
			// solver.setSearchParams(new SearchParams(1.1, 100));
			solver.SetSimplifier(solver.ExpensiveSimplification);
			return solver;
		}

		/// <since>2.2</since>
		public static ICDCL<DataStructureFactory> NewDefaultAutoErasePhaseSaving()
		{
			ICDCL<DataStructureFactory> solver = NewBestWL();
			solver.SetOrder(new VarOrderHeap(new PhaseCachingAutoEraseStrategy()));
			return solver;
		}

		/// <since>2.2.3</since>
		public static ICDCL<DataStructureFactory> NewDefaultMS21PhaseSaving()
		{
			ICDCL<DataStructureFactory> solver = NewBestWL();
			solver.SetOrder(new VarOrderHeap(new RSATLastLearnedClausesPhaseSelectionStrategy()));
			return solver;
		}

		/// <since>2.1</since>
		public static Solver<DataStructureFactory> NewBestWL()
		{
			return NewBestCurrentSolverConfiguration(new MixedDataStructureDanielWL());
		}

		/// <since>2.1</since>
		public static ICDCL<DataStructureFactory> NewBestHT()
		{
			return NewBestCurrentSolverConfiguration(new MixedDataStructureDanielHT());
		}

		/// <since>2.2</since>
		public static ICDCL<DataStructureFactory> NewBest17()
		{
			Solver<DataStructureFactory> solver = NewBestCurrentSolverConfiguration(new MixedDataStructureSingleWL());
			solver.SetSimplifier(solver.ExpensiveSimplificationWlonly);
			solver.SetLearnedConstraintsDeletionStrategy(solver.activity_based_low_memory);
			LimitedLearning<DataStructureFactory> learning = new PercentLengthLearning<DataStructureFactory>(10);
			solver.SetLearningStrategy(learning);
			return solver;
		}

		/// <since>2.1</since>
		public static Solver<DataStructureFactory> NewGlucose()
		{
			Solver<DataStructureFactory> solver = NewBestWL();
			solver.SetLearnedConstraintsDeletionStrategy(solver.lbd_based);
			solver.SetRestartStrategy(new LubyRestarts(512));
			return solver;
		}

		/// <param name="dsf">a specific data structure factory</param>
		/// <returns>
		/// a default "minilearning" solver using a specific data structure
		/// factory, learning clauses of length smaller or equals to 10 % of
		/// the number of variables and a heap based VSIDS heuristics
		/// </returns>
		public static Solver<DataStructureFactory> NewMiniLearningHeap(DataStructureFactory dsf)
		{
			return NewMiniLearning(dsf, new VarOrderHeap());
		}

		/// <param name="dsf">
		/// the data structure factory used to represent literals and
		/// clauses
		/// </param>
		/// <param name="order">the heuristics</param>
		/// <returns>
		/// a SAT solver with learning limited to clauses of length smaller
		/// or equal to 10 percent of the total number of variables, the dsf
		/// data structure, the FirstUIP clause generator and order as
		/// heuristics.
		/// </returns>
		public static Solver<DataStructureFactory> NewMiniLearning(DataStructureFactory dsf, IOrder order)
		{
			// LimitedLearning<DataStructureFactory> learning = new
			// PercentLengthLearning<DataStructureFactory>(10);
			MiniSATLearning<DataStructureFactory> learning = new MiniSATLearning<DataStructureFactory>();
			Solver<DataStructureFactory> solver = new Solver<DataStructureFactory>(learning, dsf, order, new MiniSATRestarts());
			return solver;
		}

		/// <returns>a default MiniLearning without restarts.</returns>
		public static ICDCL<DataStructureFactory> NewMiniLearningHeapEZSimpNoRestarts()
		{
			LimitedLearning<DataStructureFactory> learning = new PercentLengthLearning<DataStructureFactory>(10);
			Solver<DataStructureFactory> solver = new Solver<DataStructureFactory>(learning, new MixedDataStructureDanielWL(), new SearchParams(int.MaxValue), new VarOrderHeap(), new MiniSATRestarts());
			learning.SetSolver(solver);
			solver.SetSimplifier(solver.SimpleSimplification);
			return solver;
		}

		/// <returns>a default MiniLearning with restarts beginning at 1000 conflicts.</returns>
		public static ICDCL<DataStructureFactory> NewMiniLearningHeapEZSimpLongRestarts()
		{
			LimitedLearning<DataStructureFactory> learning = new PercentLengthLearning<DataStructureFactory>(10);
			Solver<DataStructureFactory> solver = new Solver<DataStructureFactory>(learning, new MixedDataStructureDanielWL(), new SearchParams(1000), new VarOrderHeap(), new MiniSATRestarts());
			learning.SetSolver(solver);
			solver.SetSimplifier(solver.SimpleSimplification);
			return solver;
		}

		/// <returns>a SAT solver very close to the original MiniSAT sat solver.</returns>
		public static Solver<DataStructureFactory> NewMiniSATHeap()
		{
			return NewMiniSATHeap(new MixedDataStructureDanielWL());
		}

		/// <returns>
		/// a SAT solver very close to the original MiniSAT sat solver
		/// including easy reason simplification.
		/// </returns>
		public static ICDCL<DataStructureFactory> NewMiniSATHeapEZSimp()
		{
			Solver<DataStructureFactory> solver = NewMiniSATHeap();
			solver.SetSimplifier(solver.SimpleSimplification);
			return solver;
		}

		public static ICDCL<DataStructureFactory> NewMiniSATHeapExpSimp()
		{
			Solver<DataStructureFactory> solver = NewMiniSATHeap();
			solver.SetSimplifier(solver.ExpensiveSimplification);
			return solver;
		}

		public static Solver<DataStructureFactory> NewMiniSATHeap(DataStructureFactory dsf)
		{
			MiniSATLearning<DataStructureFactory> learning = new MiniSATLearning<DataStructureFactory>();
			Solver<DataStructureFactory> solver = new Solver<DataStructureFactory>(learning, dsf, new VarOrderHeap(), new MiniSATRestarts());
			learning.SetDataStructureFactory(solver.GetDSFactory());
			learning.SetVarActivityListener(solver);
			return solver;
		}

		/// <returns>
		/// MiniSAT with VSIDS heuristics, FirstUIP clause generator for
		/// backjumping but no learning.
		/// </returns>
		public static ICDCL<MixedDataStructureDanielWL> NewBackjumping()
		{
			NoLearningButHeuristics<MixedDataStructureDanielWL> learning = new NoLearningButHeuristics<MixedDataStructureDanielWL>();
			Solver<MixedDataStructureDanielWL> solver = new Solver<MixedDataStructureDanielWL>(learning, new MixedDataStructureDanielWL(), new VarOrderHeap(), new MiniSATRestarts());
			learning.SetVarActivityListener(solver);
			return solver;
		}

		/// <returns>
		/// a solver computing models with a minimum number of satisfied
		/// literals.
		/// </returns>
		public static ISolver NewMinOneSolver()
		{
			return new OptToSatAdapter(new MinOneDecorator(NewDefault()));
		}

		/// <returns>the default solver with an aggressive LCDS based on age</returns>
		public static ISolver NewAgeLCDS()
		{
			Solver<object> solver = (Solver<object>)NewGlucose21();
			solver.SetLearnedConstraintsDeletionStrategy(solver.age_based);
			return solver;
		}

		/// <returns>the default solver with an aggressive LCDS based on activity</returns>
		public static ISolver NewActivityLCDS()
		{
			Solver<object> solver = (Solver<object>)NewGlucose21();
			solver.SetLearnedConstraintsDeletionStrategy(solver.activity_based);
			return solver;
		}

		/// <returns>the default solver with an aggressive LCDS based on size</returns>
		public static ISolver NewSizeLCDS()
		{
			Solver<object> solver = (Solver<object>)NewGlucose21();
			solver.SetLearnedConstraintsDeletionStrategy(solver.size_based);
			return solver;
		}

		/// <summary>Default solver of the SolverFactory.</summary>
		/// <remarks>
		/// Default solver of the SolverFactory. This solver is meant to be used on
		/// challenging SAT benchmarks.
		/// </remarks>
		/// <returns>the best "general purpose" SAT solver available in the factory.</returns>
		/// <seealso cref="DefaultSolver()">
		/// the same method, polymorphic, to be called from an
		/// instance of ASolverFactory.
		/// </seealso>
		public static ISolver NewDefault()
		{
			return NewGlucose21();
		}

		// newMiniLearningHeapRsatExpSimpBiere();
		public override ISolver DefaultSolver()
		{
			return NewDefault();
		}

		/// <summary>Small footprint SAT solver.</summary>
		/// <returns>a SAT solver suitable for solving small/easy SAT benchmarks.</returns>
		/// <seealso cref="LightSolver()">
		/// the same method, polymorphic, to be called from an
		/// instance of ASolverFactory.
		/// </seealso>
		public static ISolver NewLight()
		{
			return NewMiniLearningHeap();
		}

		public override ISolver LightSolver()
		{
			return NewLight();
		}

		public static ISolver NewDimacsOutput()
		{
			return new DimacsOutputSolver();
		}

		public static ISolver NewStatistics()
		{
			return new StatisticsSolver();
		}

		public static ISolver NewParallel()
		{
			return new ManyCore<ISolver>(NewSAT(), NewUNSAT(), NewMiniLearningHeapRsatExpSimpLuby(), NewMiniLearningHeapRsatExpSimp(), NewDefaultAutoErasePhaseSaving(), NewMiniLearningHeap(), NewMiniSATHeapExpSimp(), NewMiniSATHeapEZSimp());
		}

		/// <summary>
		/// Two solvers are running in //: one for solving SAT instances, the other
		/// one for solving unsat instances.
		/// </summary>
		/// <returns>a parallel solver for both SAT and UNSAT problems.</returns>
		public static ISolver NewSATUNSAT()
		{
			return new ManyCore<ISolver>(NewSAT(), NewUNSAT());
		}

		/// <summary>That solver is expected to perform better on satisfiable benchmarks.</summary>
		/// <returns>a solver for satisfiable benchmarks.</returns>
		public static Solver<DataStructureFactory> NewSAT()
		{
			Solver<DataStructureFactory> solver = (Solver<DataStructureFactory>)NewGlucose21();
			solver.SetRestartStrategy(new LubyRestarts(100));
			solver.SetLearnedConstraintsDeletionStrategy(solver.activity_based_low_memory);
			return solver;
		}

		/// <summary>That solver is expected to perform better on unsatisfiable benchmarks.</summary>
		/// <returns>a solver for unsatisfiable benchmarks.</returns>
		public static Solver<DataStructureFactory> NewUNSAT()
		{
			Solver<DataStructureFactory> solver = (Solver<DataStructureFactory>)NewGlucose21();
			solver.SetRestartStrategy(new NoRestarts());
			solver.SetSimplifier(solver.SimpleSimplification);
			return solver;
		}

		public static Solver<DataStructureFactory> NewConcise()
		{
			Solver<DataStructureFactory> solver = (Solver<DataStructureFactory>)NewGlucose21();
			solver.SetDataStructureFactory(new MixedDataStructureDanielWLConciseBinary());
			solver.SetSimplifier(Solver.NoSimplification);
			return solver;
		}

		public static Solver<DataStructureFactory> NewNoSimplification()
		{
			Solver<DataStructureFactory> solver = (Solver<DataStructureFactory>)NewGlucose21();
			solver.SetSimplifier(Solver.NoSimplification);
			return solver;
		}
	}
}
