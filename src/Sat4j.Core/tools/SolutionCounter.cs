using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>Another solver decorator that counts the number of solutions.</summary>
	/// <remarks>
	/// Another solver decorator that counts the number of solutions.
	/// Note that this approach is quite naive so do not expect it to work on large
	/// examples. The number of solutions will be wrong if the SAT solver does not
	/// provide a complete assignment.
	/// The class is expected to be used that way:
	/// <pre>
	/// SolutionCounter counter = new SolverCounter(SolverFactory.newDefault());
	/// try {
	/// int nbSol = counter.countSolutions();
	/// // the exact number of solutions is nbSol
	/// ...
	/// } catch (TimeoutException te) {
	/// int lowerBound = counter.lowerBound();
	/// // the solver found lowerBound solutions so far.
	/// ...
	/// }
	/// </pre>
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public class SolutionCounter : SolverDecorator<ISolver>
	{
		private const long serialVersionUID = 1L;

		private int lowerBound;

		public SolutionCounter(ISolver solver)
			: base(solver)
		{
		}

		/// <summary>Get the number of solutions found before the timeout occurs.</summary>
		/// <returns>the number of solutions found so far.</returns>
		/// <since>2.1</since>
		public virtual int LowerBound()
		{
			return this.lowerBound;
		}

		/// <summary>
		/// Naive approach to count the solutions available in a boolean formula:
		/// each time a solution is found, a new clause is added to prevent it to be
		/// found again.
		/// </summary>
		/// <returns>the number of solution found.</returns>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException">if the timeout given to the solver is reached.</exception>
		public virtual long CountSolutions()
		{
			this.lowerBound = 0;
			bool trivialfalsity = false;
			while (!trivialfalsity && IsSatisfiable(true))
			{
				this.lowerBound++;
				int[] last = Model();
				IVecInt clause = new VecInt(last.Length);
				foreach (int q in last)
				{
					clause.Push(-q);
				}
				try
				{
					AddClause(clause);
				}
				catch (ContradictionException)
				{
					trivialfalsity = true;
				}
			}
			return this.lowerBound;
		}
	}
}
