using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Interface providing the undoable service.</summary>
	/// <author>leberre</author>
	public interface Undoable
	{
		/// <summary>Method called when backtracking</summary>
		/// <param name="p">a literal to be unassigned.</param>
		void Undo(int p);
	}
}
