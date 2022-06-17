using System;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Org.Sat4j.Tools;
using Sharpen;

namespace Org.Sat4j.Opt
{
	/// <summary>Computes a solution with the smallest number of satisfied literals.</summary>
	/// <remarks>
	/// Computes a solution with the smallest number of satisfied literals.
	/// Please make sure that newVar(howmany) is called first to setup the decorator.
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class MinOneDecorator : SolverDecorator<ISolver>, IOptimizationProblem
	{
		private const long serialVersionUID = 1L;

		private int[] prevmodel;

		private int[] prevmodelWithInternalVariables;

		private bool isSolutionOptimal;

		public MinOneDecorator(ISolver solver)
			: base(solver)
		{
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public bool AdmitABetterSolution()
		{
			return AdmitABetterSolution(VecInt.Empty);
		}

		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public bool AdmitABetterSolution(IVecInt assumps)
		{
			this.isSolutionOptimal = false;
			bool result = IsSatisfiable(assumps, true);
			if (result)
			{
				this.prevmodel = base.Model();
				this.prevmodelWithInternalVariables = base.ModelWithInternalVariables();
				CalculateObjectiveValue();
			}
			else
			{
				this.isSolutionOptimal = true;
			}
			return result;
		}

		public bool HasNoObjectiveFunction()
		{
			return false;
		}

		public bool NonOptimalMeansSatisfiable()
		{
			return true;
		}

		private int counter;

		public Number CalculateObjective()
		{
			CalculateObjectiveValue();
			return this.counter;
		}

		private void CalculateObjectiveValue()
		{
			this.counter = 0;
			foreach (int p in this.prevmodel)
			{
				if (p > 0)
				{
					this.counter++;
				}
			}
		}

		private readonly IVecInt literals = new VecInt();

		private IConstr previousConstr;

		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public void DiscardCurrentSolution()
		{
			if (this.literals.IsEmpty())
			{
				for (int i = 1; i <= NVars(); i++)
				{
					this.literals.Push(i);
				}
			}
			if (this.previousConstr != null)
			{
				base.RemoveConstr(this.previousConstr);
			}
			this.previousConstr = AddAtMost(this.literals, this.counter - 1);
		}

		public override int[] Model()
		{
			// DLB findbugs ok
			return this.prevmodel;
		}

		public override int[] ModelWithInternalVariables()
		{
			return this.prevmodelWithInternalVariables;
		}

		public override void Reset()
		{
			this.literals.Clear();
			this.previousConstr = null;
			base.Reset();
		}

		/// <since>2.1</since>
		public Number GetObjectiveValue()
		{
			return this.counter;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public void Discard()
		{
			DiscardCurrentSolution();
		}

		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public void ForceObjectiveValueTo(Number forcedValue)
		{
			try
			{
				AddAtMost(this.literals, forcedValue);
			}
			catch (ContradictionException ce)
			{
				this.isSolutionOptimal = true;
				throw;
			}
		}

		public bool IsOptimal()
		{
			return this.isSolutionOptimal;
		}

		public void SetTimeoutForFindingBetterSolution(int seconds)
		{
			// TODO
			throw new NotSupportedException("No implemented yet");
		}
	}
}
