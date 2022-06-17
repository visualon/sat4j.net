using System;
using System.Reflection;
using System.Text;
using Sharpen;
using Sharpen.Logging;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Some parameters used during the search.</summary>
	/// <author>daniel</author>
	[System.Serializable]
	public class SearchParams
	{
		private const long serialVersionUID = 1L;

		private static readonly Logger Logger = Logger.GetLogger("org.sat4j.core");

		/// <summary>Default search parameters.</summary>
		public SearchParams()
			: this(0.95, 0.999, 1.5, 100)
		{
		}

		/// <param name="conflictBound">the initial conflict bound for the first restart.</param>
		public SearchParams(int conflictBound)
			: this(0.95, 0.999, 1.5, conflictBound)
		{
		}

		public SearchParams(double confincfactor, int conflictBound)
			: this(0.95, 0.999, confincfactor, conflictBound)
		{
		}

		/// <param name="d">variable decay</param>
		/// <param name="e">clause decay</param>
		/// <param name="f">conflict bound increase factor</param>
		/// <param name="i">initialConflictBound</param>
		public SearchParams(double d, double e, double f, int i)
		{
			this.varDecay = d;
			this.claDecay = e;
			this.conflictBoundIncFactor = f;
			this.initConflictBound = i;
		}

		/// <returns>la valeur de clause decay</returns>
		public virtual double GetClaDecay()
		{
			return this.claDecay;
		}

		/// <returns>la valeur de var decay</returns>
		public virtual double GetVarDecay()
		{
			return this.varDecay;
		}

		private double claDecay;

		private double varDecay;

		private double conflictBoundIncFactor;

		private int initConflictBound;

		/*
		* (non-Javadoc)
		*
		* @see java.lang.Object#toString()
		*/
		public override string ToString()
		{
			StringBuilder stb = new StringBuilder();
			foreach (FieldInfo field in Sharpen.Runtime.GetDeclaredFields(typeof(Org.Sat4j.Minisat.Core.SearchParams)))
			{
				if (!field.Name.StartsWith("serial") && !field.Name.StartsWith("class"))
				{
					stb.Append(field.Name);
					stb.Append("=");
					//$NON-NLS-1$
					try
					{
						stb.Append(field.GetValue(this));
					}
					catch (ArgumentException e)
					{
						Logger.Log(Level.Info, "Issue when reflectively accessing field", e);
					}
					catch (MemberAccessException e)
					{
						Logger.Log(Level.Info, "Access issue when reflectively accessing field", e);
					}
					stb.Append(" ");
				}
			}
			//$NON-NLS-1$
			return stb.ToString();
		}

		/// <param name="conflictBoundIncFactor">the conflictBoundIncFactor to set</param>
		public virtual void SetConflictBoundIncFactor(double conflictBoundIncFactor)
		{
			this.conflictBoundIncFactor = conflictBoundIncFactor;
		}

		/// <param name="initConflictBound">the initConflictBound to set</param>
		public virtual void SetInitConflictBound(int initConflictBound)
		{
			this.initConflictBound = initConflictBound;
		}

		/// <returns>the conflictBoundIncFactor</returns>
		public virtual double GetConflictBoundIncFactor()
		{
			return this.conflictBoundIncFactor;
		}

		/// <returns>the initConflictBound</returns>
		public virtual int GetInitConflictBound()
		{
			return this.initConflictBound;
		}

		/// <param name="claDecay">the claDecay to set</param>
		public virtual void SetClaDecay(double claDecay)
		{
			this.claDecay = claDecay;
		}

		/// <param name="varDecay">the varDecay to set</param>
		public virtual void SetVarDecay(double varDecay)
		{
			this.varDecay = varDecay;
		}
	}
}
