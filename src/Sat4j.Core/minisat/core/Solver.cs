using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;
using Sharpen.Logging;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>
	/// The backbone of the library providing the modular implementation of a MiniSAT
	/// (Chaff) like solver.
	/// </summary>
	/// <author>leberre</author>
	[System.Serializable]
	public class Solver<D> : ISolverService, ICDCL<D>
		where D : DataStructureFactory
	{
		private const long serialVersionUID = 1L;

		private const double ClauseRescaleFactor = 1e-20;

		private const double ClauseRescaleBound = 1 / ClauseRescaleFactor;

		protected internal ILogAble @out;

		/// <summary>Set of original constraints.</summary>
		protected internal readonly IVec<Constr> constrs = new Vec<Constr>();

		/// <summary>Set of learned constraints.</summary>
		protected internal readonly IVec<Constr> learnts = new Vec<Constr>();

		/// <summary>Increment for clause activity.</summary>
		private double claInc = 1.0;

		/// <summary>decay factor pour l'activit? des clauses.</summary>
		private double claDecay = 1.0;

		/// <summary>propagation queue</summary>
		protected internal int qhead = 0;

		/// <summary>variable assignments (literals) in chronological order.</summary>
		protected internal readonly IVecInt trail = new VecInt();

		/// <summary>position of the decision levels on the trail.</summary>
		protected internal readonly IVecInt trailLim = new VecInt();

		/// <summary>position of assumptions before starting the search.</summary>
		protected internal int rootLevel;

		private int[] model = null;

		protected internal ILits voc;

		private IOrder order;

		private readonly ActivityComparator comparator = new ActivityComparator();

		internal SolverStats stats = new SolverStats();

		private LearningStrategy<D> learner;

		protected internal volatile bool undertimeout;

		private long timeout = int.MaxValue;

		private bool timeBasedTimeout = true;

		protected internal D dsfactory;

		private SearchParams @params;

		private readonly IVecInt __dimacs_out = new VecInt();

		protected internal SearchListener<ISolverService> slistener = new VoidTracing();

		private RestartStrategy restarter;

		private readonly IDictionary<string, Counter> constrTypes = new Dictionary<string, Counter>();

		private bool isDBSimplificationAllowed = false;

		internal readonly IVecInt learnedLiterals = new VecInt();

		internal bool verbose = false;

		private bool keepHot = false;

		private string prefix = "c ";

		private int declaredMaxVarId = 0;

		private UnitClauseProvider unitClauseProvider = UnitClauseProviderConstants.Void;

		// head of the queue in trail ... (taken from MiniSAT 1.14)
		/// <summary>
		/// Translates an IvecInt containing Dimacs formatted variables into and
		/// IVecInt containing internal formatted variables.
		/// </summary>
		/// <remarks>
		/// Translates an IvecInt containing Dimacs formatted variables into and
		/// IVecInt containing internal formatted variables.
		/// Note that for sake of efficiency, the IVecInt returned by this method is
		/// always the same. DO NOT STORE IT N A CONSTRAINT.
		/// </remarks>
		/// <param name="in">a vector of Dimacs formatted variables (e.g. 1,-2)</param>
		/// <returns>a vector of variables using internal representation (e.g 2,5)</returns>
		/// <seealso cref="Org.Sat4j.Core.LiteralsUtils"/>
		/// <since>2.3.6</since>
		public virtual IVecInt Dimacs2internal(IVecInt @in)
		{
			this.__dimacs_out.Clear();
			this.__dimacs_out.Ensure(@in.Size());
			int p;
			for (int i = 0; i < @in.Size(); i++)
			{
				p = @in.Get(i);
				if (p == 0)
				{
					throw new ArgumentException("0 is not a valid variable identifier");
				}
				this.__dimacs_out.UnsafePush(this.voc.GetFromPool(p));
			}
			return this.__dimacs_out;
		}

		/*
		* @since 2.3.1
		*/
		public virtual void RegisterLiteral(int p)
		{
			this.voc.GetFromPool(p);
		}

		/// <summary>creates a Solver without LearningListener.</summary>
		/// <remarks>
		/// creates a Solver without LearningListener. A learningListener must be
		/// added to the solver, else it won't backtrack!!! A data structure factory
		/// must be provided, else it won't work either.
		/// </remarks>
		public Solver(LearningStrategy<D> learner, D dsf, IOrder order, RestartStrategy restarter)
			: this(learner, dsf, new SearchParams(), order, restarter)
		{
			SimpleSimplification = new _ISimplifier_757(this);
			ExpensiveSimplification = new _ISimplifier_773(this);
			ExpensiveSimplificationWlonly = new _ISimplifier_790(this);
			memoryTimer = new MemoryBasedConflictTimer(this, 500);
			activity_based_low_memory = new ActivityLCDS(this, this.memoryTimer);
			lbdTimer = new LBDConflictTimer(this, 1000);
			lbd_based = new Glucose2LCDS<D>(this, this.lbdTimer);
			age_based = new AgeLCDS(this, this.lbdTimer);
			activity_based = new ActivityLCDS(this, this.lbdTimer);
			size_based = new SizeLCDS(this, this.lbdTimer);
			dimacsLevel = new _IComparer_2400(this);
		}

		public Solver(LearningStrategy<D> learner, D dsf, SearchParams @params, IOrder order, RestartStrategy restarter)
			: this(learner, dsf, @params, order, restarter, ILogAbleConstants.Console)
		{
			SimpleSimplification = new _ISimplifier_757(this);
			ExpensiveSimplification = new _ISimplifier_773(this);
			ExpensiveSimplificationWlonly = new _ISimplifier_790(this);
			memoryTimer = new MemoryBasedConflictTimer(this, 500);
			activity_based_low_memory = new ActivityLCDS(this, this.memoryTimer);
			lbdTimer = new LBDConflictTimer(this, 1000);
			lbd_based = new Glucose2LCDS<D>(this, this.lbdTimer);
			age_based = new AgeLCDS(this, this.lbdTimer);
			activity_based = new ActivityLCDS(this, this.lbdTimer);
			size_based = new SizeLCDS(this, this.lbdTimer);
			dimacsLevel = new _IComparer_2400(this);
		}

		public Solver(LearningStrategy<D> learner, D dsf, SearchParams @params, IOrder order, RestartStrategy restarter, ILogAble logger)
		{
			SimpleSimplification = new _ISimplifier_757(this);
			ExpensiveSimplification = new _ISimplifier_773(this);
			ExpensiveSimplificationWlonly = new _ISimplifier_790(this);
			memoryTimer = new MemoryBasedConflictTimer(this, 500);
			activity_based_low_memory = new ActivityLCDS(this, this.memoryTimer);
			lbdTimer = new LBDConflictTimer(this, 1000);
			lbd_based = new Glucose2LCDS<D>(this, this.lbdTimer);
			age_based = new AgeLCDS(this, this.lbdTimer);
			activity_based = new ActivityLCDS(this, this.lbdTimer);
			size_based = new SizeLCDS(this, this.lbdTimer);
			dimacsLevel = new _IComparer_2400(this);
			this.order = order;
			this.@params = @params;
			this.restarter = restarter;
			this.@out = logger;
			SetDataStructureFactory(dsf);
			// should be called after dsf has been set up
			SetLearningStrategy(learner);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.ICDCL#setDataStructureFactory(D)
		*/
		public void SetDataStructureFactory(D dsf)
		{
			this.dsfactory = dsf;
			this.dsfactory.SetUnitPropagationListener(this);
			this.dsfactory.SetLearner(this);
			this.voc = dsf.GetVocabulary();
			this.order.SetLits(this.voc);
		}

		/// <since>2.2</since>
		public virtual bool IsVerbose()
		{
			return this.verbose;
		}

		/// <param name="value"/>
		/// <since>2.2</since>
		public virtual void SetVerbose(bool value)
		{
			this.verbose = value;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.ICDCL#setSearchListener(org.sat4j.specs.
		* SearchListener )
		*/
		public virtual void SetSearchListener<S>(SearchListener<S> sl)
			where S : ISolverService
		{
			this.slistener = sl;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.ICDCL#getSearchListener()
		*/
		public virtual SearchListener<S> GetSearchListener<S>()
			where S : ISolverService
		{
			return this.slistener;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.ICDCL#setLearner(org.sat4j.minisat.core.
		* LearningStrategy)
		*/
		public virtual void SetLearner(LearningStrategy<D> strategy)
		{
			SetLearningStrategy(strategy);
		}

		/*
		* (non-Javadoc)
		*
		* @see
		* org.sat4j.minisat.core.ICDCL#setLearningStrategy(org.sat4j.minisat.core.
		* LearningStrategy)
		*/
		public virtual void SetLearningStrategy(LearningStrategy<D> strategy)
		{
			if (this.learner != null)
			{
				this.learner.SetSolver(null);
			}
			this.learner = strategy;
			strategy.SetSolver(this);
		}

		public virtual void SetTimeout(int t)
		{
			this.timeout = t * 1000L;
			this.timeBasedTimeout = true;
			this.undertimeout = true;
		}

		public virtual void SetTimeoutMs(long t)
		{
			this.timeout = t;
			this.timeBasedTimeout = true;
			this.undertimeout = true;
		}

		public virtual void SetTimeoutOnConflicts(int count)
		{
			this.timeout = count;
			this.timeBasedTimeout = false;
			this.undertimeout = true;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.ICDCL#setSearchParams(org.sat4j.minisat.core.
		* SearchParams)
		*/
		public virtual void SetSearchParams(SearchParams sp)
		{
			this.@params = sp;
		}

		public virtual SearchParams GetSearchParams()
		{
			return this.@params;
		}

		/*
		* (non-Javadoc)
		*
		* @see
		* org.sat4j.minisat.core.ICDCL#setRestartStrategy(org.sat4j.minisat.core
		* .RestartStrategy)
		*/
		public virtual void SetRestartStrategy(RestartStrategy restarter)
		{
			this.restarter = restarter;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.ICDCL#getRestartStrategy()
		*/
		public virtual RestartStrategy GetRestartStrategy()
		{
			return this.restarter;
		}

		public virtual void ExpireTimeout()
		{
			this.undertimeout = false;
			if (this.timeBasedTimeout)
			{
				if (this.timer != null)
				{
					this.timer.Cancel();
					this.timer = null;
				}
			}
			else
			{
				if (this.conflictCount != null)
				{
					this.conflictCount = null;
				}
			}
		}

		protected internal virtual int NAssigns()
		{
			return this.trail.Size();
		}

		public virtual int NConstraints()
		{
			return this.constrs.Size();
		}

		public virtual void Learn(Constr c)
		{
			this.slistener.Learn(c);
			this.learnts.Push(c);
			c.SetLearnt();
			c.Register();
			this.stats.IncLearnedclauses();
			switch (c.Size())
			{
				case 2:
				{
					this.stats.IncLearnedbinaryclauses();
					break;
				}

				case 3:
				{
					this.stats.IncLearnedternaryclauses();
					break;
				}

				default:
				{
					break;
				}
			}
		}

		// do nothing
		public int DecisionLevel()
		{
			return this.trailLim.Size();
		}

		[Obsolete]
		public virtual int NewVar()
		{
			int index = this.voc.NVars() + 1;
			this.voc.EnsurePool(index);
			return index;
		}

		public virtual int NewVar(int howmany)
		{
			if (this.declaredMaxVarId > 0 && howmany > this.declaredMaxVarId && this.voc.NVars() > this.declaredMaxVarId)
			{
				throw new InvalidOperationException("Caution, you are making solver's internal var id public with uncontrolled consequences with features requiring internal/hidden variables.");
			}
			this.voc.EnsurePool(howmany);
			this.declaredMaxVarId = howmany;
			return howmany;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddClause(IVecInt literals)
		{
			IVecInt vlits = Dimacs2internal(literals);
			return AddConstr(this.dsfactory.CreateClause(vlits));
		}

		public virtual bool RemoveConstr(IConstr co)
		{
			if (co == null)
			{
				throw new ArgumentException("Reference to the constraint to remove needed!");
			}
			//$NON-NLS-1$
			Constr c = (Constr)co;
			c.Remove(this);
			this.constrs.RemoveFromLast(c);
			ClearLearntClauses();
			string type = c.GetType().FullName;
			this.constrTypes[type].Dec();
			return true;
		}

		/// <since>2.1</since>
		public virtual bool RemoveSubsumedConstr(IConstr co)
		{
			if (co == null)
			{
				throw new ArgumentException("Reference to the constraint to remove needed!");
			}
			//$NON-NLS-1$
			if (this.constrs.Last() != co)
			{
				throw new ArgumentException("Can only remove latest added constraint!!!");
			}
			//$NON-NLS-1$
			Constr c = (Constr)co;
			c.Remove(this);
			this.constrs.Pop();
			string type = c.GetType().FullName;
			this.constrTypes[type].Dec();
			return true;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual void AddAllClauses(IVec<IVecInt> clauses)
		{
			for (IEnumerator<IVecInt> iterator = clauses.Iterator(); iterator.HasNext(); )
			{
				AddClause(iterator.Next());
			}
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddAtMost(IVecInt literals, int degree)
		{
			int n = literals.Size();
			IVecInt opliterals = new VecInt(n);
			for (IteratorInt iterator = literals.Iterator(); iterator.HasNext(); )
			{
				opliterals.Push(-iterator.Next());
			}
			return AddAtLeast(opliterals, n - degree);
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddAtLeast(IVecInt literals, int degree)
		{
			IVecInt vlits = Dimacs2internal(literals);
			return AddConstr(this.dsfactory.CreateCardinalityConstraint(vlits, degree));
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddExactly(IVecInt literals, int n)
		{
			ConstrGroup group = new ConstrGroup(false);
			group.Add(AddAtMost(literals, n));
			group.Add(AddAtLeast(literals, n));
			return group;
		}

		public virtual IConstr AddParity(IVecInt literals, bool even)
		{
			IVecInt vlits = Dimacs2internal(literals);
			return AddConstr(Org.Sat4j.Minisat.Constraints.Xor.Xor.CreateParityConstraint(vlits, even, voc));
		}

		public virtual bool SimplifyDB()
		{
			// Simplifie la base de clauses apres la premiere propagation des
			// clauses unitaires
			IVec<Constr>[] cs = new IVec[] { this.constrs, this.learnts };
			for (int type = 0; type < 2; type++)
			{
				int j = 0;
				for (int i = 0; i < cs[type].Size(); i++)
				{
					if (cs[type].Get(i).Simplify())
					{
						// enleve les contraintes satisfaites de la base
						cs[type].Get(i).Remove(this);
					}
					else
					{
						cs[type].MoveTo(j++, i);
					}
				}
				cs[type].ShrinkTo(j);
			}
			return true;
		}

		/// <summary>Si un mod?le est trouv?, ce vecteur contient le mod?le.</summary>
		/// <returns>un mod?le de la formule.</returns>
		public virtual int[] Model()
		{
			if (this.model == null)
			{
				throw new NotSupportedException("Call the solve method first!!!");
			}
			//$NON-NLS-1$
			int[] nmodel = new int[this.model.Length];
			System.Array.Copy(this.model, 0, nmodel, 0, this.model.Length);
			return nmodel;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.ICDCL#enqueue(int)
		*/
		public virtual bool Enqueue(int p)
		{
			return Enqueue(p, null);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.ICDCL#enqueue(int,
		* org.sat4j.minisat.core.Constr)
		*/
		public virtual bool Enqueue(int p, Constr from)
		{
			System.Diagnostics.Debug.Assert(p > 1);
			if (this.voc.IsSatisfied(p))
			{
				// literal is already satisfied. Skipping.
				return true;
			}
			if (this.voc.IsFalsified(p))
			{
				// conflicting enqueued assignment
				return false;
			}
			this.slistener.Enqueueing(LiteralsUtils.ToDimacs(p), from);
			// new fact, store it
			this.voc.Satisfies(p);
			this.voc.SetLevel(p, DecisionLevel());
			this.voc.SetReason(p, from);
			this.trail.Push(p);
			if (from != null && from.Learnt())
			{
				this.learnedConstraintsDeletionStrategy.OnPropagation(from);
			}
			return true;
		}

		private bool[] mseen = new bool[0];

		private readonly IVecInt mpreason = new VecInt();

		private readonly IVecInt moutLearnt = new VecInt();

		/// <exception cref="Org.Sat4j.Specs.TimeoutException">if the timeout is reached during conflict analysis.</exception>
		public virtual void Analyze(Constr confl, Pair results)
		{
			System.Diagnostics.Debug.Assert(confl != null);
			bool[] seen = this.mseen;
			IVecInt outLearnt = this.moutLearnt;
			IVecInt preason = this.mpreason;
			outLearnt.Clear();
			System.Diagnostics.Debug.Assert(outLearnt.Size() == 0);
			for (int i = 0; i < seen.Length; i++)
			{
				seen[i] = false;
			}
			int counter = 0;
			int p = ILitsConstants.Undefined;
			// placeholder for the asserting literal
			outLearnt.Push(ILitsConstants.Undefined);
			int outBtlevel = 0;
			IConstr prevConfl = null;
			do
			{
				preason.Clear();
				System.Diagnostics.Debug.Assert(confl != null);
				if (prevConfl != confl || confl.CanBePropagatedMultipleTimes())
				{
					confl.CalcReason(p, preason);
					this.learnedConstraintsDeletionStrategy.OnConflictAnalysis(confl);
					// Trace reason for p
					for (int j = 0; j < preason.Size(); j++)
					{
						int q = preason.Get(j);
						if (!seen[q >> 1])
						{
							seen[q >> 1] = true;
							this.order.UpdateVar(q);
							if (this.voc.GetLevel(q) == DecisionLevel())
							{
								counter++;
								this.order.UpdateVarAtDecisionLevel(q);
							}
							else
							{
								if (this.voc.GetLevel(q) > 0)
								{
									// only literals assigned after decision level 0
									// part of
									// the explanation
									outLearnt.Push(q ^ 1);
									outBtlevel = Math.Max(outBtlevel, this.voc.GetLevel(q));
								}
							}
						}
					}
				}
				prevConfl = confl;
				do
				{
					// select next reason to look at
					p = this.trail.Last();
					confl = this.voc.GetReason(p);
					UndoOne();
				}
				while (!seen[p >> 1]);
			}
			while (--counter > 0);
			// seen[p.var] indique que p se trouve dans outLearnt ou dans
			// le dernier niveau de d?cision
			outLearnt.Set(0, p ^ 1);
			this.simplifier.Simplify(outLearnt);
			Constr c = this.dsfactory.CreateUnregisteredClause(outLearnt);
			this.learnedConstraintsDeletionStrategy.OnClauseLearning(c);
			results.SetReason(c);
			System.Diagnostics.Debug.Assert(outBtlevel > -1);
			results.SetBacktrackLevel(outBtlevel);
		}

		/// <summary>Derive a subset of the assumptions causing the inconsistency.</summary>
		/// <param name="confl">the last conflict of the search, occuring at root level.</param>
		/// <param name="assumps">the set of assumption literals</param>
		/// <param name="conflictingLiteral">
		/// the literal detected conflicting while propagating
		/// assumptions.
		/// </param>
		/// <returns>a subset of assumps causing the inconsistency.</returns>
		/// <since>2.2</since>
		public virtual IVecInt AnalyzeFinalConflictInTermsOfAssumptions(Constr confl, IVecInt assumps, int conflictingLiteral)
		{
			if (assumps.Size() == 0)
			{
				return null;
			}
			while (!this.trailLim.IsEmpty() && this.trailLim.Last() == this.trail.Size())
			{
				// conflict detected when assuming a value
				this.trailLim.Pop();
			}
			bool[] seen = this.mseen;
			IVecInt outLearnt = this.moutLearnt;
			IVecInt preason = this.mpreason;
			outLearnt.Clear();
			if (this.trailLim.Size() == 0)
			{
				// conflict detected on unit clauses
				return outLearnt;
			}
			System.Diagnostics.Debug.Assert(outLearnt.Size() == 0);
			for (int i = 0; i < seen.Length; i++)
			{
				seen[i] = false;
			}
			if (confl == null)
			{
				seen[conflictingLiteral >> 1] = true;
			}
			int p = ILitsConstants.Undefined;
			while (confl == null && this.trail.Size() > 0 && this.trailLim.Size() > 0)
			{
				p = this.trail.Last();
				confl = this.voc.GetReason(p);
				UndoOne();
				if (confl == null && p == (conflictingLiteral ^ 1))
				{
					outLearnt.Push(LiteralsUtils.ToDimacs(p));
				}
				if (this.trail.Size() <= this.trailLim.Last())
				{
					this.trailLim.Pop();
				}
			}
			if (confl == null)
			{
				return outLearnt;
			}
			do
			{
				preason.Clear();
				confl.CalcReason(p, preason);
				// Trace reason for p
				for (int j = 0; j < preason.Size(); j++)
				{
					int q = preason.Get(j);
					if (!seen[q >> 1])
					{
						seen[q >> 1] = true;
						if (this.voc.GetReason(q) == null && this.voc.GetLevel(q) > 0)
						{
							System.Diagnostics.Debug.Assert(assumps.Contains(LiteralsUtils.ToDimacs(q)));
							outLearnt.Push(LiteralsUtils.ToDimacs(q));
						}
					}
				}
				do
				{
					// select next reason to look at
					p = this.trail.Last();
					confl = this.voc.GetReason(p);
					UndoOne();
					if (DecisionLevel() > 0 && this.trail.Size() <= this.trailLim.Last())
					{
						this.trailLim.Pop();
					}
				}
				while (this.trail.Size() > 0 && DecisionLevel() > 0 && (!seen[p >> 1] || confl == null));
			}
			while (DecisionLevel() > 0);
			return outLearnt;
		}

		private sealed class _ISimplifier_742 : ISimplifier
		{

			private const long serialVersionUID = 1L;

			public void Simplify(IVecInt outLearnt)
			{
			}

			public override string ToString()
			{
				return "No reason simplification";
			}
		}

		public static readonly ISimplifier NoSimplification = new _ISimplifier_742();

		private sealed class _ISimplifier_757 : ISimplifier
		{
			public _ISimplifier_757(Solver<D> _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private const long serialVersionUID = 1L;

			//$NON-NLS-1$
			public void Simplify(IVecInt conflictToReduce)
			{
				this._enclosing.SimpleSimplification(conflictToReduce);
			}

			public override string ToString()
			{
				return "Simple reason simplification";
			}

			private readonly Solver<D> _enclosing;
		}

		public readonly ISimplifier SimpleSimplification;

		private sealed class _ISimplifier_773 : ISimplifier
		{
			public _ISimplifier_773(Solver<D> _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private const long serialVersionUID = 1L;

			//$NON-NLS-1$
			public void Simplify(IVecInt conflictToReduce)
			{
				this._enclosing.ExpensiveSimplification(conflictToReduce);
			}

			public override string ToString()
			{
				return "Expensive reason simplification";
			}

			private readonly Solver<D> _enclosing;
		}

		public readonly ISimplifier ExpensiveSimplification;

		private sealed class _ISimplifier_790 : ISimplifier
		{
			public _ISimplifier_790(Solver<D> _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private const long serialVersionUID = 1L;

			//$NON-NLS-1$
			public void Simplify(IVecInt conflictToReduce)
			{
				this._enclosing.ExpensiveSimplificationWLOnly(conflictToReduce);
			}

			public override string ToString()
			{
				return "Expensive reason simplification specific for WL data structure";
			}

			private readonly Solver<D> _enclosing;
		}

		public readonly ISimplifier ExpensiveSimplificationWlonly;

		private ISimplifier simplifier = NoSimplification;

		//$NON-NLS-1$
		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.ICDCL#setSimplifier(java.lang.String)
		*/
		public virtual void SetSimplifier(SimplificationType simp)
		{
			FieldInfo f;
			try
			{
				f = Sharpen.Runtime.GetDeclaredField(typeof(Org.Sat4j.Minisat.Core.Solver), simp.ToString());
				this.simplifier = (ISimplifier)f.GetValue(this);
			}
			catch (Exception e)
			{
				Logger.GetLogger("org.sat4j.core").Log(Level.Info, "Issue when assigning simplifier: disabling simplification", e);
				this.simplifier = NoSimplification;
			}
		}

		/*
		* (non-Javadoc)
		*
		* @see
		* org.sat4j.minisat.core.ICDCL#setSimplifier(org.sat4j.minisat.core.Solver
		* .ISimplifier)
		*/
		public virtual void SetSimplifier(ISimplifier simp)
		{
			this.simplifier = simp;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.ICDCL#getSimplifier()
		*/
		public virtual ISimplifier GetSimplifier()
		{
			return this.simplifier;
		}

		// MiniSat -- Copyright (c) 2003-2005, Niklas Een, Niklas Sorensson
		//
		// Permission is hereby granted, free of charge, to any person obtaining a
		// copy of this software and associated documentation files (the
		// "Software"), to deal in the Software without restriction, including
		// without limitation the rights to use, copy, modify, merge, publish,
		// distribute, sublicense, and/or sell copies of the Software, and to
		// permit persons to whom the Software is furnished to do so, subject to
		// the following conditions:
		//
		// The above copyright notice and this permission notice shall be included
		// in all copies or substantial portions of the Software.
		//
		// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
		// OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
		// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
		// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
		// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
		// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
		// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
		// Taken from MiniSAT 1.14: Simplify conflict clause (a little):
		private void SimpleSimplification(IVecInt conflictToReduce)
		{
			int i;
			int j;
			int p;
			bool[] seen = this.mseen;
			IConstr r;
			for (i = j = 1; i < conflictToReduce.Size(); i++)
			{
				r = this.voc.GetReason(conflictToReduce.Get(i));
				if (r == null || r.CanBePropagatedMultipleTimes())
				{
					conflictToReduce.MoveTo(j++, i);
				}
				else
				{
					for (int k = 0; k < r.Size(); k++)
					{
						p = r.Get(k);
						if (!seen[p >> 1] && this.voc.IsFalsified(p) && this.voc.GetLevel(p) != 0)
						{
							conflictToReduce.MoveTo(j++, i);
							break;
						}
					}
				}
			}
			conflictToReduce.Shrink(i - j);
			this.stats.IncReducedliterals(i - j);
		}

		private readonly IVecInt analyzetoclear = new VecInt();

		private readonly IVecInt analyzestack = new VecInt();

		// Taken from MiniSAT 1.14
		private void ExpensiveSimplification(IVecInt conflictToReduce)
		{
			// Simplify conflict clause (a lot):
			//
			int i;
			int j;
			// (maintain an abstraction of levels involved in conflict)
			this.analyzetoclear.Clear();
			conflictToReduce.CopyTo(this.analyzetoclear);
			for (i = 1, j = 1; i < conflictToReduce.Size(); i++)
			{
				if (this.voc.GetReason(conflictToReduce.Get(i)) == null || !AnalyzeRemovable(conflictToReduce.Get(i)))
				{
					conflictToReduce.MoveTo(j++, i);
				}
			}
			conflictToReduce.Shrink(i - j);
			this.stats.IncReducedliterals(i - j);
		}

		// Check if 'p' can be removed.' min_level' is used to abort early if
		// visiting literals at a level that cannot be removed.
		//
		private bool AnalyzeRemovable(int p)
		{
			System.Diagnostics.Debug.Assert(this.voc.GetReason(p) != null);
			ILits lvoc = this.voc;
			IVecInt lanalyzestack = this.analyzestack;
			IVecInt lanalyzetoclear = this.analyzetoclear;
			lanalyzestack.Clear();
			lanalyzestack.Push(p);
			bool[] seen = this.mseen;
			int top = lanalyzetoclear.Size();
			while (lanalyzestack.Size() > 0)
			{
				int q = lanalyzestack.Last();
				System.Diagnostics.Debug.Assert(lvoc.GetReason(q) != null);
				Constr c = lvoc.GetReason(q);
				lanalyzestack.Pop();
				if (c.CanBePropagatedMultipleTimes())
				{
					for (int j = top; j < lanalyzetoclear.Size(); j++)
					{
						seen[lanalyzetoclear.Get(j) >> 1] = false;
					}
					lanalyzetoclear.Shrink(lanalyzetoclear.Size() - top);
					return false;
				}
				for (int i = 0; i < c.Size(); i++)
				{
					int l = c.Get(i);
					if (!seen[LiteralsUtils.Var(l)] && lvoc.IsFalsified(l) && lvoc.GetLevel(l) != 0)
					{
						if (lvoc.GetReason(l) == null)
						{
							for (int j = top; j < lanalyzetoclear.Size(); j++)
							{
								seen[lanalyzetoclear.Get(j) >> 1] = false;
							}
							lanalyzetoclear.Shrink(lanalyzetoclear.Size() - top);
							return false;
						}
						seen[l >> 1] = true;
						lanalyzestack.Push(l);
						lanalyzetoclear.Push(l);
					}
				}
			}
			return true;
		}

		// Taken from MiniSAT 1.14
		private void ExpensiveSimplificationWLOnly(IVecInt conflictToReduce)
		{
			// Simplify conflict clause (a lot):
			//
			int i;
			int j;
			// (maintain an abstraction of levels involved in conflict)
			this.analyzetoclear.Clear();
			conflictToReduce.CopyTo(this.analyzetoclear);
			for (i = 1, j = 1; i < conflictToReduce.Size(); i++)
			{
				if (this.voc.GetReason(conflictToReduce.Get(i)) == null || !AnalyzeRemovableWLOnly(conflictToReduce.Get(i)))
				{
					conflictToReduce.MoveTo(j++, i);
				}
			}
			conflictToReduce.Shrink(i - j);
			this.stats.IncReducedliterals(i - j);
		}

		// Check if 'p' can be removed.' min_level' is used to abort early if
		// visiting literals at a level that cannot be removed.
		//
		private bool AnalyzeRemovableWLOnly(int p)
		{
			System.Diagnostics.Debug.Assert(this.voc.GetReason(p) != null);
			this.analyzestack.Clear();
			this.analyzestack.Push(p);
			bool[] seen = this.mseen;
			int top = this.analyzetoclear.Size();
			while (this.analyzestack.Size() > 0)
			{
				int q = this.analyzestack.Last();
				System.Diagnostics.Debug.Assert(this.voc.GetReason(q) != null);
				Constr c = this.voc.GetReason(q);
				this.analyzestack.Pop();
				for (int i = 1; i < c.Size(); i++)
				{
					int l = c.Get(i);
					if (!seen[LiteralsUtils.Var(l)] && this.voc.GetLevel(l) != 0)
					{
						if (this.voc.GetReason(l) == null)
						{
							for (int j = top; j < this.analyzetoclear.Size(); j++)
							{
								seen[this.analyzetoclear.Get(j) >> 1] = false;
							}
							this.analyzetoclear.Shrink(this.analyzetoclear.Size() - top);
							return false;
						}
						seen[l >> 1] = true;
						this.analyzestack.Push(l);
						this.analyzetoclear.Push(l);
					}
				}
			}
			return true;
		}

		// END Minisat 1.14 cut and paste
		protected internal virtual void UndoOne()
		{
			// gather last assigned literal
			int p = this.trail.Last();
			System.Diagnostics.Debug.Assert(p > 1);
			System.Diagnostics.Debug.Assert(this.voc.GetLevel(p) >= 0);
			int x = p >> 1;
			// unassign variable
			this.voc.Unassign(p);
			this.voc.SetReason(p, null);
			this.voc.SetLevel(p, -1);
			// update heuristics value
			this.order.Undo(x);
			// remove literal from the trail
			this.trail.Pop();
			// update constraints on backtrack.
			// not used if the solver uses watched literals.
			IVec<Undoable> undos = this.voc.Undos(p);
			System.Diagnostics.Debug.Assert(undos != null);
			for (int size = undos.Size(); size > 0; size--)
			{
				undos.Last().Undo(p);
				undos.Pop();
			}
		}

		/// <summary>Propagate activity to a constraint</summary>
		/// <param name="confl">a constraint</param>
		public virtual void ClaBumpActivity(Constr confl)
		{
			confl.IncActivity(this.claInc);
			if (confl.GetActivity() > ClauseRescaleBound)
			{
				ClaRescalActivity();
			}
		}

		// for (int i = 0; i < confl.size(); i++) {
		// varBumpActivity(confl.get(i));
		// }
		public virtual void VarBumpActivity(int p)
		{
			this.order.UpdateVar(p);
		}

		private void ClaRescalActivity()
		{
			for (int i = 0; i < this.learnts.Size(); i++)
			{
				this.learnts.Get(i).RescaleBy(ClauseRescaleFactor);
			}
			this.claInc *= ClauseRescaleFactor;
		}

		internal readonly IVec<Propagatable> watched = new Vec<Propagatable>();

		/// <returns>null if not conflict is found, else a conflicting constraint.</returns>
		public Constr Propagate()
		{
			IVecInt ltrail = this.trail;
			SolverStats lstats = this.stats;
			IOrder lorder = this.order;
			SearchListener lslistener = this.slistener;
			// ltrail.size() changes due to propagation
			// cannot cache that value.
			while (this.qhead < ltrail.Size())
			{
				lstats.IncPropagations();
				int p = ltrail.Get(this.qhead++);
				lslistener.Propagating(LiteralsUtils.ToDimacs(p));
				lorder.AssignLiteral(p);
				Constr confl = ReduceClausesContainingTheNegationOf(p);
				if (confl != null)
				{
					return confl;
				}
			}
			return null;
		}

		private Constr ReduceClausesContainingTheNegationOf(int p)
		{
			// p is the literal to propagate
			// Moved original MiniSAT code to dsfactory to avoid
			// watches manipulation in counter Based clauses for instance.
			System.Diagnostics.Debug.Assert(p > 1);
			IVec<Propagatable> lwatched = this.watched;
			lwatched.Clear();
			this.voc.Watches(p).MoveTo(lwatched);
			int size = lwatched.Size();
			for (int i = 0; i < size; i++)
			{
				this.stats.IncInspects();
				// try shortcut
				// shortcut = shortcuts.get(i);
				// if (shortcut != ILits.UNDEFINED && voc.isSatisfied(shortcut))
				// {
				// voc.watch(p, watched.get(i), shortcut);
				// stats.shortcuts++;
				// continue;
				// }
				if (!lwatched.Get(i).Propagate(this, p))
				{
					// Constraint is conflicting: copy remaining watches to
					// watches[p]
					// and return constraint
					int sizew = lwatched.Size();
					for (int j = i + 1; j < sizew; j++)
					{
						this.voc.Watch(p, lwatched.Get(j));
					}
					this.qhead = this.trail.Size();
					// propQ.clear();
					return lwatched.Get(i).ToConstraint();
				}
			}
			return null;
		}

		internal virtual void Record(Constr constr)
		{
			constr.AssertConstraint(this);
			int p = LiteralsUtils.ToDimacs(constr.Get(0));
			this.slistener.Adding(p);
			if (constr.Size() == 1)
			{
				this.stats.IncLearnedliterals();
				this.slistener.LearnUnit(p);
			}
			else
			{
				this.learner.Learns(constr);
			}
		}

		/// <returns>false ssi conflit imm?diat.</returns>
		public virtual bool Assume(int p)
		{
			// Precondition: assume propagation queue is empty
			// assert this.trail.size() == this.qhead; no longer true with computing
			// PI
			System.Diagnostics.Debug.Assert(!this.trailLim.Contains(this.trail.Size()));
			this.trailLim.Push(this.trail.Size());
			return Enqueue(p);
		}

		/// <summary>Revert to the state before the last assume()</summary>
		internal virtual void Cancel()
		{
			// assert trail.size() == qhead || !undertimeout;
			int decisionvar = this.trail.UnsafeGet(this.trailLim.Last());
			this.slistener.Backtracking(LiteralsUtils.ToDimacs(decisionvar));
			for (int c = this.trail.Size() - this.trailLim.Last(); c > 0; c--)
			{
				UndoOne();
			}
			this.trailLim.Pop();
			this.qhead = this.trail.Size();
		}

		/// <summary>Restore literals</summary>
		private void CancelLearntLiterals(int learnedLiteralsLimit)
		{
			this.learnedLiterals.Clear();
			// assert trail.size() == qhead || !undertimeout;
			while (this.trail.Size() > learnedLiteralsLimit)
			{
				this.learnedLiterals.Push(this.trail.Last());
				UndoOne();
			}
		}

		// qhead = 0;
		// learnedLiterals = 0;
		/// <summary>Cancel several levels of assumptions</summary>
		/// <param name="level"/>
		protected internal virtual void CancelUntil(int level)
		{
			while (DecisionLevel() > level)
			{
				Cancel();
			}
		}

		protected internal virtual void CancelUntilTrailLevel(int level)
		{
			while (!trail.IsEmpty() && trail.Size() > level)
			{
				UndoOne();
				if (!trailLim.IsEmpty() && trailLim.Last() == trail.Size())
				{
					trailLim.Pop();
					decisions.Pop();
				}
			}
		}

		private readonly Pair analysisResult = new Pair();

		private bool[] userbooleanmodel;

		private IVecInt unsatExplanationInTermsOfAssumptions;

		private Lbool Search(IVecInt assumps)
		{
			System.Diagnostics.Debug.Assert(this.rootLevel == DecisionLevel());
			this.stats.IncStarts();
			int backjumpLevel;
			// varDecay = 1 / params.varDecay;
			this.order.SetVarDecay(1 / this.@params.GetVarDecay());
			this.claDecay = 1 / this.@params.GetClaDecay();
			do
			{
				this.slistener.BeginLoop();
				// propagate unit clauses and other constraints
				Constr confl = Propagate();
				System.Diagnostics.Debug.Assert(this.trail.Size() == this.qhead);
				if (confl == null)
				{
					// No conflict found
					if (DecisionLevel() == 0 && this.isDBSimplificationAllowed)
					{
						this.stats.IncRootSimplifications();
						bool ret = SimplifyDB();
						System.Diagnostics.Debug.Assert(ret);
					}
					System.Diagnostics.Debug.Assert(NAssigns() <= this.voc.RealnVars());
					if (NAssigns() == this.voc.RealnVars())
					{
						ModelFound();
						this.slistener.SolutionFound((this.fullmodel != null) ? this.fullmodel : this.model, this);
						if (this.sharedConflict == null)
						{
							CancelUntil(this.rootLevel);
							return Lbool.True;
						}
						else
						{
							// this.sharedConflict;
							if (DecisionLevel() == rootLevel)
							{
								confl = this.sharedConflict;
								this.sharedConflict = null;
							}
							else
							{
								int level = this.sharedConflict.GetAssertionLevel(trail, DecisionLevel());
								CancelUntilTrailLevel(level);
								this.qhead = this.trail.Size();
								this.sharedConflict.AssertConstraint(this);
								this.sharedConflict = null;
								continue;
							}
						}
					}
					else
					{
						if (this.restarter.ShouldRestart())
						{
							CancelUntil(this.rootLevel);
							return Lbool.Undefined;
						}
						if (this.needToReduceDB)
						{
							ReduceDB();
							this.needToReduceDB = false;
						}
						if (this.sharedConflict == null)
						{
							// New variable decision
							this.stats.IncDecisions();
							int p = this.order.Select();
							if (p == ILitsConstants.Undefined)
							{
								// check (expensive) if all the constraints are not
								// satisfied
								bool allsat = true;
								for (int i = 0; i < this.constrs.Size(); i++)
								{
									if (!this.constrs.Get(i).IsSatisfied())
									{
										allsat = false;
										break;
									}
								}
								if (allsat)
								{
									ModelFound();
									this.slistener.SolutionFound((this.fullmodel != null) ? this.fullmodel : this.model, this);
									return Lbool.True;
								}
								else
								{
									confl = PreventTheSameDecisionsToBeMade();
									this.lastConflictMeansUnsat = false;
								}
							}
							else
							{
								System.Diagnostics.Debug.Assert(p > 1);
								this.slistener.Assuming(LiteralsUtils.ToDimacs(p));
								bool ret = Assume(p);
								System.Diagnostics.Debug.Assert(ret);
							}
						}
						else
						{
							confl = this.sharedConflict;
							this.sharedConflict = null;
						}
					}
				}
				if (confl != null)
				{
					// conflict found
					this.stats.IncConflicts();
					this.slistener.ConflictFound(confl, DecisionLevel(), this.trail.Size());
					this.conflictCount.NewConflict();
					if (DecisionLevel() == this.rootLevel)
					{
						if (this.lastConflictMeansUnsat)
						{
							// conflict at root level, the formula is inconsistent
							this.unsatExplanationInTermsOfAssumptions = AnalyzeFinalConflictInTermsOfAssumptions(confl, assumps, ILitsConstants.Undefined);
							return Lbool.False;
						}
						return Lbool.Undefined;
					}
					int conflictTrailLevel = this.trail.Size();
					// analyze conflict
					try
					{
						Analyze(confl, this.analysisResult);
					}
					catch (Specs.TimeoutException)
					{
						return Lbool.Undefined;
					}
					System.Diagnostics.Debug.Assert(this.analysisResult.GetBacktrackLevel() < DecisionLevel());
					backjumpLevel = Math.Max(this.analysisResult.GetBacktrackLevel(), this.rootLevel);
					this.slistener.Backjump(backjumpLevel);
					CancelUntil(backjumpLevel);
					if (backjumpLevel == this.rootLevel)
					{
						this.restarter.OnBackjumpToRootLevel();
					}
					System.Diagnostics.Debug.Assert(DecisionLevel() >= this.rootLevel && DecisionLevel() >= this.analysisResult.GetBacktrackLevel());
					if (this.analysisResult.GetReason() == null)
					{
						return Lbool.False;
					}
					Record(this.analysisResult.GetReason());
					this.restarter.NewLearnedClause(this.analysisResult.GetReason(), conflictTrailLevel);
					this.analysisResult.SetReason(null);
					DecayActivities();
				}
			}
			while (this.undertimeout);
			return Lbool.Undefined;
		}

		// timeout occured
		private Constr PreventTheSameDecisionsToBeMade()
		{
			IVecInt clause = new VecInt(NVars());
			int p;
			for (int i = this.trail.Size() - 1; i >= this.rootLevel; i--)
			{
				p = this.trail.Get(i);
				if (this.voc.GetReason(p) == null)
				{
					clause.Push(p ^ 1);
				}
			}
			return this.dsfactory.CreateUnregisteredClause(clause);
		}

		protected internal virtual void AnalyzeAtRootLevel(Constr conflict)
		{
		}

		internal readonly IVecInt implied = new VecInt();

		internal readonly IVecInt decisions = new VecInt();

		internal int[] fullmodel;

		internal virtual void ModelFound()
		{
			decisions.Clear();
			IVecInt tempmodel = new VecInt(NVars());
			this.userbooleanmodel = new bool[RealNumberOfVariables()];
			this.fullmodel = null;
			Constr reason;
			for (int i = 1; i <= NVars(); i++)
			{
				if (this.voc.BelongsToPool(i))
				{
					int p = this.voc.GetFromPool(i);
					if (!this.voc.IsUnassigned(p))
					{
						tempmodel.Push(this.voc.IsSatisfied(p) ? i : -i);
						this.userbooleanmodel[i - 1] = this.voc.IsSatisfied(p);
						reason = this.voc.GetReason(p);
						if (reason == null && this.voc.GetLevel(p) > 0 || reason != null && reason.Learnt())
						{
							// we consider literals propagated by learned
							// clauses
							// as decisions to allow blocking models by
							// decisions.
							this.decisions.Push(tempmodel.Last());
						}
						else
						{
							this.implied.Push(tempmodel.Last());
						}
					}
				}
			}
			this.model = new int[tempmodel.Size()];
			tempmodel.CopyTo(this.model);
			if (RealNumberOfVariables() > NVars())
			{
				for (int i_1 = NVars() + 1; i_1 <= RealNumberOfVariables(); i_1++)
				{
					if (this.voc.BelongsToPool(i_1))
					{
						int p = this.voc.GetFromPool(i_1);
						if (!this.voc.IsUnassigned(p))
						{
							tempmodel.Push(this.voc.IsSatisfied(p) ? i_1 : -i_1);
							this.userbooleanmodel[i_1 - 1] = this.voc.IsSatisfied(p);
							if (this.voc.GetReason(p) == null)
							{
								this.decisions.Push(tempmodel.Last());
							}
							else
							{
								this.implied.Push(tempmodel.Last());
							}
						}
					}
				}
				this.fullmodel = new int[tempmodel.Size()];
				tempmodel.MoveTo(this.fullmodel);
			}
			else
			{
				this.fullmodel = this.model;
			}
		}

		/// <summary>
		/// Forget a variable in the formula by falsifying both its positive and
		/// negative literals.
		/// </summary>
		/// <param name="var">a variable</param>
		/// <returns>
		/// a conflicting constraint resulting from the disparition of those
		/// literals.
		/// </returns>
		internal virtual Constr Forget(int var)
		{
			bool satisfied = this.voc.IsSatisfied(LiteralsUtils.ToInternal(var));
			this.voc.Forgets(var);
			Constr confl;
			if (satisfied)
			{
				confl = ReduceClausesContainingTheNegationOf(LiteralsUtils.ToInternal(-var));
			}
			else
			{
				confl = ReduceClausesContainingTheNegationOf(LiteralsUtils.ToInternal(var));
			}
			return confl;
		}

		protected internal int[] prime;

		public virtual int[] PrimeImplicant()
		{
			string primeApproach = Runtime.GetProperty("prime");
			PrimeImplicantStrategy strategy;
			if ("OLD".Equals(primeApproach))
			{
				strategy = new QuadraticPrimeImplicantStrategy();
			}
			else
			{
				if ("ALGO2".Equals(primeApproach))
				{
					strategy = new CounterBasedPrimeImplicantStrategy();
				}
				else
				{
					strategy = new WatcherBasedPrimeImplicantStrategy();
				}
			}
			int[] implicant = strategy.Compute(this);
			this.prime = strategy.GetPrimeImplicantAsArrayWithHoles();
			return implicant;
		}

		public virtual bool PrimeImplicant(int p)
		{
			if (p == 0 || Math.Abs(p) > RealNumberOfVariables())
			{
				throw new ArgumentException("Use a valid Dimacs var id as argument!");
			}
			//$NON-NLS-1$
			if (this.prime == null)
			{
				throw new NotSupportedException("Call the primeImplicant method first!!!");
			}
			//$NON-NLS-1$
			return this.prime[Math.Abs(p)] == p;
		}

		public virtual bool Model(int var)
		{
			if (var <= 0 || var > RealNumberOfVariables())
			{
				throw new ArgumentException("Use a valid Dimacs var id as argument!");
			}
			//$NON-NLS-1$
			if (this.userbooleanmodel == null)
			{
				throw new NotSupportedException("Call the solve method first!!!");
			}
			//$NON-NLS-1$
			return this.userbooleanmodel[var - 1];
		}

		public virtual void ClearLearntClauses()
		{
			for (IEnumerator<Constr> iterator = this.learnts.Iterator(); iterator.HasNext(); )
			{
				iterator.Next().Remove(this);
			}
			this.learnts.Clear();
			this.learnedLiterals.Clear();
		}

		protected internal void ReduceDB()
		{
			this.stats.IncReduceddb();
			this.slistener.Cleaning();
			this.learnedConstraintsDeletionStrategy.Reduce(this.learnts);
		}

		protected internal virtual void SortOnActivity()
		{
			this.learnts.Sort(this.comparator);
		}

		protected internal virtual void DecayActivities()
		{
			this.order.VarDecayActivity();
			ClaDecayActivity();
		}

		private void ClaDecayActivity()
		{
			this.claInc *= this.claDecay;
		}

		/// <returns>true iff the set of constraints is satisfiable, else false.</returns>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable()
		{
			return IsSatisfiable(VecInt.Empty);
		}

		/// <returns>true iff the set of constraints is satisfiable, else false.</returns>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(bool global)
		{
			return IsSatisfiable(VecInt.Empty, global);
		}

		private double timebegin = 0;

		private bool needToReduceDB;

		private ConflictTimerContainer conflictCount;

		[System.NonSerialized]
		private Timer timer;

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(IVecInt assumps)
		{
			return IsSatisfiable(assumps, false);
		}

		public LearnedConstraintsDeletionStrategy FixedSize(int maxsize)
		{
			return new _LearnedConstraintsDeletionStrategy_1549(this, maxsize);
		}

		private sealed class _ConflictTimerAdapter_1553 : ConflictTimerAdapter
		{
			public _ConflictTimerAdapter_1553(Org.Sat4j.Minisat.Core.Solver<DataStructureFactory> baseArg1, int baseArg2)
				: base(baseArg1, baseArg2)
			{
			}

			private const long serialVersionUID = 1L;

			public override void Run()
			{
				this.GetSolver().SetNeedToReduceDB(true);
			}
		}

		private sealed class _LearnedConstraintsDeletionStrategy_1549 : LearnedConstraintsDeletionStrategy
		{
			public _LearnedConstraintsDeletionStrategy_1549(Solver<D> _enclosing, int maxsize)
			{
				this._enclosing = _enclosing;
				this.maxsize = maxsize;
				this.aTimer = new _ConflictTimerAdapter_1553(this._enclosing._enclosing, maxsize);
			}

			private const long serialVersionUID = 1L;

			private readonly ConflictTimer aTimer;

			public void Reduce(IVec<Constr> learnedConstrs)
			{
				int i;
				int j;
				int k;
				for (i = j = k = 0; i < this._enclosing._enclosing.learnts.Size() && this._enclosing._enclosing.learnts.Size() - k > maxsize; i++)
				{
					Constr c = this._enclosing._enclosing.learnts.Get(i);
					if (c.Locked() || c.Size() == 2)
					{
						this._enclosing._enclosing.learnts.Set(j++, this._enclosing._enclosing.learnts.Get(i));
					}
					else
					{
						c.Remove(this._enclosing._enclosing);
						k++;
					}
				}
				for (; i < this._enclosing._enclosing.learnts.Size(); i++)
				{
					this._enclosing._enclosing.learnts.Set(j++, this._enclosing._enclosing.learnts.Get(i));
				}
				if (this._enclosing._enclosing.verbose)
				{
					this._enclosing._enclosing.@out.Log(this._enclosing.GetLogPrefix() + "cleaning " + (this._enclosing._enclosing.learnts.Size() - j) + " clauses out of " + this._enclosing._enclosing.learnts.Size());
				}
				//$NON-NLS-1$
				//$NON-NLS-1$
				// out.flush();
				this._enclosing._enclosing.learnts.ShrinkTo(j);
			}

			public void OnConflictAnalysis(Constr reason)
			{
			}

			// TODO Auto-generated method stub
			public void OnClauseLearning(Constr outLearnt)
			{
			}

			// TODO Auto-generated method stub
			public override string ToString()
			{
				return "Fixed size (" + maxsize + ") learned constraints deletion strategy";
			}

			public void Init()
			{
			}

			public ConflictTimer GetTimer()
			{
				return this.aTimer;
			}

			public void OnPropagation(Constr from)
			{
			}

			private readonly Solver<D> _enclosing;

			private readonly int maxsize;
		}

		private readonly ConflictTimer memoryTimer;

		/// <since>2.1</since>
		public readonly LearnedConstraintsDeletionStrategy activity_based_low_memory;

		private readonly ConflictTimer lbdTimer;

		/// <since>2.1</since>
		public readonly LearnedConstraintsDeletionStrategy lbd_based;

		/// <since>2.3.6</since>
		public readonly LearnedConstraintsDeletionStrategy age_based;

		/// <since>2.3.6</since>
		public readonly LearnedConstraintsDeletionStrategy activity_based;

		/// <since>2.3.6</since>
		public readonly LearnedConstraintsDeletionStrategy size_based;

		protected internal LearnedConstraintsDeletionStrategy learnedConstraintsDeletionStrategy = this.lbd_based;

		// TODO Auto-generated method stub
		/*
		* (non-Javadoc)
		*
		* @see
		* org.sat4j.minisat.core.ICDCL#setLearnedConstraintsDeletionStrategy(org
		* .sat4j.minisat.core.Solver.LearnedConstraintsDeletionStrategy)
		*/
		public virtual void SetLearnedConstraintsDeletionStrategy(LearnedConstraintsDeletionStrategy lcds)
		{
			if (this.conflictCount != null)
			{
				this.conflictCount.Add(lcds.GetTimer());
				System.Diagnostics.Debug.Assert(this.learnedConstraintsDeletionStrategy != null);
				this.conflictCount.Remove(this.learnedConstraintsDeletionStrategy.GetTimer());
			}
			this.learnedConstraintsDeletionStrategy = lcds;
		}

		private bool lastConflictMeansUnsat;

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(IVecInt assumps, bool global)
		{
			Lbool status = Lbool.Undefined;
			bool alreadylaunched = this.conflictCount != null;
			int howmany = this.voc.NVars();
			if (this.mseen.Length <= howmany)
			{
				this.mseen = new bool[howmany + 1];
			}
			this.trail.Ensure(howmany);
			this.trailLim.Ensure(howmany);
			this.learnedLiterals.Ensure(howmany);
			this.decisions.Clear();
			this.implied.Clear();
			this.slistener.Init(this);
			this.slistener.Start();
			this.model = null;
			// forget about previous model
			this.fullmodel = null;
			this.userbooleanmodel = null;
			this.prime = null;
			this.unsatExplanationInTermsOfAssumptions = null;
			// To make sure that new literals in the assumptions appear in the
			// solver data structures
			IVecInt localAssumps = new VecInt(assumps.Size());
			for (IteratorInt iterator = assumps.Iterator(); iterator.HasNext(); )
			{
				int assump = iterator.Next();
				localAssumps.Push(this.voc.GetFromPool(assump));
			}
			if (!alreadylaunched || !this.keepHot)
			{
				this.order.Init();
			}
			this.learnedConstraintsDeletionStrategy.Init();
			int learnedLiteralsLimit = this.trail.Size();
			// Fix for Bug SAT37
			this.qhead = 0;
			// Apply undos on unit literals because they are getting propagated
			// again now that qhead is 0.
			for (int i = learnedLiteralsLimit - 1; i >= 0; i--)
			{
				int p = this.trail.Get(i);
				IVec<Undoable> undos = this.voc.Undos(p);
				System.Diagnostics.Debug.Assert(undos != null);
				for (int size = undos.Size(); size > 0; size--)
				{
					undos.Last().Undo(p);
					undos.Pop();
				}
			}
			// push previously learned literals
			for (IteratorInt iterator_1 = this.learnedLiterals.Iterator(); iterator_1.HasNext(); )
			{
				Enqueue(iterator_1.Next());
			}
			// propagate constraints
			Constr confl = Propagate();
			if (confl != null)
			{
				AnalyzeAtRootLevel(confl);
				this.slistener.ConflictFound(confl, 0, 0);
				this.slistener.End(Lbool.False);
				CancelUntil(0);
				CancelLearntLiterals(learnedLiteralsLimit);
				return false;
			}
			// push incremental assumptions
			for (IteratorInt iterator_2 = localAssumps.Iterator(); iterator_2.HasNext(); )
			{
				int p = iterator_2.Next();
				if (!this.voc.IsSatisfied(p) && !Assume(p) || (confl = Propagate()) != null)
				{
					if (confl == null)
					{
						this.slistener.ConflictFound(p);
						this.unsatExplanationInTermsOfAssumptions = AnalyzeFinalConflictInTermsOfAssumptions(null, assumps, p);
						this.unsatExplanationInTermsOfAssumptions.Push(LiteralsUtils.ToDimacs(p));
					}
					else
					{
						this.slistener.ConflictFound(confl, DecisionLevel(), this.trail.Size());
						this.unsatExplanationInTermsOfAssumptions = AnalyzeFinalConflictInTermsOfAssumptions(confl, assumps, ILitsConstants.Undefined);
					}
					this.slistener.End(Lbool.False);
					CancelUntil(0);
					CancelLearntLiterals(learnedLiteralsLimit);
					return false;
				}
			}
			this.rootLevel = DecisionLevel();
			// moved initialization here if new literals are added in the
			// assumptions.
			this.learner.Init();
			if (!alreadylaunched)
			{
				this.conflictCount = new ConflictTimerContainer();
				this.conflictCount.Add(this.restarter);
				this.conflictCount.Add(this.learnedConstraintsDeletionStrategy.GetTimer());
			}
			bool firstTimeGlobal = false;
			if (this.timeBasedTimeout)
			{
				if (!global || this.timer == null)
				{
					firstTimeGlobal = true;
					this.undertimeout = true;
					TimerTask stopMe = new _TimerTask_1778(this);
					this.timer = new Timer(true);
					this.timer.Schedule(stopMe, this.timeout);
				}
			}
			else
			{
				if (!global || !alreadylaunched)
				{
					firstTimeGlobal = true;
					this.undertimeout = true;
					ConflictTimer conflictTimeout = new _ConflictTimerAdapter_1799(this, (int)this.timeout);
					this.conflictCount.Add(conflictTimeout);
				}
			}
			if (!global || firstTimeGlobal)
			{
				this.restarter.Init(this.@params, this.stats);
				this.timebegin = Runtime.CurrentTimeMillis();
			}
			this.needToReduceDB = false;
			// this is used to allow the solver to be incomplete,
			// when using a heuristics limited to a subset of variables
			this.lastConflictMeansUnsat = true;
			// Solve
			while (status == Lbool.Undefined && this.undertimeout && this.lastConflictMeansUnsat)
			{
				int before = this.trail.Size();
				unitClauseProvider.ProvideUnitClauses(this);
				this.stats.IncImportedUnits(this.trail.Size() - before);
				status = Search(assumps);
				if (status == Lbool.Undefined)
				{
					this.restarter.OnRestart();
					this.slistener.Restarting();
				}
			}
			CancelUntil(0);
			CancelLearntLiterals(learnedLiteralsLimit);
			if (!global && this.timeBasedTimeout)
			{
				lock (this)
				{
					if (this.timer != null)
					{
						this.timer.Cancel();
						this.timer = null;
					}
				}
			}
			this.slistener.End(status);
			if (!this.undertimeout)
			{
				string message = " Timeout (" + this.timeout + (this.timeBasedTimeout ? "ms" : " conflicts") + ") exceeded";
				throw new TimeoutException(message);
			}
			if (status == Lbool.Undefined && !this.lastConflictMeansUnsat)
			{
				throw new TimeoutException("Cannot decide the satisfiability");
			}
			// When using a search enumerator (to compute all models)
			// the final answer is FALSE, however we are aware of at least one model
			// (the last one)
			return model != null;
		}

		private sealed class _TimerTask_1778 : TimerTask
		{
			public _TimerTask_1778(Solver<D> _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public override void Run()
			{
				this._enclosing._enclosing.undertimeout = false;
				lock (this._enclosing._enclosing)
				{
					if (this._enclosing._enclosing.timer != null)
					{
						this._enclosing._enclosing.timer.Cancel();
						this._enclosing._enclosing.timer = null;
					}
				}
			}

			private readonly Solver<D> _enclosing;
		}

		private sealed class _ConflictTimerAdapter_1799 : ConflictTimerAdapter
		{
			public _ConflictTimerAdapter_1799(Org.Sat4j.Minisat.Core.Solver<DataStructureFactory> baseArg1, int baseArg2)
				: base(baseArg1, baseArg2)
			{
			}

			private const long serialVersionUID = 1L;

			public override void Run()
			{
				this.GetSolver().ExpireTimeout();
			}
		}

		public virtual void PrintInfos(PrintWriter @out)
		{
			PrintInfos(@out, prefix);
		}

		public virtual void PrintInfos(PrintWriter @out, string prefix)
		{
			@out.Write(prefix);
			@out.WriteLine("constraints type ");
			long total = 0;
			foreach (KeyValuePair<string, Counter> entry in this.constrTypes)
			{
				@out.WriteLine(prefix + entry.Key + " => " + entry.Value);
				total += entry.Value.GetValue();
			}
			@out.Write(prefix);
			@out.Write(total);
			@out.WriteLine(" constraints processed.");
		}

		/// <since>2.1</since>
		public virtual void PrintLearntClausesInfos(PrintWriter @out, string prefix)
		{
			if (this.learnts.IsEmpty())
			{
				return;
			}
			IDictionary<string, Counter> learntTypes = new Dictionary<string, Counter>();
			for (IEnumerator<Constr> it = this.learnts.Iterator(); it.HasNext(); )
			{
				string type = it.Next().GetType().FullName;
				Counter count = learntTypes[type];
				if (count == null)
				{
					learntTypes[type] = new Counter();
				}
				else
				{
					count.Inc();
				}
			}
			foreach (KeyValuePair<string, Counter> entry in learntTypes)
			{
				@out.WriteLine(prefix + "learnt constraints type " + entry.Key + "\t: " + entry.Value);
			}
		}

		public virtual SolverStats GetStats()
		{
			return this.stats;
		}

		/// <param name="myStats"/>
		/// <since>2.2</since>
		protected internal virtual void InitStats(SolverStats myStats)
		{
			this.stats = myStats;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.ICDCL#getOrder()
		*/
		public virtual IOrder GetOrder()
		{
			return this.order;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.ICDCL#setOrder(org.sat4j.minisat.core.IOrder)
		*/
		public virtual void SetOrder(IOrder h)
		{
			this.order = h;
			this.order.SetLits(this.voc);
		}

		public virtual ILits GetVocabulary()
		{
			return this.voc;
		}

		public virtual void Reset()
		{
			if (this.timer != null)
			{
				this.timer.Cancel();
				this.timer = null;
			}
			this.trail.Clear();
			this.trailLim.Clear();
			this.qhead = 0;
			for (IEnumerator<Constr> iterator = this.constrs.Iterator(); iterator.HasNext(); )
			{
				iterator.Next().Remove(this);
			}
			this.constrs.Clear();
			ClearLearntClauses();
			this.voc.ResetPool();
			this.dsfactory.Reset();
			this.stats.Reset();
			this.constrTypes.Clear();
			this.undertimeout = true;
			this.declaredMaxVarId = 0;
		}

		public virtual int NVars()
		{
			if (this.declaredMaxVarId == 0)
			{
				return this.voc.NVars();
			}
			return this.declaredMaxVarId;
		}

		/// <param name="constr">a constraint implementing the Constr interface.</param>
		/// <returns>a reference to the constraint for external use.</returns>
		public virtual IConstr AddConstr(Constr constr)
		{
			if (constr == null)
			{
				Counter count = this.constrTypes["ignored satisfied constraints"];
				if (count == null)
				{
					this.constrTypes["ignored satisfied constraints"] = new Counter();
				}
				else
				{
					count.Inc();
				}
			}
			else
			{
				this.constrs.Push(constr);
				string type = constr.GetType().FullName;
				Counter count = this.constrTypes[type];
				if (count == null)
				{
					this.constrTypes[type] = new Counter();
				}
				else
				{
					count.Inc();
				}
			}
			return constr;
		}

		public virtual DataStructureFactory GetDSFactory()
		{
			return this.dsfactory;
		}

		public virtual IVecInt GetOutLearnt()
		{
			return this.moutLearnt;
		}

		/// <summary>returns the ith constraint in the solver.</summary>
		/// <param name="i">the constraint number (begins at 0)</param>
		/// <returns>the ith constraint</returns>
		public virtual IConstr GetIthConstr(int i)
		{
			return this.constrs.Get(i);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.specs.ISolver#printStat(java.io.PrintStream,
		* java.lang.String)
		*/
		public virtual void PrintStat(TextWriter @out, string prefix)
		{
			PrintStat(new PrintWriter(@out, true), prefix);
		}

		public virtual void PrintStat(PrintWriter @out)
		{
			PrintStat(@out, prefix);
		}

		public virtual void PrintStat(PrintWriter @out, string prefix)
		{
			this.stats.PrintStat(@out, prefix);
			double cputime = (Runtime.CurrentTimeMillis() - this.timebegin) / 1000;
			@out.WriteLine(prefix + "speed (assignments/second)\t: " + this.stats.GetPropagations() / cputime);
			//$NON-NLS-1$
			this.order.PrintStat(@out, prefix);
			PrintLearntClausesInfos(@out, prefix);
		}

		/*
		* (non-Javadoc)
		*
		* @see java.lang.Object#toString()
		*/
		public virtual string ToString(string prefix)
		{
			StringBuilder stb = new StringBuilder();
			object[] objs = new object[] { this.dsfactory, this.learner, this.@params, this.order, this.simplifier, this.restarter, this.learnedConstraintsDeletionStrategy };
			stb.Append(prefix);
			stb.Append("--- Begin Solver configuration ---");
			//$NON-NLS-1$
			stb.Append("\n");
			//$NON-NLS-1$
			foreach (object o in objs)
			{
				stb.Append(prefix);
				stb.Append(o.ToString());
				stb.Append("\n");
			}
			//$NON-NLS-1$
			stb.Append(prefix);
			stb.Append("timeout=");
			if (this.timeBasedTimeout)
			{
				stb.Append(this.timeout / 1000);
				stb.Append("s\n");
			}
			else
			{
				stb.Append(this.timeout);
				stb.Append(" conflicts\n");
			}
			stb.Append(prefix);
			stb.Append("DB Simplification allowed=");
			stb.Append(this.isDBSimplificationAllowed);
			stb.Append("\n");
			stb.Append(prefix);
			if (IsSolverKeptHot())
			{
				stb.Append("Heuristics kept accross calls (keep the solver \"hot\")\n");
				stb.Append(prefix);
			}
			stb.Append("Listener: ");
			stb.Append(slistener);
			stb.Append("\n");
			stb.Append(prefix);
			stb.Append("--- End Solver configuration ---");
			//$NON-NLS-1$
			return stb.ToString();
		}

		/*
		* (non-Javadoc)
		*
		* @see java.lang.Object#toString()
		*/
		public override string ToString()
		{
			return ToString(string.Empty);
		}

		//$NON-NLS-1$
		public virtual int GetTimeout()
		{
			return (int)(this.timeBasedTimeout ? this.timeout / 1000 : this.timeout);
		}

		/// <since>2.1</since>
		public virtual long GetTimeoutMs()
		{
			if (!this.timeBasedTimeout)
			{
				throw new NotSupportedException("The timeout is given in number of conflicts!");
			}
			return this.timeout;
		}

		public virtual void SetExpectedNumberOfClauses(int nb)
		{
			this.constrs.Ensure(nb);
		}

		public virtual IDictionary<string, Number> GetStat()
		{
			return this.stats.ToMap();
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual int[] FindModel()
		{
			if (IsSatisfiable())
			{
				return Model();
			}
			// DLB findbugs ok
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
			// DLB findbugs ok
			// A zero length array would mean that the formula is a tautology.
			return null;
		}

		public virtual bool IsDBSimplificationAllowed()
		{
			return this.isDBSimplificationAllowed;
		}

		public virtual void SetDBSimplificationAllowed(bool status)
		{
			this.isDBSimplificationAllowed = status;
		}

		/// <since>2.1</since>
		public virtual int NextFreeVarId(bool reserve)
		{
			return this.voc.NextFreeVarId(reserve);
		}

		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddBlockingClause(IVecInt literals)
		{
			return AddClause(literals);
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr DiscardCurrentModel()
		{
			IVecInt blockingClause = CreateBlockingClauseForCurrentModel();
			if (blockingClause.IsEmpty())
			{
				throw new ContradictionException();
			}
			return AddBlockingClause(blockingClause);
		}

		public virtual IVecInt CreateBlockingClauseForCurrentModel()
		{
			IVecInt clause = new VecInt(decisions.Size());
			if (RealNumberOfVariables() > NVars())
			{
				// we rely on the model projection in that case
				foreach (int p in this.model)
				{
					clause.Push(-p);
				}
			}
			else
			{
				for (int i = 0; i < decisions.Size(); i++)
				{
					clause.Push(-decisions.Get(i));
				}
			}
			return clause;
		}

		/// <since>2.1</since>
		public virtual void Unset(int p)
		{
			// the literal might already have been
			// removed from the trail.
			if (this.voc.IsUnassigned(p) || this.trail.IsEmpty())
			{
				return;
			}
			int current = this.trail.Last();
			while (current != p)
			{
				UndoOne();
				if (this.trail.IsEmpty())
				{
					return;
				}
				if (!trailLim.IsEmpty() && trailLim.Last() == trail.Size())
				{
					trailLim.Pop();
				}
				current = this.trail.Last();
			}
			UndoOne();
			if (!trailLim.IsEmpty() && trailLim.Last() == trail.Size())
			{
				trailLim.Pop();
			}
			this.qhead = this.trail.Size();
		}

		/// <since>2.2</since>
		public virtual void SetLogPrefix(string prefix)
		{
			this.prefix = prefix;
		}

		/// <since>2.2</since>
		public virtual string GetLogPrefix()
		{
			return this.prefix;
		}

		/// <since>2.2</since>
		public virtual IVecInt UnsatExplanation()
		{
			if (this.unsatExplanationInTermsOfAssumptions == null)
			{
				return null;
			}
			IVecInt copy = new VecInt(this.unsatExplanationInTermsOfAssumptions.Size());
			this.unsatExplanationInTermsOfAssumptions.CopyTo(copy);
			return copy;
		}

		/// <since>2.3.1</since>
		public virtual int[] ModelWithInternalVariables()
		{
			if (this.model == null)
			{
				throw new NotSupportedException("Call the solve method first!!!");
			}
			//$NON-NLS-1$
			int[] nmodel;
			if (NVars() == RealNumberOfVariables())
			{
				nmodel = new int[this.model.Length];
				System.Array.Copy(this.model, 0, nmodel, 0, nmodel.Length);
			}
			else
			{
				nmodel = new int[this.fullmodel.Length];
				System.Array.Copy(this.fullmodel, 0, nmodel, 0, nmodel.Length);
			}
			return nmodel;
		}

		/// <since>2.3.1</since>
		public virtual int RealNumberOfVariables()
		{
			return this.voc.NVars();
		}

		/// <since>2.3.2</since>
		public virtual void Stop()
		{
			ExpireTimeout();
		}

		protected internal Constr sharedConflict;

		/// <since>2.3.2</since>
		public virtual void Backtrack(int[] reason)
		{
			IVecInt clause = new VecInt(reason.Length);
			foreach (int d in reason)
			{
				clause.Push(LiteralsUtils.ToInternal(d));
			}
			this.sharedConflict = this.dsfactory.CreateUnregisteredClause(clause);
			Learn(this.sharedConflict);
		}

		/// <since>2.3.2</since>
		public virtual Lbool TruthValue(int literal)
		{
			int p = LiteralsUtils.ToInternal(literal);
			if (this.voc.IsFalsified(p))
			{
				return Lbool.False;
			}
			if (this.voc.IsSatisfied(p))
			{
				return Lbool.True;
			}
			return Lbool.Undefined;
		}

		/// <since>2.3.2</since>
		public virtual int CurrentDecisionLevel()
		{
			return DecisionLevel();
		}

		/// <since>2.3.2</since>
		public virtual int[] GetLiteralsPropagatedAt(int decisionLevel)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		/// <since>2.3.2</since>
		public virtual void SuggestNextLiteralToBranchOn(int l)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		protected internal virtual bool IsNeedToReduceDB()
		{
			return this.needToReduceDB;
		}

		public virtual void SetNeedToReduceDB(bool needToReduceDB)
		{
			this.needToReduceDB = needToReduceDB;
		}

		public virtual void SetLogger(ILogAble @out)
		{
			this.@out = @out;
		}

		public virtual ILogAble GetLogger()
		{
			return this.@out;
		}

		public virtual double[] GetVariableHeuristics()
		{
			return this.order.GetVariableHeuristics();
		}

		public virtual IVec<Constr> GetLearnedConstraints()
		{
			return this.learnts;
		}

		/// <since>2.3.2</since>
		public virtual void SetLearnedConstraintsDeletionStrategy(ConflictTimer timer, LearnedConstraintsEvaluationType evaluation)
		{
			if (this.conflictCount != null)
			{
				this.conflictCount.Add(timer);
				this.conflictCount.Remove(this.learnedConstraintsDeletionStrategy.GetTimer());
			}
			switch (evaluation)
			{
				case LearnedConstraintsEvaluationType.Activity:
				{
					this.learnedConstraintsDeletionStrategy = new ActivityLCDS(this, timer);
					break;
				}

				case LearnedConstraintsEvaluationType.Lbd:
				{
					this.learnedConstraintsDeletionStrategy = new GlucoseLCDS<D>(this, timer);
					break;
				}

				case LearnedConstraintsEvaluationType.Lbd2:
				{
					this.learnedConstraintsDeletionStrategy = new Glucose2LCDS<D>(this, timer);
					break;
				}
			}
			if (this.conflictCount != null)
			{
				this.learnedConstraintsDeletionStrategy.Init();
			}
		}

		/// <since>2.3.2</since>
		public virtual void SetLearnedConstraintsDeletionStrategy(LearnedConstraintsEvaluationType evaluation)
		{
			ConflictTimer aTimer = this.learnedConstraintsDeletionStrategy.GetTimer();
			switch (evaluation)
			{
				case LearnedConstraintsEvaluationType.Activity:
				{
					this.learnedConstraintsDeletionStrategy = new ActivityLCDS(this, aTimer);
					break;
				}

				case LearnedConstraintsEvaluationType.Lbd:
				{
					this.learnedConstraintsDeletionStrategy = new GlucoseLCDS<D>(this, aTimer);
					break;
				}

				case LearnedConstraintsEvaluationType.Lbd2:
				{
					this.learnedConstraintsDeletionStrategy = new Glucose2LCDS<D>(this, aTimer);
					break;
				}
			}
			if (this.conflictCount != null)
			{
				this.learnedConstraintsDeletionStrategy.Init();
			}
		}

		public virtual bool IsSolverKeptHot()
		{
			return this.keepHot;
		}

		public virtual void SetKeepSolverHot(bool keepHot)
		{
			this.keepHot = keepHot;
		}

		private sealed class _IComparer_2400 : IComparer<int>
		{
			public _IComparer_2400(Solver<D> _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public int Compare(int i1, int i2)
			{
				return this._enclosing.voc.GetLevel(Math.Abs(i2)) - this._enclosing.voc.GetLevel(Math.Abs(i1));
			}

			private readonly Solver<D> _enclosing;
		}

		private readonly IComparer<int> dimacsLevel;

		private IComparer<int> TrailComparator()
		{
			return new _IComparer_2407(this);
		}

		private sealed class _IComparer_2407 : IComparer<int>
		{
			public _IComparer_2407(Solver<D> _enclosing)
			{
				{
					for (int i = 0; i < _enclosing.trail.Size(); i++)
					{
						this.trailLevel[LiteralsUtils.Var(_enclosing.trail.Get(i))] = i;
					}
				}
				this.trailLevel = new Dictionary<int, int>();
			}

			private readonly IDictionary<int, int> trailLevel;

			public int Compare(int i1, int i2)
			{
				int trail1 = this.trailLevel[Math.Abs(i1)];
				int trail2 = this.trailLevel[Math.Abs(i2)];
				if (trail1 == null)
				{
					return -1;
				}
				if (trail2 == null)
				{
					return -1;
				}
				return trail2 - trail1;
			}
		}

		public virtual IConstr AddClauseOnTheFly(int[] literals)
		{
			IList<int> lliterals = new AList<int>();
			foreach (int d in literals)
			{
				lliterals.Add(d);
			}
			lliterals.Sort(TrailComparator());
			IVecInt clause = new VecInt(literals.Length);
			foreach (int d_1 in lliterals)
			{
				clause.Push(LiteralsUtils.ToInternal(d_1));
			}
			this.sharedConflict = this.dsfactory.CreateUnregisteredClause(clause);
			this.sharedConflict.Register();
			AddConstr(this.sharedConflict);
			return this.sharedConflict;
		}

		public virtual ISolver GetSolvingEngine()
		{
			return this;
		}

		/// <param name="literals"/>
		public virtual IConstr AddAtMostOnTheFly(int[] literals, int degree)
		{
			IVecInt clause = new VecInt(literals.Length);
			foreach (int d in literals)
			{
				clause.Push(LiteralsUtils.ToInternal(-d));
			}
			IVecInt copy = new VecInt(clause.Size());
			clause.CopyTo(copy);
			this.sharedConflict = this.dsfactory.CreateUnregisteredCardinalityConstraint(copy, literals.Length - degree);
			this.sharedConflict.Register();
			AddConstr(this.sharedConflict);
			return this.sharedConflict;
		}

		protected internal virtual ICollection<int> FromLastDecisionLevel(IVecInt lits)
		{
			ICollection<int> subset = new HashSet<int>();
			int max = -1;
			int q;
			int level;
			for (int i = 0; i < lits.Size(); i++)
			{
				q = lits.Get(i);
				level = voc.GetLevel(q);
				if (level > max)
				{
					subset.Clear();
					subset.Add(q);
					max = level;
				}
				else
				{
					if (level == max)
					{
						subset.Add(q);
					}
				}
			}
			return subset;
		}

		public virtual void SetUnitClauseProvider(UnitClauseProvider ucp)
		{
			this.unitClauseProvider = ucp;
		}
	}
}
