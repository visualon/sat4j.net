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
	public class CardinalityDataStructureYanMax : AbstractCardinalityDataStructure
	{
		private const long serialVersionUID = 1L;

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
			return MaxWatchCard.MaxWatchCardNew(this.solver, GetVocabulary(), literals, MinWatchCard.Atleast, 1);
		}

		public override Constr CreateUnregisteredClause(IVecInt literals)
		{
			return new LearntWLClause(literals, GetVocabulary());
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
			return MaxWatchCard.MaxWatchCardNew(this.solver, GetVocabulary(), literals, MinWatchCard.Atleast, degree);
		}

		public override Constr CreateUnregisteredCardinalityConstraint(IVecInt literals, int degree)
		{
			return new MaxWatchCard(GetVocabulary(), literals, MinWatchCard.Atleast, degree);
		}
	}
}
