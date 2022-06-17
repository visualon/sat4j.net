using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Learning
{
	/// <summary>Allows MiniSAT to do backjumping without learning.</summary>
	/// <remarks>
	/// Allows MiniSAT to do backjumping without learning. The literals appearing in
	/// the reason do not see their activity increased. That solution looks the best
	/// for VLIW-SAT-1.0 benchmarks (1346s vs 1785s).
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class NoLearningNoHeuristics<D> : AbstractLearning<D>
		where D : DataStructureFactory
	{
		private const long serialVersionUID = 1L;

		public override void Learns(Constr reason)
		{
		}
	}
}
