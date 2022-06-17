using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	[System.Serializable]
	public class OptToSatAdapter : SolverDecorator<ISolver>
	{
		private const long serialVersionUID = 1L;

		private readonly IOptimizationProblem problem;

		private readonly IVecInt assumps = new VecInt();

		private long begin;

		private readonly SolutionFoundListener sfl;

		public OptToSatAdapter(IOptimizationProblem problem)
			: this(problem, SolutionFoundListenerConstants.Void)
		{
		}

		public OptToSatAdapter(IOptimizationProblem problem, SolutionFoundListener sfl)
			: base((ISolver)problem)
		{
			this.problem = problem;
			this.sfl = sfl;
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override bool IsSatisfiable()
		{
			return IsSatisfiable(VecInt.Empty);
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override bool IsSatisfiable(bool global)
		{
			return IsSatisfiable();
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override bool IsSatisfiable(IVecInt myAssumps, bool global)
		{
			return IsSatisfiable(myAssumps);
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override bool IsSatisfiable(IVecInt myAssumps)
		{
			this.assumps.Clear();
			myAssumps.CopyTo(this.assumps);
			this.begin = Runtime.CurrentTimeMillis();
			if (this.problem.HasNoObjectiveFunction())
			{
				return this.problem.IsSatisfiable(myAssumps);
			}
			if (!this.problem.AdmitABetterSolution(myAssumps))
			{
				return false;
			}
			try
			{
				do
				{
					sfl.OnSolutionFound(this.problem.Model());
					this.problem.DiscardCurrentSolution();
					if (IsVerbose())
					{
						System.Console.Out.WriteLine(GetLogPrefix() + "Current objective function value: " + this.problem.GetObjectiveValue() + "(" + (Runtime.CurrentTimeMillis() - this.begin) / 1000.0 + "s)");
					}
				}
				while (this.problem.AdmitABetterSolution(myAssumps));
				ExpireTimeout();
				sfl.OnUnsatTermination();
			}
			catch (TimeoutException)
			{
				if (IsVerbose())
				{
					System.Console.Out.WriteLine(GetLogPrefix() + "Solver timed out after " + (Runtime.CurrentTimeMillis() - this.begin) / 1000.0 + "s)");
				}
			}
			catch (ContradictionException)
			{
				ExpireTimeout();
				sfl.OnUnsatTermination();
			}
			return true;
		}

		public override int[] Model()
		{
			return this.problem.Model();
		}

		public override bool Model(int var)
		{
			return this.problem.Model(var);
		}

		public override int[] ModelWithInternalVariables()
		{
			return Decorated().ModelWithInternalVariables();
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override int[] FindModel()
		{
			if (IsSatisfiable())
			{
				return Model();
			}
			return null;
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override int[] FindModel(IVecInt assumps)
		{
			if (IsSatisfiable(assumps))
			{
				return Model();
			}
			return null;
		}

		public override string ToString(string prefix)
		{
			return prefix + "Optimization to SAT adapter\n" + base.ToString(prefix);
		}

		/// <summary>
		/// Allow to easily check is the solution returned by isSatisfiable is
		/// optimal or not.
		/// </summary>
		/// <returns>true is the solution found is indeed optimal.</returns>
		public virtual bool IsOptimal()
		{
			return this.problem.IsOptimal();
		}
	}
}
