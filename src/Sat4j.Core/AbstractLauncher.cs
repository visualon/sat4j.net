using System;
using System.IO;
using System.Text;
using Org.Sat4j.Core;
using Org.Sat4j.Reader;
using Org.Sat4j.Specs;
using Org.Sat4j.Tools;
using Sharpen;

using Properties = System.Collections.Hashtable;

namespace Org.Sat4j
{
	/// <summary>That class is used by launchers used to solve decision problems, i.e.</summary>
	/// <remarks>
	/// That class is used by launchers used to solve decision problems, i.e.
	/// problems with YES/NO/UNKNOWN answers.
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public abstract class AbstractLauncher : ILogAble
	{
		private const long serialVersionUID = 1L;

		public const string CommentPrefix = "c ";

		protected internal long beginTime;

		protected internal Org.Sat4j.Reader.Reader reader;

		protected internal bool feedWithDecorated = false;

		[System.NonSerialized]
		protected internal PrintWriter @out = new PrintWriter(System.Console.Out, true);

		private StringBuilder logBuffer;

		private bool displaySolutionLine = true;

		private sealed class _Thread_87 : Sharpen.Thread
		{
			public _Thread_87(AbstractLauncher _enclosing)
			{
				this._enclosing = _enclosing;
			}

			//$NON-NLS-1$
			public override void Run()
			{
				// stop the solver before displaying solutions
				if (this._enclosing.solver != null)
				{
					this._enclosing.solver.ExpireTimeout();
				}
				this._enclosing.DisplayResult();
			}

			private readonly AbstractLauncher _enclosing;
		}

		[System.NonSerialized]
		protected internal Sharpen.Thread shutdownHook;

		protected internal ISolver solver;

		protected internal IProblem problem;

		private bool silent = false;

		protected internal bool prime = Runtime.GetProperty("prime") != null;

		private ILauncherMode launcherMode = ILauncherModeConstants.Decision;

		protected internal virtual void SetLauncherMode(ILauncherMode launcherMode)
		{
			this.launcherMode = launcherMode;
		}

		protected internal virtual ILauncherMode GetLauncherMode()
		{
			return this.launcherMode;
		}

		protected internal virtual void SetIncomplete(bool isIncomplete)
		{
			this.launcherMode.SetIncomplete(isIncomplete);
		}

		public void AddHook()
		{
			Runtime.GetRuntime().AddShutdownHook(this.shutdownHook);
		}

		protected internal virtual void DisplayResult()
		{
			launcherMode.DisplayResult(solver, problem, this, @out, reader, beginTime, displaySolutionLine);
		}

		public abstract void Usage();

		protected internal void DisplayHeader()
		{
			DisplayLicense();
			Uri url = Runtime.GetRuntime().GetResource("/sat4j.version");
			//$NON-NLS-1$
			if (url == null)
			{
				Log("no version file found!!!");
			}
			else
			{
				//$NON-NLS-1$
				BufferedReader @in = null;
				try
				{
					@in = new BufferedReader(new InputStreamReader(url.OpenStream()));
					Log("version " + @in.ReadLine());
				}
				catch (IOException e)
				{
					//$NON-NLS-1$
					Log("c ERROR: " + e.Message);
				}
				finally
				{
					if (@in != null)
					{
						try
						{
							@in.Close();
						}
						catch (IOException e)
						{
							Log("c ERROR: " + e.Message);
						}
					}
				}
			}
			Properties prop = Runtime.GetProperties();
			string[] infoskeys = new string[] { "java.runtime.name", "java.vm.name", "java.vm.version", "java.vm.vendor", "sun.arch.data.model", "java.version", "os.name", "os.version", "os.arch" };
			//$NON-NLS-1$//$NON-NLS-2$
			//$NON-NLS-1$ //$NON-NLS-2$//$NON-NLS-3$
			foreach (string key in infoskeys)
			{
				Log(key + (key.Length < 14 ? "\t\t" : "\t") + prop.GetProperty(key));
			}
			//$NON-NLS-1$
			Runtime runtime = Runtime.GetRuntime();
			Log("Free memory \t\t" + runtime.FreeMemory());
			//$NON-NLS-1$
			Log("Max memory \t\t" + runtime.MaxMemory());
			//$NON-NLS-1$
			Log("Total memory \t\t" + runtime.TotalMemory());
			//$NON-NLS-1$
			Log("Number of processors \t" + runtime.AvailableProcessors());
		}

		//$NON-NLS-1$
		public virtual void DisplayLicense()
		{
			Log("SAT4J: a SATisfiability library for Java (c) 2004-2013 Artois University and CNRS");
			//$NON-NLS-1$
			Log("This is free software under the dual EPL/GNU LGPL licenses.");
			//$NON-NLS-1$
			Log("See www.sat4j.org for details.");
		}

		//$NON-NLS-1$
		/// <summary>Reads a problem file from the command line.</summary>
		/// <param name="problemname">the fully qualified name of the problem.</param>
		/// <returns>a reference to the problem to solve</returns>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException">if the problem is not expressed using the right format</exception>
		/// <exception cref="System.IO.IOException">for other IO problems</exception>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException">if the problem is found trivially unsat</exception>
		protected internal virtual IProblem ReadProblem(string problemname)
		{
			Log("solving " + problemname);
			//$NON-NLS-1$
			Log("reading problem ... ");
			//$NON-NLS-1$
			SolverDecorator<ISolver> decorator = null;
			ISolver originalProblem;
			if (feedWithDecorated)
			{
				decorator = (SolverDecorator<ISolver>)this.solver;
				originalProblem = decorator.Decorated();
			}
			else
			{
				originalProblem = this.solver;
			}
			this.reader = CreateReader(originalProblem, problemname);
			IProblem aProblem = this.reader.ParseInstance(problemname);
			if (this.reader.HasAMapping())
			{
				SearchListener<ISolverService> listener = this.solver.GetSearchListener<ISolverService>();
				if (listener is DotSearchTracing<string>)
				{
					((DotSearchTracing<string>)listener).SetMapping(this.reader.GetMapping());
				}
			}
			Log("... done. Wall clock time " + (Runtime.CurrentTimeMillis() - this.beginTime) / 1000.0 + "s.");
			//$NON-NLS-1$
			//$NON-NLS-1$
			Log("declared #vars     " + aProblem.NVars());
			//$NON-NLS-1$
			if (this.solver.NVars() < this.solver.RealNumberOfVariables())
			{
				Log("internal #vars     " + this.solver.RealNumberOfVariables());
			}
			//$NON-NLS-1$
			Log("#constraints  " + aProblem.NConstraints());
			//$NON-NLS-1$
			aProblem.PrintInfos(this.@out);
			if (Runtime.GetProperty("UNSATPROOF") != null)
			{
				string proofFile = problemname + ".rupproof";
				this.solver.SetSearchListener(new RupSearchListener<ISolverService>(proofFile));
				if (!this.IsSilent())
				{
					System.Console.Out.WriteLine(this.solver.GetLogPrefix() + "Generating unsat proof in file " + proofFile);
				}
			}
			if (feedWithDecorated)
			{
				return decorator;
			}
			return aProblem;
		}

		protected internal abstract Org.Sat4j.Reader.Reader CreateReader(ISolver theSolver, string problemname);

		public virtual void Run(string[] args)
		{
			try
			{
				DisplayHeader();
				this.solver = ConfigureSolver(args);
				if (this.solver == null)
				{
					Usage();
					return;
				}
				if (!this.IsSilent())
				{
					this.solver.SetVerbose(true);
				}
				ConfigureLauncher();
				string instanceName = GetInstanceName(args);
				if (instanceName == null)
				{
					Usage();
					return;
				}
				this.beginTime = Runtime.CurrentTimeMillis();
				this.problem = ReadProblem(instanceName);
				try
				{
					Solve(this.problem);
				}
				catch (Specs.TimeoutException)
				{
					Log("timeout");
				}
			}
			catch (FileNotFoundException e)
			{
				//$NON-NLS-1$
				System.Console.Error.WriteLine("FATAL " + e.GetMessage());
			}
			catch (IOException e)
			{
				System.Console.Error.WriteLine("FATAL " + e.GetMessage());
			}
			catch (ContradictionException)
			{
				this.launcherMode.SetExitCode(ExitCode.Unsatisfiable);
				Log("(trivial inconsistency)");
			}
			catch (ParseFormatException e)
			{
				//$NON-NLS-1$
				System.Console.Error.WriteLine("FATAL " + e.GetMessage());
			}
		}

		protected internal virtual void ConfigureLauncher()
		{
			string all = Runtime.GetProperty("all");
			if (all != null)
			{
				if ("external".Equals(all))
				{
					feedWithDecorated = true;
					this.solver = new ModelIteratorToSATAdapter(this.solver, launcherMode);
					System.Console.Out.WriteLine(this.solver.GetLogPrefix() + "model enumeration using the external way");
				}
				else
				{
					SearchEnumeratorListener enumerator = new SearchEnumeratorListener(launcherMode);
					this.solver.SetSearchListener(enumerator);
					System.Console.Out.WriteLine(this.solver.GetLogPrefix() + "model enumeration using the internal way");
				}
			}
			if (Runtime.GetProperty("minone") != null)
			{
				SearchMinOneListener minone = new SearchMinOneListener(launcherMode);
				this.solver.SetSearchListener(minone);
			}
		}

		protected internal abstract string GetInstanceName(string[] args);

		protected internal abstract ISolver ConfigureSolver(string[] args);

		/// <summary>Display messages as comments on STDOUT</summary>
		/// <param name="message">a textual information</param>
		public virtual void Log(string message)
		{
			if (this.IsSilent())
			{
				return;
			}
			if (this.logBuffer != null)
			{
				this.logBuffer.Append(CommentPrefix).Append(message).Append('\n');
			}
			else
			{
				this.@out.WriteLine(CommentPrefix + message);
			}
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		protected internal virtual void Solve(IProblem problem)
		{
			launcherMode.Solve(problem, reader, this, @out, beginTime);
		}

		/// <summary>To change the display so that solution line appears or not.</summary>
		/// <remarks>
		/// To change the display so that solution line appears or not. Recommended
		/// if solution is very large.
		/// </remarks>
		/// <param name="value">true to display the message</param>
		protected internal virtual void SetDisplaySolutionLine(bool value)
		{
			this.displaySolutionLine = value;
		}

		/// <summary>Change the value of the exit code in the Launcher</summary>
		/// <param name="exitCode">the new ExitCode</param>
		public void SetExitCode(ExitCode exitCode)
		{
			this.launcherMode.SetExitCode(exitCode);
		}

		/// <summary>Get the value of the ExitCode</summary>
		/// <returns>the current value of the Exitcode</returns>
		public ExitCode GetExitCode()
		{
			return this.launcherMode.GetCurrentExitCode();
		}

		/// <summary>
		/// Obtaining the current time spent since the beginning of the solving
		/// process.
		/// </summary>
		/// <returns>the time signature at the beginning of the run() method.</returns>
		public long GetBeginTime()
		{
			return this.beginTime;
		}

		/// <returns>the reader used to parse the instance</returns>
		public Org.Sat4j.Reader.Reader GetReader()
		{
			return this.reader;
		}

		/// <summary>To change the output stream on which statistics are displayed.</summary>
		/// <remarks>
		/// To change the output stream on which statistics are displayed. By
		/// default, the solver displays everything on System.out.
		/// </remarks>
		/// <param name="out">a new output for the statistics.</param>
		public virtual void SetLogWriter(PrintWriter @out)
		{
			this.@out = @out;
		}

		public virtual PrintWriter GetLogWriter()
		{
			return this.@out;
		}

		protected internal virtual void SetSilent(bool b)
		{
			this.silent = b;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.TypeLoadException"/>
		private void ReadObject(ObjectInputStream stream)
		{
			stream.DefaultReadObject();
			this.@out = new PrintWriter(System.Console.Out, true);
			this.shutdownHook = new _Thread_392(this);
		}

		private sealed class _Thread_392 : Sharpen.Thread
		{
			public _Thread_392(AbstractLauncher _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public override void Run()
			{
				this._enclosing.DisplayResult();
			}

			private readonly AbstractLauncher _enclosing;
		}

		protected internal virtual void ShowAvailableSolvers<T>(ASolverFactory<T> afactory)
			where T : ISolver
		{
			// if (afactory != null) {
			// log("Available solvers: "); //$NON-NLS-1$
			// String[] names = afactory.solverNames();
			// for (int i = 0; i < names.length; i++) {
			// log(names[i]);
			// }
			// }
			ShowAvailableSolvers(afactory, string.Empty);
		}

		protected internal virtual void ShowAvailableSolvers<T>(ASolverFactory<T> afactory, string framework)
			where T : ISolver
		{
			if (afactory != null)
			{
				if (framework.Length > 0)
				{
					Log("Available solvers for " + framework + ": ");
				}
				else
				{
					//$NON-NLS-1$
					Log("Available solvers: ");
				}
				//$NON-NLS-1$
				string[] names = afactory.SolverNames();
				foreach (string name in names)
				{
					Log(name);
				}
			}
		}

		protected internal virtual void BufferizeLog()
		{
			this.logBuffer = new StringBuilder();
		}

		protected internal virtual void FlushLog()
		{
			if (logBuffer != null)
			{
				this.@out.Write(logBuffer.ToString());
			}
			logBuffer = null;
		}

		public virtual bool IsSilent()
		{
			return silent;
		}

		public AbstractLauncher()
		{
			shutdownHook = new _Thread_87(this);
		}
	}
}
