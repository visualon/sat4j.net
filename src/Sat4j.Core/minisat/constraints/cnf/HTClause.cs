using System;
using System.Text;
using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Cnf
{
	/// <summary>
	/// Lazy data structure for clause using the Head Tail data structure from SATO,
	/// The original scheme is improved by avoiding moving pointers to literals but
	/// moving the literals themselves.
	/// </summary>
	/// <remarks>
	/// Lazy data structure for clause using the Head Tail data structure from SATO,
	/// The original scheme is improved by avoiding moving pointers to literals but
	/// moving the literals themselves.
	/// We suppose here that the clause contains at least 3 literals. Use the
	/// BinaryClause or UnaryClause clause data structures to deal with binary and
	/// unit clauses.
	/// </remarks>
	/// <author>leberre</author>
	/// <seealso cref="BinaryClause"/>
	/// <seealso cref="UnitClause"/>
	/// <since>2.1</since>
	[System.Serializable]
	public abstract class HTClause : Propagatable, Constr
	{
		private const long serialVersionUID = 1L;

		protected internal double activity;

		protected internal readonly int[] middleLits;

		protected internal readonly ILits voc;

		protected internal int head;

		protected internal int tail;

		/// <summary>Creates a new basic clause</summary>
		/// <param name="voc">the vocabulary of the formula</param>
		/// <param name="ps">A VecInt that WILL BE EMPTY after calling that method.</param>
		public HTClause(IVecInt ps, ILits voc)
		{
			System.Diagnostics.Debug.Assert(ps.Size() > 1);
			this.head = ps.Get(0);
			this.tail = ps.Last();
			int size = ps.Size() - 2;
			System.Diagnostics.Debug.Assert(size > 0);
			this.middleLits = new int[size];
			System.Array.Copy(ps.ToArray(), 1, this.middleLits, 0, size);
			ps.Clear();
			System.Diagnostics.Debug.Assert(ps.Size() == 0);
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
			int[] mylits = this.middleLits;
			foreach (int mylit in mylits)
			{
				if (this.voc.IsFalsified(mylit))
				{
					outReason.Push(LiteralsUtils.Neg(mylit));
				}
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
			foreach (int middleLit in this.middleLits)
			{
				if (this.voc.IsSatisfied(middleLit))
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool Propagate(UnitPropagationListener s, int p)
		{
			if (this.head == LiteralsUtils.Neg(p))
			{
				int[] mylits = this.middleLits;
				int temphead = 0;
				// moving head on the right
				while (temphead < mylits.Length && this.voc.IsFalsified(mylits[temphead]))
				{
					temphead++;
				}
				System.Diagnostics.Debug.Assert(temphead <= mylits.Length);
				if (temphead == mylits.Length)
				{
					this.voc.Watch(p, this);
					return s.Enqueue(this.tail, this);
				}
				this.head = mylits[temphead];
				mylits[temphead] = LiteralsUtils.Neg(p);
				this.voc.Watch(LiteralsUtils.Neg(this.head), this);
				return true;
			}
			System.Diagnostics.Debug.Assert(this.tail == LiteralsUtils.Neg(p));
			int[] mylits_1 = this.middleLits;
			int temptail = mylits_1.Length - 1;
			// moving tail on the left
			while (temptail >= 0 && this.voc.IsFalsified(mylits_1[temptail]))
			{
				temptail--;
			}
			System.Diagnostics.Debug.Assert(-1 <= temptail);
			if (-1 == temptail)
			{
				this.voc.Watch(p, this);
				return s.Enqueue(this.head, this);
			}
			this.tail = mylits_1[temptail];
			mylits_1[temptail] = LiteralsUtils.Neg(p);
			this.voc.Watch(LiteralsUtils.Neg(this.tail), this);
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
			foreach (int middleLit in this.middleLits)
			{
				stb.Append(Lits.ToString(middleLit));
				stb.Append("[");
				//$NON-NLS-1$
				stb.Append(this.voc.ValueToString(middleLit));
				stb.Append("]");
				//$NON-NLS-1$
				stb.Append(" ");
			}
			//$NON-NLS-1$
			stb.Append(Lits.ToString(this.tail));
			stb.Append("[");
			//$NON-NLS-1$
			stb.Append(this.voc.ValueToString(this.tail));
			stb.Append("]");
			//$NON-NLS-1$
			return stb.ToString();
		}

		/// <summary>Return the ith literal of the clause.</summary>
		/// <remarks>
		/// Return the ith literal of the clause. Note that the order of the literals
		/// does change during the search...
		/// </remarks>
		/// <param name="i">the index of the literal</param>
		/// <returns>the literal</returns>
		public virtual int Get(int i)
		{
			if (i == 0)
			{
				return this.head;
			}
			if (i == this.middleLits.Length + 1)
			{
				return this.tail;
			}
			return this.middleLits[i - 1];
		}

		/// <param name="d"/>
		public virtual void RescaleBy(double d)
		{
			this.activity *= d;
		}

		public virtual int Size()
		{
			return this.middleLits.Length + 2;
		}

		public virtual void AssertConstraint(UnitPropagationListener s)
		{
			System.Diagnostics.Debug.Assert(this.voc.IsUnassigned(this.head));
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
			int[] tmp = new int[Size()];
			System.Array.Copy(this.middleLits, 0, tmp, 1, this.middleLits.Length);
			tmp[0] = this.head;
			tmp[tmp.Length - 1] = this.tail;
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
				Org.Sat4j.Minisat.Constraints.Cnf.HTClause wcl = (Org.Sat4j.Minisat.Constraints.Cnf.HTClause)obj;
				if (wcl.head != this.head || wcl.tail != this.tail)
				{
					return false;
				}
				if (this.middleLits.Length != wcl.middleLits.Length)
				{
					return false;
				}
				bool ok;
				foreach (int lit in this.middleLits)
				{
					ok = false;
					foreach (int lit2 in wcl.middleLits)
					{
						if (lit == lit2)
						{
							ok = true;
							break;
						}
					}
					if (!ok)
					{
						return false;
					}
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
			foreach (int p in this.middleLits)
			{
				sum += p;
			}
			return (int)sum / this.middleLits.Length;
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
			foreach (int p in this.middleLits)
			{
				if (voc.IsSatisfied(p))
				{
					return true;
				}
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
			StringBuilder stb = new StringBuilder();
			stb.Append(mapper.Map(LiteralsUtils.ToDimacs(this.head)));
			stb.Append("[");
			//$NON-NLS-1$
			stb.Append(this.voc.ValueToString(this.head));
			stb.Append("]");
			//$NON-NLS-1$
			stb.Append(" ");
			//$NON-NLS-1$
			foreach (int middleLit in this.middleLits)
			{
				stb.Append(mapper.Map(LiteralsUtils.ToDimacs(middleLit)));
				stb.Append("[");
				//$NON-NLS-1$
				stb.Append(this.voc.ValueToString(middleLit));
				stb.Append("]");
				//$NON-NLS-1$
				stb.Append(" ");
			}
			//$NON-NLS-1$
			stb.Append(mapper.Map(LiteralsUtils.ToDimacs(this.tail)));
			stb.Append("[");
			//$NON-NLS-1$
			stb.Append(this.voc.ValueToString(this.tail));
			stb.Append("]");
			//$NON-NLS-1$
			return stb.ToString();
		}

		public abstract bool PropagatePI(MandatoryLiteralListener arg1, int arg2);

		public abstract bool Learnt();

		public abstract void ForwardActivity(double arg1);

		public abstract void IncActivity(double arg1);

		public abstract void Register();

		public abstract void SetActivity(double arg1);

		public abstract void SetLearnt();
	}
}
