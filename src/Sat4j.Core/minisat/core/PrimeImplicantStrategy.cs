using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>
	/// The aim of those classes is to reduce the model found by a SAT solver into a
	/// prime implicant.
	/// </summary>
	/// <remarks>
	/// The aim of those classes is to reduce the model found by a SAT solver into a
	/// prime implicant. See Computing prime implicants, David Deharbe, Pascal
	/// Fontaine, Daniel Le Berre and Bertrand Mazure @ FMCAD 2013 for details.
	/// </remarks>
	/// <author>leberre</author>
	public interface PrimeImplicantStrategy
	{
		/// <summary>returns a prime implicant from a solver object known to contain a model.</summary>
		/// <param name="solver"/>
		/// <returns>an sequence of Dimacs literals corresponding to the implicant.</returns>
		int[] Compute<_T0>(Solver<_T0> solver)
			where _T0 : DataStructureFactory;

		/// <summary>returns the prime implicant as an array with hole.</summary>
		/// <remarks>
		/// returns the prime implicant as an array with hole. This is convenient to
		/// check if a particular variable belong to the prime implicant computed by
		/// <see cref="Compute(Solver{D})"/>
		/// . Must be called after
		/// <see cref="Compute(Solver{D})"/>
		/// .
		/// </remarks>
		/// <returns/>
		int[] GetPrimeImplicantAsArrayWithHoles();
	}
}
