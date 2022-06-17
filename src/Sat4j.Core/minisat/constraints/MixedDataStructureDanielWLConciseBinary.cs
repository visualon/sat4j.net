using Org.Sat4j.Minisat.Constraints.Card;
using Org.Sat4j.Minisat.Constraints.Cnf;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints
{
	/// <author>leberre</author>
	/// <since>2.1</since>
	[System.Serializable]
	public class MixedDataStructureDanielWLConciseBinary : AbstractDataStructureFactory
	{
		private BinaryClauses[] binaryClauses;

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
				// return OriginalBinaryClause.brandNewClause(this.solver,
				// getVocabulary(), v);
				return CreateConciseBinaryClause(v);
			}
			return OriginalWLClause.BrandNewClause(this.solver, GetVocabulary(), v);
		}

		private Constr CreateConciseBinaryClause(IVecInt literals)
		{
			System.Diagnostics.Debug.Assert(literals.Size() == 2);
			if (binaryClauses == null)
			{
				binaryClauses = new BinaryClauses[GetVocabulary().NVars() * 2 + 2];
			}
			else
			{
				if (binaryClauses.Length < GetVocabulary().NVars() * 2)
				{
					BinaryClauses[] newBinaryClauses = new BinaryClauses[GetVocabulary().NVars() * 2 + 2];
					System.Array.Copy(binaryClauses, 0, newBinaryClauses, 0, binaryClauses.Length);
					binaryClauses = newBinaryClauses;
				}
			}
			BinaryClauses first;
			BinaryClauses second;
			if ((first = binaryClauses[literals.Get(0)]) == null)
			{
				first = new BinaryClauses(GetVocabulary(), literals.Get(0));
				GetVocabulary().Watch(literals.Get(0) ^ 1, first);
				binaryClauses[literals.Get(0)] = first;
			}
			if ((second = binaryClauses[literals.Get(1)]) == null)
			{
				second = new BinaryClauses(GetVocabulary(), literals.Get(1));
				GetVocabulary().Watch(literals.Get(1) ^ 1, second);
				binaryClauses[literals.Get(1)] = second;
			}
			first.AddBinaryClause(literals.Get(1));
			second.AddBinaryClause(literals.Get(0));
			return first;
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
			return new LearntWLClause(literals, GetVocabulary());
		}

		protected internal override ILits CreateLits()
		{
			return new Lits();
		}
	}
}
