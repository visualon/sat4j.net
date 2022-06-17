using System;
using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>Basic constraint abstraction used in this solvers.</summary>
	/// <remarks>
	/// Basic constraint abstraction used in this solvers.
	/// Any new constraint type should implement that interface.
	/// That interface was moved in this package from org.sat4j.minisat.core package
	/// in release 2.3.6 to avoid cyclic dependencies with org.sat4j.specs.
	/// </remarks>
	/// <author>leberre</author>
	public interface Constr : IConstr
	{
		private sealed class _Constr_48 : Constr
		{
			public _Constr_48()
			{
			}

			/*
			* Created on 16 oct. 2003
			*/
			public bool Learnt()
			{
				return false;
			}

			public int Size()
			{
				return 0;
			}

			public int Get(int i)
			{
				throw new NotSupportedException("No elements in a tautology");
			}

			public double GetActivity()
			{
				return 0.0;
			}

			public bool CanBePropagatedMultipleTimes()
			{
				return false;
			}

			public string ToString(VarMapper mapper)
			{
				return "TAUT";
			}

			public void Remove(UnitPropagationListener upl)
			{
			}

			// do nothing
			public bool Simplify()
			{
				return false;
			}

			public void CalcReason(int p, IVecInt outReason)
			{
				throw new NotSupportedException("A tautology cannot be a reason");
			}

			public void CalcReasonOnTheFly(int p, IVecInt trail, IVecInt outReason)
			{
				throw new NotSupportedException("A tautology cannot be a reason");
			}

			public void IncActivity(double claInc)
			{
			}

			// do nothing
			public void ForwardActivity(double claInc)
			{
			}

			// do nothing
			public bool Locked()
			{
				return false;
			}

			public void SetLearnt()
			{
			}

			// do nothing
			public void Register()
			{
			}

			// do nothing
			public void RescaleBy(double d)
			{
			}

			// do nothing
			public void SetActivity(double d)
			{
			}

			// do nothing
			public void AssertConstraint(UnitPropagationListener s)
			{
			}

			// do nothing
			public void AssertConstraintIfNeeded(UnitPropagationListener s)
			{
			}

			// do nothing
			public bool CanBeSatisfiedByCountingLiterals()
			{
				return false;
			}

			public int RequiredNumberOfSatisfiedLiterals()
			{
				return 0;
			}

			public bool IsSatisfied()
			{
				return true;
			}

			public int GetAssertionLevel(IVecInt trail, int decisionLevel)
			{
				return 0;
			}
		}

		/// <summary>Remove a constraint from the solver.</summary>
		/// <param name="upl"/>
		/// <since>2.1</since>
		void Remove(UnitPropagationListener upl);

		/// <summary>
		/// Simplifies a constraint, by removing top level falsified literals for
		/// instance.
		/// </summary>
		/// <returns>
		/// true iff the constraint is satisfied and can be removed from the
		/// database.
		/// </returns>
		bool Simplify();

		/// <summary>Compute the reason for a given assignment.</summary>
		/// <remarks>
		/// Compute the reason for a given assignment.
		/// If the constraint is a clause, it is supposed to be either a unit clause
		/// or a falsified one. It is expected that the falsification of the
		/// constraint has been detected as soon at is occurs (e.g. using
		/// <see cref="Propagatable.Propagate(UnitPropagationListener, int)"/>
		/// .
		/// </remarks>
		/// <param name="p">a satisfied literal (or Lit.UNDEFINED)</param>
		/// <param name="outReason">
		/// the list of falsified literals whose negation is the reason of
		/// the assignment of p to true.
		/// </param>
		void CalcReason(int p, IVecInt outReason);

		/// <summary>
		/// Compute the reason for a given assignment in a the constraint created on
		/// the fly in the solver.
		/// </summary>
		/// <remarks>
		/// Compute the reason for a given assignment in a the constraint created on
		/// the fly in the solver. Compared to the method
		/// <see cref="CalcReason(int, IVecInt)"/>
		/// , the falsification may not have been
		/// detected as soon as possible. As such, it is necessary to take into
		/// account the order of the literals in the trail.
		/// </remarks>
		/// <param name="p">a satisfied literal (or Lit.UNDEFINED)</param>
		/// <param name="trail">
		/// all the literals satisfied in the solvers, should not be
		/// modified.
		/// </param>
		/// <param name="outReason">
		/// a list of falsified literals whose negation is the reason of
		/// the assignment of p to true.
		/// </param>
		/// <since>2.3.3</since>
		void CalcReasonOnTheFly(int p, IVecInt trail, IVecInt outReason);

		/// <summary>Increase the constraint activity.</summary>
		/// <param name="claInc">the value to increase the activity with</param>
		void IncActivity(double claInc);

		/// <param name="claInc"/>
		/// <since>2.1</since>
		[Obsolete]
		void ForwardActivity(double claInc);

		/// <summary>Indicate wether a constraint is responsible from an assignment.</summary>
		/// <returns>true if a constraint is a "reason" for an assignment.</returns>
		bool Locked();

		/// <summary>Mark a constraint as learnt.</summary>
		void SetLearnt();

		/// <summary>Register the constraint to the solver.</summary>
		void Register();

		/// <summary>Rescale the clause activity by a value.</summary>
		/// <param name="d">the value to rescale the clause activity with.</param>
		void RescaleBy(double d);

		/// <summary>Set the activity at a specific value</summary>
		/// <param name="d">the new activity</param>
		/// <since>2.3.1</since>
		void SetActivity(double d);

		/// <summary>Method called when the constraint is to be asserted.</summary>
		/// <remarks>
		/// Method called when the constraint is to be asserted. It means that the
		/// constraint was learned during the search and it should now propagate some
		/// truth values. In the clausal case, only one literal should be propagated.
		/// In other cases, it might be different.
		/// </remarks>
		/// <param name="s">a UnitPropagationListener to use for unit propagation.</param>
		void AssertConstraint(UnitPropagationListener s);

		/// <summary>Method called when the constraint is added to the solver "on the fly".</summary>
		/// <remarks>
		/// Method called when the constraint is added to the solver "on the fly". In
		/// that case, the constraint may or may not have to propagate some literals,
		/// unlike the
		/// <see cref="AssertConstraint(UnitPropagationListener)"/>
		/// method.
		/// </remarks>
		/// <param name="s">a UnitPropagationListener to use for unit propagation.</param>
		/// <since>2.3.4</since>
		void AssertConstraintIfNeeded(UnitPropagationListener s);

		/// <summary>
		/// Check that a specific constraint can be checked for satisfiability by
		/// simply counting its number of satisfied literals.
		/// </summary>
		/// <remarks>
		/// Check that a specific constraint can be checked for satisfiability by
		/// simply counting its number of satisfied literals. This is the case for
		/// clauses and cardinality constraints. It is not the case for pseudo
		/// boolean constraints.
		/// </remarks>
		/// <returns>
		/// true iff the constraints can be satisfied by satisfying a given
		/// number of literals;
		/// </returns>
		/// <since>2.3.6</since>
		bool CanBeSatisfiedByCountingLiterals();

		/// <summary>Returns the number of literals necessary to satisfy that constraint.</summary>
		/// <remarks>
		/// Returns the number of literals necessary to satisfy that constraint. That
		/// method only make sense if the
		/// <see cref="CanBeSatisfiedByCountingLiterals()"/>
		/// returns true. For clauses, the value returned will be 1. For cardinality
		/// constraints, the value returned will be its degree.
		/// </remarks>
		/// <returns>the number of literals</returns>
		/// <since>2.3.6</since>
		int RequiredNumberOfSatisfiedLiterals();

		/// <summary>Checks that a constraint is satisfied.</summary>
		/// <remarks>
		/// Checks that a constraint is satisfied. That method may be time consuming
		/// since it may require to go through all the literals of the constraints.
		/// </remarks>
		/// <returns>
		/// true iff the constraint is satisfied under the current
		/// assignment.
		/// </returns>
		/// <since>2.3.6</since>
		bool IsSatisfied();

		/// <summary>Returns the level at which a constraint becomes assertive.</summary>
		/// <remarks>
		/// Returns the level at which a constraint becomes assertive. Note that if a
		/// constraint can propagate multiple times, such method should return the
		/// first decision level at which the clause is assertive. Note that the
		/// implementation of isAssertive must be in sync with
		/// <see cref="AssertConstraint(UnitPropagationListener)"/>
		/// since the usual step
		/// after detecting that a constraint is assertive will be to assert it.
		/// </remarks>
		/// <param name="trail">the internal solver trail</param>
		/// <param name="decisionLevel">the current decision level</param>
		/// <returns>
		/// the decision level under which the constraint becomes assertive,
		/// else -1.
		/// </returns>
		/// <since>2.3.6</since>
		/// <seealso cref="IConstr.CanBePropagatedMultipleTimes()"/>
		int GetAssertionLevel(IVecInt trail, int decisionLevel);
	}

	public static class ConstrConstants
	{
		public const Constr Tautology = new _Constr_48();
	}
}
