using System;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>
	/// Computes models with a minimal number (with respect to cardinality) of
	/// negative literals.
	/// </summary>
	/// <remarks>
	/// Computes models with a minimal number (with respect to cardinality) of
	/// negative literals. This is done be adding a constraint on the number of
	/// negative literals each time a model if found (the number of negative literals
	/// occuring in the model minus one).
	/// </remarks>
	/// <author>leberre</author>
	/// <seealso cref="Org.Sat4j.Specs.ISolver.AddAtMost(Org.Sat4j.Specs.IVecInt, int)"/>
	[System.Serializable]
	public class Minimal4CardinalityModel : AbstractMinimalModel
	{
		private const long serialVersionUID = 1L;

		private int[] prevfullmodel;

		/// <param name="solver"/>
		public Minimal4CardinalityModel(ISolver solver)
			: base(solver)
		{
		}

		public Minimal4CardinalityModel(ISolver solver, IVecInt p, SolutionFoundListener modelListener)
			: base(solver, p, modelListener)
		{
		}

		public Minimal4CardinalityModel(ISolver solver, IVecInt p)
			: base(solver, p)
		{
		}

		public Minimal4CardinalityModel(ISolver solver, SolutionFoundListener modelListener)
			: base(solver, modelListener)
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
			IConstr lastOne = null;
			IVecInt literals = new VecInt(pLiterals.Count);
			foreach (int p in pLiterals)
			{
				literals.Push(p);
			}
			try
			{
				do
				{
					prevfullmodel = base.ModelWithInternalVariables();
					prevmodel = base.Model();
					int counter = 0;
					foreach (int q in prevfullmodel)
					{
						if (pLiterals.Contains(q))
						{
							counter++;
						}
					}
					lastOne = AddAtMost(literals, counter - 1);
				}
				while (IsSatisfiable());
			}
			catch (TimeoutException)
			{
				throw new InvalidOperationException("Solver timed out");
			}
			catch (ContradictionException)
			{
			}
			//$NON-NLS-1$
			// added trivial unsat clauses
			if (lastOne != null)
			{
				RemoveConstr(lastOne);
			}
			return prevmodel;
		}

		public override int[] ModelWithInternalVariables()
		{
			Model();
			return prevfullmodel;
		}
	}
}
