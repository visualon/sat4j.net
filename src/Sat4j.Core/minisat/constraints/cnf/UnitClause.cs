using System;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Cnf
{
	/// <author>daniel</author>
	/// <since>2.1</since>
	public class UnitClause : Constr
	{
		protected internal readonly int literal;

		protected internal double activity;

		public UnitClause(int value)
		{
			this.literal = value;
		}

		public virtual void AssertConstraint(UnitPropagationListener s)
		{
			s.Enqueue(this.literal, this);
		}

		public virtual void AssertConstraintIfNeeded(UnitPropagationListener s)
		{
			AssertConstraint(s);
		}

		public virtual void CalcReason(int p, IVecInt outReason)
		{
			if (p == ILitsConstants.Undefined)
			{
				outReason.Push(LiteralsUtils.Neg(this.literal));
			}
		}

		public virtual double GetActivity()
		{
			return activity;
		}

		public virtual void IncActivity(double claInc)
		{
		}

		// silent to prevent problems with xplain trick.
		public virtual void SetActivity(double claInc)
		{
			activity = claInc;
		}

		public virtual bool Locked()
		{
			throw new NotSupportedException();
		}

		public virtual void Register()
		{
		}

		public virtual void Remove(UnitPropagationListener upl)
		{
			upl.Unset(this.literal);
		}

		public virtual void RescaleBy(double d)
		{
			throw new NotSupportedException();
		}

		public virtual void SetLearnt()
		{
			throw new NotSupportedException();
		}

		public virtual bool Simplify()
		{
			return false;
		}

		public virtual bool Propagate(UnitPropagationListener s, int p)
		{
			throw new NotSupportedException();
		}

		public virtual int Get(int i)
		{
			if (i > 0)
			{
				throw new ArgumentException();
			}
			return this.literal;
		}

		public virtual bool Learnt()
		{
			return false;
		}

		public virtual int Size()
		{
			return 1;
		}

		public virtual void ForwardActivity(double claInc)
		{
		}

		// silent to prevent problems with xplain trick.
		public override string ToString()
		{
			return Lits.ToString(this.literal);
		}

		public virtual bool CanBePropagatedMultipleTimes()
		{
			return false;
		}

		public virtual void CalcReasonOnTheFly(int p, IVecInt trail, IVecInt outReason)
		{
			CalcReason(p, outReason);
		}

		public virtual void PropagatePi(MandatoryLiteralListener m)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual bool CanBeSatisfiedByCountingLiterals()
		{
			return true;
		}

		public virtual int RequiredNumberOfSatisfiedLiterals()
		{
			return 1;
		}

		public virtual bool IsSatisfied()
		{
			return true;
		}

		public virtual int GetAssertionLevel(IVecInt trail, int decisionLevel)
		{
			return 0;
		}

		public virtual string ToString(VarMapper mapper)
		{
			if (mapper == null)
			{
				return ToString();
			}
			return mapper.Map(LiteralsUtils.ToDimacs(this.literal));
		}
	}
}
