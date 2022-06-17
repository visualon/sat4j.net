using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Cnf
{
	/// <author>daniel</author>
	/// <since>2.1</since>
	public abstract class Clauses
	{
		/// <summary>
		/// Perform some sanity check before constructing a clause a) if a literal is
		/// assigned true, return null (the clause is satisfied) b) if a literal is
		/// assigned false, remove it c) if a clause contains a literal and its
		/// opposite (tautology) return null d) remove duplicate literals e) if the
		/// clause is empty, return null f) if the clause if unit, transmit it to the
		/// object responsible for unit propagation
		/// </summary>
		/// <param name="ps">the list of literals</param>
		/// <param name="voc">the vocabulary used</param>
		/// <param name="s">the object responsible for unit propagation</param>
		/// <returns>
		/// null if the clause should be ignored, the (possibly modified)
		/// list of literals otherwise
		/// </returns>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException">if discovered by unit propagation</exception>
		public static IVecInt SanityCheck(IVecInt ps, ILits voc, UnitPropagationListener s)
		{
			// si un litt???ral de ps est vrai, retourner vrai
			// enlever les litt???raux falsifi???s de ps
			for (int i = 0; i < ps.Size(); )
			{
				// on verifie si le litteral est affecte
				if (voc.IsUnassigned(ps.Get(i)))
				{
					// on passe au literal suivant
					i++;
				}
				else
				{
					// Si le litteral est satisfait, la clause est
					// satisfaite
					if (voc.IsSatisfied(ps.Get(i)))
					{
						// on retourne la clause
						return null;
					}
					// on enleve le ieme litteral
					ps.Delete(i);
				}
			}
			// on trie le vecteur ps
			ps.SortUnique();
			// ???limine les clauses tautologiques
			// deux litt???raux de signe oppos???s apparaissent dans la m???me
			// clause
			for (int i_1 = 0; i_1 < ps.Size() - 1; i_1++)
			{
				if (ps.Get(i_1) == (ps.Get(i_1 + 1) ^ 1))
				{
					// la clause est tautologique
					return null;
				}
			}
			PropagationCheck(ps, s);
			return ps;
		}

		/// <summary>Check if this clause is null or unit</summary>
		/// <param name="p">
		/// the list of literals (supposed to be clean as after a call to
		/// sanityCheck())
		/// </param>
		/// <param name="s">the object responsible for unit propagation</param>
		/// <returns>true iff the clause should be ignored (because it's unit)</returns>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException">when detected by unit propagation</exception>
		internal static bool PropagationCheck(IVecInt ps, UnitPropagationListener s)
		{
			if (ps.Size() == 0)
			{
				throw new ContradictionException("Creating Empty clause ?");
			}
			else
			{
				//$NON-NLS-1$
				if (ps.Size() == 1)
				{
					if (!s.Enqueue(ps.Get(0)))
					{
						throw new ContradictionException("Contradictory Unit Clauses");
					}
					//$NON-NLS-1$
					return true;
				}
			}
			return false;
		}
	}
}
