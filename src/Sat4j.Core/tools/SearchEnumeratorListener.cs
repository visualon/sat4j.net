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
	public class SearchEnumeratorListener : SearchListenerAdapter<ISolverService>
	{
		private const long serialVersionUID = 1L;

		private ISolverService solverService;

		private int nbsolutions = 0;

		private readonly SolutionFoundListener sfl;

		public SearchEnumeratorListener(SolutionFoundListener sfl)
		{
			this.sfl = sfl;
		}

		public override void Init(ISolverService solverService)
		{
			this.solverService = solverService;
		}

		public override void SolutionFound(int[] model, RandomAccessModel lazyModel)
		{
			IVecInt clauseToAdd = this.solverService.CreateBlockingClauseForCurrentModel();
			int[] vecint = new int[clauseToAdd.Size()];
			clauseToAdd.CopyTo(vecint);
			this.solverService.AddClauseOnTheFly(vecint);
			this.nbsolutions++;
			sfl.OnSolutionFound(model);
		}

		public override void End(Lbool result)
		{
			System.Diagnostics.Debug.Assert(result != Lbool.True);
			if (result == Lbool.False)
			{
				sfl.OnUnsatTermination();
			}
		}

		public virtual int GetNumberOfSolutionFound()
		{
			return this.nbsolutions;
		}
	}
}
