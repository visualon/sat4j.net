using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>
	/// The aim of the factory is to provide a concrete implementation of clauses,
	/// cardinality constraints and pseudo boolean consraints.
	/// </summary>
	/// <author>leberre</author>
	public interface DataStructureFactory
	{
		/// <param name="literals">
		/// a set of literals using Dimacs format (signed non null
		/// integers).
		/// </param>
		/// <returns>null if the constraint is a tautology.</returns>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException">the constraint is trivially unsatisfiable.</exception>
		/// <exception cref="System.NotSupportedException">there is no concrete implementation for that constraint.</exception>
		Constr CreateClause(IVecInt literals);

		Constr CreateUnregisteredClause(IVecInt literals);

		void LearnConstraint(Constr constr);

		/// <summary>Create a cardinality constraint of the form sum li &gt;= degree.</summary>
		/// <param name="literals">a set of literals.</param>
		/// <param name="degree">the degree of the cardinality constraint.</param>
		/// <returns>a constraint stating that at least degree literals are satisfied.</returns>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		Constr CreateCardinalityConstraint(IVecInt literals, int degree);

		Constr CreateUnregisteredCardinalityConstraint(IVecInt literals, int degree);

		void SetUnitPropagationListener(UnitPropagationListener s);

		void SetLearner(Learner l);

		void Reset();

		ILits GetVocabulary();

		/// <param name="p"/>
		/// <returns>
		/// a vector containing all the objects to be notified of the
		/// satisfaction of that literal.
		/// </returns>
		IVec<Propagatable> GetWatchesFor(int p);

		/// <param name="p"/>
		/// <param name="i">the index of the conflicting constraint</param>
		void ConflictDetectedInWatchesFor(int p, int i);
	}
}
