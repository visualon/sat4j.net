using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Strategy for simplifying the conflict clause derived by the solver.</summary>
	/// <author>daniel</author>
	public interface ISimplifier
	{
		void Simplify(IVecInt outLearnt);
	}
}
