using Org.Sat4j.Specs;
using Org.Sat4j.Tools.Encoding;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>A decorator for clausal cardinalities constraints.</summary>
	/// <author>stephanieroussel</author>
	/// <since>2.3.1</since>
	/// <?/>
	[System.Serializable]
	public class ClausalCardinalitiesDecorator<T> : SolverDecorator<T>
		where T : ISolver
	{
		private const long serialVersionUID = 1L;

		private readonly EncodingStrategyAdapter encodingAdapter;

		public ClausalCardinalitiesDecorator(T solver)
			: base(solver)
		{
			this.encodingAdapter = new Policy();
		}

		public ClausalCardinalitiesDecorator(T solver, EncodingStrategyAdapter encodingAd)
			: base(solver)
		{
			this.encodingAdapter = encodingAd;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override IConstr AddAtLeast(IVecInt literals, int k)
		{
			if (k == 1)
			{
				return this.encodingAdapter.AddAtLeastOne(Decorated(), literals);
			}
			else
			{
				return this.encodingAdapter.AddAtLeast(Decorated(), literals, k);
			}
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override IConstr AddAtMost(IVecInt literals, int k)
		{
			if (k == 1)
			{
				return this.encodingAdapter.AddAtMostOne(Decorated(), literals);
			}
			else
			{
				return this.encodingAdapter.AddAtMost(Decorated(), literals, k);
			}
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override IConstr AddExactly(IVecInt literals, int k)
		{
			if (k == 1)
			{
				return this.encodingAdapter.AddExactlyOne(Decorated(), literals);
			}
			else
			{
				return this.encodingAdapter.AddExactly(Decorated(), literals, k);
			}
		}

		public override string ToString()
		{
			return ToString(string.Empty);
		}

		public override string ToString(string prefix)
		{
			return base.ToString(prefix) + "\n" + "Cardinality to SAT encoding: \n" + "Encoding: " + this.encodingAdapter + "\n";
		}
	}
}
