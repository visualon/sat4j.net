using System;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>
	/// The aim of this class is to compute efficiently the literals implied by the
	/// set of constraints (also called backbone or unit implicates).
	/// </summary>
	/// <remarks>
	/// The aim of this class is to compute efficiently the literals implied by the
	/// set of constraints (also called backbone or unit implicates).
	/// The work has been done in the context of ANR BR4CP.
	/// </remarks>
	/// <author>leberre</author>
	public sealed class Backbone
	{
		internal abstract class Backboner
		{
			protected internal IBackboneProgressListener listener = IBackboneProgressListenerConstants.Void;

			protected internal int nbSatTests;

			private bool implicant = true;

			public virtual void SetBackboneProgressListener(IBackboneProgressListener listener)
			{
				this.listener = listener;
			}

			public virtual void SetImplicantSimplification(bool b)
			{
				this.implicant = b;
			}

			public virtual int[] SimplifiedModel(ISolver solver)
			{
				if (implicant)
				{
					return solver.PrimeImplicant();
				}
				return solver.Model();
			}

			/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
			public virtual IVecInt Compute(ISolver solver, int[] implicant, IVecInt assumptions)
			{
				nbSatTests = 0;
				BitSet assumptionsSet = new BitSet(solver.NVars());
				for (IteratorInt it = assumptions.Iterator(); it.HasNext(); )
				{
					assumptionsSet.Set(Math.Abs(it.Next()));
				}
				IVecInt litsToTest = new VecInt();
				foreach (int p in implicant)
				{
					if (!assumptionsSet.Get(Math.Abs(p)))
					{
						litsToTest.Push(-p);
					}
				}
				return Compute(solver, assumptions, litsToTest);
			}

			public virtual int NbSatTests()
			{
				return this.nbSatTests;
			}

			internal virtual void IncSatTests()
			{
				nbSatTests++;
			}

			/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
			public virtual IVecInt Compute(ISolver solver, int[] implicant, IVecInt assumptions, IVecInt filter)
			{
				nbSatTests = 0;
				BitSet assumptionsSet = new BitSet(solver.NVars());
				for (IteratorInt it = assumptions.Iterator(); it.HasNext(); )
				{
					assumptionsSet.Set(Math.Abs(it.Next()));
				}
				BitSet filterSet = new BitSet();
				for (IteratorInt it_1 = filter.Iterator(); it_1.HasNext(); )
				{
					filterSet.Set(Math.Abs(it_1.Next()));
				}
				IVecInt litsToTest = new VecInt();
				foreach (int p in implicant)
				{
					if (!assumptionsSet.Get(Math.Abs(p)) && filterSet.Get(Math.Abs(p)))
					{
						litsToTest.Push(-p);
					}
				}
				return Compute(solver, assumptions, litsToTest);
			}

			/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
			internal abstract IVecInt Compute(ISolver solver, IVecInt assumptions, IVecInt litsToTest);

			internal static void RemoveVarNotPresentAndSatisfiedLits(int[] implicant, IVecInt litsToTest, int n)
			{
				int[] marks = new int[n + 1];
				foreach (int p in implicant)
				{
					marks[p > 0 ? p : -p] = p;
				}
				int q;
				int mark;
				for (int i = 0; i < litsToTest.Size(); )
				{
					q = litsToTest.Get(i);
					mark = marks[q > 0 ? q : -q];
					if (mark == 0 || mark == q)
					{
						litsToTest.Delete(i);
					}
					else
					{
						i++;
					}
				}
			}
		}

		private sealed class _Backboner_148 : Backbone.Backboner
		{
			public _Backboner_148()
			{
			}

			/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
			internal override IVecInt Compute(ISolver solver, IVecInt assumptions, IVecInt litsToTest)
			{
				int[] implicant;
				IVecInt candidates = new VecInt();
				assumptions.CopyTo(candidates);
				int p;
				int initLitsToTestSize = litsToTest.Size();
				this.listener.Start(initLitsToTestSize);
				while (!litsToTest.IsEmpty())
				{
					this.listener.InProgress(initLitsToTestSize - litsToTest.Size(), initLitsToTestSize);
					p = litsToTest.Last();
					candidates.Push(p);
					litsToTest.Pop();
					if (solver.IsSatisfiable(candidates))
					{
						candidates.Pop();
						implicant = this.SimplifiedModel(solver);
						Backbone.Backboner.RemoveVarNotPresentAndSatisfiedLits(implicant, litsToTest, solver.NVars());
					}
					else
					{
						candidates.Pop().Push(-p);
					}
					this.IncSatTests();
				}
				this.listener.End(this.nbSatTests);
				return candidates;
			}
		}

		/// <summary>
		/// Computes the backbone of a formula following the iterative algorithm
		/// described in Joao Marques-Silva, Mikolas Janota, Ines Lynce: On Computing
		/// Backbones of Propositional Theories.
		/// </summary>
		/// <remarks>
		/// Computes the backbone of a formula following the iterative algorithm
		/// described in Joao Marques-Silva, Mikolas Janota, Ines Lynce: On Computing
		/// Backbones of Propositional Theories. ECAI 2010: 15-20 and using Sat4j
		/// specific prime implicant computation.
		/// </remarks>
		private static readonly Backbone.Backboner Bb = new _Backboner_148();

		private sealed class _Backboner_187 : Backbone.Backboner
		{
			public _Backboner_187()
			{
			}

			/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
			internal override IVecInt Compute(ISolver solver, IVecInt assumptions, IVecInt litsToTest)
			{
				int[] implicant;
				IVecInt candidates = new VecInt();
				assumptions.CopyTo(candidates);
				IConstr constr;
				int initLitsToTestSize = litsToTest.Size();
				this.listener.Start(initLitsToTestSize);
				while (!litsToTest.IsEmpty())
				{
					this.listener.InProgress(initLitsToTestSize - litsToTest.Size(), initLitsToTestSize);
					try
					{
						constr = solver.AddBlockingClause(litsToTest);
						if (solver.IsSatisfiable(candidates))
						{
							implicant = this.SimplifiedModel(solver);
							Backbone.Backboner.RemoveVarNotPresentAndSatisfiedLits(implicant, litsToTest, solver.NVars());
						}
						else
						{
							for (IteratorInt it = litsToTest.Iterator(); it.HasNext(); )
							{
								candidates.Push(-it.Next());
							}
							litsToTest.Clear();
						}
						solver.RemoveConstr(constr);
					}
					catch (ContradictionException)
					{
						for (IteratorInt it = litsToTest.Iterator(); it.HasNext(); )
						{
							candidates.Push(-it.Next());
						}
						litsToTest.Clear();
					}
					this.IncSatTests();
				}
				this.listener.End(this.nbSatTests);
				return candidates;
			}
		}

		/// <summary>
		/// Computes the backbone of a formula using the iterative approach found in
		/// BB but testing a set of literals at once instead of only one.
		/// </summary>
		/// <remarks>
		/// Computes the backbone of a formula using the iterative approach found in
		/// BB but testing a set of literals at once instead of only one. This
		/// approach outperforms BB in terms of SAT calls. Both approaches are made
		/// available for testing purposes.
		/// </remarks>
		private static readonly Backbone.Backboner Ibb = new _Backboner_187();

		private readonly Backbone.Backboner bb;

		private static readonly Backbone instance = new Backbone(Ibb);

		private Backbone(Backbone.Backboner bb)
		{
			this.bb = bb;
		}

		public static Backbone Instance()
		{
			return instance;
		}

		public static Backbone Instance(IBackboneProgressListener listener, bool primeImplicantSimplification)
		{
			instance.bb.SetBackboneProgressListener(listener);
			instance.bb.SetImplicantSimplification(primeImplicantSimplification);
			return instance;
		}


		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public IVecInt Compute(ISolver solver)
		{
			return Compute(solver, VecInt.Empty);
		}

		/// <summary>Computes the backbone of a formula.</summary>
		/// <param name="solver">a solver containing a satisfiable set of constraints.</param>
		/// <param name="assumptions">a set of literals to satisfy</param>
		/// <returns>the backbone of the solver when the assumptions are satisfied</returns>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException">if the computation cannot be done within the timeout</exception>
		/// <exception cref="System.ArgumentException">if solver is unsatisfiable</exception>
		public IVecInt Compute(ISolver solver, IVecInt assumptions)
		{
			bool result = solver.IsSatisfiable(assumptions);
			if (!result)
			{
				throw new ArgumentException("Formula is UNSAT!");
			}
			return bb.Compute(solver, solver.PrimeImplicant(), assumptions);
		}

		/// <summary>Computes the backbone of a formula.</summary>
		/// <param name="solver">a solver containing a satisfiable set of constraints.</param>
		/// <param name="assumptions">a set of literals to satisfy</param>
		/// <param name="filter">a set of variables</param>
		/// <returns>
		/// the backbone of the solver restricted to the variables of filter
		/// when the assumptions are satisfied
		/// </returns>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException">if the computation cannot be done within the timeout</exception>
		/// <exception cref="System.ArgumentException">if solver is unsatisfiable</exception>
		public IVecInt Compute(ISolver solver, IVecInt assumptions, IVecInt filter)
		{
			bool result = solver.IsSatisfiable(assumptions);
			if (!result)
			{
				throw new ArgumentException("Formula is UNSAT!");
			}
			return bb.Compute(solver, solver.PrimeImplicant(), assumptions, filter);
		}

		/// <summary>Computes the backbone of a formula.</summary>
		/// <param name="solver">a solver containing a satisfiable set of constraints.</param>
		/// <param name="implicant">an implicant of solver</param>
		/// <returns>the backbone of the solver</returns>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException">if the computation cannot be done within the timeout</exception>
		public IVecInt Compute(ISolver solver, int[] implicant)
		{
			return bb.Compute(solver, implicant, VecInt.Empty);
		}

		/// <summary>Computes the backbone of a formula.</summary>
		/// <param name="solver">a solver containing a satisfiable set of constraints.</param>
		/// <param name="implicant">an implicant of solver</param>
		/// <param name="assumptions">a set of literals to satisfy</param>
		/// <returns>the backbone of the solver when the assumptions are satisfied</returns>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException">if the computation cannot be done within the timeout</exception>
		public IVecInt Compute(ISolver solver, int[] implicant, IVecInt assumptions)
		{
			return bb.Compute(solver, implicant, assumptions);
		}

		/// <summary>
		/// Returns the number of calls to the SAT solver needed to compute the
		/// backbone.
		/// </summary>
		/// <returns>the number of underlying calls to the SAT solver.</returns>
		public int GetNumberOfSatCalls()
		{
			return bb.NbSatTests();
		}
	}
}
