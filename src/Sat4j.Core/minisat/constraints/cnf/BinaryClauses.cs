using System;
using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Cnf
{
	/// <summary>Concise representation of binary clauses.</summary>
	/// <author>leberre</author>
	[System.Serializable]
	public class BinaryClauses : Constr, Propagatable
	{
		private const long serialVersionUID = 1L;

		private readonly ILits voc;

		private readonly IVecInt clauses = new VecInt();

		private readonly int reason;

		private int conflictindex = -1;

		public BinaryClauses(ILits voc, int p)
		{
			this.voc = voc;
			this.reason = p;
		}

		public virtual void AddBinaryClause(int p)
		{
			clauses.Push(p);
		}

		public virtual void RemoveBinaryClause(int p)
		{
			clauses.Remove(p);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.Constr#remove()
		*/
		public virtual void Remove()
		{
			throw new NotSupportedException();
		}

		/*
		* (non-Javadoc)
		*
		* @see
		* org.sat4j.minisat.Constr#propagate(org.sat4j.minisat.UnitPropagationListener
		* , int)
		*/
		public virtual bool Propagate(UnitPropagationListener s, int p)
		{
			// assert voc.isFalsified(this.reason);
			voc.Watch(p, this);
			for (int i = 0; i < clauses.Size(); i++)
			{
				int q = clauses.Get(i);
				if (!s.Enqueue(q, this))
				{
					conflictindex = i;
					return false;
				}
			}
			return true;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.Constr#simplify()
		*/
		public virtual bool Simplify()
		{
			return false;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.Constr#undo(int)
		*/
		public virtual void Undo(int p)
		{
		}

		// no to do
		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.Constr#calcReason(int, org.sat4j.datatype.VecInt)
		*/
		public virtual void CalcReason(int p, IVecInt outReason)
		{
			outReason.Push(this.reason ^ 1);
			if (p == ILitsConstants.Undefined)
			{
				System.Diagnostics.Debug.Assert(conflictindex > -1);
				outReason.Push(clauses.Get(conflictindex) ^ 1);
			}
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.Constr#learnt()
		*/
		public virtual bool Learnt()
		{
			return false;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.Constr#incActivity(double)
		*/
		public virtual void IncActivity(double claInc)
		{
		}

		// TODO Auto-generated method stub
		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.Constr#getActivity()
		*/
		public virtual double GetActivity()
		{
			// TODO Auto-generated method stub
			return 0;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.Constr#locked()
		*/
		public virtual bool Locked()
		{
			return false;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.Constr#setLearnt()
		*/
		public virtual void SetLearnt()
		{
		}

		// TODO Auto-generated method stub
		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.Constr#register()
		*/
		public virtual void Register()
		{
		}

		// TODO Auto-generated method stub
		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.Constr#rescaleBy(double)
		*/
		public virtual void RescaleBy(double d)
		{
		}

		// TODO Auto-generated method stub
		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.Constr#size()
		*/
		public virtual int Size()
		{
			return clauses.Size();
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.Constr#get(int)
		*/
		public virtual int Get(int i)
		{
			// TODO Auto-generated method stub
			throw new NotSupportedException();
		}

		public virtual void AssertConstraint(UnitPropagationListener s)
		{
			throw new NotSupportedException();
		}

		public virtual bool CanBePropagatedMultipleTimes()
		{
			return true;
		}

		public virtual string ToString(VarMapper mapper)
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual bool PropagatePI(MandatoryLiteralListener l, int p)
		{
			for (int i = 0; i < clauses.Size(); i++)
			{
				l.IsMandatory(clauses.Get(i));
			}
			return true;
		}

		public virtual Constr ToConstraint()
		{
			return this;
		}

		public virtual void Remove(UnitPropagationListener upl)
		{
			throw new NotSupportedException("Cannot remove all the binary clauses at once!");
		}

		// if (voc.watches(reason).contains(this)) {
		// voc.watches(reason).remove(this);
		// }
		public virtual void CalcReasonOnTheFly(int p, IVecInt trail, IVecInt outReason)
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void ForwardActivity(double claInc)
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void SetActivity(double d)
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void AssertConstraintIfNeeded(UnitPropagationListener s)
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual bool CanBeSatisfiedByCountingLiterals()
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual int RequiredNumberOfSatisfiedLiterals()
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual bool IsSatisfied()
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual int GetAssertionLevel(IVecInt trail, int decisionLevel)
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}
	}
}
