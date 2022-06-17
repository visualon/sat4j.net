using System;
using System.Text;
using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Cnf
{
	/// <summary>Lazy data structure for clause using Watched Literals.</summary>
	/// <author>leberre</author>
	[System.Serializable]
	public abstract class WLClause : Propagatable, Constr
	{
		private const long serialVersionUID = 1L;

		/// <since>2.1</since>
		protected internal double activity;

		protected internal readonly int[] lits;

		protected internal readonly ILits voc;

		/// <summary>Creates a new basic clause</summary>
		/// <param name="voc">the vocabulary of the formula</param>
		/// <param name="ps">A VecInt that WILL BE EMPTY after calling that method.</param>
		public WLClause(IVecInt ps, ILits voc)
		{
			this.lits = new int[ps.Size()];
			ps.MoveTo(this.lits);
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
			// assert outReason.size() == 0
			// && ((p == ILits.UNDEFINED) || (p == lits[0]));
			int[] mylits = this.lits;
			for (int i = p == ILitsConstants.Undefined ? 0 : 1; i < mylits.Length; i++)
			{
				System.Diagnostics.Debug.Assert(this.voc.IsFalsified(mylits[i]));
				outReason.Push(mylits[i] ^ 1);
			}
		}

		/// <since>2.1</since>
		public virtual void Remove(UnitPropagationListener upl)
		{
			this.voc.Watches(this.lits[0] ^ 1).Remove(this);
			this.voc.Watches(this.lits[1] ^ 1).Remove(this);
		}

		// la clause peut etre effacee
		/*
		* (non-Javadoc)
		*
		* @see Constr#simplify(Solver)
		*/
		public virtual bool Simplify()
		{
			foreach (int lit in this.lits)
			{
				if (this.voc.IsSatisfied(lit))
				{
					return true;
				}
			}
			return false;
		}

		public virtual bool Propagate(UnitPropagationListener s, int p)
		{
			int[] mylits = this.lits;
			// Lits[1] must contain a falsified literal
			if (mylits[0] == (p ^ 1))
			{
				mylits[0] = mylits[1];
				mylits[1] = p ^ 1;
			}
			// assert mylits[1] == (p ^ 1);
			if (this.voc.IsSatisfied(mylits[0]))
			{
				this.voc.Watch(p, this);
				return true;
			}
			int previous = p ^ 1;
			int tmp;
			// look for new literal to watch: applying move to front strategy
			for (int i = 2; i < mylits.Length; i++)
			{
				if (this.voc.IsFalsified(mylits[i]))
				{
					tmp = previous;
					previous = mylits[i];
					mylits[i] = tmp;
				}
				else
				{
					mylits[1] = mylits[i];
					mylits[i] = previous;
					this.voc.Watch(mylits[1] ^ 1, this);
					return true;
				}
			}
			// assert voc.isFalsified(mylits[1]);
			// the clause is now either unit or null
			// move back the literals to their initial position
			System.Array.Copy(mylits, 2, mylits, 1, mylits.Length - 2);
			mylits[mylits.Length - 1] = previous;
			this.voc.Watch(p, this);
			// propagates first watched literal
			return s.Enqueue(mylits[0], this);
		}

		/*
		* For learnt clauses only @author leberre
		*/
		public virtual bool Locked()
		{
			return this.voc.GetReason(this.lits[0]) == this;
		}

		/// <returns>the activity of the clause</returns>
		public virtual double GetActivity()
		{
			return this.activity;
		}

		public virtual void SetActivity(double d)
		{
			this.activity = d;
		}

		public override string ToString()
		{
			StringBuilder stb = new StringBuilder();
			foreach (int lit in this.lits)
			{
				stb.Append(Lits.ToString(lit));
				stb.Append("[");
				//$NON-NLS-1$
				stb.Append(this.voc.ValueToString(lit));
				stb.Append("]");
				//$NON-NLS-1$
				stb.Append(" ");
			}
			//$NON-NLS-1$
			return stb.ToString();
		}

		public virtual string ToString(VarMapper mapper)
		{
			if (mapper == null)
			{
				return ToString();
			}
			StringBuilder stb = new StringBuilder();
			foreach (int lit in this.lits)
			{
				stb.Append(mapper.Map(LiteralsUtils.ToDimacs(lit)));
				stb.Append("[");
				//$NON-NLS-1$
				stb.Append(this.voc.ValueToString(lit));
				stb.Append("]");
				//$NON-NLS-1$
				stb.Append(" ");
			}
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
			return this.lits[i];
		}

		/// <param name="d"/>
		public virtual void RescaleBy(double d)
		{
			this.activity *= d;
		}

		public virtual int Size()
		{
			return this.lits.Length;
		}

		public virtual void AssertConstraint(UnitPropagationListener s)
		{
			bool ret = s.Enqueue(this.lits[0], this);
			System.Diagnostics.Debug.Assert(ret);
		}

		public virtual void AssertConstraintIfNeeded(UnitPropagationListener s)
		{
			if (voc.IsFalsified(this.lits[1]))
			{
				bool ret = s.Enqueue(this.lits[0], this);
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
			System.Array.Copy(this.lits, 0, tmp, 0, Size());
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
				Org.Sat4j.Minisat.Constraints.Cnf.WLClause wcl = (Org.Sat4j.Minisat.Constraints.Cnf.WLClause)obj;
				if (this.lits.Length != wcl.lits.Length)
				{
					return false;
				}
				bool ok;
				foreach (int lit in this.lits)
				{
					ok = false;
					foreach (int lit2 in wcl.lits)
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
			long sum = 0;
			foreach (int p in this.lits)
			{
				sum += p;
			}
			return (int)sum / this.lits.Length;
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
			foreach (int p in this.lits)
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
				if (LiteralsUtils.Var(trail.Get(i)) == LiteralsUtils.Var(this.lits[0]))
				{
					return i;
				}
			}
			return -1;
		}

		public abstract bool PropagatePI(MandatoryLiteralListener arg1, int arg2);

		public abstract bool Learnt();

		public abstract void ForwardActivity(double arg1);

		public abstract void IncActivity(double arg1);

		public abstract void Register();

		public abstract void SetLearnt();
	}
}
