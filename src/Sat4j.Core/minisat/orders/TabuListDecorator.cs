using System.Collections.Generic;
using System.IO;
using Org.Sat4j.Minisat.Core;
using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	/// <summary>Uses a tabu list to prevent the solver to</summary>
	/// <since>2.3.2</since>
	public class TabuListDecorator : IOrder
	{
		private readonly VarOrderHeap decorated;

		private readonly int tabuSize;

		private ILits voc;

		private int lastVar = -1;

		private readonly List<int> tabuList = new List<int>();

		public TabuListDecorator(VarOrderHeap order)
			: this(order, 10)
		{
		}

		public TabuListDecorator(VarOrderHeap order, int tabuSize)
		{
			this.decorated = order;
			this.tabuSize = tabuSize;
		}

		public virtual void AssignLiteral(int q)
		{
			this.decorated.AssignLiteral(q);
		}

		public virtual IPhaseSelectionStrategy GetPhaseSelectionStrategy()
		{
			return this.decorated.GetPhaseSelectionStrategy();
		}

		public virtual void Init()
		{
			this.decorated.Init();
			this.lastVar = -1;
		}

		public virtual void PrintStat(PrintWriter @out, string prefix)
		{
			@out.WriteLine(prefix + "tabu list size\t: " + this.tabuSize);
			this.decorated.PrintStat(@out, prefix);
		}

		public virtual int Select()
		{
			int lit = this.decorated.Select();
			if (lit == ILitsConstants.Undefined)
			{
				int var;
				do
				{
					if (this.tabuList.IsEmpty())
					{
						return ILitsConstants.Undefined;
					}
					var = this.tabuList.RemoveFirst();
				}
				while (!this.voc.IsUnassigned(var << 1));
				return GetPhaseSelectionStrategy().Select(var);
			}
			this.lastVar = lit >> 1;
			return lit;
		}

		public virtual void SetLits(ILits lits)
		{
			this.decorated.SetLits(lits);
			this.voc = lits;
		}

		public virtual void SetPhaseSelectionStrategy(IPhaseSelectionStrategy strategy)
		{
			this.decorated.SetPhaseSelectionStrategy(strategy);
		}

		public virtual void SetVarDecay(double d)
		{
			this.decorated.SetVarDecay(d);
		}

		public virtual void Undo(int x)
		{
			if (this.tabuList.Count == this.tabuSize)
			{
				int var = this.tabuList.RemoveFirst();
				this.decorated.Undo(var);
			}
			if (x == this.lastVar)
			{
				this.tabuList.Add(x);
				this.lastVar = -1;
			}
			else
			{
				this.decorated.Undo(x);
			}
		}

		public virtual void UpdateVar(int q)
		{
			this.decorated.UpdateVar(q);
		}

		public virtual double VarActivity(int q)
		{
			return this.decorated.VarActivity(q);
		}

		public virtual void VarDecayActivity()
		{
			this.decorated.VarDecayActivity();
		}

		public virtual void UpdateVarAtDecisionLevel(int q)
		{
			this.decorated.UpdateVarAtDecisionLevel(q);
		}

		public override string ToString()
		{
			return this.decorated.ToString() + " with tabu list of size " + this.tabuSize;
		}

		public virtual double[] GetVariableHeuristics()
		{
			return this.decorated.GetVariableHeuristics();
		}
	}
}
