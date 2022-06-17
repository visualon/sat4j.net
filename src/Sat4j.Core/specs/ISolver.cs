using System;
using System.Collections.Generic;
using System.IO;
using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>This interface contains all services provided by a SAT solver.</summary>
	/// <author>leberre</author>
	public interface ISolver : IProblem
	{
		/// <summary>Create a new variable in the solver (and thus in the vocabulary).</summary>
		/// <remarks>
		/// Create a new variable in the solver (and thus in the vocabulary).
		/// WE STRONGLY ENCOURAGE TO PRECOMPUTE THE NUMBER OF VARIABLES NEEDED AND TO
		/// USE newVar(howmany) INSTEAD. IF YOU EXPERIENCE A PROBLEM OF EFFICIENCY
		/// WHEN READING/BUILDING YOUR SAT INSTANCE, PLEASE CHECK THAT YOU ARE NOT
		/// USING THAT METHOD.
		/// </remarks>
		/// <returns>
		/// the number of variables available in the vocabulary, which is the
		/// identifier of the new variable.
		/// </returns>
		[Obsolete]
		int NewVar();

		/// <summary>Ask the solver for a free variable identifier, in Dimacs format (i.e.</summary>
		/// <remarks>
		/// Ask the solver for a free variable identifier, in Dimacs format (i.e. a
		/// positive number). Note that a previous call to newVar(max) will reserve
		/// in the solver the variable identifier from 1 to max, so nextFreeVarId()
		/// would return max+1, even if some variable identifiers smaller than max
		/// are not used. By default, the method will always answer by the maximum
		/// variable identifier used so far + 1.
		/// The number of variables reserved in the solver is accessible through the
		/// <see cref="RealNumberOfVariables()"/>
		/// method.
		/// </remarks>
		/// <param name="reserve">
		/// if true, the maxVarId is updated in the solver, i.e.
		/// successive calls to nextFreeVarId(true) will return increasing
		/// variable id while successive calls to nextFreeVarId(false)
		/// will always answer the same.
		/// </param>
		/// <returns>
		/// a variable identifier not in use in the constraints already
		/// inside the solver.
		/// </returns>
		/// <seealso cref="RealNumberOfVariables()"/>
		/// <since>2.1</since>
		int NextFreeVarId(bool reserve);

		/// <summary>Tell the solver to consider that the literal is in the CNF.</summary>
		/// <remarks>
		/// Tell the solver to consider that the literal is in the CNF.
		/// Since model() only return the truth value of the literals that appear in
		/// the solver, it is sometimes required that the solver provides a default
		/// truth value for a given literal. This happens for instance for MaxSat.
		/// </remarks>
		/// <param name="p">the literal in Dimacs format that should appear in the model.</param>
		void RegisterLiteral(int p);

		/// <summary>To inform the solver of the expected number of clauses to read.</summary>
		/// <remarks>
		/// To inform the solver of the expected number of clauses to read. This is
		/// an optional method, that is called when the <code>p cnf</code> line is
		/// read in dimacs formatted input file.
		/// Note that this method is supposed to be called AFTER a call to
		/// newVar(int)
		/// </remarks>
		/// <param name="nb">the expected number of clauses.</param>
		/// <seealso cref="IProblem.NewVar(int)"/>
		/// <since>1.6</since>
		void SetExpectedNumberOfClauses(int nb);

		/// <summary>
		/// Create a clause from a set of literals The literals are represented by
		/// non null integers such that opposite literals a represented by opposite
		/// values.
		/// </summary>
		/// <remarks>
		/// Create a clause from a set of literals The literals are represented by
		/// non null integers such that opposite literals a represented by opposite
		/// values. (classical Dimacs way of representing literals).
		/// </remarks>
		/// <param name="literals">a set of literals</param>
		/// <returns>
		/// a reference to the constraint added in the solver, to use in
		/// removeConstr().
		/// </returns>
		/// <exception cref="ContradictionException">
		/// iff the vector of literals is empty or if it contains only
		/// falsified literals after unit propagation
		/// </exception>
		/// <seealso cref="RemoveConstr(IConstr)"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		IConstr AddClause(IVecInt literals);

		/// <summary>Add a clause in order to prevent an assignment to occur.</summary>
		/// <remarks>
		/// Add a clause in order to prevent an assignment to occur. This happens
		/// usually when iterating over models for instance.
		/// </remarks>
		/// <param name="literals"/>
		/// <returns/>
		/// <exception cref="ContradictionException"/>
		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		IConstr AddBlockingClause(IVecInt literals);

		/// <summary>Discards current model.</summary>
		/// <remarks>
		/// Discards current model. This can be used when iterating on models instead
		/// of adding a blocking clause.
		/// </remarks>
		/// <returns/>
		/// <exception cref="ContradictionException"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		IConstr DiscardCurrentModel();

		/// <summary>Creates a VecInt representing a clause for discarding current model</summary>
		/// <returns/>
		IVecInt CreateBlockingClauseForCurrentModel();

		/// <summary>Remove a constraint returned by one of the add method from the solver.</summary>
		/// <remarks>
		/// Remove a constraint returned by one of the add method from the solver.
		/// All learned clauses will be cleared.
		/// Current implementation does not handle properly the case of unit clauses.
		/// </remarks>
		/// <param name="c">a constraint returned by one of the add method.</param>
		/// <returns>true if the constraint was successfully removed.</returns>
		bool RemoveConstr(IConstr c);

		/// <summary>
		/// Remove a constraint returned by one of the add method from the solver
		/// that is subsumed by a constraint already in the solver or to be added to
		/// the solver.
		/// </summary>
		/// <remarks>
		/// Remove a constraint returned by one of the add method from the solver
		/// that is subsumed by a constraint already in the solver or to be added to
		/// the solver.
		/// Unlike the removeConstr() method, learned clauses will NOT be cleared.
		/// That method is expected to be used to remove constraints used in the
		/// optimization process.
		/// In order to prevent a wrong from the user, the method will only work if
		/// the argument is the last constraint added to the solver. An illegal
		/// argument exception will be thrown in other cases.
		/// </remarks>
		/// <param name="c">
		/// a constraint returned by one of the add method. It must be the
		/// latest constr added to the solver.
		/// </param>
		/// <returns>true if the constraint was successfully removed.</returns>
		/// <since>2.1</since>
		bool RemoveSubsumedConstr(IConstr c);

		/// <summary>Create clauses from a set of set of literals.</summary>
		/// <remarks>
		/// Create clauses from a set of set of literals. This is convenient to
		/// create in a single call all the clauses (mandatory for the distributed
		/// version of the solver). It is mainly a loop to addClause().
		/// </remarks>
		/// <param name="clauses">
		/// a vector of set (VecInt) of literals in the dimacs format. The
		/// vector can be reused since the solver is not supposed to keep
		/// a reference to that vector.
		/// </param>
		/// <exception cref="ContradictionException">
		/// iff the vector of literals is empty or if it contains only
		/// falsified literals after unit propagation
		/// </exception>
		/// <seealso cref="AddClause(IVecInt)"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		void AddAllClauses(IVec<IVecInt> clauses);

		/// <summary>
		/// Create a cardinality constraint of the type "at most n of those literals
		/// must be satisfied"
		/// </summary>
		/// <param name="literals">
		/// a set of literals The vector can be reused since the solver is
		/// not supposed to keep a reference to that vector.
		/// </param>
		/// <param name="degree">the degree (n) of the cardinality constraint</param>
		/// <returns>
		/// a reference to the constraint added in the solver, to use in
		/// removeConstr().
		/// </returns>
		/// <exception cref="ContradictionException">
		/// iff the vector of literals is empty or if it contains more
		/// than degree satisfied literals after unit propagation
		/// </exception>
		/// <seealso cref="RemoveConstr(IConstr)"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		IConstr AddAtMost(IVecInt literals, int degree);

		/// <summary>
		/// Create a cardinality constraint of the type "at least n of those literals
		/// must be satisfied"
		/// </summary>
		/// <param name="literals">
		/// a set of literals. The vector can be reused since the solver
		/// is not supposed to keep a reference to that vector.
		/// </param>
		/// <param name="degree">the degree (n) of the cardinality constraint</param>
		/// <returns>
		/// a reference to the constraint added in the solver, to use in
		/// removeConstr().
		/// </returns>
		/// <exception cref="ContradictionException">
		/// iff the vector of literals is empty or if degree literals are
		/// not remaining unfalsified after unit propagation
		/// </exception>
		/// <seealso cref="RemoveConstr(IConstr)"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		IConstr AddAtLeast(IVecInt literals, int degree);

		/// <summary>
		/// Create a cardinality constraint of the type "exactly n of those literals
		/// must be satisfied".
		/// </summary>
		/// <param name="literals">
		/// a set of literals. The vector can be reused since the solver
		/// is not supposed to keep a reference to that vector.
		/// </param>
		/// <param name="n">the number of literals that must be satisfied</param>
		/// <returns>
		/// a reference to the constraint added to the solver. It might
		/// return an object representing a group of constraints.
		/// </returns>
		/// <exception cref="ContradictionException">iff the constraint is trivially unsatisfiable.</exception>
		/// <since>2.3.1</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		IConstr AddExactly(IVecInt literals, int n);

		/// <summary>Add a parity constraint (aka XOR constraint) to the solver.</summary>
		/// <remarks>
		/// Add a parity constraint (aka XOR constraint) to the solver.
		/// The aim of that constraint is to enforce that an odd (or even) number of
		/// literals are satisfied.
		/// If the xor of all the literals results in false, that the number of
		/// satisfied literals must be even, else it must be odd.
		/// </remarks>
		/// <since>2.3.6</since>
		IConstr AddParity(IVecInt literals, bool even);

		/// <summary>Add a user defined constraint to the solver.</summary>
		/// <param name="constr">a constraint implementing the Constr interface.</param>
		/// <returns>a reference to the constraint for external use.</returns>
		/// <since>2.3.6</since>
		IConstr AddConstr(Constr constr);

		/// <summary>To set the internal timeout of the solver.</summary>
		/// <remarks>
		/// To set the internal timeout of the solver. When the timeout is reached, a
		/// timeout exception is launched by the solver.
		/// </remarks>
		/// <param name="t">the timeout (in s)</param>
		void SetTimeout(int t);

		/// <summary>To set the internal timeout of the solver.</summary>
		/// <remarks>
		/// To set the internal timeout of the solver. When the timeout is reached, a
		/// timeout exception is launched by the solver.
		/// Here the timeout is given in number of conflicts. That way, the behavior
		/// of the solver should be the same across different architecture.
		/// </remarks>
		/// <param name="count">the timeout (in number of conflicts)</param>
		void SetTimeoutOnConflicts(int count);

		/// <summary>To set the internal timeout of the solver.</summary>
		/// <remarks>
		/// To set the internal timeout of the solver. When the timeout is reached, a
		/// timeout exception is launched by the solver.
		/// </remarks>
		/// <param name="t">the timeout (in milliseconds)</param>
		void SetTimeoutMs(long t);

		/// <summary>Useful to check the internal timeout of the solver.</summary>
		/// <returns>the internal timeout of the solver (in seconds)</returns>
		int GetTimeout();

		/// <summary>Useful to check the internal timeout of the solver.</summary>
		/// <returns>the internal timeout of the solver (in milliseconds)</returns>
		/// <since>2.1</since>
		long GetTimeoutMs();

		/// <summary>Expire the timeout of the solver.</summary>
		void ExpireTimeout();

		/// <summary>Clean up the internal state of the solver.</summary>
		/// <remarks>
		/// Clean up the internal state of the solver.
		/// Note that such method should also be called when you no longer need the
		/// solver because the state of the solver may prevent the GC to proceed.
		/// There is a known issue for instance where failing to call reset() on a
		/// solver will keep timer threads alive and exhausts memory.
		/// </remarks>
		void Reset();

		/// <summary>
		/// Display statistics to the given output stream Please use writers instead
		/// of stream.
		/// </summary>
		/// <param name="out"/>
		/// <param name="prefix">the prefix to put in front of each line</param>
		/// <seealso cref="PrintStat(System.IO.PrintWriter, string)"/>
		[Obsolete]
		void PrintStat(TextWriter @out, string prefix);

		/// <summary>Display statistics to the given output writer</summary>
		/// <param name="out"/>
		/// <param name="prefix">the prefix to put in front of each line</param>
		/// <since>1.6</since>
		[System.ObsoleteAttribute(@"using the prefix does no longer makes sense because the solver owns it.")]
		void PrintStat(PrintWriter @out, string prefix);

		/// <summary>Display statistics to the given output writer</summary>
		/// <param name="out"/>
		/// <since>2.3.3</since>
		/// <seealso cref="SetLogPrefix(string)"/>
		void PrintStat(PrintWriter @out);

		/// <summary>To obtain a map of the available statistics from the solver.</summary>
		/// <remarks>
		/// To obtain a map of the available statistics from the solver. Note that
		/// some keys might be specific to some solvers.
		/// </remarks>
		/// <returns>a Map with the name of the statistics as key.</returns>
		IDictionary<string, Number> GetStat();

		/// <summary>Display a textual representation of the solver configuration.</summary>
		/// <param name="prefix">the prefix to use on each line.</param>
		/// <returns>a textual description of the solver internals.</returns>
		string ToString(string prefix);

		/// <summary>Remove clauses learned during the solving process.</summary>
		void ClearLearntClauses();

		/// <summary>
		/// Set whether the solver is allowed to simplify the formula by propagating
		/// the truth value of top level satisfied variables.
		/// </summary>
		/// <remarks>
		/// Set whether the solver is allowed to simplify the formula by propagating
		/// the truth value of top level satisfied variables.
		/// Note that the solver should not be allowed to perform such simplification
		/// when constraint removal is planned.
		/// </remarks>
		void SetDBSimplificationAllowed(bool status);

		/// <summary>
		/// Indicate whether the solver is allowed to simplify the formula by
		/// propagating the truth value of top level satisfied variables.
		/// </summary>
		/// <remarks>
		/// Indicate whether the solver is allowed to simplify the formula by
		/// propagating the truth value of top level satisfied variables.
		/// Note that the solver should not be allowed to perform such simplification
		/// when constraint removal is planned.
		/// </remarks>
		bool IsDBSimplificationAllowed();

		/// <summary>
		/// Allow the user to hook a listener to the solver to be notified of the
		/// main steps of the search process.
		/// </summary>
		/// <param name="sl">a Search Listener.</param>
		/// <since>2.1</since>
		void SetSearchListener<S>(SearchListener<S> sl)
			where S : ISolverService;

		/// <summary>Allow the solver to ask for unit clauses before each restarts.</summary>
		/// <param name="ucp">an object able to provide unit clauses.</param>
		/// <since>2.3.4</since>
		void SetUnitClauseProvider(UnitClauseProvider ucp);

		/// <summary>Get the current SearchListener.</summary>
		/// <returns>a Search Listener.</returns>
		/// <since>2.2</since>
		SearchListener<S> GetSearchListener<S>()
			where S : ISolverService;

		/// <summary>To know if the solver is in verbose mode (output allowed) or not.</summary>
		/// <returns>true if the solver is verbose.</returns>
		/// <since>2.2</since>
		bool IsVerbose();

		/// <summary>Set the verbosity of the solver</summary>
		/// <param name="value">
		/// true to allow the solver to output messages on the console,
		/// false either.
		/// </param>
		/// <since>2.2</since>
		void SetVerbose(bool value);

		/// <summary>Set the prefix used to display information.</summary>
		/// <param name="prefix">the prefix to be in front of each line of text</param>
		/// <since>2.2</since>
		void SetLogPrefix(string prefix);

		/// <returns>the string used to prefix the output.</returns>
		/// <since>2.2</since>
		string GetLogPrefix();

		/// <summary>
		/// Retrieve an explanation of the inconsistency in terms of assumption
		/// literals.
		/// </summary>
		/// <remarks>
		/// Retrieve an explanation of the inconsistency in terms of assumption
		/// literals. This is only application when isSatisfiable(assumps) is used.
		/// Note that
		/// <code>!isSatisfiable(assumps)&amp;&amp;assumps.contains(unsatExplanation())</code>
		/// should hold.
		/// </remarks>
		/// <returns>
		/// a subset of the assumptions used when calling
		/// isSatisfiable(assumps). Will return null if not applicable (i.e.
		/// no assumptions used).
		/// </returns>
		/// <seealso cref="IProblem.IsSatisfiable(IVecInt)"/>
		/// <seealso cref="IProblem.IsSatisfiable(IVecInt, bool)"/>
		/// <since>2.2</since>
		IVecInt UnsatExplanation();

		/// <summary>
		/// That method is designed to be used to retrieve the real model of the
		/// current set of constraints, i.e.
		/// </summary>
		/// <remarks>
		/// That method is designed to be used to retrieve the real model of the
		/// current set of constraints, i.e. to provide the truth value of boolean
		/// variables used internally in the solver (for encoding purposes for
		/// instance).
		/// If no variables are added in the solver, then
		/// Arrays.equals(model(),modelWithInternalVariables()).
		/// In case new variables are added to the solver, then the number of models
		/// returned by that method is greater of equal to the number of models
		/// returned by model() when using a ModelIterator.
		/// </remarks>
		/// <returns>
		/// an array of literals, in Dimacs format, corresponding to a model
		/// of the formula over all the variables declared in the solver.
		/// </returns>
		/// <seealso cref="IProblem.Model()"/>
		/// <seealso cref="Org.Sat4j.Tools.ModelIterator"/>
		/// <since>2.3.1</since>
		int[] ModelWithInternalVariables();

		/// <summary>Retrieve the real number of variables used in the solver.</summary>
		/// <remarks>
		/// Retrieve the real number of variables used in the solver.
		/// In many cases, <code>realNumberOfVariables()==nVars()</code>. However,
		/// when working with SAT encodings or translation from MAXSAT to PMS, one
		/// can have <code>realNumberOfVariables()&gt;nVars()</code>.
		/// Those additional variables are supposed to be created using the
		/// <see cref="NextFreeVarId(bool)"/>
		/// method.
		/// </remarks>
		/// <returns>the number of variables reserved in the solver.</returns>
		/// <seealso cref="NextFreeVarId(bool)"/>
		/// <since>2.3.1</since>
		int RealNumberOfVariables();

		/// <summary>
		/// Ask to the solver if it is in "hot" mode, meaning that the heuristics is
		/// not reset after call is isSatisfiable().
		/// </summary>
		/// <remarks>
		/// Ask to the solver if it is in "hot" mode, meaning that the heuristics is
		/// not reset after call is isSatisfiable(). This is only useful in case of
		/// repeated calls to the solver with same set of variables.
		/// </remarks>
		/// <returns>
		/// true iff the solver keep the heuristics value unchanged across
		/// calls.
		/// </returns>
		/// <since>2.3.2</since>
		bool IsSolverKeptHot();

		/// <summary>
		/// Changed the behavior of the SAT solver heuristics between successive
		/// calls.
		/// </summary>
		/// <remarks>
		/// Changed the behavior of the SAT solver heuristics between successive
		/// calls. If the value is true, then the solver will be "hot" on reuse, i.e.
		/// the heuristics will not be reset. Else the heuristics will be reset.
		/// </remarks>
		/// <param name="keepHot">true to keep the heuristics values across calls, false either.</param>
		/// <since>2.3.2</since>
		void SetKeepSolverHot(bool keepHot);

		/// <summary>
		/// Retrieve the real engine in case the engine is decorated by one or
		/// several decorator.
		/// </summary>
		/// <remarks>
		/// Retrieve the real engine in case the engine is decorated by one or
		/// several decorator. This can be used for instance to setup the engine,
		/// which requires to bypass all the decorators.
		/// It is thus safe to downcast the ISolver to an ICDCL interface. We cannot
		/// directly return an ICDCL object because we are not on the same
		/// abstraction level here.
		/// </remarks>
		/// <returns>the solver</returns>
		/// <since>2.3.3</since>
		ISolver GetSolvingEngine();
	}
}
