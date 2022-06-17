using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <since>2.2</since>
	[System.Serializable]
	public class LearnedClauseSizeTracing : SearchListenerAdapter<ISolverService>
	{
		private const long serialVersionUID = 1L;

		private readonly IVisualizationTool visuTool;

		private int counter;

		public LearnedClauseSizeTracing(IVisualizationTool visuTool)
		{
			this.visuTool = visuTool;
			this.counter = 0;
		}

		public override void ConflictFound(IConstr confl, int dlevel, int trailLevel)
		{
			this.visuTool.AddPoint(this.counter, confl.Size());
			this.counter++;
		}

		public override void End(Lbool result)
		{
			this.visuTool.End();
		}
	}
}
