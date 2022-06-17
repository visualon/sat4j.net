using System;
using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>Represents an optimization problem.</summary>
	/// <remarks>
	/// Represents an optimization problem. The SAT solver will find suboptimal
	/// solutions of the problem until no more solutions are available. The latest
	/// solution found will be the optimal one.
	/// Such kind of problem is supposed to be handled:
	/// <pre>
	/// boolean isSatisfiable = false;
	/// IOptimizationProblem optproblem = (IOptimizationProblem) problem;
	/// try {
	/// while (optproblem.admitABetterSolution()) {
	/// if (!isSatisfiable) {
	/// if (optproblem.nonOptimalMeansSatisfiable()) {
	/// setExitCode(ExitCode.SATISFIABLE);
	/// if (optproblem.hasNoObjectiveFunction()) {
	/// return;
	/// }
	/// log(&quot;SATISFIABLE&quot;); //$NON-NLS-1$
	/// }
	/// isSatisfiable = true;
	/// log(&quot;OPTIMIZING...&quot;); //$NON-NLS-1$
	/// }
	/// log(&quot;Got one! Elapsed wall clock time (in seconds):&quot; //$NON-NLS-1$
	/// + (System.currentTimeMillis() - getBeginTime()) / 1000.0);
	/// getLogWriter().println(
	/// CURRENT_OPTIMUM_VALUE_PREFIX + optproblem.getObjectiveValue());
	/// optproblem.discardCurrentSolution();
	/// }
	/// if (isSatisfiable) {
	/// setExitCode(ExitCode.OPTIMUM_FOUND);
	/// } else {
	/// setExitCode(ExitCode.UNSATISFIABLE);
	/// }
	/// } catch (ContradictionException ex) {
	/// assert isSatisfiable;
	/// setExitCode(ExitCode.OPTIMUM_FOUND);
	/// }
	/// </pre>
	/// </remarks>
	/// <author>leberre</author>
	public interface IOptimizationProblem : IProblem
	{
		/// <summary>Look for a solution of the optimization problem.</summary>
		/// <returns>true if a better solution than current one can be found.</returns>
		/// <exception cref="TimeoutException">if the solver cannot answer in reasonable time.</exception>
		/// <seealso cref="ISolver.SetTimeout(int)"/>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		bool AdmitABetterSolution();

		/// <summary>
		/// Look for a solution of the optimization problem when some literals are
		/// satisfied.
		/// </summary>
		/// <param name="assumps">a set of literals in Dimacs format.</param>
		/// <returns>true if a better solution than current one can be found.</returns>
		/// <exception cref="TimeoutException">if the solver cannot answer in reasonable time.</exception>
		/// <seealso cref="ISolver.SetTimeout(int)"/>
		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		bool AdmitABetterSolution(IVecInt assumps);

		/// <summary>
		/// If the optimization problem has no objective function, then it is a
		/// simple decision problem.
		/// </summary>
		/// <returns>
		/// true if the problem is a decision problem, false if the problem
		/// is an optimization problem.
		/// </returns>
		bool HasNoObjectiveFunction();

		/// <summary>
		/// A suboptimal solution has different meaning depending of the optimization
		/// problem considered.
		/// </summary>
		/// <remarks>
		/// A suboptimal solution has different meaning depending of the optimization
		/// problem considered.
		/// For instance, in the case of MAXSAT, a suboptimal solution does not mean
		/// that the problem is satisfiable, while in pseudo boolean optimization, it
		/// is true.
		/// </remarks>
		/// <returns>
		/// true if founding a suboptimal solution means that the problem is
		/// satisfiable.
		/// </returns>
		bool NonOptimalMeansSatisfiable();

		/// <summary>Compute the value of the objective function for the current solution.</summary>
		/// <remarks>
		/// Compute the value of the objective function for the current solution. A
		/// call to that method only makes sense if hasNoObjectiveFunction()==false.
		/// DO NOT CALL THAT METHOD THAT WILL BE CALLED AUTOMATICALLY. USE
		/// getObjectiveValue() instead!
		/// </remarks>
		/// <returns>the value of the objective function.</returns>
		/// <seealso cref="GetObjectiveValue()"/>
		[Obsolete]
		Number CalculateObjective();

		/// <summary>
		/// Read only access to the value of the objective function for the current
		/// solution.
		/// </summary>
		/// <returns>the value of the objective function for the current solution.</returns>
		/// <since>2.1</since>
		Number GetObjectiveValue();

		/// <summary>Force the value of the objective function.</summary>
		/// <remarks>
		/// Force the value of the objective function.
		/// This is especially useful to iterate over optimal solutions.
		/// </remarks>
		/// <exception cref="ContradictionException"/>
		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		void ForceObjectiveValueTo(Number forcedValue);

		/// <summary>Discard the current solution in the optimization problem.</summary>
		/// <remarks>
		/// Discard the current solution in the optimization problem.
		/// THE NAME WAS NOT NICE. STILL AVAILABLE TO AVOID BREAKING THE API. PLEASE
		/// USE THE LONGER discardCurrentSolution() instead.
		/// </remarks>
		/// <exception cref="ContradictionException">if a trivial inconsistency is detected.</exception>
		/// <seealso cref="DiscardCurrentSolution()"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		[Obsolete]
		void Discard();

		/// <summary>Discard the current solution in the optimization problem.</summary>
		/// <exception cref="ContradictionException">if a trivial inconsistency is detected.</exception>
		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		void DiscardCurrentSolution();

		/// <summary>
		/// Allows to check afterwards if the solution provided by the solver is
		/// optimal or not.
		/// </summary>
		/// <returns/>
		bool IsOptimal();

		/// <summary>Allow to set a specific timeout when the solver is in optimization mode.</summary>
		/// <remarks>
		/// Allow to set a specific timeout when the solver is in optimization mode.
		/// The solver internal timeout will be set to that value once it has found a
		/// solution. That way, the original timeout of the solver may be reduced if
		/// the solver finds quickly a solution, or increased if the solver finds
		/// regularly new solutions (thus giving more time to the solver each time).
		/// </remarks>
		/// <since>2.3.3</since>
		void SetTimeoutForFindingBetterSolution(int seconds);
	}
}
