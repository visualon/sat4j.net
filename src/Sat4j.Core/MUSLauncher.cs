using System;
using Org.Sat4j.Minisat;
using Org.Sat4j.Reader;
using Org.Sat4j.Specs;
using Org.Sat4j.Tools;
using Org.Sat4j.Tools.Xplain;
using Sharpen;

namespace Org.Sat4j
{
	[System.Serializable]
	public class MUSLauncher : AbstractLauncher
	{
		private const long serialVersionUID = 1L;

		private int[] mus;

		private Explainer xplain;

		private bool highLevel = false;

		private AllMUSes allMuses;

		public override void Usage()
		{
			Log("java -jar sat4j-mus.jar [Insertion|Deletion|QuickXplain|all] <cnffile>|<gcnffile>");
		}

		protected internal override Org.Sat4j.Reader.Reader CreateReader(ISolver theSolver, string problemname)
		{
			if (this.highLevel)
			{
				return new GroupedCNFReader((GroupClauseSelectorSolver<ISolver>)theSolver);
			}
			return new LecteurDimacs(theSolver);
		}

		protected internal override string GetInstanceName(string[] args)
		{
			if (args.Length == 0)
			{
				return null;
			}
			return args[args.Length - 1];
		}

		protected internal override ISolver ConfigureSolver(string[] args)
		{
			string problemName = args[args.Length - 1];
			if (problemName.EndsWith(".gcnf"))
			{
				this.highLevel = true;
			}
			ISolver solver;
			if (this.highLevel)
			{
				HighLevelXplain<ISolver> hlxp = new HighLevelXplain<ISolver>(SolverFactory.NewDefault());
				this.xplain = hlxp;
				solver = hlxp;
			}
			else
			{
				Org.Sat4j.Tools.Xplain.Xplain<ISolver> xp = new Org.Sat4j.Tools.Xplain.Xplain<ISolver>(SolverFactory.NewDefault(), false);
				this.xplain = xp;
				solver = xp;
			}
			solver.SetDBSimplificationAllowed(true);
			if (args.Length == 2)
			{
				// retrieve minimization strategy
				if ("all".Equals(args[0]))
				{
					allMuses = new AllMUSes(highLevel, SolverFactory.Instance());
					solver = allMuses.GetSolverInstance();
				}
				else
				{
					string className = "org.sat4j.tools.xplain." + args[0] + "Strategy";
					try
					{
						this.xplain.SetMinimizationStrategy((MinimizationStrategy)System.Activator.CreateInstance(Sharpen.Runtime.GetType(className)));
					}
					catch (Exception e)
					{
						Log(e.Message);
					}
				}
			}
			solver.SetTimeout(int.MaxValue);
			GetLogWriter().WriteLine(solver.ToString(CommentPrefix));
			return solver;
		}

		protected internal override void DisplayResult()
		{
			if (this.solver != null)
			{
				double wallclocktime = (Runtime.CurrentTimeMillis() - this.beginTime) / 1000.0;
				this.solver.PrintStat(this.@out);
				this.solver.PrintInfos(this.@out);
				this.@out.WriteLine(ILauncherModeConstants.AnswerPrefix + this.GetExitCode());
				if (this.GetExitCode() == ExitCode.Satisfiable)
				{
					int[] model = this.solver.Model();
					this.@out.Write(ILauncherModeConstants.SolutionPrefix);
					this.reader.Decode(model, this.@out);
					this.@out.WriteLine();
				}
				else
				{
					if (this.GetExitCode() == ExitCode.Unsatisfiable && this.mus != null)
					{
						this.@out.Write(ILauncherModeConstants.SolutionPrefix);
						this.reader.Decode(this.mus, this.@out);
						this.@out.WriteLine();
					}
				}
				Log("Total wall clock time (in seconds) : " + wallclocktime);
			}
		}

		//$NON-NLS-1$
		public override void Run(string[] args)
		{
			this.mus = null;
			base.Run(args);
			double wallclocktime = (Runtime.CurrentTimeMillis() - this.beginTime) / 1000.0;
			if (this.GetExitCode() == ExitCode.Unsatisfiable)
			{
				try
				{
					Log("Unsat detection wall clock time (in seconds) : " + wallclocktime);
					double beginmus = Runtime.CurrentTimeMillis();
					if (allMuses != null)
					{
						SolutionFoundListener mssListener = new _SolutionFoundListener_160(this);
						SolutionFoundListener musListener = new _SolutionFoundListener_178(this);
						allMuses.ComputeAllMSS(mssListener);
						if ("card".Equals(Runtime.GetProperty("min")))
						{
							allMuses.ComputeAllMUSesOrdered(musListener);
						}
						else
						{
							allMuses.ComputeAllMUSes(musListener);
						}
						Log("All MUSes computation wall clock time (in seconds) : " + (Runtime.CurrentTimeMillis() - beginmus) / 1000.0);
					}
					else
					{
						Log("Size of initial " + (this.highLevel ? "high level " : string.Empty) + "unsat subformula: " + this.solver.UnsatExplanation().Size());
						Log("Computing " + (this.highLevel ? "high level " : string.Empty) + "MUS ...");
						this.mus = this.xplain.MinimalExplanation();
						Log("Size of the " + (this.highLevel ? "high level " : string.Empty) + "MUS: " + this.mus.Length);
						Log("Unsat core  computation wall clock time (in seconds) : " + (Runtime.CurrentTimeMillis() - beginmus) / 1000.0);
					}
				}
				catch (TimeoutException)
				{
					Log("Cannot compute " + (this.highLevel ? "high level " : string.Empty) + "MUS within the timeout.");
				}
			}
		}

		private sealed class _SolutionFoundListener_160 : SolutionFoundListener
		{
			public _SolutionFoundListener_160(MUSLauncher _enclosing)
			{
				this._enclosing = _enclosing;
				this.msscount = 0;
			}

			private int msscount;

			public void OnUnsatTermination()
			{
				throw new NotSupportedException("Not implemented yet!");
			}

			public void OnSolutionFound(IVecInt solution)
			{
				System.Console.Out.Write("\r" + this._enclosing.solver.GetLogPrefix() + "found mss number " + ++this.msscount);
			}

			public void OnSolutionFound(int[] solution)
			{
				throw new NotSupportedException("Not implemented yet!");
			}

			private readonly MUSLauncher _enclosing;
		}

		private sealed class _SolutionFoundListener_178 : SolutionFoundListener
		{
			public _SolutionFoundListener_178(MUSLauncher _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void OnSolutionFound(int[] solution)
			{
			}

			public void OnSolutionFound(IVecInt solution)
			{
				System.Console.Out.WriteLine(this._enclosing.solver.GetLogPrefix() + "found mus number " + ++this._enclosing.muscount);
				this._enclosing.@out.Write(ILauncherModeConstants.SolutionPrefix);
				int[] localMus = new int[solution.Size()];
				solution.CopyTo(localMus);
				this._enclosing.reader.Decode(localMus, this._enclosing.@out);
				this._enclosing.@out.WriteLine();
			}

			public void OnUnsatTermination()
			{
			}

			private readonly MUSLauncher _enclosing;
		}

		private int muscount = 0;

		public static void Main(string[] args)
		{
			MUSLauncher lanceur = new MUSLauncher();
			if (args.Length < 1 || args.Length > 2)
			{
				lanceur.Usage();
				return;
			}
			lanceur.AddHook();
			lanceur.Run(args);
			System.Environment.Exit(lanceur.GetExitCode().Value());
		}
	}
}
