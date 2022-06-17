using System;
using System.Collections.Generic;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;
using Sharpen.Logging;

namespace Org.Sat4j.Tools
{
	/// <summary>
	/// This is a tool for computing all the MUSes (Minimum Unsatisfiable Sets) of a
	/// set of clauses.
	/// </summary>
	/// <author>sroussel</author>
	/// <since>2.3.3</since>
	public class AllMUSes
	{
		private readonly AbstractClauseSelectorSolver<ISolver> css;

		private readonly IList<IVecInt> mssList;

		private readonly IList<IVecInt> secondPhaseClauses;

		private readonly IList<IVecInt> musList;

		private readonly ASolverFactory<ISolver> factory;

		public AllMUSes(bool group, ASolverFactory<ISolver> factory)
			: this(group ? new GroupClauseSelectorSolver<ISolver>(factory.DefaultSolver()) : new FullClauseSelectorSolver<ISolver>(factory.DefaultSolver(), false), factory)
		{
		}

		public AllMUSes(ASolverFactory<ISolver> factory)
			: this(false, factory)
		{
		}

		public AllMUSes(AbstractClauseSelectorSolver<ISolver> css, ASolverFactory<ISolver> factory)
		{
			this.css = css;
			this.factory = factory;
			this.mssList = new AList<IVecInt>();
			this.musList = new AList<IVecInt>();
			this.secondPhaseClauses = new AList<IVecInt>();
		}

		/// <summary>Gets an instance of ISolver that can be used to compute all MUSes</summary>
		/// <returns>the instance of ISolver to which the clauses will be added</returns>
		public virtual T GetSolverInstance<T>()
			where T : ISolver
		{
			return (T)this.css;
		}

		public virtual IList<IVecInt> ComputeAllMUSes()
		{
			return ComputeAllMUSes(VecInt.Empty);
		}

		/// <summary>Reset the state of the object to allow computing new MUSes or MSSes.</summary>
		public virtual void Reset()
		{
			this.secondPhaseClauses.Clear();
		}

		public virtual IList<IVecInt> ComputeAllMUSes(SolutionFoundListener listener)
		{
			return ComputeAllMUSes(VecInt.Empty, listener);
		}

		public virtual IList<IVecInt> ComputeAllMUSes(IVecInt assumptions)
		{
			return ComputeAllMUSes(assumptions, SolutionFoundListenerConstants.Void);
		}

		/// <summary>
		/// Computes all the MUSes associated to the set of constraints added to the
		/// solver
		/// </summary>
		/// <param name="assumptions">the assumptions under which the MUSes must be computed.</param>
		/// <param name="listener">a listener to call when an MUS is found</param>
		/// <returns>a list containing all the MUSes</returns>
		public virtual IList<IVecInt> ComputeAllMUSes(IVecInt assumptions, SolutionFoundListener listener)
		{
			if (secondPhaseClauses.IsEmpty())
			{
				ComputeAllMSS(assumptions);
			}
			ISolver solver = factory.DefaultSolver();
			foreach (IVecInt v in secondPhaseClauses)
			{
				try
				{
					solver.AddClause(v);
				}
				catch (ContradictionException)
				{
					return musList;
				}
			}
			AbstractMinimalModel minSolver = new Minimal4InclusionModel(solver, Minimal4InclusionModel.PositiveLiterals(solver));
			return ComputeAllMUSes(assumptions, listener, minSolver);
		}

		public virtual IList<IVecInt> ComputeAllMUSesOrdered(SolutionFoundListener listener)
		{
			return ComputeAllMUSesOrdered(VecInt.Empty, listener);
		}

		public virtual IList<IVecInt> ComputeAllMUSesOrdered(IVecInt assumptions, SolutionFoundListener listener)
		{
			if (secondPhaseClauses.IsEmpty())
			{
				ComputeAllMSS();
			}
			ISolver solver = factory.DefaultSolver();
			foreach (IVecInt v in secondPhaseClauses)
			{
				try
				{
					solver.AddClause(v);
				}
				catch (ContradictionException)
				{
					return musList;
				}
			}
			AbstractMinimalModel minSolver = new Minimal4CardinalityModel(solver, Minimal4InclusionModel.PositiveLiterals(solver));
			return ComputeAllMUSes(assumptions, listener, minSolver);
		}

		private IList<IVecInt> ComputeAllMUSes(IVecInt assumptions, SolutionFoundListener listener, ISolver minSolver)
		{
			if (css.IsVerbose())
			{
				System.Console.Out.WriteLine(css.GetLogPrefix() + "Computing all MUSes ...");
			}
			css.InternalState();
			IVecInt mus;
			IVecInt blockingClause;
			try
			{
				while (minSolver.IsSatisfiable(assumptions))
				{
					blockingClause = new VecInt();
					mus = new VecInt();
					int[] model = minSolver.Model();
					for (int i = 0; i < model.Length; i++)
					{
						if (model[i] > 0)
						{
							blockingClause.Push(-model[i]);
							mus.Push(model[i]);
						}
					}
					musList.Add(mus);
					listener.OnSolutionFound(mus);
					try
					{
						minSolver.AddBlockingClause(blockingClause);
					}
					catch (ContradictionException)
					{
						break;
					}
				}
			}
			catch (TimeoutException e)
			{
				Logger.GetLogger("org.sat4j.core").Log(Level.Info, "Timeout when computing all muses", e);
			}
			if (css.IsVerbose())
			{
				System.Console.Out.WriteLine(css.GetLogPrefix() + "... done.");
			}
			css.ExternalState();
			return musList;
		}

		public virtual IList<IVecInt> ComputeAllMSS()
		{
			return ComputeAllMSS(VecInt.Empty);
		}

		public virtual IList<IVecInt> ComputeAllMSS(IVecInt assumptions)
		{
			return ComputeAllMSS(assumptions, SolutionFoundListenerConstants.Void);
		}

		public virtual IList<IVecInt> ComputeAllMSS(SolutionFoundListener mssListener)
		{
			return ComputeAllMSS(VecInt.Empty, mssListener);
		}

		public virtual IList<IVecInt> ComputeAllMSS(IVecInt assumptions, SolutionFoundListener listener)
		{
			IVecInt pLits = new VecInt();
			foreach (int i in css.GetAddedVars())
			{
				pLits.Push(i);
			}
			AbstractMinimalModel min4Inc = new Minimal4InclusionModel(css, pLits);
			return ComputeAllMSS(assumptions, listener, min4Inc, pLits);
		}

		public virtual IList<IVecInt> ComputeAllMSSOrdered(IVecInt assumptions, SolutionFoundListener listener)
		{
			IVecInt pLits = new VecInt();
			foreach (int i in css.GetAddedVars())
			{
				pLits.Push(i);
			}
			AbstractMinimalModel min4Inc = new Minimal4CardinalityModel(css, pLits);
			return ComputeAllMSS(assumptions, listener, min4Inc, pLits);
		}

		private IList<IVecInt> ComputeAllMSS(IVecInt assumptions, SolutionFoundListener listener, ISolver min4Inc, IVecInt pLits)
		{
			if (css.IsVerbose())
			{
				System.Console.Out.WriteLine(css.GetLogPrefix() + "Computing all MSSes ...");
			}
			css.InternalState();
			int nVar = css.NVars();
			IVecInt blockingClause;
			IVecInt secondPhaseClause;
			IVecInt fullMSS = new VecInt();
			IVecInt mss;
			int clause;
			for (int i = 0; i < css.GetAddedVars().Count; i++)
			{
				fullMSS.Push(i + 1);
			}
			// first phase
			try
			{
				while (min4Inc.IsSatisfiable(assumptions))
				{
					int[] fullmodel = min4Inc.ModelWithInternalVariables();
					mss = new VecInt();
					fullMSS.CopyTo(mss);
					blockingClause = new VecInt();
					secondPhaseClause = new VecInt();
					for (int i_1 = 0; i_1 < pLits.Size(); i_1++)
					{
						clause = Math.Abs(pLits.Get(i_1));
						if (fullmodel[clause - 1] > 0)
						{
							blockingClause.Push(-clause);
							secondPhaseClause.Push(clause - nVar);
							mss.Remove(clause - nVar);
						}
					}
					mssList.Add(mss);
					listener.OnSolutionFound(mss);
					secondPhaseClauses.Add(secondPhaseClause);
					try
					{
						css.AddBlockingClause(blockingClause);
					}
					catch (ContradictionException e)
					{
						Logger.GetLogger("org.sat4j.core").Log(Level.Info, "Trivial inconsistency", e);
						break;
					}
				}
			}
			catch (TimeoutException e)
			{
				Logger.GetLogger("org.sat4j.core").Log(Level.Info, "Timeout during the first stage", e);
			}
			if (css.IsVerbose())
			{
				System.Console.Out.WriteLine(css.GetLogPrefix() + "... done.");
			}
			css.ExternalState();
			return mssList;
		}

		public virtual IList<IVecInt> GetMssList()
		{
			return mssList;
		}
	}
}
