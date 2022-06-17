using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Org.Sat4j.Tools;
using Sharpen;

namespace Org.Sat4j.Opt
{
	/// <summary>
	/// Abstract class which adds a new "selector" variable for each clause entered
	/// in the solver.
	/// </summary>
	/// <remarks>
	/// Abstract class which adds a new "selector" variable for each clause entered
	/// in the solver.
	/// As a consequence, an original problem with n variables and m clauses will end
	/// up with n+m variables.
	/// </remarks>
	/// <author>daniel</author>
	[System.Serializable]
	public abstract class AbstractSelectorVariablesDecorator : SolverDecorator<ISolver>, IOptimizationProblem
	{
		private const long serialVersionUID = 1L;

		private int nbexpectedclauses;

		private int[] prevfullmodel;

		/// <since>2.1</since>
		private int[] prevmodel;

		/// <since>2.1</since>
		private bool[] prevboolmodel;

		private bool isSolutionOptimal;

		/// <since>2.3.6</since>
		private IVecInt prevBlockingClause;

		public AbstractSelectorVariablesDecorator(ISolver solver)
			: base(solver)
		{
		}

		public override void SetExpectedNumberOfClauses(int nb)
		{
			this.nbexpectedclauses = nb;
		}

		public virtual int GetExpectedNumberOfClauses()
		{
			return this.nbexpectedclauses;
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool AdmitABetterSolution()
		{
			return AdmitABetterSolution(VecInt.Empty);
		}

		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool AdmitABetterSolution(IVecInt assumps)
		{
			this.isSolutionOptimal = false;
			bool result = base.IsSatisfiable(assumps, true);
			if (result)
			{
				this.prevboolmodel = new bool[NVars()];
				for (int i = 0; i < NVars(); i++)
				{
					this.prevboolmodel[i] = Decorated().Model(i + 1);
				}
				this.prevfullmodel = base.ModelWithInternalVariables();
				this.prevmodel = base.Model();
				this.prevBlockingClause = base.CreateBlockingClauseForCurrentModel();
				CalculateObjectiveValue();
			}
			else
			{
				this.isSolutionOptimal = true;
			}
			return result;
		}

		internal abstract void CalculateObjectiveValue();

		public override int[] Model()
		{
			return this.prevmodel;
		}

		public override bool Model(int var)
		{
			return this.prevboolmodel[var - 1];
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override IConstr DiscardCurrentModel()
		{
			return AddBlockingClause(this.prevBlockingClause);
		}

		public override IVecInt CreateBlockingClauseForCurrentModel()
		{
			return this.prevBlockingClause;
		}

		public virtual bool IsOptimal()
		{
			return this.isSolutionOptimal;
		}

		public virtual int GetNbexpectedclauses()
		{
			return nbexpectedclauses;
		}

		public virtual void SetNbexpectedclauses(int nbexpectedclauses)
		{
			this.nbexpectedclauses = nbexpectedclauses;
		}

		public virtual int[] GetPrevfullmodel()
		{
			return prevfullmodel;
		}

		public virtual void SetPrevfullmodel(int[] prevfullmodel)
		{
			this.prevfullmodel = prevfullmodel.MemberwiseClone();
		}

		public virtual int[] GetPrevmodel()
		{
			return prevmodel;
		}

		public virtual void SetPrevmodel(int[] prevmodel)
		{
			this.prevmodel = prevmodel.MemberwiseClone();
		}

		public virtual bool[] GetPrevboolmodel()
		{
			return prevboolmodel;
		}

		public virtual void SetPrevboolmodel(bool[] prevboolmodel)
		{
			this.prevboolmodel = prevboolmodel.MemberwiseClone();
		}

		public virtual bool IsSolutionOptimal()
		{
			return isSolutionOptimal;
		}

		public virtual void SetSolutionOptimal(bool isSolutionOptimal)
		{
			this.isSolutionOptimal = isSolutionOptimal;
		}

		public abstract Number CalculateObjective();

		public abstract void Discard();

		public abstract void DiscardCurrentSolution();

		public abstract void ForceObjectiveValueTo(Number arg1);

		public abstract Number GetObjectiveValue();

		public abstract bool HasNoObjectiveFunction();

		public abstract bool NonOptimalMeansSatisfiable();

		public abstract void SetTimeoutForFindingBetterSolution(int arg1);
	}
}
