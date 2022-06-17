using System.IO;
using Org.Sat4j.Specs;
using Org.Sat4j.Tools;
using Sharpen;

namespace Org.Sat4j
{
	/// <summary>
	/// Allow to change the behavior of the launcher (either decision or optimization
	/// mode)
	/// </summary>
	/// <since>2.3.3</since>
	/// <author>sroussel</author>
	public interface ILauncherMode : SolutionFoundListener
	{
		//$NON-NLS-1$
		//$NON-NLS-1$
		/// <summary>Output of the launcher when the solver stops</summary>
		/// <param name="solver">the solver that is launched by the launcher</param>
		/// <param name="problem">the problem that is solved</param>
		/// <param name="logger">the element that is able to log the result</param>
		/// <param name="out">the printwriter to associate to the solver</param>
		/// <param name="reader">the problem reader</param>
		/// <param name="beginTime">the time at which the solver was launched</param>
		/// <param name="displaySolutionLine">
		/// indicates whether the solution line shound be displayed or not
		/// (not recommended for large solutions)
		/// </param>
		void DisplayResult(ISolver solver, IProblem problem, ILogAble logger, PrintWriter @out, Org.Sat4j.Reader.Reader reader, long beginTime, bool displaySolutionLine);

		/// <summary>
		/// Main solver call: one call for a decision problem, a loop for an
		/// optimization problem.
		/// </summary>
		/// <param name="problem">the problem to solve</param>
		/// <param name="reader">the reader that provided the problem object</param>
		/// <param name="logger">the element that is able to log the result</param>
		/// <param name="out">the printwriter to associate to the solver</param>
		/// <param name="beginTime">the time at which the solver starts</param>
		void Solve(IProblem problem, Org.Sat4j.Reader.Reader reader, ILogAble logger, PrintWriter @out, long beginTime);

		/// <summary>
		/// Allows the launcher to specifically return an upper bound of the optimal
		/// solution in case of a time out (for maxsat competitions for instance).
		/// </summary>
		/// <param name="isIncomplete">
		/// true if the solver should return the best solution found so
		/// far.
		/// </param>
		void SetIncomplete(bool isIncomplete);

		/// <summary>
		/// Allow the launcher to get the current status of the problem: SAT, UNSAT,
		/// UPPER_BOUND, etc.
		/// </summary>
		/// <returns>the status of the problem.</returns>
		ExitCode GetCurrentExitCode();

		/// <summary>
		/// Allow to set a specific exit code to the launcher (in case of trivial
		/// unsatisfiability for instance).
		/// </summary>
		/// <param name="exitCode">the status of the problem to solve</param>
		void SetExitCode(ExitCode exitCode);
	}

	public static class ILauncherModeConstants
	{
		public const string SolutionPrefix = "v ";

		public const string AnswerPrefix = "s ";

		public const string CurrentOptimumValuePrefix = "o ";

		/// <summary>
		/// The launcher is in decision mode: the answer is either SAT, UNSAT or
		/// UNKNOWN
		/// </summary>
		public const ILauncherMode Decision = new DecisionMode();

		/// <summary>
		/// The launcher is in optimization mode: the answer is either SAT,
		/// UPPER_BOUND, OPTIMUM_FOUND, UNSAT or UNKNOWN.
		/// </summary>
		/// <remarks>
		/// The launcher is in optimization mode: the answer is either SAT,
		/// UPPER_BOUND, OPTIMUM_FOUND, UNSAT or UNKNOWN. Using the incomplete
		/// property, the solver returns an upper bound of the optimal solution when
		/// a time out occurs.
		/// </remarks>
		public const ILauncherMode Optimization = new OptimizationMode();
	}
}
