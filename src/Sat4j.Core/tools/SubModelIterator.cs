using System;
using System.Collections.Generic;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>
	/// That class allows to iterate through all the models (implicants) of a
	/// formula.
	/// </summary>
	/// <remarks>
	/// That class allows to iterate through all the models (implicants) of a
	/// formula.
	/// <pre>
	/// IVecInt subsetVars = new VecInt();
	/// ISolver solver = new SubModelIterator(SolverFactory.OneSolver(), subsetVars);
	/// boolean unsat = true;
	/// while (solver.isSatisfiable()) {
	/// unsat = false;
	/// int[] model = solver.model();
	/// // do something with the submodel
	/// }
	/// if (unsat) {
	/// // UNSAT case
	/// }
	/// </pre>
	/// It is also possible to limit the number of models returned:
	/// <pre>
	/// ISolver solver = new OneModelIterator(SolverFactory.OneSolver(), subsetVars, 10);
	/// </pre>
	/// will return at most 10 submodels.
	/// </remarks>
	/// <author>leberre</author>
	/// <since>2.3.6</since>
	[System.Serializable]
	public class SubModelIterator : ModelIterator
	{
		private const long serialVersionUID = 1L;

		private readonly ICollection<int> subsetVars;

		/// <summary>Create an iterator over the solutions available in <code>solver</code>.</summary>
		/// <remarks>
		/// Create an iterator over the solutions available in <code>solver</code>.
		/// The iterator will look for one new model at each call to isSatisfiable()
		/// and will discard that model at each call to model().
		/// </remarks>
		/// <param name="solver">a solver containing the constraints to satisfy.</param>
		/// <seealso cref="ModelIterator.IsSatisfiable()"/>
		/// <seealso cref="ModelIterator.IsSatisfiable(bool)"/>
		/// <seealso cref="ModelIterator.IsSatisfiable(Org.Sat4j.Specs.IVecInt)"/>
		/// <seealso cref="SolverDecorator{T}.IsSatisfiable(Org.Sat4j.Specs.IVecInt, bool)"/>
		/// <seealso cref="Model()"/>
		public SubModelIterator(ISolver solver, IVecInt subsetVars)
			: this(solver, subsetVars, long.MaxValue)
		{
		}

		/// <summary>
		/// Create an iterator over a limited number of solutions available in
		/// <code>solver</code>.
		/// </summary>
		/// <remarks>
		/// Create an iterator over a limited number of solutions available in
		/// <code>solver</code>. The iterator will look for one new model at each
		/// call to isSatisfiable() and will discard that model at each call to
		/// model(). At most <code>bound</code> calls to models() will be allowed
		/// before the method <code>isSatisfiable()</code> returns false.
		/// </remarks>
		/// <param name="solver">a solver containing the constraints to satisfy.</param>
		/// <param name="bound">the maximum number of models to return.</param>
		/// <seealso cref="ModelIterator.IsSatisfiable()"/>
		/// <seealso cref="ModelIterator.IsSatisfiable(bool)"/>
		/// <seealso cref="ModelIterator.IsSatisfiable(Org.Sat4j.Specs.IVecInt)"/>
		/// <seealso cref="SolverDecorator{T}.IsSatisfiable(Org.Sat4j.Specs.IVecInt, bool)"/>
		/// <seealso cref="Model()"/>
		public SubModelIterator(ISolver solver, IVecInt subsetVars, long bound)
			: base(solver, bound)
		{
			this.subsetVars = new TreeSet<int>();
			for (IteratorInt it = subsetVars.Iterator(); it.HasNext(); )
			{
				this.subsetVars.Add(it.Next());
			}
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#model()
		*/
		public override int[] Model()
		{
			int[] last = base.Model();
			this.nbModelFound++;
			int[] sub = new int[subsetVars.Count];
			IVecInt clause = new VecInt(sub.Length);
			int var;
			int i = 0;
			foreach (int q in last)
			{
				var = Math.Abs(q);
				if (subsetVars.Contains(var))
				{
					clause.Push(-q);
					sub[i++] = q;
				}
			}
			try
			{
				AddBlockingClause(clause);
			}
			catch (ContradictionException)
			{
				this.trivialfalsity = true;
			}
			return sub;
		}

		public override int[] PrimeImplicant()
		{
			throw new NotSupportedException();
		}
	}
}
