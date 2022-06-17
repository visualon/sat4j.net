using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	[System.Serializable]
	public class StatisticsSolver : ISolver
	{
		private const string NotImplementedYet = "Not implemented yet!";

		private const string ThatSolverOnlyComputeStatistics = "That solver only compute statistics";

		private const long serialVersionUID = 1L;

		/// <summary>Number of constraints in the problem</summary>
		private int expectedNumberOfConstraints;

		/// <summary>Number of declared vars (max var id)</summary>
		private int nbvars;

		/// <summary>Size of the constraints for each occurrence of each var for each polarity</summary>
		private IVecInt[] sizeoccurrences;

		private int binarynegative = 0;

		private int binarypositive = 0;

		private int allpositive = 0;

		private int allnegative = 0;

		private int horn = 0;

		private int dualhorn = 0;

		/// <summary>Distribution of clauses size</summary>
		private readonly IDictionary<int, Counter> sizes = new Dictionary<int, Counter>();

		public virtual int[] Model()
		{
			throw new NotSupportedException(ThatSolverOnlyComputeStatistics);
		}

		public virtual bool Model(int var)
		{
			throw new NotSupportedException(ThatSolverOnlyComputeStatistics);
		}

		public virtual int[] PrimeImplicant()
		{
			throw new NotSupportedException(ThatSolverOnlyComputeStatistics);
		}

		public virtual bool PrimeImplicant(int p)
		{
			throw new NotSupportedException(ThatSolverOnlyComputeStatistics);
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable()
		{
			throw new TimeoutException(ThatSolverOnlyComputeStatistics);
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(IVecInt assumps, bool globalTimeout)
		{
			throw new TimeoutException(ThatSolverOnlyComputeStatistics);
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(bool globalTimeout)
		{
			throw new TimeoutException(ThatSolverOnlyComputeStatistics);
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual bool IsSatisfiable(IVecInt assumps)
		{
			throw new TimeoutException(ThatSolverOnlyComputeStatistics);
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual int[] FindModel()
		{
			throw new TimeoutException(ThatSolverOnlyComputeStatistics);
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public virtual int[] FindModel(IVecInt assumps)
		{
			throw new TimeoutException(ThatSolverOnlyComputeStatistics);
		}

		public virtual int NConstraints()
		{
			return expectedNumberOfConstraints;
		}

		public virtual int NewVar(int howmany)
		{
			this.nbvars = howmany;
			sizeoccurrences = new IVecInt[(howmany + 1) << 1];
			return howmany;
		}

		public virtual int NVars()
		{
			return this.nbvars;
		}

		[Obsolete]
		public virtual void PrintInfos(PrintWriter @out, string prefix)
		{
		}

		public virtual void PrintInfos(PrintWriter @out)
		{
		}

		[Obsolete]
		public virtual int NewVar()
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual int NextFreeVarId(bool reserve)
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual void RegisterLiteral(int p)
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual void SetExpectedNumberOfClauses(int nb)
		{
			this.expectedNumberOfConstraints = nb;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddClause(IVecInt literals)
		{
			int size = literals.Size();
			Counter counter = sizes[size];
			if (counter == null)
			{
				counter = new Counter(0);
				sizes[size] = counter;
			}
			counter.Inc();
			IVecInt list;
			int x;
			int p;
			int pos = 0;
			int neg = 0;
			for (IteratorInt it = literals.Iterator(); it.HasNext(); )
			{
				x = it.Next();
				if (x > 0)
				{
					pos++;
				}
				else
				{
					neg++;
				}
				p = LiteralsUtils.ToInternal(x);
				list = sizeoccurrences[p];
				if (list == null)
				{
					list = new VecInt();
					sizeoccurrences[p] = list;
				}
				list.Push(size);
			}
			if (neg == 0)
			{
				allpositive++;
				if (size == 2)
				{
					binarypositive++;
				}
			}
			else
			{
				if (pos == 0)
				{
					allnegative++;
					if (size == 2)
					{
						binarynegative++;
					}
				}
				else
				{
					if (pos == 1)
					{
						horn++;
					}
					else
					{
						if (neg == 1)
						{
							dualhorn++;
						}
					}
				}
			}
			return null;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddBlockingClause(IVecInt literals)
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr DiscardCurrentModel()
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual IVecInt CreateBlockingClauseForCurrentModel()
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual bool RemoveConstr(IConstr c)
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual bool RemoveSubsumedConstr(IConstr c)
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual void AddAllClauses(IVec<IVecInt> clauses)
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddAtMost(IVecInt literals, int degree)
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddAtLeast(IVecInt literals, int degree)
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddExactly(IVecInt literals, int n)
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual void SetTimeout(int t)
		{
		}

		public virtual void SetTimeoutOnConflicts(int count)
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual void SetTimeoutMs(long t)
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual int GetTimeout()
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual long GetTimeoutMs()
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual void ExpireTimeout()
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual void Reset()
		{
		}

		[Obsolete]
		public virtual void PrintStat(TextWriter @out, string prefix)
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		[Obsolete]
		public virtual void PrintStat(PrintWriter @out, string prefix)
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual void PrintStat(PrintWriter @out)
		{
			int realNumberOfVariables = 0;
			int realNumberOfLiterals = 0;
			int pureLiterals = 0;
			int minOccV = int.MaxValue;
			int maxOccV = int.MinValue;
			int sumV = 0;
			int sizeL;
			int sizeV;
			int minOccL = int.MaxValue;
			int maxOccL = int.MinValue;
			int sumL = 0;
			IVecInt list;
			bool oneNull;
			if (sizeoccurrences == null)
			{
				return;
			}
			int max = sizeoccurrences.Length - 1;
			for (int i = 2; i < max; i += 2)
			{
				sizeV = 0;
				oneNull = false;
				for (int k = 0; k < 2; k++)
				{
					list = sizeoccurrences[i + k];
					if (list == null)
					{
						oneNull = true;
					}
					else
					{
						realNumberOfLiterals++;
						sizeL = list.Size();
						sizeV += sizeL;
						if (minOccL > sizeL)
						{
							minOccL = sizeL;
						}
						if (sizeL > maxOccL)
						{
							maxOccL = sizeL;
						}
						sumL += sizeL;
					}
				}
				if (sizeV > 0)
				{
					if (oneNull)
					{
						pureLiterals++;
					}
					realNumberOfVariables++;
					if (minOccV > sizeV)
					{
						minOccV = sizeV;
					}
					if (sizeV > maxOccV)
					{
						maxOccV = sizeV;
					}
					sumV += sizeV;
				}
			}
			if (realNumberOfVariables > 0 && realNumberOfLiterals > 0)
			{
				System.Console.Out.WriteLine("c Distribution of constraints size:");
				int nbclauses = 0;
				foreach (KeyValuePair<int, Counter> entry in sizes)
				{
					System.Console.Out.Printf("c %d => %d%n", entry.Key, entry.Value.GetValue());
					nbclauses += entry.Value.GetValue();
				}
				System.Console.Out.Printf("c Real number of variables, literals, number of clauses, size (#literals), #pureliterals, ");
				System.Console.Out.Printf("variable occurrences (min/max/avg) ");
				System.Console.Out.Printf("literals occurrences (min/max/avg) ");
				System.Console.Out.WriteLine("Specific clauses: #positive  #negative #horn  #dualhorn #binary #binarynegative #binarypositive #binaryhorn #remaining");
				Counter binaryCounter = sizes[2];
				int nbBinary = binaryCounter == null ? 0 : binaryCounter.GetValue();
				System.Console.Out.Printf(CultureInfo.InvariantCulture, "%d %d %d %d %d %d %d %.2f %d %d %.2f ", realNumberOfVariables, realNumberOfLiterals, nbclauses, sumL, pureLiterals, minOccV, maxOccV, sumV / (realNumberOfVariables * 1.0), minOccL, maxOccL
					, sumL / (realNumberOfLiterals * 1.0));
				System.Console.Out.Printf("%d %d %d %d %d %d %d %d %d%n", allpositive, allnegative, horn, dualhorn, nbBinary, binarynegative, binarypositive, (nbBinary - binarynegative - binarypositive), nbclauses - allpositive - allnegative - horn - dualhorn
					);
			}
		}

		public virtual IDictionary<string, Number> GetStat()
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual string ToString(string prefix)
		{
			return prefix + "Statistics about the benchmarks";
		}

		public virtual void ClearLearntClauses()
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual void SetDBSimplificationAllowed(bool status)
		{
		}

		public virtual bool IsDBSimplificationAllowed()
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual void SetSearchListener<S>(SearchListener<S> sl)
			where S : ISolverService
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual SearchListener<S> GetSearchListener<S>()
			where S : ISolverService
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual bool IsVerbose()
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual void SetVerbose(bool value)
		{
		}

		public virtual void SetLogPrefix(string prefix)
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual string GetLogPrefix()
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual IVecInt UnsatExplanation()
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual int[] ModelWithInternalVariables()
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual int RealNumberOfVariables()
		{
			return nbvars;
		}

		public virtual bool IsSolverKeptHot()
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual void SetKeepSolverHot(bool keepHot)
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual ISolver GetSolvingEngine()
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual void SetUnitClauseProvider(UnitClauseProvider ucp)
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual IConstr AddConstr(Constr constr)
		{
			throw new NotSupportedException(NotImplementedYet);
		}

		public virtual IConstr AddParity(IVecInt literals, bool even)
		{
			throw new NotSupportedException(NotImplementedYet);
		}
	}
}
