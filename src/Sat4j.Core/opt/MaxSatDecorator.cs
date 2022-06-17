using System;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Opt
{
	/// <summary>Computes a solution that satisfies the maximum of clauses.</summary>
	/// <author>daniel</author>
	[System.Serializable]
	public sealed class MaxSatDecorator : AbstractSelectorVariablesDecorator
	{
		private const long serialVersionUID = 1L;

		private readonly bool equivalence;

		public MaxSatDecorator(ISolver solver)
			: this(solver, false)
		{
		}

		public MaxSatDecorator(ISolver solver, bool equivalence)
			: base(solver)
		{
			this.equivalence = equivalence;
		}

		public override void SetExpectedNumberOfClauses(int nb)
		{
			base.SetExpectedNumberOfClauses(nb);
			this.lits.Ensure(nb);
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override IConstr AddClause(IVecInt literals)
		{
			int newvar = NextFreeVarId(true);
			this.lits.Push(newvar);
			literals.Push(newvar);
			if (this.equivalence)
			{
				ConstrGroup constrs = new ConstrGroup();
				constrs.Add(base.AddClause(literals));
				IVecInt clause = new VecInt(2);
				clause.Push(-newvar);
				for (int i = 0; i < literals.Size() - 1; i++)
				{
					clause.Push(-literals.Get(i));
					constrs.Add(base.AddClause(clause));
				}
				clause.Pop();
				return constrs;
			}
			return base.AddClause(literals);
		}

		public override void Reset()
		{
			this.lits.Clear();
			base.Reset();
			this.prevConstr = null;
		}

		public override bool HasNoObjectiveFunction()
		{
			return false;
		}

		public override bool NonOptimalMeansSatisfiable()
		{
			return false;
		}

		public override Number CalculateObjective()
		{
			CalculateObjectiveValue();
			return this.counter;
		}

		private readonly IVecInt lits = new VecInt();

		private int counter;

		private IConstr prevConstr;

		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override void DiscardCurrentSolution()
		{
			if (this.prevConstr != null)
			{
				base.RemoveSubsumedConstr(this.prevConstr);
			}
			try
			{
				this.prevConstr = base.AddAtMost(this.lits, this.counter - 1);
			}
			catch (ContradictionException ce)
			{
				SetSolutionOptimal(true);
				throw;
			}
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override bool AdmitABetterSolution(IVecInt assumps)
		{
			bool result = base.AdmitABetterSolution(assumps);
			if (!result && this.prevConstr != null)
			{
				base.RemoveConstr(this.prevConstr);
				this.prevConstr = null;
			}
			return result;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override void Discard()
		{
			DiscardCurrentSolution();
		}

		/// <since>2.1</since>
		public override Number GetObjectiveValue()
		{
			return this.counter;
		}

		internal override void CalculateObjectiveValue()
		{
			this.counter = 0;
			foreach (int q in GetPrevfullmodel())
			{
				if (q > NVars())
				{
					this.counter++;
				}
			}
		}

		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override void ForceObjectiveValueTo(Number forcedValue)
		{
			base.AddAtMost(this.lits, forcedValue);
		}

		public override void SetTimeoutForFindingBetterSolution(int seconds)
		{
			// TODO
			throw new NotSupportedException("No implemented yet");
		}
	}
}
