using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>Interface providing the unit propagation capability.</summary>
	/// <remarks>
	/// Interface providing the unit propagation capability.
	/// Note that this interface was in the package org.sat4j.minisat.core prior to
	/// release 2.3.4. It was moved here because of the dependency from
	/// <see cref="UnitClauseProvider"/>
	/// .
	/// </remarks>
	/// <author>leberre</author>
	public interface UnitPropagationListener
	{
		/// <summary>satisfies a literal</summary>
		/// <param name="p">a literal</param>
		/// <returns>
		/// true if the assignment looks possible, false if a conflict
		/// occurs.
		/// </returns>
		bool Enqueue(int p);

		/// <summary>satisfies a literal</summary>
		/// <param name="p">a literal</param>
		/// <param name="from">a reason explaining why p should be satisfied.</param>
		/// <returns>
		/// true if the assignment looks possible, false if a conflict
		/// occurs.
		/// </returns>
		bool Enqueue(int p, Constr from);

		/// <summary>Unset a unit clause.</summary>
		/// <remarks>
		/// Unset a unit clause. The effect of such method is to unset all truth
		/// values on the stack until the given literal is found (that literal
		/// included).
		/// </remarks>
		/// <param name="p"/>
		/// <since>2.1</since>
		void Unset(int p);
	}
}
