using System;
using System.Collections.Generic;
using System.Text;
using Mono.Math;
using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Constraints.Cnf;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Card
{
	[System.Serializable]
	public sealed class MaxWatchCard : Propagatable, Constr, Undoable
	{
		private const long serialVersionUID = 1L;

		/// <summary>Degree (right hand side) of the constraint.</summary>
		private int degree;

		/// <summary>Literals (left hand side) of the constraint.</summary>
		private readonly int[] lits;

		/// <summary>Flag to denote greater than or lesser than constraint.</summary>
		private bool moreThan;

		/// <summary>Sum of the currently watched literals.</summary>
		private int watchCumul;

		/// <summary>Vocabulary of the constraints.</summary>
		private readonly ILits voc;

		/// <summary>Build a new constraint.</summary>
		/// <param name="voc">the vocabulary of the constraint.</param>
		/// <param name="ps">a set of literals</param>
		/// <param name="moreThan">
		/// true if the constraint is of the form "greater than", else
		/// false.
		/// </param>
		/// <param name="degree">the degree/threshold of the constraint</param>
		public MaxWatchCard(ILits voc, IVecInt ps, bool moreThan, int degree)
		{
			// update fields
			this.voc = voc;
			this.degree = degree;
			this.moreThan = moreThan;
			// Simply ps
			int[] index = new int[voc.NVars() * 2 + 2];
			for (int i = 0; i < index.Length; i++)
			{
				index[i] = 0;
			}
			// Look for opposite literals
			for (int i_1 = 0; i_1 < ps.Size(); i_1++)
			{
				if (index[ps.Get(i_1) ^ 1] == 0)
				{
					index[ps.Get(i_1)]++;
				}
				else
				{
					index[ps.Get(i_1) ^ 1]--;
				}
			}
			// Update degree according to removed literals
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
					if ((ps.Get(ind) & 1) != 0)
					{
						this.degree--;
					}
					ps.Set(ind, ps.Last());
					ps.Pop();
				}
			}
			// Copy literals to the current constraint
			this.lits = new int[ps.Size()];
			ps.MoveTo(this.lits);
			// Normalize the constraint
			Normalize();
			// Watch all non falsified literals
			this.watchCumul = 0;
			for (int i_2 = 0; i_2 < this.lits.Length; i_2++)
			{
				// Note: those falsified literals will never be unset
				if (!voc.IsFalsified(this.lits[i_2]))
				{
					this.watchCumul++;
					voc.Watch(this.lits[i_2] ^ 1, this);
				}
			}
		}

		/// <summary>Calcule la cause de l'affection d'un litt?ral</summary>
		/// <param name="p">un litt?ral falsifi? (ou Lit.UNDEFINED)</param>
		/// <param name="outReason">vecteur de litt?raux ? remplir</param>
		/// <seealso cref="Org.Sat4j.Specs.Constr.CalcReason(int, Org.Sat4j.Specs.IVecInt)"/>
		public void CalcReason(int p, IVecInt outReason)
		{
			foreach (int lit in this.lits)
			{
				if (this.voc.IsFalsified(lit))
				{
					outReason.Push(lit ^ 1);
				}
			}
		}

		/// <summary>Obtenir la valeur de l'activit? de la contrainte</summary>
		/// <returns>la valeur de l'activit? de la contrainte</returns>
		/// <seealso cref="Org.Sat4j.Specs.IConstr.GetActivity()"/>
		public double GetActivity()
		{
			// TODO getActivity
			return 0;
		}

		/// <summary>Incr?mente la valeur de l'activit? de la contrainte</summary>
		/// <param name="claInc">incr?ment de l'activit? de la contrainte</param>
		/// <seealso cref="Org.Sat4j.Specs.Constr.IncActivity(double)"/>
		public void IncActivity(double claInc)
		{
		}

		// TODO incActivity
		public void SetActivity(double d)
		{
		}

		/// <summary>D?termine si la contrainte est apprise</summary>
		/// <returns>true si la contrainte est apprise, false sinon</returns>
		/// <seealso cref="Org.Sat4j.Specs.IConstr.Learnt()"/>
		public bool Learnt()
		{
			// TODO learnt
			return false;
		}

		/// <summary>La contrainte est la cause d'une propagation unitaire</summary>
		/// <returns>true si c'est le cas, false sinon</returns>
		/// <seealso cref="Org.Sat4j.Specs.Constr.Locked()"/>
		public bool Locked()
		{
			// TODO locked
			return true;
		}

		/// <summary>Permet la cr?ation de contrainte de cardinalit? ? observation minimale</summary>
		/// <param name="s">outil pour la propagation des litt?raux</param>
		/// <param name="voc">vocabulaire utilis? par la contrainte</param>
		/// <param name="ps">liste des litt?raux de la nouvelle contrainte</param>
		/// <param name="moreThan">d?termine si c'est une sup?rieure ou ?gal ? l'origine</param>
		/// <param name="degree">fournit le degr? de la contrainte</param>
		/// <returns>une nouvelle clause si tout va bien, null sinon</returns>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public static Constr MaxWatchCardNew(UnitPropagationListener s, ILits voc, IVecInt ps, bool moreThan, int degree)
		{
			Org.Sat4j.Minisat.Constraints.Card.MaxWatchCard outclause = null;
			// La contrainte ne doit pas ?tre vide
			if (ps.Size() < degree)
			{
				throw new ContradictionException("Creating trivially inconsistent constraint");
			}
			else
			{
				//$NON-NLS-1$
				if (ps.Size() == degree)
				{
					for (int i = 0; i < ps.Size(); i++)
					{
						if (!s.Enqueue(ps.Get(i)))
						{
							throw new ContradictionException("Contradiction with implied literal");
						}
					}
					//$NON-NLS-1$
					return new UnitClauses(ps);
				}
			}
			// On cree la contrainte
			outclause = new Org.Sat4j.Minisat.Constraints.Card.MaxWatchCard(voc, ps, moreThan, degree);
			// Si le degr? est insufisant
			if (outclause.degree <= 0)
			{
				return null;
			}
			// Si il n'y a aucune chance de satisfaire la contrainte
			if (outclause.watchCumul < outclause.degree)
			{
				throw new ContradictionException();
			}
			// Si les litt?raux observ?s sont impliqu?s
			if (outclause.watchCumul == outclause.degree)
			{
				for (int i = 0; i < outclause.lits.Length; i++)
				{
					if (!s.Enqueue(outclause.lits[i]))
					{
						throw new ContradictionException("Contradiction with implied literal");
					}
				}
				//$NON-NLS-1$
				return null;
			}
			return outclause;
		}

		/// <summary>On normalise la contrainte au sens de Barth</summary>
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

		/// <summary>Propagation de la valeur de v?rit? d'un litt?ral falsifi?</summary>
		/// <param name="s">objet utilis? pour la propagation</param>
		/// <param name="p">le litt?ral propag? (il doit etre falsifie)</param>
		/// <returns>false ssi une inconsistance est d?t?ct?e</returns>
		public bool Propagate(UnitPropagationListener s, int p)
		{
			// On observe toujours tous les litt?raux
			this.voc.Watch(p, this);
			System.Diagnostics.Debug.Assert(!this.voc.IsFalsified(p));
			// Si le litt?ral p est impliqu?
			if (this.watchCumul == this.degree)
			{
				return false;
			}
			// On met en place la mise ? jour du compteur
			this.voc.Undos(p).Push(this);
			this.watchCumul--;
			// Si les litt?raux restant sont impliqu?s
			if (this.watchCumul == this.degree)
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

		/// <since>2.1</since>
		public void Remove(UnitPropagationListener upl)
		{
			foreach (int q in this.lits)
			{
				this.voc.Watches(q ^ 1).Remove(this);
			}
		}

		/// <summary>Permet le r??chantillonage de l'activit? de la contrainte</summary>
		/// <param name="d">facteur d'ajustement</param>
		public void RescaleBy(double d)
		{
		}

		/// <summary>Simplifie la contrainte(l'all?ge)</summary>
		/// <returns>true si la contrainte est satisfaite, false sinon</returns>
		public bool Simplify()
		{
			int i = 0;
			// On esp?re le maximum de la somme
			int curr = this.watchCumul;
			// Pour chaque litt?ral
			while (i < this.lits.Length)
			{
				// On d?cr?mente si l'espoir n'est pas fond?
				if (this.voc.IsUnassigned(this.lits[i++]))
				{
					curr--;
					if (curr < this.degree)
					{
						return false;
					}
				}
			}
			return false;
		}

		/// <summary>Cha?ne repr?sentant la contrainte</summary>
		/// <returns>Cha?ne repr?sentant la contrainte</returns>
		public override string ToString()
		{
			StringBuilder stb = new StringBuilder();
			if (this.lits.Length > 0)
			{
				if (this.voc.IsUnassigned(this.lits[0]))
				{
					stb.Append(Lits.ToString(this.lits[0]));
					stb.Append(" ");
				}
				//$NON-NLS-1$
				for (int i = 1; i < this.lits.Length; i++)
				{
					if (this.voc.IsUnassigned(this.lits[i]))
					{
						stb.Append(" + ");
						//$NON-NLS-1$
						stb.Append(Lits.ToString(this.lits[i]));
						stb.Append(" ");
					}
				}
				//$NON-NLS-1$
				stb.Append(">= ");
				//$NON-NLS-1$
				stb.Append(this.degree);
			}
			return stb.ToString();
		}

		/// <summary>M?thode appel?e lors du backtrack</summary>
		/// <param name="p">le litt?ral d?saffect?</param>
		public void Undo(int p)
		{
			this.watchCumul++;
		}

		public void SetLearnt()
		{
			throw new NotSupportedException();
		}

		public void Register()
		{
			throw new NotSupportedException();
		}

		public int Size()
		{
			return this.lits.Length;
		}

		public int Get(int i)
		{
			return this.lits[i];
		}

		public void AssertConstraint(UnitPropagationListener s)
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

		public void AssertConstraintIfNeeded(UnitPropagationListener s)
		{
			throw new NotSupportedException();
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.constraints.pb.PBConstr#getCoefficient(int)
		*/
		public BigInteger GetCoef(int literal)
		{
			return BigInteger.One;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.constraints.pb.PBConstr#getDegree()
		*/
		public BigInteger GetDegree()
		{
			return BigInteger.ValueOf(this.degree);
		}

		public ILits GetVocabulary()
		{
			return this.voc;
		}

		/// <since>2.1</since>
		public void ForwardActivity(double claInc)
		{
		}

		// TODO Auto-generated method stub
		public bool CanBePropagatedMultipleTimes()
		{
			return false;
		}

		public Constr ToConstraint()
		{
			return this;
		}

		public void CalcReasonOnTheFly(int p, IVecInt trail, IVecInt outReason)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public bool PropagatePI(MandatoryLiteralListener l, int p)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public bool CanBeSatisfiedByCountingLiterals()
		{
			return true;
		}

		public int RequiredNumberOfSatisfiedLiterals()
		{
			return degree;
		}

		public bool IsSatisfied()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public int GetAssertionLevel(IVecInt trail, int decisionLevel)
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
					if (nUnsat == this.lits.Length - this.degree)
					{
						return i_1;
					}
				}
			}
			return -1;
		}

		public string ToString(VarMapper mapper)
		{
			StringBuilder stb = new StringBuilder();
			if (this.lits.Length > 0)
			{
				if (this.voc.IsUnassigned(this.lits[0]))
				{
					stb.Append(mapper.Map(LiteralsUtils.ToDimacs(this.lits[0])));
					stb.Append(" ");
				}
				//$NON-NLS-1$
				for (int i = 1; i < this.lits.Length; i++)
				{
					if (this.voc.IsUnassigned(this.lits[i]))
					{
						stb.Append(" + ");
						//$NON-NLS-1$
						stb.Append(mapper.Map(LiteralsUtils.ToDimacs(this.lits[i])));
						stb.Append(" ");
					}
				}
				//$NON-NLS-1$
				stb.Append(">= ");
				//$NON-NLS-1$
				stb.Append(this.degree);
			}
			return stb.ToString();
		}
	}
}
