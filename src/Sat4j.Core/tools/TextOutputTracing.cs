using System;
using System.Collections.Generic;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>Debugging Search Listener allowing to follow the search in a textual way.</summary>
	/// <author>daniel</author>
	/// <since>2.2</since>
	[System.Serializable]
	public class TextOutputTracing<T> : SearchListener<ISolverService>
	{
		private const long serialVersionUID = 1L;

		private readonly IDictionary<int, T> mapping;

		/// <since>2.1</since>
		public TextOutputTracing(IDictionary<int, T> mapping)
		{
			this.mapping = mapping;
		}

		private string Node(int dimacs)
		{
			if (this.mapping != null)
			{
				int var = Math.Abs(dimacs);
				T t = this.mapping[var];
				if (t != null)
				{
					if (dimacs > 0)
					{
						return t.ToString();
					}
					return "-" + t.ToString();
				}
			}
			return Sharpen.Extensions.ToString(dimacs);
		}

		public virtual void Assuming(int p)
		{
			System.Console.Out.WriteLine("assuming " + Node(p));
		}

		/// <since>2.1</since>
		public virtual void Propagating(int p)
		{
			System.Console.Out.WriteLine("propagating " + Node(p));
		}

		public virtual void Enqueueing(int p, IConstr reason)
		{
			System.Console.Out.WriteLine("enqueueing " + Node(p));
		}

		public virtual void Backtracking(int p)
		{
			System.Console.Out.WriteLine("backtracking " + Node(p));
		}

		public virtual void Adding(int p)
		{
			System.Console.Out.WriteLine("adding " + Node(p));
		}

		/// <since>2.1</since>
		public virtual void Learn(IConstr clause)
		{
			System.Console.Out.WriteLine("learning " + clause);
		}

		/// <since>2.3.4</since>
		public virtual void LearnUnit(int p)
		{
			System.Console.Out.WriteLine("learning unit " + p);
		}

		public virtual void Delete(IConstr c)
		{
		}

		/// <since>2.1</since>
		public virtual void ConflictFound(IConstr confl, int dlevel, int trailLevel)
		{
			System.Console.Out.WriteLine("conflict ");
		}

		/// <since>2.1</since>
		public virtual void ConflictFound(int p)
		{
			System.Console.Out.WriteLine("conflict during propagation");
		}

		public virtual void SolutionFound(int[] model, RandomAccessModel lazyModel)
		{
			System.Console.Out.WriteLine("solution found ");
		}

		public virtual void BeginLoop()
		{
		}

		public virtual void Start()
		{
		}

		/// <since>2.1</since>
		public virtual void End(Lbool result)
		{
		}

		/// <since>2.2</since>
		public virtual void Restarting()
		{
			System.Console.Out.WriteLine("restarting ");
		}

		public virtual void Backjump(int backjumpLevel)
		{
			System.Console.Out.WriteLine("backjumping to decision level " + backjumpLevel);
		}

		/// <since>2.3.2</since>
		public virtual void Init(ISolverService solverService)
		{
		}

		/// <since>2.3.2</since>
		public virtual void Cleaning()
		{
			System.Console.Out.WriteLine("cleaning");
		}
	}
}
