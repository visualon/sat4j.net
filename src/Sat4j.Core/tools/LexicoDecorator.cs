using System;
using System.Collections.Generic;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	[System.Serializable]
	public class LexicoDecorator<T> : SolverDecorator<T>, IOptimizationProblem
		where T : ISolver
	{
		protected internal readonly IList<IVecInt> criteria = new AList<IVecInt>();

		protected internal int currentCriterion = 0;

		protected internal IConstr prevConstr;

		private Number currentValue = -1;

		protected internal int[] prevfullmodel;

		protected internal int[] prevmodelwithinternalvars;

		protected internal bool[] prevboolmodel;

		protected internal bool isSolutionOptimal;

		private const long serialVersionUID = 1L;

		public LexicoDecorator(T solver)
			: base(solver)
		{
		}

		public virtual void AddCriterion(IVecInt literals)
		{
			IVecInt copy = new VecInt(literals.Size());
			literals.CopyTo(copy);
			this.criteria.Add(copy);
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool AdmitABetterSolution()
		{
			return AdmitABetterSolution(VecInt.Empty);
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool AdmitABetterSolution(IVecInt assumps)
		{
			this.isSolutionOptimal = false;
			if (Decorated().IsSatisfiable(assumps, true))
			{
				this.prevboolmodel = new bool[NVars()];
				for (int i = 0; i < NVars(); i++)
				{
					this.prevboolmodel[i] = Decorated().Model(i + 1);
				}
				this.prevfullmodel = Decorated().Model();
				this.prevmodelwithinternalvars = Decorated().ModelWithInternalVariables();
				CalculateObjective();
				return true;
			}
			return ManageUnsatCase();
		}

		protected internal virtual bool ManageUnsatCase()
		{
			if (this.prevfullmodel == null)
			{
				// the problem is UNSAT
				return false;
			}
			// an optimal solution has been found
			// for one criteria
			if (this.currentCriterion < NumberOfCriteria() - 1)
			{
				if (this.prevConstr != null)
				{
					base.RemoveConstr(this.prevConstr);
					this.prevConstr = null;
				}
				try
				{
					FixCriterionValue();
				}
				catch (ContradictionException e)
				{
					throw new InvalidOperationException(e);
				}
				if (IsVerbose())
				{
					System.Console.Out.WriteLine(GetLogPrefix() + "Found optimal criterion number " + (this.currentCriterion + 1));
				}
				this.currentCriterion++;
				CalculateObjective();
				return true;
			}
			if (IsVerbose())
			{
				System.Console.Out.WriteLine(GetLogPrefix() + "Found optimal solution for the last criterion ");
			}
			this.isSolutionOptimal = true;
			if (this.prevConstr != null)
			{
				base.RemoveConstr(this.prevConstr);
				this.prevConstr = null;
			}
			return false;
		}

		public virtual int NumberOfCriteria()
		{
			return this.criteria.Count;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		protected internal virtual void FixCriterionValue()
		{
			base.AddExactly(this.criteria[this.currentCriterion], this.currentValue);
		}

		public override int[] Model()
		{
			return this.prevfullmodel;
		}

		public override bool Model(int var)
		{
			return this.prevboolmodel[var - 1];
		}

		public override int[] ModelWithInternalVariables()
		{
			return this.prevmodelwithinternalvars;
		}

		public virtual bool HasNoObjectiveFunction()
		{
			return false;
		}

		public virtual bool NonOptimalMeansSatisfiable()
		{
			return true;
		}

		public virtual Number CalculateObjective()
		{
			this.currentValue = Evaluate();
			return this.currentValue;
		}

		public virtual Number GetObjectiveValue()
		{
			return this.currentValue;
		}

		public virtual Number GetObjectiveValue(int criterion)
		{
			return Evaluate(criterion);
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual void ForceObjectiveValueTo(Number forcedValue)
		{
			throw new NotSupportedException();
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual void Discard()
		{
			DiscardCurrentSolution();
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual void DiscardCurrentSolution()
		{
			if (this.prevConstr != null)
			{
				base.RemoveSubsumedConstr(this.prevConstr);
			}
			try
			{
				this.prevConstr = DiscardSolutionsForOptimizing();
			}
			catch (ContradictionException c)
			{
				this.prevConstr = null;
				if (!ManageUnsatCase())
				{
					throw;
				}
			}
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		protected internal virtual IConstr DiscardSolutionsForOptimizing()
		{
			return base.AddAtMost(this.criteria[this.currentCriterion], this.currentValue - 1);
		}

		protected internal virtual Number Evaluate()
		{
			return Evaluate(this.currentCriterion);
		}

		protected internal virtual Number Evaluate(int criterion)
		{
			int value = 0;
			int lit;
			for (IteratorInt it = this.criteria[this.currentCriterion].Iterator(); it.HasNext(); )
			{
				lit = it.Next();
				if (lit > 0 && this.prevboolmodel[lit - 1] || lit < 0 && !this.prevboolmodel[-lit - 1])
				{
					value++;
				}
			}
			return value;
		}

		public virtual bool IsOptimal()
		{
			return this.isSolutionOptimal;
		}

		public virtual void SetTimeoutForFindingBetterSolution(int seconds)
		{
			// TODO
			throw new NotSupportedException("No implemented yet");
		}
	}
}
