using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>
	/// Implementation of the strategy design pattern for allowing various learning
	/// schemes.
	/// </summary>
	/// <author>leberre</author>
	public interface LearningStrategy<D>
		where D : DataStructureFactory
	{
		/// <summary>hook method called just before the search begins.</summary>
		/// <remarks>
		/// hook method called just before the search begins. Useful to compute
		/// metrics/parameters based on the input formula.
		/// </remarks>
		void Init();

		void Learns(Constr constr);

		void SetVarActivityListener(VarActivityListener s);

		void SetSolver(Solver<D> s);
	}
}
