using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>
	/// The aim on that interface is to allow power users to communicate with the SAT
	/// solver using Dimacs format.
	/// </summary>
	/// <remarks>
	/// The aim on that interface is to allow power users to communicate with the SAT
	/// solver using Dimacs format. That way, there is no need to know the internals
	/// of the solver.
	/// </remarks>
	/// <author>leberre</author>
	/// <since>2.3.2</since>
	public interface ISolverService
	{
		/// <summary>Ask the SAT solver to stop the search.</summary>
		void Stop();

		/// <summary>Ask the SAT solver to backtrack.</summary>
		/// <remarks>
		/// Ask the SAT solver to backtrack. It is mandatory to provide a reason for
		/// backtracking, in terms of literals (which should be falsified under
		/// current assignment). The reason is not added to the clauses of the
		/// solver: only the result of the analysis is stored in the learned clauses.
		/// Note that these clauses may be removed latter.
		/// </remarks>
		/// <param name="reason">
		/// a set of literals, in Dimacs format, currently falsified, i.e.
		/// <code>for (int l : reason) assert truthValue(l) == Lbool.FALSE</code>
		/// </param>
		void Backtrack(int[] reason);

		/// <summary>Add a new clause in the SAT solver.</summary>
		/// <remarks>
		/// Add a new clause in the SAT solver. The new clause may contain new
		/// variables. The clause may be falsified, in that case, the difference with
		/// backtrack() is that the new clause is appended to the solver as a regular
		/// clause. Thus it will not be removed by aggressive clause deletion. The
		/// clause may be assertive at a given decision level. In that case, the
		/// solver should backtrack to the proper decision level. In other cases, the
		/// search should simply proceed.
		/// </remarks>
		/// <param name="literals">a set of literals in Dimacs format.</param>
		IConstr AddClauseOnTheFly(int[] literals);

		/// <summary>
		/// Add a new pseudo cardinality constraint sum literals &lt;= degree in the
		/// solver.
		/// </summary>
		/// <remarks>
		/// Add a new pseudo cardinality constraint sum literals &lt;= degree in the
		/// solver. The constraint must be falsified under current assignment.
		/// </remarks>
		/// <param name="literals">a set of literals in Dimacs format.</param>
		/// <param name="degree">the maximal number of literals which can be satisfied.</param>
		IConstr AddAtMostOnTheFly(int[] literals, int degree);

		/// <summary>Creates a VecInt representing a clause for discarding current model</summary>
		/// <returns/>
		IVecInt CreateBlockingClauseForCurrentModel();

		/// <summary>To access the truth value of a specific literal under current assignment.</summary>
		/// <param name="literal">a Dimacs literal, i.e. a non-zero integer.</param>
		/// <returns>true or false if the literal is assigned, else undefined.</returns>
		Lbool TruthValue(int literal);

		/// <summary>To access the current decision level</summary>
		int CurrentDecisionLevel();

		/// <summary>To access the literals propagated at a specific decision level.</summary>
		/// <param name="decisionLevel">a decision level between 0 and #currentDecisionLevel()</param>
		int[] GetLiteralsPropagatedAt(int decisionLevel);

		/// <summary>Suggests to the SAT solver to branch next on the given literal.</summary>
		/// <param name="l">a literal in Dimacs format.</param>
		void SuggestNextLiteralToBranchOn(int l);

		/// <summary>Read-Only access to the value of the heuristics for each variable.</summary>
		/// <remarks>
		/// Read-Only access to the value of the heuristics for each variable. Note
		/// that for efficiency reason, the real array storing the value of the
		/// heuristics is returned. DO NOT CHANGE THE VALUES IN THAT ARRAY.
		/// </remarks>
		/// <returns>
		/// the value of the heuristics for each variable (using Dimacs
		/// index).
		/// </returns>
		double[] GetVariableHeuristics();

		/// <summary>
		/// Read-Only access to the list of constraints learned and not deleted so
		/// far in the solver.
		/// </summary>
		/// <remarks>
		/// Read-Only access to the list of constraints learned and not deleted so
		/// far in the solver. Note that for efficiency reason, the real list of
		/// constraints managed by the solver is returned. DO NOT MODIFY THAT LIST
		/// NOR ITS CONSTRAINTS.
		/// </remarks>
		/// <returns>the constraints learned and kept so far by the solver.</returns>
		IVec<IConstr> GetLearnedConstraints();

		/// <summary>Read-Only access to the number of variables declared in the solver.</summary>
		/// <returns>the maximum variable id (Dimacs format) reserved in the solver.</returns>
		int NVars();

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
		bool RemoveSubsumedConstr(IConstr c);

		/// <returns>the string used to prefix the output.</returns>
		/// <since>2.3.3</since>
		string GetLogPrefix();
	}
}
