using System;
using System.IO;
using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	/// <summary>Static ordering of the decisions based on the natural order of the variables.</summary>
	/// <remarks>
	/// Static ordering of the decisions based on the natural order of the variables.
	/// It is not meant to be efficient but to allow to easily study the behavior of
	/// the solver on a known order of the decisions.
	/// </remarks>
	/// <author>leberre</author>
	public class NaturalStaticOrder : IOrder
	{
		private ILits voc;

		private IPhaseSelectionStrategy phaseSelectionStrategy = new NegativeLiteralSelectionStrategy();

		internal int index;

		public virtual void SetLits(ILits lits)
		{
			this.voc = lits;
		}

		public virtual int Select()
		{
			while (!voc.IsUnassigned(LiteralsUtils.PosLit(index)) || !voc.BelongsToPool(index))
			{
				index++;
				if (index > voc.NVars())
				{
					return ILitsConstants.Undefined;
				}
			}
			return phaseSelectionStrategy.Select(index);
		}

		public virtual void Undo(int x)
		{
			index = Math.Min(index, x);
		}

		public virtual void UpdateVar(int p)
		{
		}

		public virtual void Init()
		{
			index = 1;
		}

		public virtual void PrintStat(PrintWriter @out, string prefix)
		{
		}

		public virtual void SetVarDecay(double d)
		{
		}

		public virtual void VarDecayActivity()
		{
		}

		public virtual double VarActivity(int p)
		{
			return 0.0d;
		}

		public virtual void AssignLiteral(int p)
		{
		}

		public virtual void SetPhaseSelectionStrategy(IPhaseSelectionStrategy strategy)
		{
			this.phaseSelectionStrategy = strategy;
		}

		public virtual IPhaseSelectionStrategy GetPhaseSelectionStrategy()
		{
			return this.phaseSelectionStrategy;
		}

		public virtual void UpdateVarAtDecisionLevel(int q)
		{
		}

		public virtual double[] GetVariableHeuristics()
		{
			return new double[0];
		}

		public override string ToString()
		{
			return "Natural static ordering";
		}
	}
}
