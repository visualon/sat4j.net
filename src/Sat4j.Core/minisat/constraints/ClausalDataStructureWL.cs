using Org.Sat4j.Minisat.Constraints.Cnf;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints
{
	/// <author>
	/// leberre To change the template for this generated type comment go to
	/// Window - Preferences - Java - Code Generation - Code and Comments
	/// </author>
	[System.Serializable]
	public class ClausalDataStructureWL : AbstractDataStructureFactory
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
			return OriginalWLClause.BrandNewClause(this.solver, GetVocabulary(), v);
		}

		public override Constr CreateUnregisteredClause(IVecInt literals)
		{
			return new LearntWLClause(literals, GetVocabulary());
		}

		protected internal override ILits CreateLits()
		{
			return new Lits();
		}
	}
}
