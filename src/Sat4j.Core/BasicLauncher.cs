using Org.Sat4j.Core;
using Org.Sat4j.Minisat;
using Org.Sat4j.Reader;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j
{
	/// <summary>
	/// Very simple launcher, to be used during the SAT competition or the SAT race
	/// for instance.
	/// </summary>
	/// <author>daniel</author>
	[System.Serializable]
	public class BasicLauncher<T> : AbstractLauncher
		where T : ISolver
	{
		private const long serialVersionUID = 1L;

		private readonly ASolverFactory<T> factory;

		public BasicLauncher(ASolverFactory<T> factory)
		{
			this.factory = factory;
		}

		/// <summary>Lance le prouveur sur un fichier Dimacs.</summary>
		/// <param name="args">
		/// doit contenir le nom d'un fichier Dimacs, eventuellement
		/// compress?.
		/// </param>
		public static void Main(string[] args)
		{
			Org.Sat4j.BasicLauncher<ISolver> lanceur = new Org.Sat4j.BasicLauncher<ISolver>(SolverFactory.Instance());
			if (args.Length == 0 || args.Length > 2)
			{
				lanceur.Usage();
				return;
			}
			lanceur.AddHook();
			lanceur.Run(args);
			System.Environment.Exit(lanceur.GetExitCode().Value());
		}

		protected internal override ISolver ConfigureSolver(string[] args)
		{
			ISolver asolver;
			if (args.Length == 2)
			{
				asolver = this.factory.CreateSolverByName(args[0]);
			}
			else
			{
				asolver = this.factory.DefaultSolver();
			}
			asolver.SetTimeout(int.MaxValue);
			if (!"BRESIL".Equals(Runtime.GetProperty("prime")) && Runtime.GetProperty("all") == null)
			{
				asolver.SetDBSimplificationAllowed(true);
			}
			GetLogWriter().WriteLine(asolver.ToString(CommentPrefix));
			return asolver;
		}

		protected internal override Org.Sat4j.Reader.Reader CreateReader(ISolver theSolver, string problemname)
		{
			return new InstanceReader(theSolver);
		}

		public override void Usage()
		{
			Log("java -jar org.sat4j.core.jar <cnffile>");
		}

		protected internal override string GetInstanceName(string[] args)
		{
			if (args.Length == 0)
			{
				return null;
			}
			return args[args.Length - 1];
		}
	}
}
