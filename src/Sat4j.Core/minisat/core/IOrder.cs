using System.IO;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Interface for the variable ordering heuristics.</summary>
	/// <remarks>
	/// Interface for the variable ordering heuristics. It has both the
	/// responsibility to choose the next variable to branch on and the phase of the
	/// literal (positive or negative one).
	/// </remarks>
	/// <author>daniel</author>
	public interface IOrder
	{
		/// <summary>Method used to provide an easy access the the solver vocabulary.</summary>
		/// <param name="lits">the vocabulary</param>
		void SetLits(ILits lits);

		/// <summary>Selects the next "best" unassigned literal.</summary>
		/// <remarks>
		/// Selects the next "best" unassigned literal.
		/// Note that it means selecting the best variable and the phase to branch on
		/// first.
		/// </remarks>
		/// <returns>an unassigned literal or Lit.UNDEFINED no such literal exists.</returns>
		int Select();

		/// <summary>Method called when a variable is unassigned.</summary>
		/// <remarks>
		/// Method called when a variable is unassigned.
		/// It is useful to add back a variable in the pool of variables to order.
		/// </remarks>
		/// <param name="x">a variable.</param>
		void Undo(int x);

		/// <summary>To be called when the activity of a literal changed.</summary>
		/// <param name="p">a literal. The associated variable will be updated.</param>
		void UpdateVar(int p);

		/// <summary>
		/// that method has the responsibility to initialize all arrays in the
		/// heuristics.
		/// </summary>
		/// <remarks>
		/// that method has the responsibility to initialize all arrays in the
		/// heuristics. PLEASE CALL super.init() IF YOU OVERRIDE THAT METHOD.
		/// </remarks>
		void Init();

		/// <summary>Display statistics regarding the heuristics.</summary>
		/// <param name="out">the writer to display the information in</param>
		/// <param name="prefix">to be used in front of each newline.</param>
		void PrintStat(PrintWriter @out, string prefix);

		/// <summary>
		/// Sets the variable activity decay as a growing factor for the next
		/// variable activity.
		/// </summary>
		/// <param name="d">
		/// a number bigger than 1 that will increase the activity of the
		/// variables involved in future conflict. This is similar but
		/// more efficient than decaying all the activities by a similar
		/// factor.
		/// </param>
		void SetVarDecay(double d);

		/// <summary>Decay the variables activities.</summary>
		void VarDecayActivity();

		/// <summary>To obtain the current activity of a variable.</summary>
		/// <param name="p">a literal</param>
		/// <returns>the activity of the variable associated to that literal.</returns>
		double VarActivity(int p);

		/// <summary>indicate that a literal has been satisfied.</summary>
		/// <param name="p"/>
		void AssignLiteral(int p);

		void SetPhaseSelectionStrategy(IPhaseSelectionStrategy strategy);

		IPhaseSelectionStrategy GetPhaseSelectionStrategy();

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

		/// <summary>Read-Only access to the value of the heuristics for each variable.</summary>
		/// <remarks>
		/// Read-Only access to the value of the heuristics for each variable. Note
		/// that for efficiency reason, the real array storing the value of the
		/// heuristics is returned. DO NOT CHANGE THE VALUES IN THAT ARRAY.
		/// </remarks>
		/// <returns>
		/// the value of the heuristics for each variable (using Dimacs
		/// index).
		/// </returns>
		/// <since>2.3.2</since>
		double[] GetVariableHeuristics();
	}
}
