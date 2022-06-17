using System;
using System.Collections.Generic;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>
	/// Watcher based implementation of the model minimization strategy to compute a
	/// prime implicant.
	/// </summary>
	/// <remarks>
	/// Watcher based implementation of the model minimization strategy to compute a
	/// prime implicant. The main advantage of this method is to be linear in the
	/// size (in number of literals) of the formula as long as a specific property
	/// holds for the constraints. That approach also takes advantage of the lazy
	/// data structures found in modern SAT solvers See our FMCAD 2013 paper for
	/// details.
	/// </remarks>
	/// <author>leberre</author>
	public class WatcherBasedPrimeImplicantStrategy : PrimeImplicantStrategy, MandatoryLiteralListener
	{
		private int[] prime;

		private readonly IComparer<int> comparator;

		public WatcherBasedPrimeImplicantStrategy(IComparer<int> comparator)
		{
			this.comparator = comparator;
		}

		public WatcherBasedPrimeImplicantStrategy()
			: this(null)
		{
		}

		public virtual void IsMandatory(int p)
		{
			prime[LiteralsUtils.Var(p)] = LiteralsUtils.ToDimacs(p);
		}

		public virtual int[] Compute<_T0>(Solver<_T0> solver)
			where _T0 : DataStructureFactory
		{
			System.Diagnostics.Debug.Assert(solver.qhead == solver.trail.Size() + solver.learnedLiterals.Size());
			long begin = Runtime.CurrentTimeMillis();
			if (solver.learnedLiterals.Size() > 0)
			{
				solver.qhead = solver.trail.Size();
			}
			this.prime = new int[solver.voc.NVars() + 1];
			int p;
			for (int i = 0; i < this.prime.Length; i++)
			{
				this.prime[i] = 0;
			}
			// unit clauses need to be handled specifically
			for (int i_1 = 0; i_1 < solver.trail.Size(); i_1++)
			{
				IsMandatory(solver.trail.Get(i_1));
			}
			foreach (int d in solver.fullmodel)
			{
				p = LiteralsUtils.ToInternal(d);
				if (solver.voc.IsUnassigned(p))
				{
					solver.Assume(p);
				}
			}
			foreach (int d_1 in solver.fullmodel)
			{
				ReduceClausesContainingTheNegationOfPI(solver, LiteralsUtils.ToInternal(d_1));
			}
			int removed = 0;
			int posremoved = 0;
			int propagated = 0;
			foreach (int d_2 in FullModel(solver))
			{
				if (this.prime[Math.Abs(d_2)] != 0)
				{
					// d has been propagated
					propagated++;
				}
				else
				{
					// it is not a mandatory literal
					solver.Forget(Math.Abs(d_2));
					ReduceClausesContainingTheNegationOfPI(solver, LiteralsUtils.ToInternal(-d_2));
					removed++;
					if (d_2 > 0 && d_2 > solver.NVars())
					{
						posremoved++;
					}
				}
			}
			solver.CancelUntil(0);
			int[] implicant = new int[propagated];
			int index = 0;
			foreach (int i_2 in this.prime)
			{
				if (i_2 != 0)
				{
					implicant[index++] = i_2;
				}
			}
			long end = Runtime.CurrentTimeMillis();
			if (solver.IsVerbose())
			{
				System.Console.Out.Printf("%s prime implicant computation statistics BRESIL (reverse = %b)%n", solver.GetLogPrefix(), this.comparator != null);
				System.Console.Out.Printf("%s implied: %d, decision: %d, removed %d (+%d), propagated %d, time(ms):%d %n", solver.GetLogPrefix(), solver.implied.Size(), solver.decisions.Size(), removed, posremoved, propagated, end - begin);
			}
			return implicant;
		}

		internal virtual Constr ReduceClausesContainingTheNegationOfPI<_T0>(Solver<_T0> solver, int p)
			where _T0 : DataStructureFactory
		{
			System.Diagnostics.Debug.Assert(p > 1);
			IVec<Propagatable> lwatched = solver.watched;
			lwatched.Clear();
			solver.voc.Watches(p).MoveTo(lwatched);
			int size = lwatched.Size();
			for (int i = 0; i < size; i++)
			{
				solver.stats.IncInspects();
				lwatched.Get(i).PropagatePI(this, p);
			}
			return null;
		}

		public virtual int[] GetPrimeImplicantAsArrayWithHoles()
		{
			if (prime == null)
			{
				throw new NotSupportedException("Call the compute method first!");
			}
			return prime;
		}

		private int[] FullModel<_T0>(Solver<_T0> solver)
			where _T0 : DataStructureFactory
		{
			if (this.comparator == null)
			{
				return solver.fullmodel;
			}
			int n = solver.fullmodel.Length;
			IVecInt reversed = new VecInt(n);
			foreach (int i in solver.fullmodel)
			{
				reversed.Push(i);
			}
			reversed.Sort(comparator);
			return reversed.ToArray();
		}
	}
}
