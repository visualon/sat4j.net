using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>
	/// This solver decorator allows to detect whether or not the set of constraints
	/// available in the solver has only one solution or not.
	/// </summary>
	/// <remarks>
	/// This solver decorator allows to detect whether or not the set of constraints
	/// available in the solver has only one solution or not.
	/// NOTE THAT THIS DECORATOR CANNOT BE USED WITH SOLVERS USING SPECIFIC DATA
	/// STRUCTURES FOR BINARY OR TERNARY CLAUSES!
	/// <code>
	/// SingleSolutionDetector problem =
	/// new SingleSolutionDetector(SolverFactory.newMiniSAT());
	/// // feed problem/solver as usual
	/// if (problem.isSatisfiable()) {
	/// if (problem.hasASingleSolution()) {
	/// // great, the instance has a unique solution
	/// int [] uniquesolution = problem.getModel();
	/// } else {
	/// // too bad, got more than one
	/// }
	/// }
	/// </code>
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public class SingleSolutionDetector : SolverDecorator<ISolver>
	{
		private const long serialVersionUID = 1L;

		public SingleSolutionDetector(ISolver solver)
			: base(solver)
		{
		}

		/// <summary>
		/// Please use that method only after a positive answer from isSatisfiable()
		/// (else a runtime exception will be launched).
		/// </summary>
		/// <remarks>
		/// Please use that method only after a positive answer from isSatisfiable()
		/// (else a runtime exception will be launched).
		/// NOTE THAT THIS FUNCTION SHOULD NOT ONLY BE USED ONCE THE FINAL SOLUTION
		/// IS FOUND, SINCE THE METHOD ADDS CONSTRAINTS INTO THE SOLVER THAT MAY NOT
		/// BE REMOVED UNDER CERTAIN CONDITIONS (UNIT CONSTRAINTS LEARNT FOR
		/// INSTANCE). THAT ISSUE WILL BE RESOLVED ONCE REMOVECONSTR WILL WORK
		/// PROPERLY.
		/// </remarks>
		/// <returns>
		/// true iff there is only one way to satisfy all the constraints in
		/// the solver.
		/// </returns>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		/// <seealso cref="Org.Sat4j.Specs.ISolver.RemoveConstr(Org.Sat4j.Specs.IConstr)"/>
		public virtual bool HasASingleSolution()
		{
			return HasASingleSolution(new VecInt());
		}

		/// <summary>
		/// Please use that method only after a positive answer from
		/// isSatisfiable(assumptions) (else a runtime exception will be launched).
		/// </summary>
		/// <param name="assumptions">a set of literals (dimacs numbering) that must be satisfied.</param>
		/// <returns>
		/// true iff there is only one way to satisfy all the constraints in
		/// the solver using the provided set of assumptions.
		/// </returns>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool HasASingleSolution(IVecInt assumptions)
		{
			int[] firstmodel = Model();
			System.Diagnostics.Debug.Assert(firstmodel != null);
			IVecInt clause = new VecInt(firstmodel.Length);
			foreach (int q in firstmodel)
			{
				clause.Push(-q);
			}
			bool result = false;
			try
			{
				IConstr added = AddClause(clause);
				result = !IsSatisfiable(assumptions);
				RemoveConstr(added);
			}
			catch (ContradictionException)
			{
				result = true;
			}
			return result;
		}
	}
}
