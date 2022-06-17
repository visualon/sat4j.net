using System.Collections.Generic;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;
using Sharpen.Logging;

namespace Org.Sat4j.Tools
{
	public class CheckMUSSolutionListener : SolutionFoundListener
	{
		private IList<IVecInt> clauses;

		private string explain;

		private readonly ASolverFactory<ISolver> factory;

		public CheckMUSSolutionListener(ASolverFactory<ISolver> factory)
		{
			this.clauses = new AList<IVecInt>();
			this.factory = factory;
		}

		public virtual void AddOriginalClause(IVecInt clause)
		{
			IVecInt newClause = new VecInt(clause.Size());
			if (clauses == null)
			{
				this.clauses = new AList<IVecInt>();
			}
			clause.CopyTo(newClause);
			clauses.Add(newClause);
		}

		/// <param name="mus">containing the clauses identifiers</param>
		/// <returns>true if mus is really minimal unsatisfiable.</returns>
		public virtual bool CheckThatItIsAMUS(IVecInt mus)
		{
			bool result = false;
			ISolver solver = factory.DefaultSolver();
			try
			{
				for (int i = 0; i < mus.Size(); i++)
				{
					solver.AddClause(clauses[mus.Get(i) - 1]);
				}
				result = !solver.IsSatisfiable();
				if (!result)
				{
					explain = "The set of clauses in the MUS is SAT : " + Arrays.ToString(solver.Model());
					return result;
				}
			}
			catch (ContradictionException e)
			{
				Logger.GetLogger("org.sat4j.core").Log(Level.Info, "Trivial inconsistency", e);
				result = true;
			}
			catch (TimeoutException e)
			{
				Logger.GetLogger("org.sat4j.core").Log(Level.Info, "Timeout when checking unsatisfiability", e);
			}
			try
			{
				for (int i = 0; i < mus.Size(); i++)
				{
					solver = factory.DefaultSolver();
					for (int j = 0; j < mus.Size(); j++)
					{
						if (j != i)
						{
							solver.AddClause(clauses[mus.Get(j) - 1]);
						}
					}
					result = result && solver.IsSatisfiable();
					if (!result)
					{
						explain = "The subset of clauses in the MUS not containing clause " + (i + 1) + " is SAT : " + Arrays.ToString(solver.Model());
						return result;
					}
				}
			}
			catch (ContradictionException e)
			{
				Logger.GetLogger("org.sat4j.core").Log(Level.Info, "Trivial inconsistency", e);
				result = false;
			}
			catch (TimeoutException e)
			{
				Logger.GetLogger("org.sat4j.core").Log(Level.Info, "Timeout when checking satisfiability", e);
			}
			return result;
		}

		public virtual void OnSolutionFound(int[] solution)
		{
		}

		public virtual void OnSolutionFound(IVecInt solution)
		{
			if (CheckThatItIsAMUS(solution))
			{
				System.Console.Out.WriteLine(solution + " is a MUS");
			}
			else
			{
				System.Console.Out.WriteLine(solution + " is not a MUS \n" + explain);
			}
		}

		public virtual void OnUnsatTermination()
		{
		}
		// do nothing
	}
}
