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
	[System.Serializable]
	public class MinWatchCard : Propagatable, Constr, Undoable
	{
		private const long serialVersionUID = 1L;

		public const bool Atleast = true;

		public const bool Atmost = false;

		/// <summary>degree of the cardinality constraint</summary>
		protected internal int degree;

		/// <summary>literals involved in the constraint</summary>
		private readonly int[] lits;

		/// <summary>contains the sign of the constraint : ATLEAT or ATMOST</summary>
		private bool moreThan;

		/// <summary>contains the sum of the coefficients of the watched literals</summary>
		protected internal int watchCumul;

		/// <summary>Vocabulary of the constraint</summary>
		private readonly ILits voc;

		/// <summary>Maximum number of falsified literal in the constraint.</summary>
		private readonly int maxUnsatisfied;

		/// <summary>Constructs and normalizes a cardinality constraint.</summary>
		/// <remarks>
		/// Constructs and normalizes a cardinality constraint. used by
		/// minWatchCardNew in the non-normalized case.
		/// </remarks>
		/// <param name="voc">vocabulary used by the constraint</param>
		/// <param name="ps">literals involved in the constraint</param>
		/// <param name="moreThan">should be ATLEAST or ATMOST;</param>
		/// <param name="degree">degree of the constraint</param>
		public MinWatchCard(ILits voc, IVecInt ps, bool moreThan, int degree)
		{
			// On met en place les valeurs
			this.voc = voc;
			this.degree = degree;
			this.moreThan = moreThan;
			// On simplifie ps
			int[] index = new int[voc.NVars() * 2 + 2];
			// Fresh array should have all elements set to 0
			// On repertorie les litt?raux utiles
			for (int i = 0; i < ps.Size(); i++)
			{
				int p = ps.Get(i);
				if (index[p ^ 1] == 0)
				{
					index[p]++;
				}
				else
				{
					index[p ^ 1]--;
				}
			}
			// On supprime les litt?raux inutiles
			int ind = 0;
			while (ind < ps.Size())
			{
				if (index[ps.Get(ind)] > 0)
				{
					index[ps.Get(ind)]--;
					ind++;
				}
				else
				{
					// ??
					if ((ps.Get(ind) & 1) != 0)
					{
						this.degree--;
					}
					ps.Delete(ind);
				}
			}
			// On copie les litt?raux de la contrainte
			this.lits = new int[ps.Size()];
			ps.MoveTo(this.lits);
			// On normalise la contrainte au sens de Barth
			Normalize();
			this.maxUnsatisfied = lits.Length - this.degree;
		}

		/// <summary>Constructs and normalizes a cardinality constraint.</summary>
		/// <remarks>
		/// Constructs and normalizes a cardinality constraint. used by
		/// MinWatchCardPB.normalizedMinWatchCardNew() in the normalized case.
		/// <strong>Should not be used if parameters are not already
		/// normalized</strong> This constraint is always an ATLEAST constraint.
		/// </remarks>
		/// <param name="voc">vocabulary used by the constraint</param>
		/// <param name="ps">literals involved in the constraint</param>
		/// <param name="degree">degree of the constraint</param>
		protected internal MinWatchCard(ILits voc, IVecInt ps, int degree)
		{
			// On met en place les valeurs
			this.voc = voc;
			this.degree = degree;
			this.moreThan = Atleast;
			// On copie les litt?raux de la contrainte
			this.lits = new int[ps.Size()];
			ps.MoveTo(this.lits);
			this.maxUnsatisfied = lits.Length - this.degree;
		}

		/// <summary>computes the reason for a literal</summary>
		/// <param name="p">falsified literal (or Lit.UNDEFINED)</param>
		/// <param name="outReason">the reason to be computed. Vector of literals.</param>
		/// <seealso cref="Org.Sat4j.Specs.Constr.CalcReason(int, Org.Sat4j.Specs.IVecInt)"/>
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

		/// <summary>Returns the activity of the constraint</summary>
		/// <returns>activity value of the constraint</returns>
		/// <seealso cref="Org.Sat4j.Specs.IConstr.GetActivity()"/>
		public virtual double GetActivity()
		{
			return 0;
		}

		/// <summary>Increments activity of the constraint</summary>
		/// <param name="claInc">value to be added to the activity of the constraint</param>
		/// <seealso cref="Org.Sat4j.Specs.Constr.IncActivity(double)"/>
		public virtual void IncActivity(double claInc)
		{
		}

		public virtual void SetActivity(double d)
		{
		}

		/// <summary>Returns wether the constraint is learnt or not.</summary>
		/// <returns>false : a MinWatchCard cannot be learnt.</returns>
		/// <seealso cref="Org.Sat4j.Specs.IConstr.Learnt()"/>
		public virtual bool Learnt()
		{
			return false;
		}

		/// <summary>Simplifies the constraint w.r.t.</summary>
		/// <remarks>Simplifies the constraint w.r.t. the assignments of the literals</remarks>
		/// <param name="voc">vocabulary used</param>
		/// <param name="ps">literals involved</param>
		/// <returns>
		/// value to be added to the degree. This value is less than or equal
		/// to 0.
		/// </returns>
		protected internal static int Linearisation(ILits voc, IVecInt ps)
		{
			// Stockage de l'influence des modifications
			int modif = 0;
			for (int i = 0; i < ps.Size(); )
			{
				// on verifie si le litteral est affecte
				if (voc.IsUnassigned(ps.Get(i)))
				{
					i++;
				}
				else
				{
					// Si le litteral est satisfait,
					// ?a revient ? baisser le degr?
					if (voc.IsSatisfied(ps.Get(i)))
					{
						modif--;
					}
					// dans tous les cas, s'il est assign?,
					// on enleve le ieme litteral
					ps.Set(i, ps.Last());
					ps.Pop();
				}
			}
			System.Diagnostics.Debug.Assert(modif <= 0);
			return modif;
		}

		/// <summary>Returns if the constraint is the reason for a unit propagation.</summary>
		/// <returns>true</returns>
		/// <seealso cref="Org.Sat4j.Specs.Constr.Locked()"/>
		public virtual bool Locked()
		{
			return true;
		}

		/// <summary>
		/// Constructs a cardinality constraint with a minimal set of watched
		/// literals Permet la cr?ation de contrainte de cardinalit? ? observation
		/// minimale
		/// </summary>
		/// <param name="s">tool for propagation</param>
		/// <param name="voc">vocalulary used by the constraint</param>
		/// <param name="ps">literals involved in the constraint</param>
		/// <param name="moreThan">sign of the constraint. Should be ATLEAST or ATMOST.</param>
		/// <param name="degree">degree of the constraint</param>
		/// <returns>a new cardinality constraint, null if it is a tautology</returns>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public static Constr MinWatchCardNew(UnitPropagationListener s, ILits voc, IVecInt ps, bool moreThan, int degree)
		{
			int mydegree = degree + Linearisation(voc, ps);
			if (ps.Size() < mydegree)
			{
				throw new ContradictionException();
			}
			else
			{
				if (ps.Size() == mydegree)
				{
					for (int i = 0; i < ps.Size(); i++)
					{
						if (!s.Enqueue(ps.Get(i)))
						{
							throw new ContradictionException();
						}
					}
					return new UnitClauses(ps);
				}
			}
			// La contrainte est maintenant cr??e
			Org.Sat4j.Minisat.Constraints.Card.MinWatchCard retour = new Org.Sat4j.Minisat.Constraints.Card.MinWatchCard(voc, ps, moreThan, mydegree);
			if (retour.degree <= 0)
			{
				return null;
			}
			retour.ComputeWatches();
			retour.ComputePropagation(s);
			return retour;
		}

		/// <summary>normalize the constraint (cf.</summary>
		/// <remarks>normalize the constraint (cf. P.Barth normalization)</remarks>
		public void Normalize()
		{
			// Gestion du signe
			if (!this.moreThan)
			{
				// On multiplie le degr? par -1
				this.degree = 0 - this.degree;
				// On r?vise chaque litt?ral
				for (int indLit = 0; indLit < this.lits.Length; indLit++)
				{
					this.lits[indLit] = this.lits[indLit] ^ 1;
					this.degree++;
				}
				this.moreThan = true;
			}
		}

		/// <summary>propagates the value of a falsified literal</summary>
		/// <param name="s">tool for literal propagation</param>
		/// <param name="p">falsified literal</param>
		/// <returns>false if an inconistency is detected, else true</returns>
		public virtual bool Propagate(UnitPropagationListener s, int p)
		{
			this.savedindex = this.degree + 1;
			// Si la contrainte est responsable de propagation unitaire
			if (this.watchCumul == this.degree)
			{
				this.voc.Watch(p, this);
				return false;
			}
			// Recherche du litt?ral falsifi?
			int indFalsified = 0;
			while ((this.lits[indFalsified] ^ 1) != p)
			{
				indFalsified++;
			}
			System.Diagnostics.Debug.Assert(this.watchCumul > this.degree);
			// Recherche du litt?ral swap
			int indSwap = this.degree + 1;
			while (indSwap < this.lits.Length && this.voc.IsFalsified(this.lits[indSwap]))
			{
				indSwap++;
			}
			// Mise ? jour de la contrainte
			if (indSwap == this.lits.Length)
			{
				// Si aucun litt?ral n'a ?t? trouv?
				this.voc.Watch(p, this);
				// La limite est atteinte
				this.watchCumul--;
				System.Diagnostics.Debug.Assert(this.watchCumul == this.degree);
				this.voc.Undos(p).Push(this);
				// On met en queue les litt?raux impliqu?s
				for (int i = 0; i <= this.degree; i++)
				{
					if (p != (this.lits[i] ^ 1) && !s.Enqueue(this.lits[i], this))
					{
						return false;
					}
				}
				return true;
			}
			// Si un litt?ral a ?t? trouv? on les ?change
			int tmpInt = this.lits[indSwap];
			this.lits[indSwap] = this.lits[indFalsified];
			this.lits[indFalsified] = tmpInt;
			// On observe le nouveau litt?ral
			this.voc.Watch(tmpInt ^ 1, this);
			return true;
		}

		/// <summary>Removes a constraint from the solver</summary>
		/// <since>2.1</since>
		public virtual void Remove(UnitPropagationListener upl)
		{
			for (int i = 0; i < Math.Min(this.degree + 1, this.lits.Length); i++)
			{
				this.voc.Watches(this.lits[i] ^ 1).Remove(this);
			}
		}

		/// <summary>Rescales the activity value of the constraint</summary>
		/// <param name="d">rescale factor</param>
		public virtual void RescaleBy(double d)
		{
		}

		// TODO rescaleBy
		/// <summary>simplifies the constraint</summary>
		/// <returns>true if the constraint is satisfied, else false</returns>
		public virtual bool Simplify()
		{
			// Calcul de la valeur actuelle
			for (int i = 0; i < this.lits.Length; i++)
			{
				if (this.voc.IsSatisfied(this.lits[i]) && ++count == this.degree)
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>Returns a string representation of the constraint.</summary>
		/// <returns>representation of the constraint.</returns>
		public override string ToString()
		{
			StringBuilder stb = new StringBuilder();
			// stb.append("Card (" + this.lits.length + ") : ");
			if (this.lits.Length > 0)
			{
				// if (voc.isUnassigned(lits[0])) {
				stb.Append(Lits.ToStringX(this.lits[0]));
				stb.Append("[");
				stb.Append(this.voc.ValueToString(this.lits[0]));
				// stb.append("@");
				// stb.append(this.voc.getLevel(this.lits[0]));
				stb.Append("]");
				stb.Append(" ");
				//$NON-NLS-1$
				// }
				for (int i = 1; i < this.lits.Length; i++)
				{
					// if (voc.isUnassigned(lits[i])) {
					// stb.append(" + "); //$NON-NLS-1$
					stb.Append(Lits.ToStringX(this.lits[i]));
					stb.Append("[");
					stb.Append(this.voc.ValueToString(this.lits[i]));
					// stb.append("@");
					// stb.append(this.voc.getLevel(this.lits[i]));
					stb.Append("]");
					stb.Append(" ");
				}
				//$NON-NLS-1$
				// }
				stb.Append(">= ");
				//$NON-NLS-1$
				stb.Append(this.degree);
			}
			return stb.ToString();
		}

		/// <summary>Updates information on the constraint in case of a backtrack</summary>
		/// <param name="p">unassigned literal</param>
		public virtual void Undo(int p)
		{
			// Le litt?ral observ? et falsifi? devient non assign?
			this.watchCumul++;
		}

		public virtual void SetLearnt()
		{
			throw new NotSupportedException();
		}

		public virtual void Register()
		{
			ComputeWatches();
		}

		public virtual int Size()
		{
			return this.lits.Length;
		}

		public virtual int Get(int i)
		{
			return this.lits[i];
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
			if (this.watchCumul == this.degree)
			{
				for (int i = 0; i < this.watchCumul; i++)
				{
					s.Enqueue(this.lits[i]);
				}
			}
		}

		protected internal virtual void ComputeWatches()
		{
			int indSwap = this.lits.Length;
			int tmpInt;
			for (int i = 0; i <= this.degree && i < indSwap; i++)
			{
				while (this.voc.IsFalsified(this.lits[i]) && --indSwap > i)
				{
					tmpInt = this.lits[i];
					this.lits[i] = this.lits[indSwap];
					this.lits[indSwap] = tmpInt;
				}
				// Si le litteral est observable
				if (!this.voc.IsFalsified(this.lits[i]))
				{
					this.watchCumul++;
					this.voc.Watch(this.lits[i] ^ 1, this);
				}
			}
			if (this.watchCumul <= this.degree)
			{
				// chercher tous les litteraux a regarder
				// par ordre de niveau decroissant
				int free = 1;
				while (this.watchCumul <= this.degree && free > 0)
				{
					free = 0;
					// regarder le litteral falsifie au plus bas niveau
					int maxlevel = -1;
					int maxi = -1;
					for (int i_1 = this.watchCumul; i_1 < this.lits.Length; i_1++)
					{
						if (this.voc.IsFalsified(this.lits[i_1]))
						{
							free++;
							int level = this.voc.GetLevel(this.lits[i_1]);
							if (level > maxlevel)
							{
								maxi = i_1;
								maxlevel = level;
							}
						}
					}
					if (free > 0)
					{
						System.Diagnostics.Debug.Assert(maxi >= 0);
						this.voc.Watch(this.lits[maxi] ^ 1, this);
						tmpInt = this.lits[maxi];
						this.lits[maxi] = this.lits[this.watchCumul];
						this.lits[this.watchCumul] = tmpInt;
						this.watchCumul++;
						free--;
						System.Diagnostics.Debug.Assert(free >= 0);
					}
				}
				System.Diagnostics.Debug.Assert(this.lits.Length == 1 || this.watchCumul > 1);
			}
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		protected internal virtual Org.Sat4j.Minisat.Constraints.Card.MinWatchCard ComputePropagation(UnitPropagationListener s)
		{
			// Si on a des litteraux impliques
			if (this.watchCumul == this.degree)
			{
				for (int i = 0; i < this.lits.Length; i++)
				{
					if (!s.Enqueue(this.lits[i]))
					{
						throw new ContradictionException();
					}
				}
				return null;
			}
			// Si on n'observe pas suffisamment
			if (this.watchCumul < this.degree)
			{
				throw new ContradictionException();
			}
			return this;
		}

		public virtual int[] GetLits()
		{
			int[] tmp = new int[Size()];
			System.Array.Copy(this.lits, 0, tmp, 0, Size());
			return tmp;
		}

		public virtual ILits GetVocabulary()
		{
			return this.voc;
		}

		public override bool Equals(object card)
		{
			if (card == null)
			{
				return false;
			}
			if (this.GetType() != card.GetType())
			{
				return false;
			}
			try
			{
				Org.Sat4j.Minisat.Constraints.Card.MinWatchCard mcard = (Org.Sat4j.Minisat.Constraints.Card.MinWatchCard)card;
				if (mcard.degree != this.degree)
				{
					return false;
				}
				if (this.lits.Length != mcard.lits.Length)
				{
					return false;
				}
				bool ok;
				foreach (int lit in this.lits)
				{
					ok = false;
					foreach (int lit2 in mcard.lits)
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
			sum += this.degree;
			return (int)sum / (this.lits.Length + 1);
		}

		/// <since>2.1</since>
		public virtual void ForwardActivity(double claInc)
		{
		}

		// do nothing
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
			int bound = p == ILitsConstants.Undefined ? this.watchCumul : this.watchCumul - 1;
			for (int i = 0; i < bound; i++)
			{
				int q = lits[i];
				System.Diagnostics.Debug.Assert(voc.IsFalsified(q));
				outReason.Push(q ^ 1);
			}
		}

		private int savedindex = this.degree + 1;

		public virtual bool PropagatePI(MandatoryLiteralListener l, int p)
		{
			// Recherche du litt?ral falsifi?
			int indFalsified = 0;
			while ((this.lits[indFalsified] ^ 1) != p)
			{
				indFalsified++;
			}
			System.Diagnostics.Debug.Assert(this.watchCumul >= this.degree);
			// Recherche du litt?ral swap
			int indSwap = this.savedindex;
			while (indSwap < this.lits.Length && this.voc.IsFalsified(this.lits[indSwap]))
			{
				indSwap++;
			}
			// Mise ? jour de la contrainte
			if (indSwap == this.lits.Length)
			{
				// Si aucun litt?ral n'a ?t? trouv?
				this.voc.Watch(p, this);
				// On met en queue les litt?raux impliqu?s
				for (int i = 0; i <= this.degree; i++)
				{
					if (p != (this.lits[i] ^ 1))
					{
						l.IsMandatory(this.lits[i]);
					}
				}
				return true;
			}
			this.savedindex = indSwap + 1;
			// Si un litt?ral a ?t? trouv? on les ?change
			int tmpInt = this.lits[indSwap];
			this.lits[indSwap] = this.lits[indFalsified];
			this.lits[indFalsified] = tmpInt;
			// On observe le nouveau litt?ral
			this.voc.Watch(tmpInt ^ 1, this);
			return true;
		}

		public virtual bool CanBeSatisfiedByCountingLiterals()
		{
			return true;
		}

		public virtual int RequiredNumberOfSatisfiedLiterals()
		{
			return degree;
		}

		public virtual bool IsSatisfied()
		{
			throw new NotSupportedException("Not implemented yet!");
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
			if (mapper == null)
			{
				return ToString();
			}
			StringBuilder stb = new StringBuilder();
			// stb.append("Card (" + this.lits.length + ") : ");
			if (this.lits.Length > 0)
			{
				// if (voc.isUnassigned(lits[0])) {
				stb.Append(mapper.Map(LiteralsUtils.ToDimacs(this.lits[0])));
				stb.Append("[");
				stb.Append(this.voc.ValueToString(this.lits[0]));
				// stb.append("@");
				// stb.append(this.voc.getLevel(this.lits[0]));
				stb.Append("]");
				stb.Append(" ");
				//$NON-NLS-1$
				// }
				for (int i = 1; i < this.lits.Length; i++)
				{
					// if (voc.isUnassigned(lits[i])) {
					// stb.append(" + "); //$NON-NLS-1$
					stb.Append(mapper.Map(LiteralsUtils.ToDimacs(this.lits[i])));
					stb.Append("[");
					stb.Append(this.voc.ValueToString(this.lits[i]));
					// stb.append("@");
					// stb.append(this.voc.getLevel(this.lits[i]));
					stb.Append("]");
					stb.Append(" ");
				}
				//$NON-NLS-1$
				// }
				stb.Append(">= ");
				//$NON-NLS-1$
				stb.Append(this.degree);
			}
			return stb.ToString();
		}
	}
}
