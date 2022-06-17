using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>Allows the end user to react when a new solution is found.</summary>
	/// <remarks>
	/// Allows the end user to react when a new solution is found. This is typically
	/// the case when doing some upper bound optimization, or iterating on the
	/// models, or computing all MUses.
	/// </remarks>
	/// <author>leberre</author>
	/// <since>2.3.3</since>
	public interface SolutionFoundListener
	{
		

		// do nothing
		/// <summary>Callback method called when a new solution is found.</summary>
		/// <remarks>
		/// Callback method called when a new solution is found. While a solution
		/// will often be a model, it might also be the case that the solution is
		/// something else (MUS, group MUS, etc).
		/// </remarks>
		/// <param name="solution">a set of Dimacs literals.</param>
		void OnSolutionFound(int[] solution);

		/// <summary>Callback method called when a new solution is found.</summary>
		/// <remarks>
		/// Callback method called when a new solution is found. While a solution
		/// will often be a model, it might also be the case that the solution is
		/// something else (MUS, group MUS, etc).
		/// </remarks>
		/// <param name="solution">a set of Dimacs literals.</param>
		void OnSolutionFound(IVecInt solution);

		/// <summary>
		/// Callback method called when the search is finished (either unsat problem
		/// or no more solutions found)
		/// </summary>
		void OnUnsatTermination();
	}

	public static class SolutionFoundListenerConstants
	{
        private sealed class _SolutionFoundListener_45 : SolutionFoundListener
        {
            public _SolutionFoundListener_45()
            {
            }

            public void OnSolutionFound(int[] model)
            {
            }

            // do nothing
            public void OnSolutionFound(IVecInt solution)
            {
            }

            // do nothing
            public void OnUnsatTermination()
            {
            }
        }

        public static readonly SolutionFoundListener Void = new _SolutionFoundListener_45();
	}
}
