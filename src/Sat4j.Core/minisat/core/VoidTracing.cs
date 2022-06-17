using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Do-nothing search listener.</summary>
	/// <remarks>
	/// Do-nothing search listener. Used by default by the solver when no
	/// SearchListener is provided to the solver.
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	internal sealed class VoidTracing : SearchListenerAdapter<ISolverService>
	{
		private const long serialVersionUID = 1L;
	}
}
