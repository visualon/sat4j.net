using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <author>leberre</author>
	internal interface ConstrActivityListener
	{
		/// <param name="confl">the conflictual constraint</param>
		void ClaBumpActivity(Constr confl);
	}
}
