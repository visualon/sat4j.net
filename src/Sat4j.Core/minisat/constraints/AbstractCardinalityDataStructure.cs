using Org.Sat4j.Minisat.Constraints.Cnf;
using Org.Sat4j.Minisat.Core;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints
{
	/// <author>
	/// leberre To change the template for this generated type comment go to
	/// Window - Preferences - Java - Code Generation - Code and Comments
	/// </author>
	[System.Serializable]
	public abstract class AbstractCardinalityDataStructure : AbstractDataStructureFactory
	{
		private const long serialVersionUID = 1L;

		protected internal override ILits CreateLits()
		{
			return new Lits();
		}
	}
}
