using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>Simple interface to check the outcome of running a solver in parallel.</summary>
	/// <author>leberre</author>
	/// <since>2.3.2 (public API level, was not public before)</since>
	/// <seealso cref="ManyCore{S}"/>
	public interface OutcomeListener
	{
		void OnFinishWithAnswer(bool finished, bool result, int index);
	}
}
