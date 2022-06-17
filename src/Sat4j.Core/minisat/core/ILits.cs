using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>That interface manages the solver's internal vocabulary.</summary>
	/// <remarks>
	/// That interface manages the solver's internal vocabulary. Everything related
	/// to variables and literals is available from here.
	/// For sake of efficiency, literals and variables are not object in SAT4J. They
	/// are represented by numbers. If the vocabulary contains n variables, then
	/// variables should be accessed by numbers from 1 to n and literals by numbers
	/// from 2 to 2*n+1.
	/// For a Dimacs variable v, the variable index in SAT4J is v, it's positive
	/// literal is <code>2*v (v &lt;&lt; 1)</code> and it's negative literal is
	/// <code>2*v+1 ((v&lt;&lt;1)^1)</code>. Note that one can easily access to the
	/// complementary literal of p by using bitwise operation ^.
	/// In SAT4J, literals are usualy denoted by p or q and variables by v or x.
	/// </remarks>
	/// <author>leberre</author>
	public interface ILits
	{
		void Init(int nvar);

		/// <summary>Translates a Dimacs literal into an internal representation literal.</summary>
		/// <param name="x">the Dimacs literal (a non null integer).</param>
		/// <returns>the literal in the internal representation.</returns>
		int GetFromPool(int x);

		/// <summary>Returns true iff the variable is used in the set of constraints.</summary>
		/// <param name="x"/>
		/// <returns>true iff the variable belongs to the formula.</returns>
		bool BelongsToPool(int x);

		/// <summary>reset the vocabulary.</summary>
		void ResetPool();

		/// <summary>
		/// Make sure that all data structures are ready to manage howmany boolean
		/// variables.
		/// </summary>
		/// <param name="howmany">the new capacity (in boolean variables) of the vocabulary.</param>
		void EnsurePool(int howmany);

		/// <summary>Unassigns a boolean variable (truth value if UNDEF).</summary>
		/// <param name="lit">a literal in internal format.</param>
		void Unassign(int lit);

		/// <summary>Satisfies a boolean variable (truth value is TRUE).</summary>
		/// <param name="lit">a literal in internal format.</param>
		void Satisfies(int lit);

		/// <summary>Removes a variable from the formula.</summary>
		/// <remarks>
		/// Removes a variable from the formula. All occurrences of that variables
		/// are removed. It is equivalent in our implementation to falsify the two
		/// phases of that variable.
		/// </remarks>
		/// <param name="var">a variable in Dimacs format.</param>
		/// <since>2.3.2</since>
		void Forgets(int var);

		/// <summary>Check if a literal is satisfied.</summary>
		/// <param name="lit">a literal in internal format.</param>
		/// <returns>true if that literal is satisfied.</returns>
		bool IsSatisfied(int lit);

		/// <summary>Check if a literal is falsified.</summary>
		/// <param name="lit">a literal in internal format.</param>
		/// <returns>
		/// true if the literal is falsified. Note that a forgotten variable
		/// will also see its literals as falsified.
		/// </returns>
		bool IsFalsified(int lit);

		/// <summary>Check if a literal is assigned a truth value.</summary>
		/// <param name="lit">a literal in internal format.</param>
		/// <returns>true if the literal is neither satisfied nor falsified.</returns>
		bool IsUnassigned(int lit);

		/// <param name="lit"/>
		/// <returns>
		/// true iff the truth value of that literal is due to a unit
		/// propagation or a decision.
		/// </returns>
		bool IsImplied(int lit);

		/// <summary>to obtain the max id of the variable</summary>
		/// <returns>the maximum number of variables in the formula</returns>
		int NVars();

		/// <summary>to obtain the real number of variables appearing in the formula</summary>
		/// <returns>the number of variables used in the pool</returns>
		int RealnVars();

		/// <summary>Ask the solver for a free variable identifier, in Dimacs format (i.e.</summary>
		/// <remarks>
		/// Ask the solver for a free variable identifier, in Dimacs format (i.e. a
		/// positive number). Note that a previous call to ensurePool(max) will
		/// reserve in the solver the variable identifier from 1 to max, so
		/// nextFreeVarId() would return max+1, even if some variable identifiers
		/// smaller than max are not used.
		/// </remarks>
		/// <returns>
		/// a variable identifier not in use in the constraints already
		/// inside the solver.
		/// </returns>
		/// <since>2.1</since>
		int NextFreeVarId(bool reserve);

		/// <summary>Reset a literal in the vocabulary.</summary>
		/// <param name="lit">a literal in internal representation.</param>
		void Reset(int lit);

		/// <summary>
		/// Returns the level at which that literal has been assigned a value, else
		/// -1.
		/// </summary>
		/// <param name="lit">a literal in internal representation.</param>
		/// <returns>
		/// -1 if the literal is unassigned, or the decision level of the
		/// literal.
		/// </returns>
		int GetLevel(int lit);

		/// <summary>Sets the decision level of a literal.</summary>
		/// <param name="lit">a literal in internal representation.</param>
		/// <param name="l">a decision level, or -1</param>
		void SetLevel(int lit, int l);

		/// <summary>Returns the reason of the assignment of a literal.</summary>
		/// <param name="lit">a literal in internal representation.</param>
		/// <returns>the constraint that propagated that literal, else null.</returns>
		Constr GetReason(int lit);

		/// <summary>Sets the reason of the assignment of a literal.</summary>
		/// <param name="lit">a literal in internal representation.</param>
		/// <param name="r">
		/// the constraint that forces the assignment of that literal,
		/// null if there are none.
		/// </param>
		void SetReason(int lit, Constr r);

		/// <summary>Retrieve the methods to call when the solver backtracks.</summary>
		/// <remarks>
		/// Retrieve the methods to call when the solver backtracks. Useful for
		/// counter based data structures.
		/// </remarks>
		/// <param name="lit">a literal in internal representation.</param>
		/// <returns>a list of methods to call on bactracking.</returns>
		IVec<Undoable> Undos(int lit);

		/// <summary>Record a new constraint to watch when a literal is satisfied.</summary>
		/// <param name="lit">a literal in internal representation.</param>
		/// <param name="c">a constraint that contains the negation of that literal.</param>
		void Watch(int lit, Propagatable c);

		/// <param name="lit">a literal in internal representation.</param>
		/// <returns>the list of all the constraints that watch the negation of lit</returns>
		IVec<Propagatable> Watches(int lit);

		/// <summary>Returns a textual representation of the truth value of that literal.</summary>
		/// <param name="lit">a literal in internal representation.</param>
		/// <returns>one of T for true, F for False or ? for unassigned.</returns>
		string ValueToString(int lit);
	}

	public static class ILitsConstants
	{
		public const int Undefined = -1;
	}
}
