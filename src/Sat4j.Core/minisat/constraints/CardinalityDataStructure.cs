using Org.Sat4j.Minisat.Constraints.Card;
using Org.Sat4j.Minisat.Constraints.Cnf;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints
{
	/// <author>
	/// leberre To change the template for this generated type comment go to
	/// Window - Preferences - Java - Code Generation - Code and Comments
	/// </author>
	[System.Serializable]
	public class CardinalityDataStructure : AbstractCardinalityDataStructure
	{
		private const long serialVersionUID = 1L;

		public override Constr CreateUnregisteredClause(IVecInt literals)
		{
			return new LearntWLClause(literals, GetVocabulary());
		}

		/*
		* (non-Javadoc)
		*
		* @see
		* org.sat4j.minisat.DataStructureFactory#createClause(org.sat4j.datatype
		* .VecInt)
		*/
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override Constr CreateClause(IVecInt literals)
		{
			return AtLeast.AtLeastNew(this.solver, GetVocabulary(), literals, 1);
		}

		/*
		* (non-Javadoc)
		*
		* @see
		* org.sat4j.minisat.DataStructureFactory#createCardinalityConstraint(org
		* .sat4j.datatype.VecInt, int)
		*/
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override Constr CreateCardinalityConstraint(IVecInt literals, int degree)
		{
			return AtLeast.AtLeastNew(this.solver, GetVocabulary(), literals, degree);
		}

		public override Constr CreateUnregisteredCardinalityConstraint(IVecInt literals, int degree)
		{
			return new AtLeast(GetVocabulary(), literals, degree);
		}
	}
}
