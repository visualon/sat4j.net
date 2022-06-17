using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	[System.Serializable]
	public class HeuristicsTracing : SearchListenerAdapter<ISolverService>
	{
		private const long serialVersionUID = 1L;

		private ISolverService solverService;

		private readonly IVisualizationTool visuTool;

		public HeuristicsTracing(IVisualizationTool visuTool)
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
			int n = this.solverService.NVars();
			double[] heuristics = this.solverService.GetVariableHeuristics();
			for (int i = 1; i <= n; i++)
			{
				this.visuTool.AddPoint(heuristics[i], i);
			}
			this.visuTool.End();
		}

		public override void Init(ISolverService solverService)
		{
			this.solverService = solverService;
		}
	}
}
