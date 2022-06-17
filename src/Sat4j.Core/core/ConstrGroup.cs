using System;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Core
{
	/// <summary>
	/// A utility class used to manage easily group of clauses to be deleted at some
	/// point in the solver.
	/// </summary>
	/// <author>dlb</author>
	/// <since>2.0</since>
	public class ConstrGroup : IConstr
	{
		private readonly IVec<IConstr> constrs = new Vec<IConstr>();

		private readonly bool disallowNullConstraints;

		/// <summary>Create a ConstrGroup that cannot contain null constrs.</summary>
		public ConstrGroup()
			: this(true)
		{
		}

		/// <summary>Create a new constrGroup.</summary>
		/// <param name="disallowNullConstraints">
		/// should be set to false to allow adding null constraints to the
		/// group.
		/// </param>
		public ConstrGroup(bool disallowNullConstraints)
		{
			this.disallowNullConstraints = disallowNullConstraints;
		}

		public virtual void Add(IConstr constr)
		{
			if (constr == null && this.disallowNullConstraints)
			{
				throw new ArgumentException("The constraint you entered cannot be removed from the solver.");
			}
			this.constrs.Push(constr);
		}

		public virtual void Clear()
		{
			this.constrs.Clear();
		}

		public virtual void RemoveFrom(ISolver solver)
		{
			for (int i = constrs.Size() - 1; i >= 0; i--)
			{
				solver.RemoveConstr(constrs.Get(i));
			}
		}

		public virtual IConstr GetConstr(int i)
		{
			return this.constrs.Get(i);
		}

		public virtual int Size()
		{
			return this.constrs.Size();
		}

		public virtual bool Learnt()
		{
			if (this.constrs.Size() == 0)
			{
				return false;
			}
			return this.constrs.Get(0).Learnt();
		}

		public virtual double GetActivity()
		{
			return 0;
		}

		public virtual int Get(int i)
		{
			throw new NotSupportedException();
		}

		public virtual bool CanBePropagatedMultipleTimes()
		{
			return false;
		}

		public override string ToString()
		{
			return this.constrs.ToString();
		}

		public virtual string ToString(VarMapper mapper)
		{
			throw new NotSupportedException();
		}
	}
}
