using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Learning
{
	/// <summary>Allows MiniSAT to do backjumping without learning.</summary>
	/// <remarks>
	/// Allows MiniSAT to do backjumping without learning. The literals appearing in
	/// the reason have their activity increased. That solution does not look good
	/// for VLIW-SAT-1.0 benchmarks (1785s vs 1346s).
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class NoLearningButHeuristics<D> : AbstractLearning<D>
		where D : DataStructureFactory
	{
		private const long serialVersionUID = 1L;

		public override void Learns(Constr reason)
		{
			ClaBumpActivity(reason);
		}
	}
}
