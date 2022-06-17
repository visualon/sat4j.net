using Sharpen;

namespace Org.Sat4j.Tools
{
	public interface IVisualizationTool
	{
		void AddPoint(double x, double y);

		void AddInvisiblePoint(double x, double y);

		void Init();

		void End();
	}

	public static class IVisualizationToolConstants
	{
		public const int Notgood = int.MinValue;
	}
}
