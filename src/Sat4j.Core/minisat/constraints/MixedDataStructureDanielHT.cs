using Org.Sat4j.Minisat.Constraints.Card;
using Org.Sat4j.Minisat.Constraints.Cnf;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints
{
	/// <summary>Uses specific data structure for cardinality constraints.</summary>
	/// <author>leberre</author>
	/// <since>2.1</since>
	[System.Serializable]
	public class MixedDataStructureDanielHT : AbstractDataStructureFactory
	{
		private const long serialVersionUID = 1L;

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
			IVecInt v = Clauses.SanityCheck(literals, GetVocabulary(), this.solver);
			if (v == null)
			{
				// tautological clause
				return null;
			}
			if (v.Size() == 1)
			{
				return new UnitClause(v.Last());
			}
			if (v.Size() == 2)
			{
				return OriginalBinaryClause.BrandNewClause(this.solver, GetVocabulary(), v);
			}
			return OriginalHTClause.BrandNewClause(this.solver, GetVocabulary(), v);
		}

		public override Constr CreateUnregisteredClause(IVecInt literals)
		{
			if (literals.Size() == 1)
			{
				return new UnitClause(literals.Last());
			}
			if (literals.Size() == 2)
			{
				return new LearntBinaryClause(literals, GetVocabulary());
			}
			return new LearntHTClause(literals, GetVocabulary());
		}

		protected internal override ILits CreateLits()
		{
			return new Lits();
		}
	}
}
