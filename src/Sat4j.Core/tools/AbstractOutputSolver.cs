using System;
using System.Collections.Generic;
using System.IO;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	[System.Serializable]
	public abstract class AbstractOutputSolver : ISolver
	{
		protected internal int nbvars;

		protected internal int nbclauses;

		protected internal bool fixedNbClauses = false;

		protected internal bool firstConstr = true;

		private bool verbose;

		private const long serialVersionUID = 1L;

		public virtual bool RemoveConstr(IConstr c)
		{
			throw new NotSupportedException();
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual void AddAllClauses(IVec<IVecInt> clauses)
		{
			throw new NotSupportedException();
		}

		public virtual void SetTimeout(int t)
		{
		}

		// TODO Auto-generated method stub
		public virtual void SetTimeoutMs(long t)
		{
		}

		// TODO Auto-generated method stub
		public virtual int GetTimeout()
		{
			return 0;
		}

		/// <since>2.1</since>
		public virtual long GetTimeoutMs()
		{
			return 0L;
		}

		public virtual void ExpireTimeout()
		{
		}

		// TODO Auto-generated method stub
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(IVecInt assumps, bool global)
		{
			throw new TimeoutException("There is no real solver behind!");
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(bool global)
		{
			throw new TimeoutException("There is no real solver behind!");
		}

		public virtual void PrintInfos(PrintWriter output, string prefix)
		{
		}

		public virtual void SetTimeoutOnConflicts(int count)
		{
		}

		public virtual bool IsDBSimplificationAllowed()
		{
			return false;
		}

		public virtual void SetDBSimplificationAllowed(bool status)
		{
		}

		public virtual void PrintStat(TextWriter output, string prefix)
		{
		}

		// TODO Auto-generated method stub
		public virtual void PrintStat(PrintWriter output, string prefix)
		{
		}

		// TODO Auto-generated method stub
		public virtual IDictionary<string, Number> GetStat()
		{
			// TODO Auto-generated method stub
			return null;
		}

		public virtual void ClearLearntClauses()
		{
		}

		// TODO Auto-generated method stub
		public virtual int[] Model()
		{
			throw new NotSupportedException();
		}

		public virtual bool Model(int var)
		{
			throw new NotSupportedException();
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable()
		{
			throw new TimeoutException("There is no real solver behind!");
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(IVecInt assumps)
		{
			throw new TimeoutException("There is no real solver behind!");
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual int[] FindModel()
		{
			throw new NotSupportedException();
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual int[] FindModel(IVecInt assumps)
		{
			throw new NotSupportedException();
		}

		/// <since>2.1</since>
		public virtual bool RemoveSubsumedConstr(IConstr c)
		{
			return false;
		}

		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddBlockingClause(IVecInt literals)
		{
			throw new NotSupportedException();
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr DiscardCurrentModel()
		{
			throw new NotSupportedException();
		}

		public virtual IVecInt CreateBlockingClauseForCurrentModel()
		{
			throw new NotSupportedException();
		}

		/// <since>2.2</since>
		public virtual SearchListener<S> GetSearchListener<S>()
			where S : ISolverService
		{
			return null;
		}

		/// <since>2.1</since>
		public virtual void SetSearchListener<S>(SearchListener<S> sl)
			where S : ISolverService
		{
		}

		/// <since>2.2</since>
		public virtual bool IsVerbose()
		{
			return this.verbose;
		}

		/// <since>2.2</since>
		public virtual void SetVerbose(bool value)
		{
			this.verbose = value;
		}

		/// <since>2.2</since>
		public virtual void SetLogPrefix(string prefix)
		{
		}

		// do nothing
		/// <since>2.2</since>
		public virtual string GetLogPrefix()
		{
			return string.Empty;
		}

		/// <since>2.2</since>
		public virtual IVecInt UnsatExplanation()
		{
			throw new NotSupportedException();
		}

		public virtual int[] PrimeImplicant()
		{
			throw new NotSupportedException();
		}

		public virtual int NConstraints()
		{
			// TODO Auto-generated method stub
			return 0;
		}

		public virtual int NewVar(int howmany)
		{
			// TODO Auto-generated method stub
			return 0;
		}

		public virtual int NVars()
		{
			// TODO Auto-generated method stub
			return 0;
		}

		public virtual bool IsSolverKeptHot()
		{
			return false;
		}

		public virtual void SetKeepSolverHot(bool value)
		{
		}

		public virtual ISolver GetSolvingEngine()
		{
			throw new NotSupportedException();
		}

		public virtual void SetUnitClauseProvider(UnitClauseProvider upl)
		{
			throw new NotSupportedException();
		}

		public virtual IConstr AddConstr(Constr constr)
		{
			throw new NotSupportedException();
		}

		public abstract bool PrimeImplicant(int arg1);

		public abstract void PrintInfos(PrintWriter arg1);

		public abstract IConstr AddAtLeast(IVecInt arg1, int arg2);

		public abstract IConstr AddAtMost(IVecInt arg1, int arg2);

		public abstract IConstr AddClause(IVecInt arg1);

		public abstract IConstr AddExactly(IVecInt arg1, int arg2);

		public abstract IConstr AddParity(IVecInt arg1, bool arg2);

		public abstract int[] ModelWithInternalVariables();

		public abstract int NewVar();

		public abstract int NextFreeVarId(bool arg1);

		public abstract void PrintStat(PrintWriter arg1);

		public abstract int RealNumberOfVariables();

		public abstract void RegisterLiteral(int arg1);

		public abstract void Reset();

		public abstract void SetExpectedNumberOfClauses(int arg1);

		public abstract string ToString(string arg1);
	}
}
