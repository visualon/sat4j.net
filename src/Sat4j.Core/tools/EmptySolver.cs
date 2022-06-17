using System;
using System.Collections.Generic;
using System.IO;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>
	/// Empty solver meant to be specialized to be used instead of real solvers
	/// inside e.g.
	/// </summary>
	/// <remarks>
	/// Empty solver meant to be specialized to be used instead of real solvers
	/// inside e.g. parsers.
	/// </remarks>
	/// <author>leberre</author>
	/// <since>2.3.6</since>
	[System.Serializable]
	public abstract class EmptySolver : ISolver
	{
		private const long serialVersionUID = 1L;

		private sealed class _IConstr_63 : IConstr
		{
			public _IConstr_63()
			{
			}

			public string ToString(VarMapper mapper)
			{
				throw new NotSupportedException("Not implemented yet!");
			}

			public int Size()
			{
				throw new NotSupportedException("Not implemented yet!");
			}

			public bool Learnt()
			{
				throw new NotSupportedException("Not implemented yet!");
			}

			public double GetActivity()
			{
				throw new NotSupportedException("Not implemented yet!");
			}

			public int Get(int i)
			{
				throw new NotSupportedException("Not implemented yet!");
			}

			public bool CanBePropagatedMultipleTimes()
			{
				throw new NotSupportedException("Not implemented yet!");
			}
		}

		private readonly IConstr Fakeconstr = new _IConstr_63();

		private int nbVars;

		private int nbClauses;

		public virtual int[] Model()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual int[] PrimeImplicant()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual bool PrimeImplicant(int p)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(IVecInt assumps, bool globalTimeout)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(bool globalTimeout)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(IVecInt assumps)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual int[] FindModel()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual int[] FindModel(IVecInt assumps)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual int NConstraints()
		{
			return nbClauses;
		}

		public virtual int NewVar(int howmany)
		{
			this.nbVars = howmany;
			return howmany;
		}

		public virtual int NVars()
		{
			return nbVars;
		}

		public virtual void PrintInfos(PrintWriter @out, string prefix)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void PrintInfos(PrintWriter @out)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual bool Model(int var)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual int NewVar()
		{
			nbVars++;
			return nbVars;
		}

		public virtual int NextFreeVarId(bool reserve)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void RegisterLiteral(int p)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void SetExpectedNumberOfClauses(int nb)
		{
			this.nbClauses = nb;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddClause(IVecInt literals)
		{
			return Fakeconstr;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddBlockingClause(IVecInt literals)
		{
			return Fakeconstr;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr DiscardCurrentModel()
		{
			return Fakeconstr;
		}

		public virtual IVecInt CreateBlockingClauseForCurrentModel()
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual bool RemoveConstr(IConstr c)
		{
			return false;
		}

		public virtual bool RemoveSubsumedConstr(IConstr c)
		{
			return false;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual void AddAllClauses(IVec<IVecInt> clauses)
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddAtMost(IVecInt literals, int degree)
		{
			return Fakeconstr;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddAtLeast(IVecInt literals, int degree)
		{
			return Fakeconstr;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddExactly(IVecInt literals, int n)
		{
			return Fakeconstr;
		}

		public virtual IConstr AddConstr(Constr constr)
		{
			return Fakeconstr;
		}

		public virtual void SetTimeout(int t)
		{
		}

		public virtual void SetTimeoutOnConflicts(int count)
		{
		}

		public virtual void SetTimeoutMs(long t)
		{
		}

		public virtual int GetTimeout()
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual long GetTimeoutMs()
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void ExpireTimeout()
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void Reset()
		{
		}

		public virtual void PrintStat(TextWriter @out, string prefix)
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void PrintStat(PrintWriter @out, string prefix)
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void PrintStat(PrintWriter @out)
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual IDictionary<string, Number> GetStat()
		{
			// TODO: implement this method !
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual string ToString(string prefix)
		{
			return "Empty Solver";
		}

		public virtual void ClearLearntClauses()
		{
		}

		public virtual void SetDBSimplificationAllowed(bool status)
		{
		}

		public virtual bool IsDBSimplificationAllowed()
		{
			return false;
		}

		public virtual void SetSearchListener<S>(SearchListener<S> sl)
			where S : ISolverService
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void SetUnitClauseProvider(UnitClauseProvider ucp)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual SearchListener<S> GetSearchListener<S>()
			where S : ISolverService
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual bool IsVerbose()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void SetVerbose(bool value)
		{
		}

		public virtual void SetLogPrefix(string prefix)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual string GetLogPrefix()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual IVecInt UnsatExplanation()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual int[] ModelWithInternalVariables()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual int RealNumberOfVariables()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual bool IsSolverKeptHot()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual void SetKeepSolverHot(bool keepHot)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public virtual ISolver GetSolvingEngine()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public abstract IConstr AddParity(IVecInt arg1, bool arg2);
	}
}
