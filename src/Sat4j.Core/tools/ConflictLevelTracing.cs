using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <since>2.2</since>
	[System.Serializable]
	public class ConflictLevelTracing : SearchListenerAdapter<ISolverService>
	{
		private int counter;

		private const long serialVersionUID = 1L;

		private int nVar;

		private int maxDLevel;

		private readonly IVisualizationTool visuTool;

		private readonly IVisualizationTool restartVisuTool;

		private readonly IVisualizationTool cleanTool;

		public ConflictLevelTracing(IVisualizationTool visuTool, IVisualizationTool restartVisuTool, IVisualizationTool cleanTool)
		{
			this.visuTool = visuTool;
			this.restartVisuTool = restartVisuTool;
			this.cleanTool = cleanTool;
			this.counter = 1;
			this.maxDLevel = 0;
		}

		public override void ConflictFound(IConstr confl, int dlevel, int trailLevel)
		{
			if (dlevel > this.maxDLevel)
			{
				this.maxDLevel = dlevel;
			}
			this.visuTool.AddPoint(this.counter, dlevel);
			this.restartVisuTool.AddInvisiblePoint(this.counter, this.maxDLevel);
			this.cleanTool.AddInvisiblePoint(this.counter, this.maxDLevel);
			this.counter++;
		}

		public override void Restarting()
		{
			this.restartVisuTool.AddPoint(this.counter, this.maxDLevel);
			this.cleanTool.AddPoint(this.counter, 0);
			this.visuTool.AddInvisiblePoint(this.counter, this.nVar);
		}

		public override void End(Lbool result)
		{
			this.visuTool.End();
			this.cleanTool.End();
			this.restartVisuTool.End();
		}

		public override void Start()
		{
			this.visuTool.Init();
			this.restartVisuTool.Init();
			this.cleanTool.Init();
			this.counter = 1;
			this.maxDLevel = 0;
		}

		public override void Init(ISolverService solverService)
		{
			this.nVar = solverService.NVars();
		}

		public override void Cleaning()
		{
			this.restartVisuTool.AddPoint(this.counter, 0);
			this.cleanTool.AddPoint(this.counter, this.maxDLevel);
			this.visuTool.AddInvisiblePoint(this.counter, this.nVar);
		}
	}
}
