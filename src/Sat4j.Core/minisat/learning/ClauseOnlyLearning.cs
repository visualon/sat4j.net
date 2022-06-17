using Org.Sat4j.Minisat.Constraints.Cnf;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Learning
{
	/// <summary>The solver only records among all the constraints only the clauses.</summary>
	/// <author>daniel</author>
	/// <?/>
	[System.Serializable]
	public sealed class ClauseOnlyLearning<D> : LimitedLearning<D>
		where D : DataStructureFactory
	{
		private const long serialVersionUID = 1L;

		protected internal override bool LearningCondition(Constr constr)
		{
			return constr is WLClause;
		}

		public override string ToString()
		{
			return "Limit learning to clauses using watched literals only";
		}
	}
}
