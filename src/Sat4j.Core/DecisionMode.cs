using System;
using System.IO;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Org.Sat4j.Tools;
using Sharpen;

namespace Org.Sat4j
{
	/// <summary>Behavior of the solver when one single solution is needed.</summary>
	/// <author>leberre</author>
	internal sealed class DecisionMode : ILauncherMode
	{
		private volatile ExitCode exitCode = ExitCode.Unknown;

		private int nbSolutionFound;

		private PrintWriter @out;

		private long beginTime;

		public void DisplayResult(ISolver solver, IProblem problem, ILogAble logger, PrintWriter @out, Org.Sat4j.Reader.Reader reader, long beginTime, bool displaySolutionLine)
		{
			if (solver != null)
			{
				@out.Flush();
				double wallclocktime = (Runtime.CurrentTimeMillis() - beginTime) / 1000.0;
				solver.PrintStat(@out);
				@out.WriteLine(ILauncherModeConstants.AnswerPrefix + exitCode);
				if (exitCode != ExitCode.Unknown && exitCode != ExitCode.Unsatisfiable)
				{
					int[] model = problem.Model();
					if (Runtime.GetProperty("prime") != null)
					{
						int initiallength = model.Length;
						logger.Log("returning a prime implicant ...");
						long beginpi = Runtime.CurrentTimeMillis();
						model = solver.PrimeImplicant();
						long endpi = Runtime.CurrentTimeMillis();
						logger.Log("removed " + (initiallength - model.Length) + " literals");
						logger.Log("pi computation time: " + (endpi - beginpi) + " ms");
					}
					if (Runtime.GetProperty("backbone") != null)
					{
						logger.Log("computing the backbone of the formula ...");
						long beginpi = Runtime.CurrentTimeMillis();
						model = solver.PrimeImplicant();
						try
						{
							IVecInt backbone = Backbone.Instance().Compute(solver, model);
							long endpi = Runtime.CurrentTimeMillis();
							@out.Write(solver.GetLogPrefix());
							reader.Decode(backbone.ToArray(), @out);
							@out.WriteLine();
							logger.Log("backbone computation time: " + (endpi - beginpi) + " ms");
						}
						catch (TimeoutException)
						{
							logger.Log("timeout, cannot compute the backbone.");
						}
					}
					if (nbSolutionFound >= 1)
					{
						logger.Log("Found " + nbSolutionFound + " solution(s)");
					}
					else
					{
						@out.Write(ILauncherModeConstants.SolutionPrefix);
						reader.Decode(model, @out);
						@out.WriteLine();
					}
				}
				logger.Log("Total wall clock time (in seconds) : " + wallclocktime);
			}
		}

		//$NON-NLS-1$
		public void Solve(IProblem problem, Org.Sat4j.Reader.Reader reader, ILogAble logger, PrintWriter @out, long beginTime)
		{
			this.exitCode = ExitCode.Unknown;
			this.@out = @out;
			this.nbSolutionFound = 0;
			this.beginTime = beginTime;
			try
			{
				if (problem.IsSatisfiable())
				{
					if (this.exitCode == ExitCode.Unknown)
					{
						this.exitCode = ExitCode.Satisfiable;
					}
				}
				else
				{
					if (this.exitCode == ExitCode.Unknown)
					{
						this.exitCode = ExitCode.Unsatisfiable;
					}
				}
			}
			catch (TimeoutException)
			{
				logger.Log("timeout");
			}
		}

		public void SetIncomplete(bool isIncomplete)
		{
		}

		public ExitCode GetCurrentExitCode()
		{
			return this.exitCode;
		}

		public void OnSolutionFound(int[] solution)
		{
			this.nbSolutionFound++;
			this.exitCode = ExitCode.Satisfiable;
			this.@out.Printf("c Found solution #%d  (%.2f)s%n", nbSolutionFound, (Runtime.CurrentTimeMillis() - beginTime) / 1000.0);
			if (Runtime.GetProperty("printallmodels") != null)
			{
				this.@out.WriteLine(ILauncherModeConstants.SolutionPrefix + new VecInt(solution));
			}
		}

		public void OnSolutionFound(IVecInt solution)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public void OnUnsatTermination()
		{
			if (this.exitCode == ExitCode.Satisfiable)
			{
				this.exitCode = ExitCode.OptimumFound;
			}
		}

		public void SetExitCode(ExitCode exitCode)
		{
			this.exitCode = exitCode;
		}
	}
}
