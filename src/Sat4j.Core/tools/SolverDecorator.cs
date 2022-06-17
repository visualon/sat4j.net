using System;
using System.Collections.Generic;
using System.IO;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>
	/// The aim of that class is to allow adding dynamic responsibilities to SAT
	/// solvers using the Decorator design pattern.
	/// </summary>
	/// <remarks>
	/// The aim of that class is to allow adding dynamic responsibilities to SAT
	/// solvers using the Decorator design pattern. The class is abstract because it
	/// does not makes sense to use it "as is".
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public abstract class SolverDecorator<T> : ISolver
		where T : ISolver
	{
		private const long serialVersionUID = 1L;

		public virtual bool IsDBSimplificationAllowed()
		{
			return this.solver.IsDBSimplificationAllowed();
		}

		public virtual void SetDBSimplificationAllowed(bool status)
		{
			this.solver.SetDBSimplificationAllowed(status);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.specs.ISolver#setTimeoutOnConflicts(int)
		*/
		public virtual void SetTimeoutOnConflicts(int count)
		{
			this.solver.SetTimeoutOnConflicts(count);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.specs.IProblem#printInfos(java.io.PrintWriter,
		* java.lang.String)
		*/
		public virtual void PrintInfos(PrintWriter @out, string prefix)
		{
			this.solver.PrintInfos(@out, prefix);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.specs.IProblem#printInfos(java.io.PrintWriter,
		* java.lang.String)
		*/
		public virtual void PrintInfos(PrintWriter @out)
		{
			this.solver.PrintInfos(@out);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.specs.IProblem#isSatisfiable(boolean)
		*/
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(bool global)
		{
			return this.solver.IsSatisfiable(global);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.specs.IProblem#isSatisfiable(org.sat4j.specs.IVecInt,
		* boolean)
		*/
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(IVecInt assumps, bool global)
		{
			return this.solver.IsSatisfiable(assumps, global);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.specs.ISolver#clearLearntClauses()
		*/
		public virtual void ClearLearntClauses()
		{
			this.solver.ClearLearntClauses();
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.specs.IProblem#findModel()
		*/
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual int[] FindModel()
		{
			return this.solver.FindModel();
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.specs.IProblem#findModel(org.sat4j.specs.IVecInt)
		*/
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual int[] FindModel(IVecInt assumps)
		{
			return this.solver.FindModel(assumps);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.specs.IProblem#model(int)
		*/
		public virtual bool Model(int var)
		{
			return this.solver.Model(var);
		}

		public virtual void SetExpectedNumberOfClauses(int nb)
		{
			this.solver.SetExpectedNumberOfClauses(nb);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.specs.ISolver#getTimeout()
		*/
		public virtual int GetTimeout()
		{
			return this.solver.GetTimeout();
		}

		/// <since>2.1</since>
		public virtual long GetTimeoutMs()
		{
			return this.solver.GetTimeoutMs();
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.specs.ISolver#toString(java.lang.String)
		*/
		public virtual string ToString(string prefix)
		{
			return this.solver.ToString(prefix);
		}

		public override string ToString()
		{
			return this.solver.ToString();
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.specs.ISolver#printStat(java.io.PrintStream,
		* java.lang.String)
		*/
		[Obsolete]
		public virtual void PrintStat(TextWriter @out, string prefix)
		{
			this.solver.PrintStat(@out, prefix);
		}

		public virtual void PrintStat(PrintWriter @out, string prefix)
		{
			this.solver.PrintStat(@out, prefix);
		}

		public virtual void PrintStat(PrintWriter @out)
		{
			this.solver.PrintStat(@out);
		}

		private T solver;

		public SolverDecorator(T solver)
		{
			this.solver = solver;
		}

		[Obsolete]
		public virtual int NewVar()
		{
			return this.solver.NewVar();
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#newVar(int)
		*/
		public virtual int NewVar(int howmany)
		{
			return this.solver.NewVar(howmany);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#addClause(org.sat4j.datatype.VecInt)
		*/
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddClause(IVecInt literals)
		{
			return this.solver.AddClause(literals);
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual void AddAllClauses(IVec<IVecInt> clauses)
		{
			this.solver.AddAllClauses(clauses);
		}

		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddBlockingClause(IVecInt literals)
		{
			return this.solver.AddBlockingClause(literals);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#discardCurrentModel()
		*/
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr DiscardCurrentModel()
		{
			return this.solver.DiscardCurrentModel();
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#createBlockingClauseForCurrentModel()
		*/
		public virtual IVecInt CreateBlockingClauseForCurrentModel()
		{
			return this.solver.CreateBlockingClauseForCurrentModel();
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#addAtMost(org.sat4j.datatype.VecInt, int)
		*/
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddAtMost(IVecInt literals, int degree)
		{
			return this.solver.AddAtMost(literals, degree);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#addAtLeast(org.sat4j.datatype.VecInt, int)
		*/
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddAtLeast(IVecInt literals, int degree)
		{
			return this.solver.AddAtLeast(literals, degree);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#model()
		*/
		public virtual int[] Model()
		{
			return this.solver.Model();
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#isSatisfiable()
		*/
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable()
		{
			return this.solver.IsSatisfiable();
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#isSatisfiable(org.sat4j.datatype.VecInt)
		*/
		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(IVecInt assumps)
		{
			return this.solver.IsSatisfiable(assumps);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#setTimeout(int)
		*/
		public virtual void SetTimeout(int t)
		{
			this.solver.SetTimeout(t);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#setTimeoutMs(int)
		*/
		public virtual void SetTimeoutMs(long t)
		{
			this.solver.SetTimeoutMs(t);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#expireTimeout()
		*/
		public virtual void ExpireTimeout()
		{
			this.solver.ExpireTimeout();
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#nConstraints()
		*/
		public virtual int NConstraints()
		{
			return this.solver.NConstraints();
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#nVars()
		*/
		public virtual int NVars()
		{
			return this.solver.NVars();
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.ISolver#reset()
		*/
		public virtual void Reset()
		{
			this.solver.Reset();
		}

		public virtual T Decorated()
		{
			return this.solver;
		}

		/// <summary>Method to be called to clear the decorator from its decorated solver.</summary>
		/// <remarks>
		/// Method to be called to clear the decorator from its decorated solver.
		/// This is especially useful to avoid memory leak in a program.
		/// </remarks>
		/// <returns>the decorated solver.</returns>
		public virtual T ClearDecorated()
		{
			T decorated = this.solver;
			this.solver = default(T);
			return decorated;
		}

		protected internal virtual void SetDecorated(T solver)
		{
			this.solver = solver;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.specs.ISolver#removeConstr(org.sat4j.minisat.core.Constr)
		*/
		public virtual bool RemoveConstr(IConstr c)
		{
			return this.solver.RemoveConstr(c);
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.specs.ISolver#getStat()
		*/
		public virtual IDictionary<string, Number> GetStat()
		{
			return this.solver.GetStat();
		}

		/// <since>2.1</since>
		public virtual void SetSearchListener<S>(SearchListener<S> sl)
			where S : ISolverService
		{
			this.solver.SetSearchListener(sl);
		}

		/// <since>2.1</since>
		public virtual int NextFreeVarId(bool reserve)
		{
			return this.solver.NextFreeVarId(reserve);
		}

		/// <since>2.1</since>
		public virtual bool RemoveSubsumedConstr(IConstr c)
		{
			return this.solver.RemoveSubsumedConstr(c);
		}

		/// <since>2.2</since>
		public virtual SearchListener<S> GetSearchListener<S>()
			where S : ISolverService
		{
			return this.solver.GetSearchListener<S>();
		}

		/// <since>2.2</since>
		public virtual bool IsVerbose()
		{
			return this.solver.IsVerbose();
		}

		/// <since>2.2</since>
		public virtual void SetVerbose(bool value)
		{
			this.solver.SetVerbose(value);
		}

		/// <since>2.2</since>
		public virtual void SetLogPrefix(string prefix)
		{
			this.solver.SetLogPrefix(prefix);
		}

		/// <since>2.2</since>
		public virtual string GetLogPrefix()
		{
			return this.solver.GetLogPrefix();
		}

		/// <since>2.2</since>
		public virtual IVecInt UnsatExplanation()
		{
			return this.solver.UnsatExplanation();
		}

		/// <since>2.3</since>
		public virtual int[] PrimeImplicant()
		{
			return this.solver.PrimeImplicant();
		}

		/// <since>2.3.1</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddExactly(IVecInt literals, int n)
		{
			return this.solver.AddExactly(literals, n);
		}

		/// <since>2.3.1</since>
		public virtual int[] ModelWithInternalVariables()
		{
			return this.solver.ModelWithInternalVariables();
		}

		/// <since>2.3.1</since>
		public virtual int RealNumberOfVariables()
		{
			return this.solver.RealNumberOfVariables();
		}

		/// <since>2.3.1</since>
		public virtual void RegisterLiteral(int p)
		{
			this.solver.RegisterLiteral(p);
		}

		/// <since>2.3.2</since>
		public virtual bool IsSolverKeptHot()
		{
			return this.solver.IsSolverKeptHot();
		}

		/// <since>2.3.2</since>
		public virtual void SetKeepSolverHot(bool value)
		{
			this.solver.SetKeepSolverHot(value);
		}

		/// <since>2.3.2</since>
		public virtual bool PrimeImplicant(int p)
		{
			return this.solver.PrimeImplicant(p);
		}

		/// <since>2.3.3</since>
		public virtual ISolver GetSolvingEngine()
		{
			return this.solver.GetSolvingEngine();
		}

		/// <since>2.3.4</since>
		public virtual void SetUnitClauseProvider(UnitClauseProvider ucp)
		{
			this.solver.SetUnitClauseProvider(ucp);
		}

		/// <since>2.3.6</since>
		public virtual IConstr AddConstr(Constr constr)
		{
			return this.solver.AddConstr(constr);
		}

		/// <since>2.3.6</since>
		public virtual IConstr AddParity(IVecInt literals, bool even)
		{
			return this.solver.AddParity(literals, even);
		}
	}
}
