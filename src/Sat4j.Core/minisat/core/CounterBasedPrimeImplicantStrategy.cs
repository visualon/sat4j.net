using System;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>
	/// Counter based implementation of the model minimization strategy to compute a
	/// prime implicant.
	/// </summary>
	/// <remarks>
	/// Counter based implementation of the model minimization strategy to compute a
	/// prime implicant. The main advantage of this method is to be linear in the
	/// size (in number of literals) of the formula. It's main drawback is to be
	/// limited to clauses (and cardinality constraints with some modifications). See
	/// our FMCAD 2013 paper for details.
	/// </remarks>
	/// <author>leberre</author>
	public class CounterBasedPrimeImplicantStrategy : PrimeImplicantStrategy
	{
		private int[] prime;

		public virtual int[] Compute<_T0>(Solver<_T0> solver)
			where _T0 : DataStructureFactory
		{
			long begin = Runtime.CurrentTimeMillis();
			IVecInt[] watched = new IVecInt[solver.voc.NVars() * 2 + 2];
			foreach (int d in solver.fullmodel)
			{
				watched[LiteralsUtils.ToInternal(d)] = new VecInt();
			}
			int[] count = new int[solver.constrs.Size()];
			Constr constr;
			IVecInt watch;
			for (int i = 0; i < solver.constrs.Size(); i++)
			{
				constr = solver.constrs.Get(i);
				if (!constr.CanBeSatisfiedByCountingLiterals())
				{
					throw new InvalidOperationException("Algo2 does not work with constraints other than clauses and cardinalities" + constr.GetType());
				}
				count[i] = 0;
				for (int j = 0; j < constr.Size(); j++)
				{
					watch = watched[constr.Get(j)];
					if (watch != null)
					{
						// satisfied literal
						watch.Push(i);
					}
				}
			}
			foreach (int d_1 in solver.fullmodel)
			{
				for (IteratorInt it = watched[LiteralsUtils.ToInternal(d_1)].Iterator(); it.HasNext(); )
				{
					count[it.Next()]++;
				}
			}
			this.prime = new int[solver.voc.NVars() + 1];
			int d_2;
			for (int i_1 = 0; i_1 < this.prime.Length; i_1++)
			{
				this.prime[i_1] = 0;
			}
			for (IteratorInt it_1 = solver.implied.Iterator(); it_1.HasNext(); )
			{
				d_2 = it_1.Next();
				this.prime[Math.Abs(d_2)] = d_2;
			}
			int removed = 0;
			int posremoved = 0;
			int propagated = 0;
			int constrNumber;
			for (int i_2 = 0; i_2 < solver.decisions.Size(); i_2++)
			{
				d_2 = solver.decisions.Get(i_2);
				for (IteratorInt it = watched[LiteralsUtils.ToInternal(d_2)].Iterator(); it.HasNext(); )
				{
					constrNumber = it.Next();
					if (count[constrNumber] == solver.constrs.Get(constrNumber).RequiredNumberOfSatisfiedLiterals())
					{
						this.prime[Math.Abs(d_2)] = d_2;
						propagated++;
						goto top_continue;
					}
				}
				removed++;
				if (d_2 > 0 && d_2 > solver.NVars())
				{
					posremoved++;
				}
				for (IteratorInt it_2 = watched[LiteralsUtils.ToInternal(d_2)].Iterator(); it_2.HasNext(); )
				{
					count[it_2.Next()]--;
				}
top_continue: ;
			}
top_break: ;
			int[] implicant = new int[this.prime.Length - removed - 1];
			int index = 0;
			foreach (int i_3 in this.prime)
			{
				if (i_3 != 0)
				{
					implicant[index++] = i_3;
				}
			}
			long end = Runtime.CurrentTimeMillis();
			if (solver.IsVerbose())
			{
				System.Console.Out.Printf("%s prime implicant computation statistics ALGO2%n", solver.GetLogPrefix());
				System.Console.Out.Printf("%s implied: %d, decision: %d, removed %d (+%d), propagated %d, time(ms):%d %n", solver.GetLogPrefix(), solver.implied.Size(), solver.decisions.Size(), removed, posremoved, propagated, end - begin);
			}
			return implicant;
		}

		public virtual int[] GetPrimeImplicantAsArrayWithHoles()
		{
			if (prime == null)
			{
				throw new NotSupportedException("Call the compute method first!");
			}
			return prime;
		}
	}
}
