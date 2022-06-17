using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Provide the learning service.</summary>
	/// <author>leberre</author>
	public interface Learner
	{
		void Learn(Constr c);
	}
}
