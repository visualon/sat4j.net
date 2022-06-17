using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>
	/// The responsibility of that class is to choose the phase (positive or
	/// negative) of the variable that was selected by the IOrder.
	/// </summary>
	/// <author>leberre</author>
	public interface IPhaseSelectionStrategy
	{
		/// <summary>To be called when the activity of a literal changed.</summary>
		/// <param name="p">a literal. The associated variable will be updated.</param>
		void UpdateVar(int p);

		/// <summary>
		/// that method has the responsibility to initialize all arrays in the
		/// heuristics.
		/// </summary>
		/// <param name="nlength">the number of variables managed by the heuristics.</param>
		void Init(int nlength);

		/// <summary>initialize the phase of a given variable to the given value.</summary>
		/// <remarks>
		/// initialize the phase of a given variable to the given value. That method
		/// is suppose to be called AFTER init(int).
		/// </remarks>
		/// <param name="var">a variable</param>
		/// <param name="p">it's initial phase</param>
		void Init(int var, int p);

		/// <summary>indicate that a literal has been satisfied.</summary>
		/// <param name="p"/>
		void AssignLiteral(int p);

		/// <summary>
		/// selects the phase of the variable according to a phase selection
		/// strategy.
		/// </summary>
		/// <param name="var">a variable chosen by the heuristics</param>
		/// <returns>either var or not var, depending of the selection strategy.</returns>
		int Select(int var);

		/// <summary>
		/// Allow to perform a specific action when a literal of the current decision
		/// level is updated.
		/// </summary>
		/// <remarks>
		/// Allow to perform a specific action when a literal of the current decision
		/// level is updated. That method is called after
		/// <see cref="UpdateVar(int)"/>
		/// .
		/// </remarks>
		/// <param name="q">a literal</param>
		void UpdateVarAtDecisionLevel(int q);
	}
}
