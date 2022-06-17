using System;
using System.Collections.Generic;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	[System.Serializable]
	public class GroupClauseSelectorSolver<T> : AbstractClauseSelectorSolver<T>, IGroupSolver
		where T : ISolver
	{
		private const long serialVersionUID = 1L;

		private readonly IDictionary<int, int> varToHighLevel = new Dictionary<int, int>();

		private readonly IDictionary<int, int> highLevelToVar = new Dictionary<int, int>();

		public GroupClauseSelectorSolver(T solver)
			: base(solver)
		{
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddControlableClause(IVecInt literals, int desc)
		{
			if (desc == 0)
			{
				return base.AddClause(literals);
			}
			int hlvar = GetGroupVar(literals, desc);
			literals.Push(hlvar);
			return base.AddClause(literals);
		}

		protected internal virtual int GetGroupVar(IVecInt literals, int groupid)
		{
			int hlvar = this.highLevelToVar[groupid];
			if (hlvar == null)
			{
				hlvar = CreateNewVar(literals);
				this.highLevelToVar[groupid] = hlvar;
				this.varToHighLevel[hlvar] = groupid;
			}
			return hlvar;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddNonControlableClause(IVecInt literals)
		{
			return base.AddClause(literals);
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddClause(IVecInt literals, int desc)
		{
			return AddControlableClause(literals, desc);
		}

		public override ICollection<int> GetAddedVars()
		{
			return varToHighLevel.Keys;
		}

		public override int[] Model()
		{
			int[] fullmodel = base.ModelWithInternalVariables();
			if (fullmodel == null)
			{
				return null;
			}
			int[] model = new int[fullmodel.Length - this.varToHighLevel.Count];
			int j = 0;
			foreach (int element in fullmodel)
			{
				if (this.varToHighLevel[Math.Abs(element)] == null)
				{
					model[j++] = element;
				}
			}
			return model;
		}

		public virtual IDictionary<int, int> GetVarToHighLevel()
		{
			return varToHighLevel;
		}

		public override IVecInt UnsatExplanation()
		{
			IVecInt @internal = base.UnsatExplanation();
			IVecInt external = new VecInt(@internal.Size());
			int p;
			int group;
			for (IteratorInt it = @internal.Iterator(); it.HasNext(); )
			{
				p = it.Next();
				if (p > 0)
				{
					group = varToHighLevel[p];
				}
				else
				{
					int negGroup = varToHighLevel[-p];
					group = (negGroup == null) ? (null) : (-negGroup);
				}
				if (group != null)
				{
					external.Push(group);
				}
			}
			return external;
		}
	}
}
