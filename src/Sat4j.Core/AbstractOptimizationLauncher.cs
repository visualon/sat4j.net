using System.IO;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j
{
	/// <summary>
	/// This class is intended to be used by launchers to solve optimization
	/// problems, i.e.
	/// </summary>
	/// <remarks>
	/// This class is intended to be used by launchers to solve optimization
	/// problems, i.e. problems for which a loop is needed to find the optimal
	/// solution.
	/// This class is no longer used since 2.3.3
	/// </remarks>
	/// <seealso cref="ILauncherMode"/>
	/// <author>leberre</author>
	[System.Serializable]
	public abstract class AbstractOptimizationLauncher : AbstractLauncher
	{
		private const long serialVersionUID = 1L;

		private const string CurrentOptimumValuePrefix = "o ";

		private bool incomplete = false;

		private bool displaySolutionLine = true;

		//$NON-NLS-1$
		protected internal override void SetIncomplete(bool value)
		{
			this.incomplete = value;
		}

		protected internal override void SetDisplaySolutionLine(bool value)
		{
			this.displaySolutionLine = value;
		}

		protected internal override void DisplayResult()
		{
			DisplayAnswer();
			Log("Total wall clock time (in seconds): " + (Runtime.CurrentTimeMillis() - GetBeginTime()) / 1000.0);
		}

		//$NON-NLS-1$
		protected internal virtual void DisplayAnswer()
		{
			if (this.solver == null)
			{
				return;
			}
			System.Console.Out.Flush();
			PrintWriter @out = GetLogWriter();
			@out.Flush();
			this.solver.PrintStat(@out, CommentPrefix);
			this.solver.PrintInfos(@out, CommentPrefix);
			ExitCode exitCode = GetExitCode();
			@out.WriteLine(ILauncherModeConstants.AnswerPrefix + exitCode);
			if (exitCode == ExitCode.Satisfiable || exitCode == ExitCode.OptimumFound || this.incomplete && exitCode == ExitCode.UpperBound)
			{
				if (this.displaySolutionLine)
				{
					@out.Write(ILauncherModeConstants.SolutionPrefix);
					GetReader().Decode(this.solver.Model(), @out);
					@out.WriteLine();
				}
				IOptimizationProblem optproblem = (IOptimizationProblem)this.solver;
				if (!optproblem.HasNoObjectiveFunction())
				{
					Log("objective function=" + optproblem.GetObjectiveValue());
				}
			}
		}

		//$NON-NLS-1$
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		protected internal override void Solve(IProblem problem)
		{
			bool isSatisfiable = false;
			IOptimizationProblem optproblem = (IOptimizationProblem)problem;
			try
			{
				while (optproblem.AdmitABetterSolution())
				{
					if (!isSatisfiable)
					{
						if (optproblem.NonOptimalMeansSatisfiable())
						{
							SetExitCode(ExitCode.Satisfiable);
							if (optproblem.HasNoObjectiveFunction())
							{
								return;
							}
							Log("SATISFIABLE");
						}
						else
						{
							//$NON-NLS-1$
							if (this.incomplete)
							{
								SetExitCode(ExitCode.UpperBound);
							}
						}
						isSatisfiable = true;
						Log("OPTIMIZING...");
					}
					//$NON-NLS-1$
					Log("Got one! Elapsed wall clock time (in seconds):" + (Runtime.CurrentTimeMillis() - GetBeginTime()) / 1000.0);
					//$NON-NLS-1$
					GetLogWriter().WriteLine(CurrentOptimumValuePrefix + optproblem.GetObjectiveValue());
					optproblem.DiscardCurrentSolution();
				}
				if (isSatisfiable)
				{
					SetExitCode(ExitCode.OptimumFound);
				}
				else
				{
					SetExitCode(ExitCode.Unsatisfiable);
				}
			}
			catch (ContradictionException)
			{
				System.Diagnostics.Debug.Assert(isSatisfiable);
				SetExitCode(ExitCode.OptimumFound);
			}
		}
	}
}
