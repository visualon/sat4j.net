using System;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Cnf
{
	/// <since>2.1</since>
	public class UnitClauses : Constr
	{
		protected internal readonly int[] literals;

		public UnitClauses(IVecInt values)
		{
			this.literals = new int[values.Size()];
			values.CopyTo(this.literals);
		}

		public virtual void AssertConstraint(UnitPropagationListener s)
		{
			foreach (int p in this.literals)
			{
				s.Enqueue(p, this);
			}
		}

		public virtual void AssertConstraintIfNeeded(UnitPropagationListener s)
		{
			AssertConstraint(s);
		}

		public virtual void CalcReason(int p, IVecInt outReason)
		{
			throw new NotSupportedException();
		}

		public virtual double GetActivity()
		{
			throw new NotSupportedException();
		}

		public virtual void IncActivity(double claInc)
		{
		}

		// silent to prevent problems with xplain trick.
		public virtual void SetActivity(double claInc)
		{
		}

		// do nothing
		public virtual bool Locked()
		{
			throw new NotSupportedException();
		}

		public virtual void Register()
		{
			throw new NotSupportedException();
		}

		public virtual void Remove(UnitPropagationListener upl)
		{
			for (int i = this.literals.Length - 1; i >= 0; i--)
			{
				upl.Unset(this.literals[i]);
			}
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
			throw new NotSupportedException();
		}

		public virtual bool Learnt()
		{
			throw new NotSupportedException();
		}

		public virtual int Size()
		{
			throw new NotSupportedException();
		}

		public virtual void ForwardActivity(double claInc)
		{
		}

		// silent to prevent problems with xplain trick.
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
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual int RequiredNumberOfSatisfiedLiterals()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual bool IsSatisfied()
		{
			return true;
		}

		public virtual int GetAssertionLevel(IVecInt trail, int decisionLevel)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual string ToString(VarMapper mapper)
		{
			throw new NotSupportedException("Not implemented yet!");
		}
	}
}
