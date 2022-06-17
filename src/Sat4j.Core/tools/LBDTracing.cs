using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	[System.Serializable]
	public class LBDTracing : SearchListenerAdapter<ISolverService>
	{
		private const long serialVersionUID = 1L;

		private readonly IVisualizationTool visuTool;

		private int counter;

		public LBDTracing(IVisualizationTool visuTool)
		{
			this.visuTool = visuTool;
			this.counter = 0;
		}

		public override void ConflictFound(IConstr confl, int dlevel, int trailLevel)
		{
			this.visuTool.AddPoint(this.counter, ((Constr)confl).GetActivity());
		}

		public override void Start()
		{
			this.visuTool.Init();
			this.counter = 0;
		}

		public override void End(Lbool result)
		{
			this.visuTool.End();
		}
	}
}
