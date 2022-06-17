using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>This class allow to use the ModelIterator class as a solver.</summary>
	/// <author>lonca</author>
	[System.Serializable]
	public class ModelIteratorToSATAdapter : ModelIterator
	{
		private const long serialVersionUID = 1L;

		private int[] lastModel = null;

		private readonly SolutionFoundListener sfl;

		public ModelIteratorToSATAdapter(ISolver solver, SolutionFoundListener sfl)
			: this(solver, long.MaxValue, sfl)
		{
		}

		public ModelIteratorToSATAdapter(ISolver solver, long bound, SolutionFoundListener sfl)
			: base(solver, bound)
		{
			this.sfl = sfl;
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override bool IsSatisfiable()
		{
			bool isSat = false;
			while (base.IsSatisfiable())
			{
				isSat = true;
				lastModel = base.Model();
				this.sfl.OnSolutionFound(lastModel);
			}
			ExpireTimeout();
			return isSat;
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override bool IsSatisfiable(IVecInt assumps)
		{
			bool isSat = false;
			while (base.IsSatisfiable(assumps))
			{
				isSat = true;
				lastModel = base.Model();
				this.sfl.OnSolutionFound(lastModel);
			}
			ExpireTimeout();
			return isSat;
		}

		public override int[] Model()
		{
			return this.lastModel;
		}
	}
}
