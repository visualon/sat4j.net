using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <since>2.2</since>
	[System.Serializable]
	public class ConflictDepthTracing : SearchListenerAdapter<ISolverService>
	{
		private const long serialVersionUID = 1L;

		private int counter;

		private int nVar;

		private readonly IVisualizationTool conflictDepthVisu;

		private readonly IVisualizationTool conflictDepthRestartVisu;

		private readonly IVisualizationTool conflictDepthCleanVisu;

		public ConflictDepthTracing(IVisualizationTool conflictDepthVisu, IVisualizationTool conflictDepthRestartVisu, IVisualizationTool conflictDepthCleanVisu)
		{
			this.conflictDepthVisu = conflictDepthVisu;
			this.conflictDepthRestartVisu = conflictDepthRestartVisu;
			this.conflictDepthCleanVisu = conflictDepthCleanVisu;
			this.counter = 0;
		}

		public override void ConflictFound(IConstr confl, int dlevel, int trailLevel)
		{
			this.conflictDepthVisu.AddPoint(this.counter, trailLevel);
			this.conflictDepthRestartVisu.AddInvisiblePoint(this.counter, trailLevel);
			this.conflictDepthCleanVisu.AddInvisiblePoint(this.counter, trailLevel);
			this.counter++;
		}

		public override void End(Lbool result)
		{
			this.conflictDepthVisu.End();
			this.conflictDepthRestartVisu.End();
			this.conflictDepthCleanVisu.End();
		}

		public override void Start()
		{
			this.conflictDepthVisu.Init();
			this.conflictDepthRestartVisu.Init();
			this.conflictDepthCleanVisu.Init();
			this.counter = 0;
		}

		public override void Restarting()
		{
			this.conflictDepthRestartVisu.AddPoint(this.counter, this.nVar);
			this.conflictDepthCleanVisu.AddPoint(this.counter, 0);
			this.conflictDepthVisu.AddInvisiblePoint(this.counter, this.nVar);
		}

		public override void Init(ISolverService solverService)
		{
			this.nVar = solverService.NVars();
		}

		public override void Cleaning()
		{
			this.conflictDepthRestartVisu.AddPoint(this.counter, 0);
			this.conflictDepthCleanVisu.AddPoint(this.counter, this.nVar);
			this.conflictDepthVisu.AddInvisiblePoint(this.counter, this.nVar);
		}
	}
}
