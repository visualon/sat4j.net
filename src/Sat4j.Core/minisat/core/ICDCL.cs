using System;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Abstraction for Conflict Driven Clause Learning Solver.</summary>
	/// <remarks>
	/// Abstraction for Conflict Driven Clause Learning Solver.
	/// Allows to easily access the various options available to setup the solver.
	/// </remarks>
	/// <author>daniel</author>
	/// <?/>
	public interface ICDCL<D> : ISolver, UnitPropagationListener, ActivityListener, Learner
		where D : DataStructureFactory
	{
		/// <summary>Change the internal representation of the constraints.</summary>
		/// <remarks>
		/// Change the internal representation of the constraints. Note that the
		/// heuristics must be changed prior to calling that method.
		/// </remarks>
		/// <param name="dsf">the internal factory</param>
		void SetDataStructureFactory(D dsf);

		/// <since>2.2</since>
		/// <seealso cref="ICDCL{D}.SetLearningStrategy(LearningStrategy{D})"/>
		[System.ObsoleteAttribute(@"renamed into setLearningStrategy()")]
		void SetLearner(LearningStrategy<D> learner);

		/// <summary>Allow to change the learning strategy, i.e.</summary>
		/// <remarks>
		/// Allow to change the learning strategy, i.e. to decide which
		/// clauses/constraints should be learned by the solver after conflict
		/// analysis.
		/// </remarks>
		/// <since>2.3.3</since>
		void SetLearningStrategy(LearningStrategy<D> strategy);

		void SetSearchParams(SearchParams sp);

		SearchParams GetSearchParams();

		SolverStats GetStats();

		void SetRestartStrategy(RestartStrategy restarter);

		RestartStrategy GetRestartStrategy();

		/// <summary>Setup the reason simplification strategy.</summary>
		/// <remarks>
		/// Setup the reason simplification strategy. By default, there is no reason
		/// simplification. NOTE THAT REASON SIMPLIFICATION DOES NOT WORK WITH
		/// SPECIFIC DATA STRUCTURE FOR HANDLING BOTH BINARY AND TERNARY CLAUSES.
		/// </remarks>
		/// <param name="simp">a simplification type.</param>
		void SetSimplifier(SimplificationType simp);

		/// <summary>Setup the reason simplification strategy.</summary>
		/// <remarks>
		/// Setup the reason simplification strategy. By default, there is no reason
		/// simplification. NOTE THAT REASON SIMPLIFICATION IS ONLY ALLOWED FOR WL
		/// CLAUSAL data structures. USING REASON SIMPLIFICATION ON CB CLAUSES,
		/// CARDINALITY CONSTRAINTS OR PB CONSTRAINTS MIGHT RESULT IN INCORRECT
		/// RESULTS.
		/// </remarks>
		/// <param name="simp"/>
		void SetSimplifier(ISimplifier simp);

		ISimplifier GetSimplifier();

		/// <param name="lcds"/>
		/// <since>2.1</since>
		void SetLearnedConstraintsDeletionStrategy(LearnedConstraintsDeletionStrategy lcds);

		/// <param name="timer">when to apply constraints cleanup.</param>
		/// <param name="evaluation">the strategy used to evaluate learned clauses.</param>
		/// <since>2.3.2</since>
		void SetLearnedConstraintsDeletionStrategy(ConflictTimer timer, LearnedConstraintsEvaluationType evaluation);

		/// <param name="evaluation">the strategy used to evaluate learned clauses.</param>
		/// <since>2.3.2</since>
		void SetLearnedConstraintsDeletionStrategy(LearnedConstraintsEvaluationType evaluation);

		IOrder GetOrder();

		void SetOrder(IOrder h);

		void SetNeedToReduceDB(bool needToReduceDB);

		void SetLogger(ILogAble @out);

		ILogAble GetLogger();
	}
}
