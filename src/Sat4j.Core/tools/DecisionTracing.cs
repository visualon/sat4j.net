using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <since>2.2</since>
	[System.Serializable]
	public class DecisionTracing : SearchListenerAdapter<ISolverService>
	{
		private const long serialVersionUID = 1L;

		private int counter;

		private readonly IVisualizationTool positiveVisu;

		private readonly IVisualizationTool negativeVisu;

		private readonly IVisualizationTool restartVisu;

		private readonly IVisualizationTool cleanVisu;

		private int nVar;

		public DecisionTracing(IVisualizationTool positiveVisu, IVisualizationTool negativeVisu, IVisualizationTool restartVisu, IVisualizationTool cleanVisu)
		{
			this.positiveVisu = positiveVisu;
			this.negativeVisu = negativeVisu;
			this.restartVisu = restartVisu;
			this.cleanVisu = cleanVisu;
			this.counter = 1;
		}

		public override void Assuming(int p)
		{
			if (p > 0)
			{
				this.positiveVisu.AddPoint(this.counter, p);
				this.negativeVisu.AddInvisiblePoint(this.counter, 0);
			}
			else
			{
				this.negativeVisu.AddPoint(this.counter, -p);
				this.positiveVisu.AddInvisiblePoint(this.counter, 0);
			}
			this.restartVisu.AddInvisiblePoint(this.counter, 0);
			this.cleanVisu.AddInvisiblePoint(this.counter, 0);
			this.counter++;
		}

		public override void Restarting()
		{
			this.restartVisu.AddPoint(this.counter, this.nVar);
			this.cleanVisu.AddPoint(this.counter, 0);
			this.positiveVisu.AddInvisiblePoint(this.counter, 0);
			this.negativeVisu.AddInvisiblePoint(this.counter, 0);
		}

		public override void End(Lbool result)
		{
			this.positiveVisu.End();
			this.negativeVisu.End();
			this.restartVisu.End();
			this.cleanVisu.End();
		}

		public override void Start()
		{
			this.counter = 1;
		}

		public override void Init(ISolverService solverService)
		{
			this.nVar = solverService.NVars();
			this.positiveVisu.Init();
			this.negativeVisu.Init();
			this.restartVisu.Init();
			this.cleanVisu.Init();
		}

		public override void Cleaning()
		{
			this.restartVisu.AddPoint(this.counter, 0);
			this.cleanVisu.AddPoint(this.counter, this.nVar);
			this.positiveVisu.AddInvisiblePoint(this.counter, 0);
			this.negativeVisu.AddInvisiblePoint(this.counter, 0);
		}
	}
}
