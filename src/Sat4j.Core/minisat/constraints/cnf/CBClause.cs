using System;
using System.Text;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Cnf
{
	/// <author>leberre</author>
	[System.Serializable]
	public class CBClause : Constr, Undoable, Propagatable
	{
		private const long serialVersionUID = 1L;

		protected internal int falsified;

		private bool learnt;

		protected internal readonly int[] lits;

		protected internal readonly ILits voc;

		private double activity;

		public static Org.Sat4j.Minisat.Constraints.Cnf.CBClause BrandNewClause(UnitPropagationListener s, ILits voc, IVecInt literals)
		{
			Org.Sat4j.Minisat.Constraints.Cnf.CBClause c = new Org.Sat4j.Minisat.Constraints.Cnf.CBClause(literals, voc);
			c.Register();
			return c;
		}

		public CBClause(IVecInt ps, ILits voc, bool learnt)
		{
			this.learnt = learnt;
			this.lits = new int[ps.Size()];
			this.voc = voc;
			ps.MoveTo(this.lits);
		}

		public CBClause(IVecInt ps, ILits voc)
			: this(ps, voc, false)
		{
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.Constr#remove()
		*/
		public virtual void Remove()
		{
			for (int i = 0; i < lits.Length; i++)
			{
				voc.Watches(lits[i] ^ 1).Remove(this);
			}
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.Constr#propagate(org.sat4j.minisat.core.
		* UnitPropagationListener, int)
		*/
		public virtual bool Propagate(UnitPropagationListener s, int p)
		{
			voc.Undos(p).Push(this);
			falsified++;
			System.Diagnostics.Debug.Assert(falsified != lits.Length);
			if (falsified == lits.Length - 1)
			{
				// find unassigned literal
				for (int i = 0; i < lits.Length; i++)
				{
					if (!voc.IsFalsified(lits[i]))
					{
						return s.Enqueue(lits[i], this);
					}
				}
				// one of the variable in to be propagated to false.
				// (which explains why the falsified counter has not
				// been increased yet)
				return false;
			}
			return true;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.Constr#simplify()
		*/
		public virtual bool Simplify()
		{
			foreach (int p in lits)
			{
				if (voc.IsSatisfied(p))
				{
					return true;
				}
			}
			return false;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.Constr#undo(int)
		*/
		public virtual void Undo(int p)
		{
			falsified--;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.Constr#calcReason(int,
		* org.sat4j.specs.VecInt)
		*/
		public virtual void CalcReason(int p, IVecInt outReason)
		{
			System.Diagnostics.Debug.Assert(outReason.Size() == 0);
			foreach (int q in lits)
			{
				System.Diagnostics.Debug.Assert(voc.IsFalsified(q) || q == p);
				if (voc.IsFalsified(q))
				{
					outReason.Push(q ^ 1);
				}
			}
			System.Diagnostics.Debug.Assert((p == ILitsConstants.Undefined) || (outReason.Size() == lits.Length - 1));
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.Constr#learnt()
		*/
		public virtual bool Learnt()
		{
			return learnt;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.Constr#incActivity(double)
		*/
		public virtual void IncActivity(double claInc)
		{
			activity += claInc;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.Constr#getActivity()
		*/
		public virtual double GetActivity()
		{
			return activity;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.Constr#locked()
		*/
		public virtual bool Locked()
		{
			return voc.GetReason(lits[0]) == this;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.Constr#setLearnt()
		*/
		public virtual void SetLearnt()
		{
			learnt = true;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.Constr#register()
		*/
		public virtual void Register()
		{
			foreach (int p in lits)
			{
				voc.Watch(p ^ 1, this);
			}
			if (learnt)
			{
				foreach (int p_1 in lits)
				{
					if (voc.IsFalsified(p_1))
					{
						voc.Undos(p_1 ^ 1).Push(this);
						falsified++;
					}
				}
				System.Diagnostics.Debug.Assert(falsified == lits.Length - 1);
			}
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.Constr#rescaleBy(double)
		*/
		public virtual void RescaleBy(double d)
		{
			activity *= d;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.Constr#size()
		*/
		public virtual int Size()
		{
			return lits.Length;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.Constr#get(int)
		*/
		public virtual int Get(int i)
		{
			return lits[i];
		}

		/*
		* (non-Javadoc)
		*
		* @see
		* org.sat4j.minisat.core.Constr#assertConstraint(org.sat4j.minisat.core
		* .UnitPropagationListener)
		*/
		public virtual void AssertConstraint(UnitPropagationListener s)
		{
			System.Diagnostics.Debug.Assert(voc.IsUnassigned(lits[0]));
			bool ret = s.Enqueue(lits[0], this);
			System.Diagnostics.Debug.Assert(ret);
		}

		public override string ToString()
		{
			StringBuilder stb = new StringBuilder();
			for (int i = 0; i < lits.Length; i++)
			{
				stb.Append(lits[i]);
				stb.Append("[");
				//$NON-NLS-1$
				stb.Append(voc.ValueToString(lits[i]));
				stb.Append("]");
				//$NON-NLS-1$
				stb.Append(" ");
			}
			//$NON-NLS-1$
			return stb.ToString();
		}

		public virtual bool CanBePropagatedMultipleTimes()
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual string ToString(VarMapper mapper)
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void Remove(UnitPropagationListener upl)
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

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

		public virtual bool PropagatePI(MandatoryLiteralListener l, int p)
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual Constr ToConstraint()
		{
			return this;
		}
	}
}
