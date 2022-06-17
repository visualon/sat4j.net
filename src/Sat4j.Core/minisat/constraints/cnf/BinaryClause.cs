using System;
using System.Text;
using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Cnf
{
	/// <summary>Data structure for binary clause.</summary>
	/// <author>leberre</author>
	/// <since>2.1</since>
	[System.Serializable]
	public abstract class BinaryClause : Propagatable, Constr
	{
		private const long serialVersionUID = 1L;

		protected internal double activity;

		private readonly ILits voc;

		protected internal int head;

		protected internal int tail;

		/// <summary>Creates a new basic clause</summary>
		/// <param name="voc">the vocabulary of the formula</param>
		/// <param name="ps">A VecInt that WILL BE EMPTY after calling that method.</param>
		public BinaryClause(IVecInt ps, ILits voc)
		{
			System.Diagnostics.Debug.Assert(ps.Size() == 2);
			this.head = ps.Get(0);
			this.tail = ps.Get(1);
			this.voc = voc;
			this.activity = 0;
		}

		/*
		* (non-Javadoc)
		*
		* @see Constr#calcReason(Solver, Lit, Vec)
		*/
		public virtual void CalcReason(int p, IVecInt outReason)
		{
			if (this.voc.IsFalsified(this.head))
			{
				outReason.Push(LiteralsUtils.Neg(this.head));
			}
			if (this.voc.IsFalsified(this.tail))
			{
				outReason.Push(LiteralsUtils.Neg(this.tail));
			}
		}

		/*
		* (non-Javadoc)
		*
		* @see Constr#remove(Solver)
		*/
		public virtual void Remove(UnitPropagationListener upl)
		{
			this.voc.Watches(LiteralsUtils.Neg(this.head)).Remove(this);
			this.voc.Watches(LiteralsUtils.Neg(this.tail)).Remove(this);
		}

		/*
		* (non-Javadoc)
		*
		* @see Constr#simplify(Solver)
		*/
		public virtual bool Simplify()
		{
			if (this.voc.IsSatisfied(this.head) || this.voc.IsSatisfied(this.tail))
			{
				return true;
			}
			return false;
		}

		public virtual bool Propagate(UnitPropagationListener s, int p)
		{
			this.voc.Watch(p, this);
			if (this.head == LiteralsUtils.Neg(p))
			{
				return s.Enqueue(this.tail, this);
			}
			System.Diagnostics.Debug.Assert(this.tail == LiteralsUtils.Neg(p));
			return s.Enqueue(this.head, this);
		}

		public virtual bool PropagatePI(MandatoryLiteralListener m, int p)
		{
			this.voc.Watch(p, this);
			if (this.head == LiteralsUtils.Neg(p))
			{
				m.IsMandatory(this.tail);
				return true;
			}
			System.Diagnostics.Debug.Assert(this.tail == LiteralsUtils.Neg(p));
			m.IsMandatory(this.head);
			return true;
		}

		/*
		* For learnt clauses only @author leberre
		*/
		public virtual bool Locked()
		{
			return this.voc.GetReason(this.head) == this || this.voc.GetReason(this.tail) == this;
		}

		/// <returns>the activity of the clause</returns>
		public virtual double GetActivity()
		{
			return this.activity;
		}

		public override string ToString()
		{
			StringBuilder stb = new StringBuilder();
			stb.Append(Lits.ToString(this.head));
			stb.Append("[");
			//$NON-NLS-1$
			stb.Append(this.voc.ValueToString(this.head));
			stb.Append("]");
			//$NON-NLS-1$
			stb.Append(" ");
			//$NON-NLS-1$
			stb.Append(Lits.ToString(this.tail));
			stb.Append("[");
			//$NON-NLS-1$
			stb.Append(this.voc.ValueToString(this.tail));
			stb.Append("]");
			//$NON-NLS-1$
			return stb.ToString();
		}

		/// <summary>Retourne le ieme literal de la clause.</summary>
		/// <remarks>
		/// Retourne le ieme literal de la clause. Attention, cet ordre change durant
		/// la recherche.
		/// </remarks>
		/// <param name="i">the index of the literal</param>
		/// <returns>the literal</returns>
		public virtual int Get(int i)
		{
			if (i == 0)
			{
				return this.head;
			}
			System.Diagnostics.Debug.Assert(i == 1);
			return this.tail;
		}

		/// <param name="d"/>
		public virtual void RescaleBy(double d)
		{
			this.activity *= d;
		}

		public virtual int Size()
		{
			return 2;
		}

		public virtual void AssertConstraint(UnitPropagationListener s)
		{
			// assert this.voc.isUnassigned(this.head);
			bool ret = s.Enqueue(this.head, this);
			System.Diagnostics.Debug.Assert(ret);
		}

		public virtual void AssertConstraintIfNeeded(UnitPropagationListener s)
		{
			if (voc.IsFalsified(this.tail))
			{
				bool ret = s.Enqueue(this.head, this);
				System.Diagnostics.Debug.Assert(ret);
			}
		}

		public virtual ILits GetVocabulary()
		{
			return this.voc;
		}

		public virtual int[] GetLits()
		{
			int[] tmp = new int[2];
			tmp[0] = this.head;
			tmp[1] = this.tail;
			return tmp;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (this.GetType() != obj.GetType())
			{
				return false;
			}
			try
			{
				Org.Sat4j.Minisat.Constraints.Cnf.BinaryClause wcl = (Org.Sat4j.Minisat.Constraints.Cnf.BinaryClause)obj;
				if (wcl.head != this.head || wcl.tail != this.tail)
				{
					return false;
				}
				return true;
			}
			catch (InvalidCastException)
			{
				return false;
			}
		}

		public override int GetHashCode()
		{
			long sum = (long)this.head + this.tail;
			return (int)sum / 2;
		}

		public virtual void Register()
		{
			this.voc.Watch(LiteralsUtils.Neg(this.head), this);
			this.voc.Watch(LiteralsUtils.Neg(this.tail), this);
		}

		public virtual bool CanBePropagatedMultipleTimes()
		{
			return false;
		}

		public virtual Constr ToConstraint()
		{
			return this;
		}

		public virtual void CalcReasonOnTheFly(int p, IVecInt trail, IVecInt outReason)
		{
			CalcReason(p, outReason);
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
			if (voc.IsSatisfied(this.head))
			{
				return true;
			}
			if (voc.IsSatisfied(this.tail))
			{
				return true;
			}
			return false;
		}

		public virtual int GetAssertionLevel(IVecInt trail, int decisionLevel)
		{
			for (int i = trail.Size() - 1; i >= 0; i--)
			{
				if (LiteralsUtils.Var(trail.Get(i)) == LiteralsUtils.Var(this.head))
				{
					return i;
				}
			}
			return -1;
		}

		public virtual string ToString(VarMapper mapper)
		{
			if (mapper == null)
			{
				return ToString();
			}
			StringBuilder stb = new StringBuilder();
			stb.Append(mapper.Map(LiteralsUtils.ToDimacs(this.head)));
			stb.Append("[");
			//$NON-NLS-1$
			stb.Append(this.voc.ValueToString(this.head));
			stb.Append("]");
			//$NON-NLS-1$
			stb.Append(" ");
			//$NON-NLS-1$
			stb.Append(mapper.Map(LiteralsUtils.ToDimacs(this.tail)));
			stb.Append("[");
			//$NON-NLS-1$
			stb.Append(this.voc.ValueToString(this.tail));
			stb.Append("]");
			//$NON-NLS-1$
			return stb.ToString();
		}

		public abstract bool Learnt();

		public abstract void ForwardActivity(double arg1);

		public abstract void IncActivity(double arg1);

		public abstract void SetActivity(double arg1);

		public abstract void SetLearnt();
	}
}
