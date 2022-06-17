using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Conflict based timer.</summary>
	/// <remarks>
	/// Conflict based timer.
	/// Used to perform a task when a conflict occurs.
	/// </remarks>
	/// <author>daniel</author>
	public interface ConflictTimer
	{
		void Reset();

		void NewConflict();
	}
}
