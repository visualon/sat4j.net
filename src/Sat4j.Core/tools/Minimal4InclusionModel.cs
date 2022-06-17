using System;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>
	/// Computes models with a minimal subset (with respect to set inclusion) of
	/// negative literals.
	/// </summary>
	/// <remarks>
	/// Computes models with a minimal subset (with respect to set inclusion) of
	/// negative literals. This is done be adding a clause containing the negation of
	/// the negative literals appearing in the model found (which prevents any
	/// interpretation containing that subset of negative literals to be a model of
	/// the formula).
	/// Computes only one model minimal for inclusion, since there is currently no
	/// way to save the state of the solver.
	/// </remarks>
	/// <author>leberre</author>
	/// <seealso cref="Org.Sat4j.Specs.ISolver.AddClause(Org.Sat4j.Specs.IVecInt)"/>
	[System.Serializable]
	public class Minimal4InclusionModel : AbstractMinimalModel
	{
		private const long serialVersionUID = 1L;

		private int[] prevfullmodel;

		/// <param name="solver"/>
		/// <param name="p">
		/// the set of literals on which the minimality for inclusion is
		/// computed.
		/// </param>
		/// <param name="modelListener">an object to be notified when a new model is found.</param>
		public Minimal4InclusionModel(ISolver solver, IVecInt p, SolutionFoundListener modelListener)
			: base(solver, p, modelListener)
		{
		}

		/// <param name="solver"/>
		/// <param name="p">
		/// the set of literals on which the minimality for inclusion is
		/// computed.
		/// </param>
		public Minimal4InclusionModel(ISolver solver, IVecInt p)
			: this(solver, p, SolutionFoundListenerConstants.Void)
		{
		}

		/// <param name="solver"/>
		public Minimal4InclusionModel(ISolver solver)
			: this(solver, NegativeLiterals(solver), SolutionFoundListenerConstants.Void)
		{
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#model()
		*/
		public override int[] Model()
		{
			int[] prevmodel = null;
			IVecInt vec = new VecInt();
			IVecInt cube = new VecInt();
			IConstr prevConstr = null;
			try
			{
				do
				{
					prevfullmodel = base.ModelWithInternalVariables();
					prevmodel = base.Model();
					modelListener.OnSolutionFound(prevmodel);
					vec.Clear();
					cube.Clear();
					assumps.CopyTo(cube);
					foreach (int q in prevfullmodel)
					{
						if (pLiterals.Contains(q))
						{
							vec.Push(-q);
						}
						else
						{
							if (pLiterals.Contains(-q))
							{
								cube.Push(q);
							}
						}
					}
					if (prevConstr != null)
					{
						RemoveSubsumedConstr(prevConstr);
					}
					try
					{
						prevConstr = AddBlockingClause(vec);
					}
					catch (ContradictionException)
					{
						// added trivial unsat clauses
						break;
					}
				}
				while (IsSatisfiable(cube));
			}
			catch (TimeoutException)
			{
				throw new InvalidOperationException("Solver timed out");
			}
			return prevmodel;
		}

		public override int[] ModelWithInternalVariables()
		{
			Model();
			return prevfullmodel;
		}

		private IVecInt assumps = VecInt.Empty;

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override bool IsSatisfiable(IVecInt assumps)
		{
			this.assumps = assumps;
			return base.IsSatisfiable(assumps);
		}
	}
}
