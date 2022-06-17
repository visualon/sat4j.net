using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	[System.Serializable]
	public class SpeedTracing : SearchListenerAdapter<ISolverService>
	{
		private const long serialVersionUID = 1L;

		private readonly IVisualizationTool visuTool;

		private readonly IVisualizationTool cleanVisuTool;

		private readonly IVisualizationTool restartVisuTool;

		private long begin;

		private long end;

		private int counter;

		private long index;

		private double maxY;

		public SpeedTracing(IVisualizationTool visuTool, IVisualizationTool cleanVisuTool, IVisualizationTool restartVisuTool)
		{
			this.visuTool = visuTool;
			this.cleanVisuTool = cleanVisuTool;
			this.restartVisuTool = restartVisuTool;
			visuTool.Init();
			cleanVisuTool.Init();
			restartVisuTool.Init();
			this.begin = Runtime.CurrentTimeMillis();
			this.counter = 0;
			this.index = 0;
			this.maxY = 0;
		}

		public override void Propagating(int p)
		{
			this.end = Runtime.CurrentTimeMillis();
			double y;
			if (this.end - this.begin >= 2000)
			{
				long tmp = this.end - this.begin;
				this.index += tmp;
				y = (this.counter * 1000.0) / tmp;
				if (y > this.maxY)
				{
					this.maxY = y;
				}
				this.visuTool.AddPoint(this.index / 1000.0, y);
				this.cleanVisuTool.AddPoint(this.index / 1000.0, 0);
				this.restartVisuTool.AddPoint(this.index / 1000.0, 0);
				this.begin = Runtime.CurrentTimeMillis();
				this.counter = 0;
			}
			this.counter++;
		}

		public override void End(Lbool result)
		{
			this.visuTool.End();
			this.cleanVisuTool.End();
			this.restartVisuTool.End();
		}

		public override void Cleaning()
		{
			this.end = Runtime.CurrentTimeMillis();
			long indexClean = this.index + this.end - this.begin;
			this.visuTool.AddPoint(indexClean / 1000.0, (this.counter * 1000.0) / (this.end - this.begin));
			this.cleanVisuTool.AddPoint(indexClean / 1000.0, this.maxY);
			this.restartVisuTool.AddInvisiblePoint(indexClean, 0);
		}

		public override void Restarting()
		{
			this.end = Runtime.CurrentTimeMillis();
			long indexRestart = this.index + this.end - this.begin;
			double y = (this.counter * 1000.0) / (this.end - this.begin);
			this.visuTool.AddPoint(indexRestart / 1000.0, y);
			if (y > this.maxY)
			{
				this.maxY = y;
			}
			this.restartVisuTool.AddPoint(indexRestart / 1000.0, this.maxY);
			this.cleanVisuTool.AddInvisiblePoint(indexRestart, 0);
		}

		public override void Start()
		{
			this.visuTool.Init();
			this.cleanVisuTool.Init();
			this.restartVisuTool.Init();
			this.begin = Runtime.CurrentTimeMillis();
			this.counter = 0;
			this.index = 0;
		}
	}
}
