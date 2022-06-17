using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Restarts
{
	/// <summary>Luby series</summary>
	[System.Serializable]
	public sealed class LubyRestarts : RestartStrategy
	{
		public const int DefaultLubyFactor = 32;

		private const long serialVersionUID = 1L;

		private int un = 1;

		private int vn = 1;

		// 21-06-2012 back from SAT 2012
		// computing luby values the way presented by Donald Knuth in his invited
		// talk at the SAT 2012 conference
		// u1
		// v1
		/// <summary>returns the current value of the luby sequence.</summary>
		/// <returns>the current value of the luby sequence.</returns>
		public int Luby()
		{
			return this.vn;
		}

		/// <summary>Computes and return the next value of the luby sequence.</summary>
		/// <remarks>
		/// Computes and return the next value of the luby sequence. That method has
		/// a side effect of the value returned by luby(). luby()!=nextLuby() but
		/// nextLuby()==luby().
		/// </remarks>
		/// <returns>the new current value of the luby sequence.</returns>
		/// <seealso cref="Luby()"/>
		public int NextLuby()
		{
			if ((this.un & -this.un) == this.vn)
			{
				this.un = this.un + 1;
				this.vn = 1;
			}
			else
			{
				this.vn = this.vn << 1;
			}
			return this.vn;
		}

		private int factor;

		private int bound;

		private int conflictcount;

		public LubyRestarts()
			: this(DefaultLubyFactor)
		{
		}

		/// <param name="factor">the factor used for the Luby series.</param>
		/// <since>2.1</since>
		public LubyRestarts(int factor)
		{
			// uses TiniSAT default
			SetFactor(factor);
		}

		public void SetFactor(int factor)
		{
			this.factor = factor;
		}

		public int GetFactor()
		{
			return this.factor;
		}

		public void Init(SearchParams @params, SolverStats stats)
		{
			this.un = 1;
			this.vn = 1;
			this.bound = Luby() * this.factor;
		}

		public long NextRestartNumberOfConflict()
		{
			return this.bound;
		}

		public void OnRestart()
		{
			this.bound = NextLuby() * this.factor;
			this.conflictcount = 0;
		}

		public override string ToString()
		{
			return "luby style (SATZ_rand, TiniSAT) restarts strategy with factor " + this.factor;
		}

		public bool ShouldRestart()
		{
			return this.conflictcount >= this.bound;
		}

		public void OnBackjumpToRootLevel()
		{
			this.conflictcount = 0;
		}

		public void Reset()
		{
			this.conflictcount = 0;
		}

		public void NewConflict()
		{
			this.conflictcount++;
		}

		public void NewLearnedClause(Constr learned, int trailLevel)
		{
		}
	}
}
