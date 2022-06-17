using System;
using System.Collections.Generic;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	[System.Serializable]
	public class FullClauseSelectorSolver<T> : AbstractClauseSelectorSolver<T>
		where T : ISolver
	{
		private const long serialVersionUID = 1L;

		private readonly IDictionary<int, IConstr> constrs = new Dictionary<int, IConstr>();

		private readonly IVecInt lastClause = new VecInt();

		private IConstr lastConstr;

		private readonly bool skipDuplicatedEntries;

		public FullClauseSelectorSolver(T solver, bool skipDuplicatedEntries)
			: base(solver)
		{
			this.skipDuplicatedEntries = skipDuplicatedEntries;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddControlableClause(IVecInt literals)
		{
			if (this.skipDuplicatedEntries)
			{
				if (literals.Equals(this.lastClause))
				{
					return null;
				}
				this.lastClause.Clear();
				literals.CopyTo(this.lastClause);
			}
			int newvar = CreateNewVar(literals);
			literals.Push(newvar);
			this.lastConstr = base.AddClause(literals);
			if (this.lastConstr == null)
			{
				DiscardLastestVar();
			}
			else
			{
				this.constrs[newvar] = this.lastConstr;
			}
			return this.lastConstr;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddNonControlableClause(IVecInt literals)
		{
			return base.AddClause(literals);
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override IConstr AddClause(IVecInt literals)
		{
			return AddControlableClause(literals);
		}

		public override int[] Model()
		{
			int[] fullmodel = base.ModelWithInternalVariables();
			if (fullmodel == null)
			{
				return null;
			}
			int[] model = new int[fullmodel.Length - this.constrs.Count];
			int j = 0;
			foreach (int element in fullmodel)
			{
				if (this.constrs[Math.Abs(element)] == null)
				{
					model[j++] = element;
				}
			}
			return model;
		}

		/// <since>2.1</since>
		public virtual ICollection<IConstr> GetConstraints()
		{
			return this.constrs.Values;
		}

		public override ICollection<int> GetAddedVars()
		{
			return this.constrs.Keys;
		}

		public virtual IConstr GetLastConstr()
		{
			return lastConstr;
		}

		public virtual void SetLastConstr(IConstr lastConstr)
		{
			this.lastConstr = lastConstr;
		}

		public virtual IDictionary<int, IConstr> GetConstrs()
		{
			return constrs;
		}

		public virtual IVecInt GetLastClause()
		{
			return lastClause;
		}

		public virtual bool IsSkipDuplicatedEntries()
		{
			return skipDuplicatedEntries;
		}
	}
}
