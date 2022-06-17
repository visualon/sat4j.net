using System;
using System.Collections.Generic;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>Negates the formula inside a solver.</summary>
	/// <author>leberre</author>
	/// <?/>
	[System.Serializable]
	public class NegationDecorator<T> : AbstractClauseSelectorSolver<T>
		where T : ISolver
	{
		private const long serialVersionUID = 1L;

		private readonly ICollection<int> addedVars = new AList<int>();

		public NegationDecorator(T decorated)
			: base(decorated)
		{
			InternalState();
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override IConstr AddClause(IVecInt literals)
		{
			int newVar = CreateNewVar(literals);
			IVecInt clause = new VecInt(2);
			clause.Push(newVar);
			ConstrGroup group = new ConstrGroup();
			for (IteratorInt it = literals.Iterator(); it.HasNext(); )
			{
				clause.Push(-it.Next());
				group.Add(base.AddClause(clause));
				clause.Pop();
			}
			addedVars.Add(newVar);
			return group;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override IConstr AddAtMost(IVecInt literals, int degree)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override IConstr AddAtLeast(IVecInt literals, int degree)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override IConstr AddExactly(IVecInt literals, int n)
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override bool IsSatisfiable(IVecInt assumps, bool global)
		{
			IVecInt vars = new VecInt();
			foreach (int var in GetAddedVars())
			{
				vars.Push(var);
			}
			try
			{
				IConstr constr = base.AddClause(vars);
				bool returnValue;
				try
				{
					returnValue = base.IsSatisfiable(assumps, global);
				}
				finally
				{
					RemoveConstr(constr);
				}
				return returnValue;
			}
			catch (ContradictionException)
			{
				return false;
			}
		}

		public override ICollection<int> GetAddedVars()
		{
			return addedVars;
		}
	}
}
