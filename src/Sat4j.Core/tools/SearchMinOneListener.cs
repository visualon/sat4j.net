using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>
	/// That class allows to iterate over the models from the inside: conflicts are
	/// created to ask the solver to backtrack.
	/// </summary>
	/// <author>leberre</author>
	[System.Serializable]
	public class SearchMinOneListener : SearchListenerAdapter<ISolverService>
	{
		private const long serialVersionUID = 1L;

		private ISolverService solverService;

		private readonly SolutionFoundListener sfl;

		public SearchMinOneListener(SolutionFoundListener sfl)
		{
			this.sfl = sfl;
		}

		public override void Init(ISolverService solverService)
		{
			this.solverService = solverService;
		}

		public override void SolutionFound(int[] model, RandomAccessModel lazyModel)
		{
			int degree = 0;
			int[] variables = new int[model.Length];
			for (int i = 0; i < model.Length; i++)
			{
				if (model[i] > 0)
				{
					degree++;
					variables[i] = model[i];
				}
				else
				{
					variables[i] = -model[i];
				}
			}
			System.Console.Out.WriteLine(solverService.GetLogPrefix() + " #one " + degree);
			this.solverService.AddAtMostOnTheFly(variables, degree - 1);
			sfl.OnSolutionFound(model);
		}

		public override void End(Lbool result)
		{
			System.Diagnostics.Debug.Assert(result != Lbool.True);
		}
	}
}
