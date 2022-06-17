using System;
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
	/// ISolver solver = new ModelIterator(SolverFactory.OneSolver());
	/// boolean unsat = true;
	/// while (solver.isSatisfiable()) {
	/// unsat = false;
	/// int[] model = solver.model();
	/// // do something with model
	/// }
	/// if (unsat) {
	/// // UNSAT case
	/// }
	/// </pre>
	/// It is also possible to limit the number of models returned:
	/// <pre>
	/// ISolver solver = new ModelIterator(SolverFactory.OneSolver(), 10);
	/// </pre>
	/// will return at most 10 models.
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public class ModelIterator : SolverDecorator<ISolver>
	{
		private const long serialVersionUID = 1L;

		protected internal bool trivialfalsity = false;

		private readonly long bound;

		protected internal long nbModelFound = 0;

		/// <summary>Create an iterator over the solutions available in <code>solver</code>.</summary>
		/// <remarks>
		/// Create an iterator over the solutions available in <code>solver</code>.
		/// The iterator will look for one new model at each call to isSatisfiable()
		/// and will discard that model at each call to model().
		/// </remarks>
		/// <param name="solver">a solver containing the constraints to satisfy.</param>
		/// <seealso cref="IsSatisfiable()"/>
		/// <seealso cref="IsSatisfiable(bool)"/>
		/// <seealso cref="IsSatisfiable(Org.Sat4j.Specs.IVecInt)"/>
		/// <seealso cref="SolverDecorator{T}.IsSatisfiable(Org.Sat4j.Specs.IVecInt, bool)"/>
		/// <seealso cref="Model()"/>
		public ModelIterator(ISolver solver)
			: this(solver, long.MaxValue)
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
		/// <since>2.1</since>
		/// <seealso cref="IsSatisfiable()"/>
		/// <seealso cref="IsSatisfiable(bool)"/>
		/// <seealso cref="IsSatisfiable(Org.Sat4j.Specs.IVecInt)"/>
		/// <seealso cref="SolverDecorator{T}.IsSatisfiable(Org.Sat4j.Specs.IVecInt, bool)"/>
		/// <seealso cref="Model()"/>
		public ModelIterator(ISolver solver, long bound)
			: base(solver)
		{
			this.bound = bound;
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
			try
			{
				DiscardCurrentModel();
			}
			catch (ContradictionException)
			{
				this.trivialfalsity = true;
			}
			return last;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#isSatisfiable()
		*/
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override bool IsSatisfiable()
		{
			if (this.trivialfalsity || this.nbModelFound >= this.bound)
			{
				ExpireTimeout();
				return false;
			}
			this.trivialfalsity = false;
			return base.IsSatisfiable(true);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#isSatisfiable(org.sat4j.datatype.VecInt)
		*/
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override bool IsSatisfiable(IVecInt assumps)
		{
			if (this.trivialfalsity || this.nbModelFound >= this.bound)
			{
				return false;
			}
			this.trivialfalsity = false;
			return base.IsSatisfiable(assumps, true);
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override bool IsSatisfiable(bool global)
		{
			return IsSatisfiable();
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#reset()
		*/
		public override void Reset()
		{
			this.trivialfalsity = false;
			this.nbModelFound = 0;
			base.Reset();
		}

		public override int[] PrimeImplicant()
		{
			int[] last = base.PrimeImplicant();
			this.nbModelFound += Math.Pow(2, NVars() - last.Length);
			IVecInt clause = new VecInt(last.Length);
			foreach (int q in last)
			{
				clause.Push(-q);
			}
			try
			{
				AddBlockingClause(clause);
			}
			catch (ContradictionException)
			{
				this.trivialfalsity = true;
			}
			return last;
		}

		/// <summary>To know the number of models already found.</summary>
		/// <returns>the number of models found so far.</returns>
		/// <since>2.3</since>
		public virtual long NumberOfModelsFoundSoFar()
		{
			return this.nbModelFound;
		}
	}
}
