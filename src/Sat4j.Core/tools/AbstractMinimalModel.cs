using System.Collections.Generic;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	[System.Serializable]
	public class AbstractMinimalModel : SolverDecorator<ISolver>
	{
		private const long serialVersionUID = 1L;

		protected internal readonly ICollection<int> pLiterals;

		protected internal readonly SolutionFoundListener modelListener;

		public static IVecInt PositiveLiterals(ISolver solver)
		{
			IVecInt literals = new VecInt(solver.NVars());
			for (int i = 1; i <= solver.NVars(); i++)
			{
				literals.Push(i);
			}
			return literals;
		}

		public static IVecInt NegativeLiterals(ISolver solver)
		{
			IVecInt literals = new VecInt(solver.NVars());
			for (int i = 1; i <= solver.NVars(); i++)
			{
				literals.Push(-i);
			}
			return literals;
		}

		public AbstractMinimalModel(ISolver solver)
			: this(solver, SolutionFoundListenerConstants.Void)
		{
		}

		public AbstractMinimalModel(ISolver solver, IVecInt p)
			: this(solver, p, SolutionFoundListenerConstants.Void)
		{
		}

		public AbstractMinimalModel(ISolver solver, SolutionFoundListener modelListener)
			: this(solver, NegativeLiterals(solver), modelListener)
		{
		}

		public AbstractMinimalModel(ISolver solver, IVecInt p, SolutionFoundListener modelListener)
			: base(solver)
		{
			this.pLiterals = new TreeSet<int>();
			for (IteratorInt it = p.Iterator(); it.HasNext(); )
			{
				this.pLiterals.Add(it.Next());
			}
			this.modelListener = modelListener;
		}
	}
}
