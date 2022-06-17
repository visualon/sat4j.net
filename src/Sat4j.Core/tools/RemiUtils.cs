using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>Class dedicated to Remi Coletta utility methods :-)</summary>
	/// <author>leberre</author>
	public sealed class RemiUtils
	{
		private RemiUtils()
		{
		}

		// no instanceof that class are expected to be used.
		/// <summary>Compute the set of literals common to all models of the formula.</summary>
		/// <param name="s">a solver already feeded</param>
		/// <returns>
		/// the set of literals common to all models of the formula contained
		/// in the solver, in dimacs format.
		/// </returns>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public static IVecInt Backbone(ISolver s)
		{
			IVecInt backbone = new VecInt();
			int nvars = s.NVars();
			for (int i = 1; i <= nvars; i++)
			{
				backbone.Push(i);
				if (s.IsSatisfiable(backbone))
				{
					backbone.Pop().Push(-i);
					if (s.IsSatisfiable(backbone))
					{
						backbone.Pop();
					}
					else
					{
						// i is in the backbone
						backbone.Pop().Push(i);
					}
				}
				else
				{
					// -i is in the backbone
					backbone.Pop().Push(-i);
				}
			}
			return backbone;
		}
	}
}
