using System.IO;
using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	/// <author>
	/// leberre Heuristique du prouveur. Changement par rapport au MiniSAT
	/// original : la gestion activity est faite ici et non plus dans Solver.
	/// </author>
	[System.Serializable]
	public class VarOrderHeap : IOrder
	{
		private const long serialVersionUID = 1L;

		private const double VarRescaleFactor = 1e-100;

		private const double VarRescaleBound = 1 / VarRescaleFactor;

		/// <summary>mesure heuristique de l'activite d'une variable.</summary>
		protected internal double[] activity = new double[1];

		private double varDecay = 1.0;

		/// <summary>increment pour l'activite des variables.</summary>
		private double varInc = 1.0;

		protected internal ILits lits;

		private long nullchoice = 0;

		protected internal Heap heap;

		protected internal IPhaseSelectionStrategy phaseStrategy;

		public VarOrderHeap()
			: this(new PhaseInLastLearnedClauseSelectionStrategy())
		{
		}

		public VarOrderHeap(IPhaseSelectionStrategy strategy)
		{
			/*
			* Created on 16 oct. 2003
			*/
			this.phaseStrategy = strategy;
		}

		/// <summary>Change the selection strategy.</summary>
		/// <param name="strategy"/>
		public virtual void SetPhaseSelectionStrategy(IPhaseSelectionStrategy strategy)
		{
			this.phaseStrategy = strategy;
		}

		public virtual IPhaseSelectionStrategy GetPhaseSelectionStrategy()
		{
			return this.phaseStrategy;
		}

		public virtual void SetLits(ILits lits)
		{
			this.lits = lits;
		}

		/// <summary>
		/// Selectionne une nouvelle variable, non affectee, ayant l'activite la plus
		/// elevee.
		/// </summary>
		/// <returns>Lit.UNDEFINED si aucune variable n'est trouvee</returns>
		public virtual int Select()
		{
			while (!this.heap.Empty())
			{
				int var = this.heap.Getmin();
				int next = this.phaseStrategy.Select(var);
				if (this.lits.IsUnassigned(next))
				{
					if (this.activity[var] < 0.0001)
					{
						this.nullchoice++;
					}
					return next;
				}
			}
			return ILitsConstants.Undefined;
		}

		/// <summary>Change la valeur de varDecay.</summary>
		/// <param name="d">la nouvelle valeur de varDecay</param>
		public virtual void SetVarDecay(double d)
		{
			this.varDecay = d;
		}

		/// <summary>Methode appelee quand la variable x est desaffectee.</summary>
		/// <param name="x"/>
		public virtual void Undo(int x)
		{
			if (!this.heap.InHeap(x))
			{
				this.heap.Insert(x);
			}
		}

		/// <summary>Appelee lorsque l'activite de la variable x a change.</summary>
		/// <param name="p">a literal</param>
		public virtual void UpdateVar(int p)
		{
			int var = LiteralsUtils.Var(p);
			UpdateActivity(var);
			this.phaseStrategy.UpdateVar(p);
			if (this.heap.InHeap(var))
			{
				this.heap.Increase(var);
			}
		}

		protected internal virtual void UpdateActivity(int var)
		{
			if ((this.activity[var] += this.varInc) > VarRescaleBound)
			{
				VarRescaleActivity();
			}
		}

		public virtual void VarDecayActivity()
		{
			this.varInc *= this.varDecay;
		}

		private void VarRescaleActivity()
		{
			for (int i = 1; i < this.activity.Length; i++)
			{
				this.activity[i] *= VarRescaleFactor;
			}
			this.varInc *= VarRescaleFactor;
		}

		public virtual double VarActivity(int p)
		{
			return this.activity[LiteralsUtils.Var(p)];
		}

		public virtual int NumberOfInterestingVariables()
		{
			int cpt = 0;
			for (int i = 1; i < this.activity.Length; i++)
			{
				if (this.activity[i] > 1.0)
				{
					cpt++;
				}
			}
			return cpt;
		}

		protected internal virtual Heap CreateHeap(double[] activity)
		{
			return new Heap(new ActivityBasedVariableComparator(activity));
		}

		/// <summary>
		/// that method has the responsibility to initialize all arrays in the
		/// heuristics.
		/// </summary>
		/// <remarks>
		/// that method has the responsibility to initialize all arrays in the
		/// heuristics. PLEASE CALL super.init() IF YOU OVERRIDE THAT METHOD.
		/// </remarks>
		public virtual void Init()
		{
			int nlength = this.lits.NVars() + 1;
			if (this.activity == null || this.activity.Length < nlength)
			{
				this.activity = new double[nlength];
			}
			this.phaseStrategy.Init(nlength);
			this.activity[0] = -1;
			this.heap = CreateHeap(this.activity);
			this.heap.SetBounds(nlength);
			for (int i = 1; i < nlength; i++)
			{
				System.Diagnostics.Debug.Assert(i > 0);
				System.Diagnostics.Debug.Assert(i <= this.lits.NVars(), string.Empty + this.lits.NVars() + "/" + i);
				//$NON-NLS-1$ //$NON-NLS-2$
				this.activity[i] = 0.0;
				if (this.lits.BelongsToPool(i))
				{
					this.heap.Insert(i);
				}
			}
		}

		public override string ToString()
		{
			return "VSIDS like heuristics from MiniSAT using a heap " + this.phaseStrategy;
		}

		//$NON-NLS-1$
		public virtual ILits GetVocabulary()
		{
			return this.lits;
		}

		public virtual void PrintStat(PrintWriter @out, string prefix)
		{
			@out.WriteLine(prefix + "non guided choices\t: " + this.nullchoice);
		}

		//$NON-NLS-1$
		public virtual void AssignLiteral(int p)
		{
			this.phaseStrategy.AssignLiteral(p);
		}

		public virtual void UpdateVarAtDecisionLevel(int q)
		{
			this.phaseStrategy.UpdateVarAtDecisionLevel(q);
		}

		public virtual double[] GetVariableHeuristics()
		{
			return this.activity;
		}
	}
}
