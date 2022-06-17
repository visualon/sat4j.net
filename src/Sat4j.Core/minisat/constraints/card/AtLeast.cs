using System;
using System.Collections.Generic;
using System.Text;
using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Constraints.Cnf;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Card
{
	/// <author>leberre Contrainte de cardinalit?</author>
	[System.Serializable]
	public class AtLeast : Propagatable, Constr, Undoable
	{
		private const long serialVersionUID = 1L;

		/// <summary>number of allowed falsified literal</summary>
		protected internal int maxUnsatisfied;

		/// <summary>current number of falsified literals</summary>
		private int counter;

		/// <summary>constraint literals</summary>
		protected internal readonly int[] lits;

		protected internal readonly ILits voc;

		/// <param name="ps">a vector of literals</param>
		/// <param name="degree">the minimal number of satisfied literals</param>
		public AtLeast(ILits voc, IVecInt ps, int degree)
		{
			if (degree == 1)
			{
				throw new ArgumentException("cards with degree 1 are clauses!!!!");
			}
			this.maxUnsatisfied = ps.Size() - degree;
			this.voc = voc;
			this.counter = 0;
			this.lits = new int[ps.Size()];
			ps.MoveTo(this.lits);
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		protected internal static int NiceParameters(UnitPropagationListener s, ILits voc, IVecInt ps, int deg)
		{
			if (ps.Size() < deg)
			{
				throw new ContradictionException();
			}
			int degree = deg;
			for (int i = 0; i < ps.Size(); )
			{
				// on verifie si le litteral est affecte
				if (voc.IsUnassigned(ps.Get(i)))
				{
					// go to next literal
					i++;
				}
				else
				{
					// Si le litteral est satisfait,
					// ?a revient ? baisser le degr?
					if (voc.IsSatisfied(ps.Get(i)))
					{
						degree--;
					}
					// dans tous les cas, s'il est assign?,
					// on enleve le ieme litteral
					ps.Delete(i);
				}
			}
			// on trie le vecteur ps
			ps.SortUnique();
			// ?limine les clauses tautologiques
			// deux litt?raux de signe oppos?s apparaissent dans la m?me
			// clause
			if (ps.Size() == degree)
			{
				for (int i_1 = 0; i_1 < ps.Size(); i_1++)
				{
					if (!s.Enqueue(ps.Get(i_1)))
					{
						throw new ContradictionException();
					}
				}
				return 0;
			}
			if (ps.Size() < degree)
			{
				throw new ContradictionException();
			}
			return degree;
		}

		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public static Constr AtLeastNew(UnitPropagationListener s, ILits voc, IVecInt ps, int n)
		{
			int degree = NiceParameters(s, voc, ps, n);
			if (degree == 0)
			{
				return new UnitClauses(ps);
			}
			if (degree == 1)
			{
				return OriginalWLClause.BrandNewClause(s, voc, ps);
			}
			Constr constr = new Org.Sat4j.Minisat.Constraints.Card.AtLeast(voc, ps, degree);
			constr.Register();
			return constr;
		}

		/// <since>2.1</since>
		public virtual void Remove(UnitPropagationListener upl)
		{
			foreach (int q in this.lits)
			{
				this.voc.Watches(q ^ 1).Remove(this);
			}
		}

		/*
		* (non-Javadoc)
		*
		* @see Constr#propagate(Solver, Lit)
		*/
		public virtual bool Propagate(UnitPropagationListener s, int p)
		{
			// remet la clause dans la liste des clauses regardees
			this.voc.Watch(p, this);
			if (this.counter == this.maxUnsatisfied)
			{
				return false;
			}
			this.counter++;
			this.voc.Undos(p).Push(this);
			// If no more can be false, enqueue the rest:
			if (this.counter == this.maxUnsatisfied)
			{
				foreach (int q in this.lits)
				{
					if (this.voc.IsUnassigned(q) && !s.Enqueue(q, this))
					{
						return false;
					}
				}
			}
			return true;
		}

		/*
		* (non-Javadoc)
		*
		* @see Constr#simplify(Solver)
		*/
		public virtual bool Simplify()
		{
			return false;
		}

		/*
		* (non-Javadoc)
		*
		* @see Constr#undo(Solver, Lit)
		*/
		public virtual void Undo(int p)
		{
			this.counter--;
		}

		/*
		* (non-Javadoc)
		*
		* @see Constr#calcReason(Solver, Lit, Vec)
		*/
		public virtual void CalcReason(int p, IVecInt outReason)
		{
			int c = p == ILitsConstants.Undefined ? -1 : 0;
			foreach (int q in this.lits)
			{
				if (this.voc.IsFalsified(q))
				{
					outReason.Push(q ^ 1);
					if (++c >= this.maxUnsatisfied)
					{
						return;
					}
				}
			}
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.datatype.Constr#learnt()
		*/
		public virtual bool Learnt()
		{
			// Ces contraintes ne sont pas apprises pour le moment.
			return false;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.datatype.Constr#getActivity()
		*/
		public virtual double GetActivity()
		{
			return 0;
		}

		public virtual void SetActivity(double d)
		{
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.datatype.Constr#incActivity(double)
		*/
		public virtual void IncActivity(double claInc)
		{
		}

		/*
		* For learnt clauses only @author leberre
		*/
		public virtual bool Locked()
		{
			// FIXME need to be adapted to AtLeast
			// return lits[0].getReason() == this;
			return true;
		}

		public virtual void SetLearnt()
		{
			throw new NotSupportedException();
		}

		public virtual void Register()
		{
			this.counter = 0;
			foreach (int q in this.lits)
			{
				voc.Watch(q ^ 1, this);
				if (voc.IsFalsified(q))
				{
					this.counter++;
					this.voc.Undos(q ^ 1).Push(this);
				}
			}
		}

		public virtual int Size()
		{
			return this.lits.Length;
		}

		public virtual int Get(int i)
		{
			return this.lits[i];
		}

		public virtual void RescaleBy(double d)
		{
			throw new NotSupportedException();
		}

		public virtual void AssertConstraint(UnitPropagationListener s)
		{
			bool ret = true;
			foreach (int lit in this.lits)
			{
				if (this.voc.IsUnassigned(lit))
				{
					ret &= s.Enqueue(lit, this);
				}
			}
			System.Diagnostics.Debug.Assert(ret == true);
		}

		public virtual void AssertConstraintIfNeeded(UnitPropagationListener s)
		{
			throw new NotSupportedException();
		}

		/// <summary>Textual representation of the constraint</summary>
		/// <returns>a string representing the constraint.</returns>
		public override string ToString()
		{
			StringBuilder stb = new StringBuilder();
			stb.Append("Card (" + this.lits.Length + ") : ");
			foreach (int lit in this.lits)
			{
				// if (voc.isUnassigned(lits[i])) {
				stb.Append(" + ");
				//$NON-NLS-1$
				stb.Append(Lits.ToString(lit));
				stb.Append("[");
				stb.Append(this.voc.ValueToString(lit));
				stb.Append("@");
				stb.Append(this.voc.GetLevel(lit));
				stb.Append("]  ");
			}
			stb.Append(">= ");
			//$NON-NLS-1$
			stb.Append(Size() - this.maxUnsatisfied);
			return stb.ToString();
		}

		/// <since>2.1</since>
		public virtual void ForwardActivity(double claInc)
		{
		}

		// TODO Auto-generated method stub
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
			int c = p == ILitsConstants.Undefined ? -1 : 0;
			IVecInt vlits = new VecInt(this.lits);
			for (IteratorInt it = trail.Iterator(); it.HasNext(); )
			{
				int q = it.Next();
				if (vlits.Contains(q ^ 1))
				{
					outReason.Push(q);
					if (++c >= this.maxUnsatisfied)
					{
						return;
					}
				}
			}
		}

		public virtual bool PropagatePI(MandatoryLiteralListener l, int p)
		{
			// remet la clause dans la liste des clauses regardees
			this.voc.Watch(p, this);
			this.counter++;
			this.voc.Undos(p).Push(this);
			// If no more can be false, the remaining literals are mandatory
			if (this.counter == this.maxUnsatisfied)
			{
				foreach (int q in this.lits)
				{
					if (!this.voc.IsFalsified(q))
					{
						l.IsMandatory(q);
					}
				}
			}
			return true;
		}

		public virtual bool CanBeSatisfiedByCountingLiterals()
		{
			return true;
		}

		public virtual int RequiredNumberOfSatisfiedLiterals()
		{
			return this.lits.Length - maxUnsatisfied;
		}

		public virtual bool IsSatisfied()
		{
			int nbSatisfied = 0;
			int degree = Size() - this.maxUnsatisfied;
			foreach (int p in this.lits)
			{
				if (voc.IsSatisfied(p))
				{
					nbSatisfied++;
					if (nbSatisfied >= degree)
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual int GetAssertionLevel(IVecInt trail, int decisionLevel)
		{
			int nUnsat = 0;
			ICollection<int> litsSet = new HashSet<int>();
			foreach (int i in this.lits)
			{
				litsSet.Add(i);
			}
			for (int i_1 = 0; i_1 < trail.Size(); ++i_1)
			{
				if (litsSet.Contains(trail.Get(i_1) ^ 1))
				{
					++nUnsat;
					if (nUnsat == this.maxUnsatisfied)
					{
						return i_1;
					}
				}
			}
			return -1;
		}

		public virtual string ToString(VarMapper mapper)
		{
			StringBuilder stb = new StringBuilder();
			foreach (int lit in this.lits)
			{
				stb.Append(" + ");
				//$NON-NLS-1$
				stb.Append(mapper.Map(LiteralsUtils.ToDimacs(lit)));
				stb.Append("[");
				stb.Append(this.voc.ValueToString(lit));
				stb.Append("]  ");
			}
			stb.Append(">= ");
			//$NON-NLS-1$
			stb.Append(Size() - this.maxUnsatisfied);
			return stb.ToString();
		}
	}
}
