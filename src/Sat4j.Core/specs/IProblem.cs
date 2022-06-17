using System;
using System.IO;
using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>Access to the information related to a given problem instance.</summary>
	/// <author>leberre</author>
	public interface IProblem : RandomAccessModel
	{
		/// <summary>Provide a model (if any) for a satisfiable formula.</summary>
		/// <remarks>
		/// Provide a model (if any) for a satisfiable formula. That method should be
		/// called AFTER isSatisfiable() or isSatisfiable(IVecInt) if the formula is
		/// satisfiable. Else an exception UnsupportedOperationException is launched.
		/// </remarks>
		/// <returns>a model of the formula as an array of literals to satisfy.</returns>
		/// <seealso cref="IsSatisfiable()"/>
		/// <seealso cref="IsSatisfiable(IVecInt)"/>
		int[] Model();

		/// <summary>Provide a prime implicant, i.e.</summary>
		/// <remarks>
		/// Provide a prime implicant, i.e. a set of literal that is sufficient to
		/// satisfy all constraints of the problem.
		/// </remarks>
		/// <returns>
		/// a prime implicant of the formula as an array of literal, Dimacs
		/// format.
		/// </returns>
		/// <since>2.3</since>
		int[] PrimeImplicant();

		/// <summary>
		/// Check if a given literal is part of the prime implicant computed by the
		/// <see cref="PrimeImplicant()"/>
		/// method.
		/// </summary>
		/// <param name="p">a literal in Dimacs format</param>
		/// <returns>
		/// true iff p belongs to
		/// <see cref="PrimeImplicant()"/>
		/// </returns>
		bool PrimeImplicant(int p);

		/// <summary>
		/// Check the satisfiability of the set of constraints contained inside the
		/// solver.
		/// </summary>
		/// <returns>true if the set of constraints is satisfiable, else false.</returns>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		bool IsSatisfiable();

		/// <summary>
		/// Check the satisfiability of the set of constraints contained inside the
		/// solver.
		/// </summary>
		/// <param name="assumps">
		/// a set of literals (represented by usual non null integers in
		/// Dimacs format).
		/// </param>
		/// <param name="globalTimeout">
		/// whether that call is part of a global process (i.e.
		/// optimization) or not. if (global), the timeout will not be
		/// reset between each call.
		/// </param>
		/// <returns>
		/// true if the set of constraints is satisfiable when literals are
		/// satisfied, else false.
		/// </returns>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		bool IsSatisfiable(IVecInt assumps, bool globalTimeout);

		/// <summary>
		/// Check the satisfiability of the set of constraints contained inside the
		/// solver.
		/// </summary>
		/// <param name="globalTimeout">
		/// whether that call is part of a global process (i.e.
		/// optimization) or not. if (global), the timeout will not be
		/// reset between each call.
		/// </param>
		/// <returns>true if the set of constraints is satisfiable, else false.</returns>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		bool IsSatisfiable(bool globalTimeout);

		/// <summary>
		/// Check the satisfiability of the set of constraints contained inside the
		/// solver.
		/// </summary>
		/// <param name="assumps">
		/// a set of literals (represented by usual non null integers in
		/// Dimacs format).
		/// </param>
		/// <returns>
		/// true if the set of constraints is satisfiable when literals are
		/// satisfied, else false.
		/// </returns>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		bool IsSatisfiable(IVecInt assumps);

		/// <summary>Look for a model satisfying all the clauses available in the problem.</summary>
		/// <remarks>
		/// Look for a model satisfying all the clauses available in the problem. It
		/// is an alternative to isSatisfiable() and model() methods, as shown in the
		/// pseudo-code: <code>
		/// if (isSatisfiable()) {
		/// return model();
		/// }
		/// return null;
		/// </code>
		/// </remarks>
		/// <returns>
		/// a model of the formula as an array of literals to satisfy, or
		/// <code>null</code> if no model is found
		/// </returns>
		/// <exception cref="TimeoutException">if a model cannot be found within the given timeout.</exception>
		/// <since>1.7</since>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		int[] FindModel();

		/// <summary>Look for a model satisfying all the clauses available in the problem.</summary>
		/// <remarks>
		/// Look for a model satisfying all the clauses available in the problem. It
		/// is an alternative to isSatisfiable(IVecInt) and model() methods, as shown
		/// in the pseudo-code: <code>
		/// if (isSatisfiable(assumpt)) {
		/// return model();
		/// }
		/// return null;
		/// </code>
		/// </remarks>
		/// <returns>
		/// a model of the formula as an array of literals to satisfy, or
		/// <code>null</code> if no model is found
		/// </returns>
		/// <exception cref="TimeoutException">if a model cannot be found within the given timeout.</exception>
		/// <since>1.7</since>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		int[] FindModel(IVecInt assumps);

		/// <summary>To know the number of constraints currently available in the solver.</summary>
		/// <remarks>
		/// To know the number of constraints currently available in the solver.
		/// (without taking into account learned constraints).
		/// </remarks>
		/// <returns>the number of constraints added to the solver</returns>
		int NConstraints();

		/// <summary>
		/// Declare <code>howmany</code> variables in the problem (and thus in the
		/// vocabulary), that will be represented using the Dimacs format by integers
		/// ranging from 1 to howmany.
		/// </summary>
		/// <remarks>
		/// Declare <code>howmany</code> variables in the problem (and thus in the
		/// vocabulary), that will be represented using the Dimacs format by integers
		/// ranging from 1 to howmany. That feature allows encodings to create
		/// additional variables with identifier starting at howmany+1.
		/// </remarks>
		/// <param name="howmany">number of variables to create</param>
		/// <returns>
		/// the total number of variables available in the solver (the
		/// highest variable number)
		/// </returns>
		/// <seealso cref="NVars()"/>
		int NewVar(int howmany);

		/// <summary>
		/// To know the number of variables used in the solver as declared by
		/// newVar()
		/// In case the method newVar() has not been used, the method returns the
		/// number of variables used in the solver.
		/// </summary>
		/// <returns>the number of variables created using newVar().</returns>
		/// <seealso cref="NewVar(int)"/>
		int NVars();

		/// <summary>To print additional informations regarding the problem.</summary>
		/// <param name="out">the place to print the information</param>
		/// <param name="prefix">the prefix to put in front of each line</param>
		[Obsolete]
		void PrintInfos(PrintWriter @out, string prefix);

		/// <summary>To print additional informations regarding the problem.</summary>
		/// <param name="out">the place to print the information</param>
		/// <since>2.3.3</since>
		void PrintInfos(PrintWriter @out);
	}
}
