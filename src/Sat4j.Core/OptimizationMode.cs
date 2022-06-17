using System;
using System.IO;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Org.Sat4j.Tools;
using Sharpen;

namespace Org.Sat4j
{
	/// <summary>Behavior of the solver when an optimal solution is needed.</summary>
	/// <author>leberre</author>
	internal sealed class OptimizationMode : ILauncherMode
	{
		private int nbSolutions;

		private volatile ExitCode exitCode = ExitCode.Unknown;

		private bool isIncomplete = false;

		private PrintWriter @out;

		private long beginTime;

		private IOptimizationProblem problem;

		public void SetIncomplete(bool isIncomplete)
		{
			this.isIncomplete = isIncomplete;
		}

		public void DisplayResult(ISolver solver, IProblem problem, ILogAble logger, PrintWriter @out, Org.Sat4j.Reader.Reader reader, long beginTime, bool displaySolutionLine)
		{
			if (solver == null)
			{
				return;
			}
			System.Console.Out.Flush();
			@out.Flush();
			solver.PrintStat(@out);
			@out.WriteLine(ILauncherModeConstants.AnswerPrefix + exitCode);
			if (exitCode == ExitCode.Satisfiable || exitCode == ExitCode.OptimumFound || isIncomplete && exitCode == ExitCode.UpperBound)
			{
				System.Diagnostics.Debug.Assert(this.nbSolutions > 0);
				logger.Log("Found " + this.nbSolutions + " solution(s)");
				if (displaySolutionLine)
				{
					@out.Write(ILauncherModeConstants.SolutionPrefix);
					reader.Decode(problem.Model(), @out);
					@out.WriteLine();
				}
				IOptimizationProblem optproblem = (IOptimizationProblem)problem;
				if (!optproblem.HasNoObjectiveFunction())
				{
					string objvalue;
					if (optproblem is LexicoDecorator<object>)
					{
						IVec<Number> values = new Vec<Number>();
						LexicoDecorator<object> lexico = (LexicoDecorator<object>)optproblem;
						for (int i = 0; i < lexico.NumberOfCriteria(); i++)
						{
							values.Push(lexico.GetObjectiveValue(i));
						}
						objvalue = values.ToString();
					}
					else
					{
						objvalue = optproblem.GetObjectiveValue().ToString();
					}
					logger.Log("objective function=" + objvalue);
				}
			}
			//$NON-NLS-1$
			logger.Log("Total wall clock time (in seconds): " + (Runtime.CurrentTimeMillis() - beginTime) / 1000.0);
			//$NON-NLS-1$
			@out.Flush();
		}

		public void Solve(IProblem problem, Org.Sat4j.Reader.Reader reader, ILogAble logger, PrintWriter @out, long beginTime)
		{
			bool isSatisfiable = false;
			this.nbSolutions = 0;
			IOptimizationProblem optproblem = (IOptimizationProblem)problem;
			exitCode = ExitCode.Unknown;
			this.@out = @out;
			this.beginTime = beginTime;
			this.problem = optproblem;
			try
			{
				while (optproblem.AdmitABetterSolution())
				{
					++this.nbSolutions;
					if (!isSatisfiable)
					{
						if (optproblem.NonOptimalMeansSatisfiable())
						{
							logger.Log("SATISFIABLE");
							exitCode = ExitCode.Satisfiable;
							if (optproblem.HasNoObjectiveFunction())
							{
								return;
							}
						}
						else
						{
							if (isIncomplete)
							{
								exitCode = ExitCode.UpperBound;
							}
						}
						isSatisfiable = true;
						logger.Log("OPTIMIZING...");
					}
					//$NON-NLS-1$
					logger.Log("Got one! Elapsed wall clock time (in seconds):" + (Runtime.CurrentTimeMillis() - beginTime) / 1000.0);
					//$NON-NLS-1$
					@out.WriteLine(ILauncherModeConstants.CurrentOptimumValuePrefix + optproblem.GetObjectiveValue());
					optproblem.DiscardCurrentSolution();
				}
				if (isSatisfiable)
				{
					exitCode = ExitCode.OptimumFound;
				}
				else
				{
					exitCode = ExitCode.Unsatisfiable;
				}
			}
			catch (ContradictionException)
			{
				System.Diagnostics.Debug.Assert(isSatisfiable);
				exitCode = ExitCode.OptimumFound;
			}
			catch (TimeoutException)
			{
				logger.Log("timeout");
			}
		}

		public ExitCode GetCurrentExitCode()
		{
			return exitCode;
		}

		public void OnSolutionFound(int[] solution)
		{
			this.nbSolutions++;
			// this.exitCode = ExitCode.SATISFIABLE;
			this.@out.Printf("c Found solution #%d  (%.2f)s%n", nbSolutions, (Runtime.CurrentTimeMillis() - beginTime) / 1000.0);
			this.@out.WriteLine("c Value of objective function : " + problem.GetObjectiveValue());
			if (Runtime.GetProperty("printallmodels") != null)
			{
				this.@out.WriteLine(new VecInt(solution));
			}
		}

		public void OnSolutionFound(IVecInt solution)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public void OnUnsatTermination()
		{
		}

		// do nothing
		public void SetExitCode(ExitCode exitCode)
		{
			this.exitCode = exitCode;
		}
	}
}
