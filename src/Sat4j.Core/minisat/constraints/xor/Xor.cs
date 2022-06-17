using System;
using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Xor
{
	/// <summary>
	/// A simple implementation of a xor constraint to be handled in Sat4j CDCL
	/// solver.
	/// </summary>
	/// <remarks>
	/// A simple implementation of a xor constraint to be handled in Sat4j CDCL
	/// solver.
	/// As for each constraint in the solver, the propagation and conflict analysis
	/// is performed locally for each constraint, not globally like in the SMT
	/// framework.
	/// As such, the constraint uses an extended form of watched literals, basic
	/// analysis in case of conflicts (it will typically return a clause of the CNF
	/// encoding), so think about that implementation as a lazy clause generation of
	/// the full CNF encoding.
	/// The normalized for of the constraint is:
	/// v1 xor v2 xor v3 xor ... xor vn = (true | false)
	/// where v1 are dimacs positive literals (using Sat4j internal representation)
	/// if rhs is false, then an even number of literals must be satisfied, else an
	/// odd number of literals must be satisfied (thus the name parity constraints).
	/// </remarks>
	/// <author>leberre</author>
	/// <since>2.3.6</since>
	public class Xor : Constr, Propagatable
	{
		private readonly int[] lits;

		private readonly bool parity;

		private readonly ILits voc;

		public static Org.Sat4j.Minisat.Constraints.Xor.Xor CreateParityConstraint(IVecInt lits, bool parity, ILits voc)
		{
			// TODO ensure normal form
			Org.Sat4j.Minisat.Constraints.Xor.Xor xor = new Org.Sat4j.Minisat.Constraints.Xor.Xor(lits, parity, voc);
			xor.Register();
			return xor;
		}

		public Xor(IVecInt lits, bool parity, ILits voc)
		{
			this.lits = new int[lits.Size()];
			lits.CopyTo(this.lits);
			this.parity = parity;
			this.voc = voc;
		}

		public virtual bool Learnt()
		{
			return false;
		}

		public virtual int Size()
		{
			return lits.Length;
		}

		public virtual int Get(int i)
		{
			return lits[i];
		}

		public virtual double GetActivity()
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual bool CanBePropagatedMultipleTimes()
		{
			return false;
		}

		public virtual string ToString(VarMapper mapper)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual bool Propagate(UnitPropagationListener s, int p)
		{
			// we use the same trick as for clauses: we move the variables inside
			// the constraint
			// to keep the two doubly watched literals in front of the constraints
			int tmp;
			int nbSatisfied = 0;
			if (p == lits[0] || LiteralsUtils.Neg(p) == lits[0])
			{
				tmp = lits[1];
				lits[1] = lits[0];
				lits[0] = tmp;
			}
			if (this.voc.IsSatisfied(lits[1]))
			{
				nbSatisfied = 1;
			}
			// look for new literal to watch and counting satisfied literals
			for (int i = 2; i < lits.Length; i++)
			{
				if (this.voc.IsSatisfied(lits[i]))
				{
					nbSatisfied++;
				}
				else
				{
					if (this.voc.IsUnassigned(lits[i]))
					{
						tmp = lits[1];
						lits[1] = lits[i];
						lits[i] = tmp;
						this.voc.Watch(lits[1] ^ 1, this);
						this.voc.Watch(lits[1], this);
						this.voc.Watches(p ^ 1).Remove(this);
						return true;
					}
				}
			}
			this.voc.Watch(p, this);
			// propagates first watched literal, depending on the number of
			// satisfied literals
			int toPropagate = ((nbSatisfied & 1) == 1) == parity ? lits[0] : LiteralsUtils.Neg(lits[0]);
			return s.Enqueue(toPropagate, this);
		}

		public virtual bool PropagatePI(MandatoryLiteralListener l, int p)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual Constr ToConstraint()
		{
			return this;
		}

		public virtual void Remove(UnitPropagationListener upl)
		{
			this.voc.Watches(this.lits[0]).Remove(this);
			this.voc.Watches(this.lits[0] ^ 1).Remove(this);
			this.voc.Watches(this.lits[1]).Remove(this);
			this.voc.Watches(this.lits[1] ^ 1).Remove(this);
		}

		public virtual bool Simplify()
		{
			return false;
		}

		public virtual void CalcReason(int p, IVecInt outReason)
		{
			for (int i = p == ILitsConstants.Undefined ? 0 : 1; i < lits.Length; i++)
			{
				if (this.voc.IsFalsified(lits[i]))
				{
					outReason.Push(lits[i] ^ 1);
				}
				else
				{
					System.Diagnostics.Debug.Assert(this.voc.IsSatisfied(lits[i]));
					outReason.Push(lits[i]);
				}
			}
		}

		public virtual void CalcReasonOnTheFly(int p, IVecInt trail, IVecInt outReason)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void IncActivity(double claInc)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void ForwardActivity(double claInc)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual bool Locked()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void SetLearnt()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void Register()
		{
			this.voc.Watch(this.lits[0], this);
			this.voc.Watch(this.lits[0] ^ 1, this);
			this.voc.Watch(this.lits[1], this);
			this.voc.Watch(this.lits[1] ^ 1, this);
		}

		public virtual void RescaleBy(double d)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void SetActivity(double d)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void AssertConstraint(UnitPropagationListener s)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void AssertConstraintIfNeeded(UnitPropagationListener s)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual bool CanBeSatisfiedByCountingLiterals()
		{
			return true;
		}

		public virtual int RequiredNumberOfSatisfiedLiterals()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual bool IsSatisfied()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual int GetAssertionLevel(IVecInt trail, int decisionLevel)
		{
			throw new NotSupportedException("Not implemented yet!");
		}
	}
}
