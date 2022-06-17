using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <since>2.3.2</since>
	[System.Serializable]
	public class LearnedClausesSizeTracing : SearchListenerAdapter<ISolverService>
	{
		private const long serialVersionUID = 1L;

		private readonly IVisualizationTool visuTool;

		private readonly IVisualizationTool restartTool;

		private readonly IVisualizationTool cleanTool;

		private int counter;

		private int maxSize;

		public LearnedClausesSizeTracing(IVisualizationTool visuTool, IVisualizationTool restartTool, IVisualizationTool cleanTool)
		{
			this.visuTool = visuTool;
			this.restartTool = restartTool;
			this.cleanTool = cleanTool;
			this.counter = 0;
			this.maxSize = 0;
		}

		public override void End(Lbool result)
		{
			this.visuTool.End();
			this.restartTool.End();
			this.cleanTool.End();
		}

		public override void Learn(IConstr c)
		{
			int s = c.Size();
			if (s > this.maxSize)
			{
				this.maxSize = s;
			}
			this.visuTool.AddPoint(this.counter, s);
			this.restartTool.AddInvisiblePoint(this.counter, 0);
			this.cleanTool.AddInvisiblePoint(this.counter, 0);
			this.counter++;
		}

		public override void Start()
		{
			this.visuTool.Init();
			this.restartTool.Init();
			this.cleanTool.Init();
			this.counter = 0;
			this.maxSize = 0;
		}

		public override void Restarting()
		{
			this.visuTool.AddInvisiblePoint(this.counter, 0);
			this.restartTool.AddPoint(this.counter, this.maxSize);
			this.cleanTool.AddPoint(this.counter, 0);
		}

		public override void Cleaning()
		{
			this.visuTool.AddInvisiblePoint(this.counter, 0);
			this.restartTool.AddPoint(this.counter, 0);
			this.cleanTool.AddPoint(this.counter, this.maxSize);
		}
	}
}
