using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>
	/// This interface is to be implemented by the classes wanted to be notified of
	/// the falsification of a literal.
	/// </summary>
	/// <remarks>
	/// This interface is to be implemented by the classes wanted to be notified of
	/// the falsification of a literal.
	/// That interface was moved here from org.sat4j.minisat.core package in release
	/// 2.3.6 to avoid cyclic dependencies with org.sat4j.specs.
	/// </remarks>
	/// <author>leberre</author>
	public interface Propagatable
	{
		/// <summary>
		/// Propagate the truth value of a literal in constraints in which that
		/// literal is falsified.
		/// </summary>
		/// <param name="s">something able to perform unit propagation</param>
		/// <param name="p">
		/// the literal being propagated. Its negation must appear in the
		/// constraint.
		/// </param>
		/// <returns>false iff an inconsistency (a contradiction) is detected.</returns>
		bool Propagate(UnitPropagationListener s, int p);

		/// <summary>Specific procedure for computing the prime implicants of a formula.</summary>
		/// <param name="l">an object to gather mandatory literals.</param>
		/// <param name="p">the falsified literal</param>
		/// <returns/>
		/// <since>2.3.6</since>
		bool PropagatePI(MandatoryLiteralListener l, int p);

		/// <summary>Allow to access a constraint view of the propagatable to avoid casting.</summary>
		/// <remarks>
		/// Allow to access a constraint view of the propagatable to avoid casting.
		/// In most cases, the constraint will implement directly propagatable thus
		/// will return itself. It will also also the implementation of more
		/// sophisticated watching strategy.
		/// </remarks>
		/// <returns>the constraint associated to that propagatable.</returns>
		/// <since>2.3.2</since>
		Constr ToConstraint();
	}
}
