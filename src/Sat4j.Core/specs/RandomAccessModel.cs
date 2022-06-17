using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>
	/// That interface allows to efficiently retrieve the truth value of a given
	/// variable in the solver.
	/// </summary>
	/// <author>daniel</author>
	public interface RandomAccessModel
	{
		/// <summary>Provide the truth value of a specific variable in the model.</summary>
		/// <remarks>
		/// Provide the truth value of a specific variable in the model.
		/// That method should be called deciding that the problem is satisfiable.
		/// Else an exception UnsupportedOperationException is launched.
		/// </remarks>
		/// <param name="var">the variable id in Dimacs format</param>
		/// <returns>the truth value of that variable in the model</returns>
		/// <since>1.6</since>
		bool Model(int var);
	}
}
