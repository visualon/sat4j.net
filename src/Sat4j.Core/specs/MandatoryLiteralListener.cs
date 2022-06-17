using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>Callback method called when a mandatory literal is found in a constraint.</summary>
	/// <author>leberre</author>
	/// <since>2.3.6</since>
	public interface MandatoryLiteralListener
	{
		/// <param name="p">a literal in internal representation.</param>
		void IsMandatory(int p);
	}
}
