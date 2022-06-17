using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>Iterator interface to avoid boxing int into Integer.</summary>
	/// <author>daniel</author>
	public interface IteratorInt
	{
		bool HasNext();

		int Next();
	}
}
