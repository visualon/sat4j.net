using System;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>
	/// Quadratic implementation of the model minimization strategy to compute a
	/// prime implicant.
	/// </summary>
	/// <remarks>
	/// Quadratic implementation of the model minimization strategy to compute a
	/// prime implicant. The main interest of that approach is to work for any kind
	/// of constraints (clauses, cardinality constraints, pseudo boolean constraints,
	/// any custom constraint).
	/// </remarks>
	/// <author>leberre</author>
	public class QuadraticPrimeImplicantStrategy : PrimeImplicantStrategy
	{
		private int[] prime;

		/// <summary>Assume literal p and perform unit propagation</summary>
		/// <param name="p">a literal</param>
		/// <returns>true if no conflict is reached, false if a conflict is found.</returns>
		internal virtual bool SetAndPropagate<_T0>(Solver<_T0> solver, int p)
			where _T0 : DataStructureFactory
		{
			if (solver.voc.IsUnassigned(p))
			{
				System.Diagnostics.Debug.Assert(!solver.trail.Contains(p));
				System.Diagnostics.Debug.Assert(!solver.trail.Contains(LiteralsUtils.Neg(p)));
				return solver.Assume(p) && solver.Propagate() == null;
			}
			return solver.voc.IsSatisfied(p);
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
			if (solver.IsVerbose())
			{
				System.Console.Out.Printf("%s implied: %d, decision: %d %n", solver.GetLogPrefix(), solver.implied.Size(), solver.decisions.Size());
			}
			prime = new int[solver.RealNumberOfVariables() + 1];
			int p;
			int d;
			for (int i = 0; i < prime.Length; i++)
			{
				prime[i] = 0;
			}
			bool noproblem;
			for (IteratorInt it = solver.implied.Iterator(); it.HasNext(); )
			{
				d = it.Next();
				p = LiteralsUtils.ToInternal(d);
				prime[Math.Abs(d)] = d;
				noproblem = SetAndPropagate(solver, p);
				System.Diagnostics.Debug.Assert(noproblem);
			}
			bool canBeRemoved;
			int rightlevel;
			int removed = 0;
			int posremoved = 0;
			int propagated = 0;
			int tested = 0;
			int l2propagation = 0;
			for (int i_1 = 0; i_1 < solver.decisions.Size(); i_1++)
			{
				d = solver.decisions.Get(i_1);
				System.Diagnostics.Debug.Assert(!solver.voc.IsFalsified(LiteralsUtils.ToInternal(d)));
				if (solver.voc.IsSatisfied(LiteralsUtils.ToInternal(d)))
				{
					// d has been propagated
					prime[Math.Abs(d)] = d;
					propagated++;
				}
				else
				{
					if (SetAndPropagate(solver, LiteralsUtils.ToInternal(-d)))
					{
						canBeRemoved = true;
						tested++;
						rightlevel = solver.CurrentDecisionLevel();
						for (int j = i_1 + 1; j < solver.decisions.Size(); j++)
						{
							l2propagation++;
							if (!SetAndPropagate(solver, LiteralsUtils.ToInternal(solver.decisions.Get(j))))
							{
								canBeRemoved = false;
								break;
							}
						}
						solver.CancelUntil(rightlevel);
						if (canBeRemoved)
						{
							// it is not a necessary literal
							solver.Forget(Math.Abs(d));
							IConstr confl = solver.Propagate();
							System.Diagnostics.Debug.Assert(confl == null);
							removed++;
							if (d > 0 && d > solver.NVars())
							{
								posremoved++;
							}
						}
						else
						{
							prime[Math.Abs(d)] = d;
							solver.Cancel();
							System.Diagnostics.Debug.Assert(solver.voc.IsUnassigned(LiteralsUtils.ToInternal(d)));
							noproblem = SetAndPropagate(solver, LiteralsUtils.ToInternal(d));
							System.Diagnostics.Debug.Assert(noproblem);
						}
					}
					else
					{
						// conflict, literal is necessary
						prime[Math.Abs(d)] = d;
						solver.Cancel();
						noproblem = SetAndPropagate(solver, LiteralsUtils.ToInternal(d));
						System.Diagnostics.Debug.Assert(noproblem);
					}
				}
			}
			solver.CancelUntil(0);
			int[] implicant = new int[prime.Length - removed - 1];
			int index = 0;
			foreach (int i_2 in prime)
			{
				if (i_2 != 0)
				{
					implicant[index++] = i_2;
				}
			}
			long end = Runtime.CurrentTimeMillis();
			if (solver.IsVerbose())
			{
				System.Console.Out.Printf("%s prime implicant computation statistics ORIG%n", solver.GetLogPrefix());
				System.Console.Out.Printf("%s implied: %d, decision: %d, removed %d (+%d), tested %d, propagated %d), l2 propagation:%d, time(ms):%d %n", solver.GetLogPrefix(), solver.implied.Size(), solver.decisions.Size(), removed, posremoved, tested, propagated
					, l2propagation, end - begin);
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
