using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <since>2.2</since>
	[System.Serializable]
	public class DecisionLevelTracing : SearchListenerAdapter<ISolverService>
	{
		private const long serialVersionUID = 1L;

		private int counter;

		private readonly IVisualizationTool visuTool;

		public DecisionLevelTracing(IVisualizationTool visuTool)
		{
			this.visuTool = visuTool;
			visuTool.Init();
			this.counter = 0;
		}

		public override void ConflictFound(IConstr confl, int dlevel, int trailLevel)
		{
			this.counter++;
		}

		public override void End(Lbool result)
		{
			this.visuTool.End();
		}

		public override void Start()
		{
			this.visuTool.Init();
		}

		public override void Backjump(int backjumpLevel)
		{
			this.visuTool.AddPoint(this.counter, backjumpLevel);
		}
	}
}
