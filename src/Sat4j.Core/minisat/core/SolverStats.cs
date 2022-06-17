using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Contains some statistics regarding the search.</summary>
	/// <author>daniel</author>
	[System.Serializable]
	public class SolverStats
	{
		private const long serialVersionUID = 1L;

		private int starts;

		private long decisions;

		private long propagations;

		private long inspects;

		private long conflicts;

		private long learnedliterals;

		private long learnedbinaryclauses;

		private long learnedternaryclauses;

		private long learnedclauses;

		private long ignoredclauses;

		private long rootSimplifications;

		private long reducedliterals;

		private long changedreason;

		private int reduceddb;

		private int shortcuts;

		private long updateLBD;

		private int importedUnits;

		public virtual void Reset()
		{
			this.starts = 0;
			this.decisions = 0;
			this.propagations = 0;
			this.inspects = 0;
			this.shortcuts = 0;
			this.conflicts = 0;
			this.learnedliterals = 0;
			this.learnedclauses = 0;
			this.ignoredclauses = 0;
			this.learnedbinaryclauses = 0;
			this.learnedternaryclauses = 0;
			this.rootSimplifications = 0;
			this.reducedliterals = 0;
			this.changedreason = 0;
			this.reduceddb = 0;
			this.updateLBD = 0;
			this.importedUnits = 0;
		}

		public virtual void PrintStat(PrintWriter @out, string prefix)
		{
			@out.WriteLine(prefix + "starts\t\t: " + this.GetStarts());
			@out.WriteLine(prefix + "conflicts\t\t: " + this.conflicts);
			@out.WriteLine(prefix + "decisions\t\t: " + this.decisions);
			@out.WriteLine(prefix + "propagations\t\t: " + this.propagations);
			@out.WriteLine(prefix + "inspects\t\t: " + this.inspects);
			@out.WriteLine(prefix + "shortcuts\t\t: " + this.shortcuts);
			@out.WriteLine(prefix + "learnt literals\t: " + this.learnedliterals);
			@out.WriteLine(prefix + "learnt binary clauses\t: " + this.learnedbinaryclauses);
			@out.WriteLine(prefix + "learnt ternary clauses\t: " + this.learnedternaryclauses);
			@out.WriteLine(prefix + "learnt constraints\t: " + this.learnedclauses);
			@out.WriteLine(prefix + "ignored constraints\t: " + this.ignoredclauses);
			@out.WriteLine(prefix + "root simplifications\t: " + this.rootSimplifications);
			@out.WriteLine(prefix + "removed literals (reason simplification)\t: " + this.reducedliterals);
			@out.WriteLine(prefix + "reason swapping (by a shorter reason)\t: " + this.changedreason);
			@out.WriteLine(prefix + "Calls to reduceDB\t: " + this.reduceddb);
			@out.WriteLine(prefix + "Number of update (reduction) of LBD\t: " + this.updateLBD);
			@out.WriteLine(prefix + "Imported unit clauses\t: " + this.importedUnits);
		}

		public virtual IDictionary<string, Number> ToMap()
		{
			IDictionary<string, Number> map = new Dictionary<string, Number>();
			foreach (FieldInfo f in this.GetType().GetFields())
			{
				try
				{
					map[f.Name] = (Number)f.GetValue(this);
				}
				catch (ArgumentException)
				{
				}
				catch (MemberAccessException)
				{
				}
			}
			// ignores silently
			// ignores silently
			return map;
		}

		public virtual int GetStarts()
		{
			return starts;
		}

		public virtual void IncStarts()
		{
			this.starts++;
		}

		public virtual long GetDecisions()
		{
			return decisions;
		}

		public virtual void IncDecisions()
		{
			this.decisions++;
		}

		public virtual long GetPropagations()
		{
			return propagations;
		}

		public virtual void IncPropagations()
		{
			this.propagations++;
		}

		public virtual long GetInspects()
		{
			return inspects;
		}

		public virtual void IncInspects()
		{
			this.inspects++;
		}

		public virtual long GetConflicts()
		{
			return conflicts;
		}

		public virtual void IncConflicts()
		{
			this.conflicts++;
		}

		public virtual long GetLearnedliterals()
		{
			return learnedliterals;
		}

		public virtual void IncLearnedliterals()
		{
			this.learnedliterals++;
		}

		public virtual long GetLearnedbinaryclauses()
		{
			return learnedbinaryclauses;
		}

		public virtual void IncLearnedbinaryclauses()
		{
			this.learnedbinaryclauses++;
		}

		public virtual long GetLearnedternaryclauses()
		{
			return learnedternaryclauses;
		}

		public virtual void IncLearnedternaryclauses()
		{
			this.learnedternaryclauses++;
		}

		public virtual long GetLearnedclauses()
		{
			return learnedclauses;
		}

		public virtual void IncLearnedclauses()
		{
			this.learnedclauses++;
		}

		public virtual long GetIgnoredclauses()
		{
			return ignoredclauses;
		}

		public virtual void IncIgnoredclauses()
		{
			this.ignoredclauses++;
		}

		public virtual long GetRootSimplifications()
		{
			return rootSimplifications;
		}

		public virtual void IncRootSimplifications()
		{
			this.rootSimplifications++;
		}

		public virtual long GetReducedliterals()
		{
			return reducedliterals;
		}

		public virtual void IncReducedliterals(int increment)
		{
			this.reducedliterals += increment;
		}

		public virtual long GetChangedreason()
		{
			return changedreason;
		}

		public virtual void IncChangedreason()
		{
			this.changedreason++;
		}

		public virtual int GetReduceddb()
		{
			return reduceddb;
		}

		public virtual void IncReduceddb()
		{
			this.reduceddb++;
		}

		public virtual int GetShortcuts()
		{
			return shortcuts;
		}

		public virtual void IncShortcuts()
		{
			this.shortcuts++;
		}

		public virtual long GetUpdateLBD()
		{
			return updateLBD;
		}

		public virtual void IncUpdateLBD()
		{
			this.updateLBD++;
		}

		public virtual int GetImportedUnits()
		{
			return importedUnits;
		}

		public virtual void IncImportedUnits(int increment)
		{
			this.importedUnits += increment;
		}
	}
}
