using System;
using System.IO;
using Org.Sat4j.Minisat.Core;
using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	/// <since>2.2</since>
	[System.Serializable]
	public class RandomWalkDecorator : IOrder
	{
		private const long serialVersionUID = 1L;

		private readonly VarOrderHeap decorated;

		private double p;

		private static readonly Random Rand = new Random(123456789);

		private ILits voc;

		private int nbRandomWalks;

		public RandomWalkDecorator(VarOrderHeap order)
			: this(order, 0.01)
		{
		}

		public RandomWalkDecorator(VarOrderHeap order, double p)
		{
			this.decorated = order;
			this.p = p;
		}

		public virtual void AssignLiteral(int q)
		{
			this.decorated.AssignLiteral(q);
		}

		public virtual IPhaseSelectionStrategy GetPhaseSelectionStrategy()
		{
			return this.decorated.GetPhaseSelectionStrategy();
		}

		public virtual double GetProbability()
		{
			return this.p;
		}

		public virtual void SetProbability(double p)
		{
			this.p = p;
		}

		public virtual void Init()
		{
			this.decorated.Init();
		}

		public virtual void PrintStat(PrintWriter @out, string prefix)
		{
			@out.WriteLine(prefix + "random walks\t: " + this.nbRandomWalks);
			this.decorated.PrintStat(@out, prefix);
		}

		public virtual int Select()
		{
			if (Rand.NextDouble() < this.p)
			{
				int var;
				int lit;
				int max;
				while (!this.decorated.heap.Empty())
				{
					max = this.decorated.heap.Size();
					var = this.decorated.heap.Get(Rand.Next(max) + 1);
					lit = GetPhaseSelectionStrategy().Select(var);
					if (this.voc.IsUnassigned(lit))
					{
						this.nbRandomWalks++;
						return lit;
					}
				}
			}
			return this.decorated.Select();
		}

		public virtual void SetLits(ILits lits)
		{
			this.decorated.SetLits(lits);
			this.voc = lits;
			this.nbRandomWalks = 0;
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
			this.decorated.Undo(x);
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
			return this.decorated.ToString() + " with random walks " + this.p;
		}

		public virtual double[] GetVariableHeuristics()
		{
			return this.decorated.GetVariableHeuristics();
		}
	}
}
