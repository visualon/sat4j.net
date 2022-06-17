using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>A class allowing to run several solvers in parallel.</summary>
	/// <remarks>
	/// A class allowing to run several solvers in parallel.
	/// Note that each solver will have its own copy of the CNF, so it is not a
	/// memory efficient implementation. There is no sharing of information yet
	/// between the solvers.
	/// </remarks>
	/// <author>leberre</author>
	/// <?/>
	[System.Serializable]
	public class ManyCore<S> : SearchListenerAdapter<ISolverService>, ISolver, OutcomeListener, UnitClauseProvider
		where S : ISolver
	{
		private const int NormalSleep = 500;

		private const int FastSleep = 50;

		private const long serialVersionUID = 1L;

		private readonly string[] availableSolvers;

		protected internal readonly IList<S> solvers;

		protected internal readonly int numberOfSolvers;

		protected internal int winnerId;

		private bool resultFound;

		private AtomicInteger remainingSolvers;

		private volatile int sleepTime;

		private volatile bool solved;

		private readonly IVecInt sharedUnitClauses = new VecInt();

		private readonly IVec<Counter> solversStats = new Vec<Counter>();

		public ManyCore(ASolverFactory<S> factory, params string[] solverNames)
			: this(factory, false, solverNames)
		{
		}

		public ManyCore(ASolverFactory<S> factory, bool shareLearnedUnitClauses, params string[] solverNames)
		{
			// = { };
			this.availableSolvers = solverNames;
			this.numberOfSolvers = solverNames.Length;
			this.solvers = new AList<S>(this.numberOfSolvers);
			S solver;
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				solver = factory.CreateSolverByName(this.availableSolvers[i]);
				solver.SetSearchListener(this);
				if (shareLearnedUnitClauses)
				{
					solver.SetUnitClauseProvider(this);
				}
				this.solvers.Add(solver);
				this.solversStats.Push(new Counter(0));
			}
		}

		/// <summary>Create a parallel solver from a list of solvers and a list of names.</summary>
		/// <param name="names">a String to describe each solver in the messages.</param>
		/// <param name="solverObjects">the solvers</param>
		public ManyCore(string[] names, params S[] solverObjects)
			: this(false, names, solverObjects)
		{
		}

		public ManyCore(bool shareLearnedUnitClauses, string[] names, params S[] solverObjects)
			: this(shareLearnedUnitClauses, solverObjects)
		{
			for (int i = 0; i < names.Length; i++)
			{
				this.availableSolvers[i] = names[i];
			}
		}

		public ManyCore(params S[] solverObjects)
			: this(false, solverObjects)
		{
		}

		public ManyCore(bool shareLearnedUnitClauses, params S[] solverObjects)
		{
			this.availableSolvers = new string[solverObjects.Length];
			for (int i = 0; i < solverObjects.Length; i++)
			{
				this.availableSolvers[i] = "solver" + i;
			}
			this.numberOfSolvers = solverObjects.Length;
			this.solvers = new AList<S>(this.numberOfSolvers);
			for (int i_1 = 0; i_1 < this.numberOfSolvers; i_1++)
			{
				this.solvers.Add(solverObjects[i_1]);
				solverObjects[i_1].SetSearchListener(this);
				if (shareLearnedUnitClauses)
				{
					solverObjects[i_1].SetUnitClauseProvider(this);
				}
				this.solversStats.Push(new Counter(0));
			}
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual void AddAllClauses(IVec<IVecInt> clauses)
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				this.solvers[i].AddAllClauses(clauses);
			}
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddAtLeast(IVecInt literals, int degree)
		{
			ConstrGroup group = new ConstrGroup(false);
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				group.Add(this.solvers[i].AddAtLeast(literals, degree));
			}
			return group;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddAtMost(IVecInt literals, int degree)
		{
			ConstrGroup group = new ConstrGroup(false);
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				group.Add(this.solvers[i].AddAtMost(literals, degree));
			}
			return group;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddExactly(IVecInt literals, int n)
		{
			ConstrGroup group = new ConstrGroup(false);
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				group.Add(this.solvers[i].AddExactly(literals, n));
			}
			return group;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddClause(IVecInt literals)
		{
			ConstrGroup group = new ConstrGroup(false);
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				group.Add(this.solvers[i].AddClause(literals));
			}
			return group;
		}

		public virtual void ClearLearntClauses()
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				this.solvers[i].ClearLearntClauses();
			}
			sharedUnitClauses.Clear();
		}

		public virtual void ExpireTimeout()
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				this.solvers[i].ExpireTimeout();
			}
			this.sleepTime = FastSleep;
		}

		public virtual IDictionary<string, Number> GetStat()
		{
			return this.solvers[this.winnerId].GetStat();
		}

		public virtual int GetTimeout()
		{
			return this.solvers[0].GetTimeout();
		}

		public virtual long GetTimeoutMs()
		{
			return this.solvers[0].GetTimeoutMs();
		}

		public virtual int NewVar()
		{
			throw new NotSupportedException();
		}

		public virtual int NewVar(int howmany)
		{
			int result = 0;
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				result = this.solvers[i].NewVar(howmany);
			}
			return result;
		}

		[Obsolete]
		public virtual void PrintStat(TextWriter @out, string prefix)
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				@out.Printf("%s>>>>>>>>>> Solver number %d (%d answers) <<<<<<<<<<<<<<<<<<%n", prefix, i, this.solversStats.Get(i).GetValue());
				this.solvers[i].PrintStat(@out, prefix);
			}
		}

		[Obsolete]
		public virtual void PrintStat(PrintWriter @out, string prefix)
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				@out.Printf("%s>>>>>>>>>> Solver number %d (%d answers) <<<<<<<<<<<<<<<<<<%n", prefix, i, this.solversStats.Get(i).GetValue());
				this.solvers[i].PrintStat(@out, prefix);
			}
		}

		public virtual bool RemoveConstr(IConstr c)
		{
			if (c is ConstrGroup)
			{
				ConstrGroup group = (ConstrGroup)c;
				bool removed = true;
				IConstr toRemove;
				for (int i = 0; i < this.numberOfSolvers; i++)
				{
					toRemove = group.GetConstr(i);
					if (toRemove != null)
					{
						removed = removed & this.solvers[i].RemoveConstr(toRemove);
					}
				}
				sharedUnitClauses.Clear();
				return removed;
			}
			throw new ArgumentException("Can only remove a group of constraints!");
		}

		public virtual void Reset()
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				this.solvers[i].Reset();
			}
			sharedUnitClauses.Clear();
		}

		public virtual void SetExpectedNumberOfClauses(int nb)
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				this.solvers[i].SetExpectedNumberOfClauses(nb);
			}
		}

		public virtual void SetTimeout(int t)
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				this.solvers[i].SetTimeout(t);
			}
		}

		public virtual void SetTimeoutMs(long t)
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				this.solvers[i].SetTimeoutMs(t);
			}
		}

		public virtual void SetTimeoutOnConflicts(int count)
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				this.solvers[i].SetTimeoutOnConflicts(count);
			}
		}

		public virtual string ToString(string prefix)
		{
			StringBuilder res = new StringBuilder();
			res.Append(prefix);
			res.Append("ManyCore solver with ");
			res.Append(this.numberOfSolvers);
			res.Append(" solvers running in parallel");
			res.Append("\n");
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				res.Append(prefix);
				res.Append(">>>>>>>>>> Solver number ");
				res.Append(i);
				res.Append(" <<<<<<<<<<<<<<<<<<\n");
				res.Append(this.solvers[i].ToString(prefix));
				if (i < this.numberOfSolvers - 1)
				{
					res.Append("\n");
				}
			}
			return res.ToString();
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual int[] FindModel()
		{
			if (IsSatisfiable())
			{
				return Model();
			}
			// A zero length array would mean that the formula is a tautology.
			return null;
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual int[] FindModel(IVecInt assumps)
		{
			if (IsSatisfiable(assumps))
			{
				return Model();
			}
			// A zero length array would mean that the formula is a tautology.
			return null;
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable()
		{
			return IsSatisfiable(VecInt.Empty, false);
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(IVecInt assumps, bool globalTimeout)
		{
			lock (this)
			{
				this.remainingSolvers = new AtomicInteger(this.numberOfSolvers);
				this.solved = false;
				for (int i = 0; i < this.numberOfSolvers; i++)
				{
					new Sharpen.Thread(new RunnableSolver(i, this.solvers[i], assumps, globalTimeout, this)).Start();
				}
				try
				{
					this.sleepTime = NormalSleep;
					do
					{
						Sharpen.Runtime.Wait(this, this.sleepTime);
					}
					while (this.remainingSolvers.Get() > 0);
				}
				catch (Exception)
				{
					// will happen when one solver found a solution
					Sharpen.Thread.CurrentThread().Interrupt();
				}
				if (!this.solved)
				{
					System.Diagnostics.Debug.Assert(this.remainingSolvers.Get() == 0);
					throw new Specs.TimeoutException();
				}
				return this.resultFound;
			}
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(bool globalTimeout)
		{
			throw new NotSupportedException();
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(IVecInt assumps)
		{
			throw new NotSupportedException();
		}

		public virtual int[] Model()
		{
			return this.solvers[this.winnerId].Model();
		}

		public virtual bool Model(int var)
		{
			return this.solvers[this.winnerId].Model(var);
		}

		public virtual int NConstraints()
		{
			return this.solvers[0].NConstraints();
		}

		public virtual int NVars()
		{
			return this.solvers[0].NVars();
		}

		public virtual void PrintInfos(PrintWriter @out, string prefix)
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				@out.Printf("%s>>>>>>>>>> Solver number %d <<<<<<<<<<<<<<<<<<%n", prefix, i);
				this.solvers[i].PrintInfos(@out, prefix);
			}
		}

		public virtual void OnFinishWithAnswer(bool finished, bool result, int index)
		{
			lock (this)
			{
				if (finished && !this.solved)
				{
					this.winnerId = index;
					this.solversStats.Get(index).Inc();
					this.solved = true;
					this.resultFound = result;
					for (int i = 0; i < this.numberOfSolvers; i++)
					{
						if (i != this.winnerId)
						{
							this.solvers[i].ExpireTimeout();
						}
					}
					this.sleepTime = FastSleep;
					if (IsVerbose())
					{
						System.Console.Out.WriteLine(GetLogPrefix() + "And the winner is " + this.availableSolvers[this.winnerId]);
					}
				}
				this.remainingSolvers.GetAndDecrement();
			}
		}

		public virtual bool IsDBSimplificationAllowed()
		{
			return this.solvers[0].IsDBSimplificationAllowed();
		}

		public virtual void SetDBSimplificationAllowed(bool status)
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				this.solvers[i].SetDBSimplificationAllowed(status);
			}
		}

		public virtual void SetSearchListener<I>(SearchListener<I> sl)
			where I : ISolverService
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				this.solvers[i].SetSearchListener(sl);
			}
		}

		/// <since>2.2</since>
		public virtual SearchListener<I> GetSearchListener<I>()
			where I : ISolverService
		{
			return this.solvers[0].GetSearchListener<I>();
		}

		public virtual int NextFreeVarId(bool reserve)
		{
			int res = -1;
			for (int i = 0; i < this.numberOfSolvers; ++i)
			{
				res = this.solvers[i].NextFreeVarId(reserve);
			}
			return res;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddBlockingClause(IVecInt literals)
		{
			ConstrGroup group = new ConstrGroup(false);
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				group.Add(this.solvers[i].AddBlockingClause(literals));
			}
			return group;
		}

		public virtual IVecInt CreateBlockingClauseForCurrentModel()
		{
			return this.solvers[this.winnerId].CreateBlockingClauseForCurrentModel();
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr DiscardCurrentModel()
		{
			ConstrGroup group = new ConstrGroup(false);
			IVecInt blockingClause = CreateBlockingClauseForCurrentModel();
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				group.Add(this.solvers[i].AddBlockingClause(blockingClause));
			}
			return group;
		}

		public virtual bool RemoveSubsumedConstr(IConstr c)
		{
			if (c is ConstrGroup)
			{
				ConstrGroup group = (ConstrGroup)c;
				bool removed = true;
				IConstr toRemove;
				for (int i = 0; i < this.numberOfSolvers; i++)
				{
					toRemove = group.GetConstr(i);
					if (toRemove != null)
					{
						removed = removed & this.solvers[i].RemoveSubsumedConstr(toRemove);
					}
				}
				return removed;
			}
			throw new ArgumentException("Can only remove a group of constraints!");
		}

		public virtual bool IsVerbose()
		{
			return this.solvers[0].IsVerbose();
		}

		public virtual void SetVerbose(bool value)
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				this.solvers[i].SetVerbose(value);
			}
		}

		public virtual void SetLogPrefix(string prefix)
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				this.solvers[i].SetLogPrefix(prefix);
			}
		}

		public virtual string GetLogPrefix()
		{
			return this.solvers[0].GetLogPrefix();
		}

		public virtual IVecInt UnsatExplanation()
		{
			return this.solvers[this.winnerId].UnsatExplanation();
		}

		public virtual int[] PrimeImplicant()
		{
			return this.solvers[this.winnerId].PrimeImplicant();
		}

		/// <since>2.3.2</since>
		public virtual bool PrimeImplicant(int p)
		{
			return this.solvers[this.winnerId].PrimeImplicant(p);
		}

		public virtual IList<S> GetSolvers()
		{
			return new AList<S>(this.solvers);
		}

		public virtual int[] ModelWithInternalVariables()
		{
			return this.solvers[this.winnerId].ModelWithInternalVariables();
		}

		public virtual int RealNumberOfVariables()
		{
			return this.solvers[0].RealNumberOfVariables();
		}

		public virtual void RegisterLiteral(int p)
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				this.solvers[i].RegisterLiteral(p);
			}
		}

		public virtual bool IsSolverKeptHot()
		{
			return this.solvers[0].IsSolverKeptHot();
		}

		public virtual void SetKeepSolverHot(bool value)
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				this.solvers[i].SetKeepSolverHot(value);
			}
		}

		public virtual ISolver GetSolvingEngine()
		{
			throw new NotSupportedException("Not supported yet in ManyCore");
		}

		/// <since>2.3.3</since>
		public virtual void PrintStat(PrintWriter @out)
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				@out.Printf("%s>>>>>>>>>> Solver number %d (%d answers) <<<<<<<<<<<<<<<<<<%n", this.solvers[i].GetLogPrefix(), i, this.solversStats.Get(i).GetValue());
				this.solvers[i].PrintStat(@out);
			}
		}

		/// <since>2.3.3</since>
		public virtual void PrintInfos(PrintWriter @out)
		{
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				@out.Printf("%s>>>>>>>>>> Solver number %d <<<<<<<<<<<<<<<<<<%n", GetLogPrefix(), i);
				this.solvers[i].PrintInfos(@out);
			}
		}

		public override void LearnUnit(int p)
		{
			lock (this)
			{
				sharedUnitClauses.Push(LiteralsUtils.ToInternal(p));
			}
		}

		public virtual void ProvideUnitClauses(UnitPropagationListener upl)
		{
			lock (this)
			{
				for (int i = 0; i < sharedUnitClauses.Size(); i++)
				{
					upl.Enqueue(sharedUnitClauses.Get(i));
				}
			}
		}

		public virtual void SetUnitClauseProvider(UnitClauseProvider ucp)
		{
			throw new NotSupportedException("Does not make sense in the parallel context");
		}

		public virtual IConstr AddConstr(Constr constr)
		{
			throw new NotSupportedException("Not implemented yet in ManyCore: cannot add a specific constraint to each solver");
		}

		public virtual IConstr AddParity(IVecInt literals, bool even)
		{
			ConstrGroup group = new ConstrGroup(false);
			for (int i = 0; i < this.numberOfSolvers; i++)
			{
				group.Add(this.solvers[i].AddParity(literals, even));
			}
			return group;
		}
	}

	internal class RunnableSolver : Runnable
	{
		private readonly int index;

		private readonly ISolver solver;

		private readonly OutcomeListener ol;

		private readonly IVecInt assumps;

		private readonly bool globalTimeout;

		public RunnableSolver(int i, ISolver solver, IVecInt assumps, bool globalTimeout, OutcomeListener ol)
		{
			this.index = i;
			this.solver = solver;
			this.ol = ol;
			this.assumps = assumps;
			this.globalTimeout = globalTimeout;
		}

		public virtual void Run()
		{
			try
			{
				bool result = this.solver.IsSatisfiable(this.assumps, this.globalTimeout);
				this.ol.OnFinishWithAnswer(true, result, this.index);
			}
			catch (Exception)
			{
				this.ol.OnFinishWithAnswer(false, false, this.index);
			}
		}
	}
}
