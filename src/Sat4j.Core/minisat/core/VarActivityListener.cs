using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>
	/// Interface providing the capability to increase the activity of a given
	/// variable.
	/// </summary>
	/// <author>leberre</author>
	public interface VarActivityListener
	{
		/// <summary>Update the activity of a variable v.</summary>
		/// <param name="p">
		/// a literal (<code>v&lt;&lt;1</code> or
		/// <code>v&lt;&lt;1^1</code>)
		/// </param>
		void VarBumpActivity(int p);
	}
}
