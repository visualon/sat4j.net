using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	[System.Serializable]
	public class LearnedTracing : SearchListenerAdapter<ISolverService>
	{
		private const long serialVersionUID = 1L;

		private ISolverService solverService;

		private readonly IVisualizationTool visuTool;

		public LearnedTracing(IVisualizationTool visuTool)
		{
			this.visuTool = visuTool;
		}

		public override void SolutionFound(int[] model, RandomAccessModel lazyModel)
		{
			Trace();
		}

		public override void Restarting()
		{
			Trace();
		}

		private void Trace()
		{
			this.visuTool.Init();
			IVec<IConstr> constrs = this.solverService.GetLearnedConstraints();
			int n = constrs.Size();
			for (int i = 0; i < n; i++)
			{
				this.visuTool.AddPoint(i, constrs.Get(i).GetActivity());
			}
			this.visuTool.End();
		}

		public override void Init(ISolverService solverService)
		{
			this.solverService = solverService;
		}

		public override void Cleaning()
		{
			Trace();
		}
	}
}
