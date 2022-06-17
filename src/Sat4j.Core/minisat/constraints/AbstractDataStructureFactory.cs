using System;
using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints
{
	/// <author>
	/// leberre To change the template for this generated type comment go to
	/// Window&gt;Preferences&gt;Java&gt;Code Generation&gt;Code and Comments
	/// </author>
	[System.Serializable]
	public abstract class AbstractDataStructureFactory : DataStructureFactory
	{
		private const long serialVersionUID = 1L;

		/*
		* (non-Javadoc)
		*
		* @see
		* org.sat4j.minisat.core.DataStructureFactory#conflictDetectedInWatchesFor
		* (int)
		*/
		public virtual void ConflictDetectedInWatchesFor(int p, int i)
		{
			for (int j = i + 1; j < this.tmp.Size(); j++)
			{
				this.lits.Watch(p, this.tmp.Get(j));
			}
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.core.DataStructureFactory#getWatchesFor(int)
		*/
		public virtual IVec<Propagatable> GetWatchesFor(int p)
		{
			this.tmp.Clear();
			this.lits.Watches(p).MoveTo(this.tmp);
			return this.tmp;
		}

		protected internal ILits lits;

		protected internal AbstractDataStructureFactory()
		{
			this.lits = CreateLits();
		}

		protected internal abstract ILits CreateLits();

		private readonly IVec<Propagatable> tmp = new Vec<Propagatable>();

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.minisat.DataStructureFactory#createVocabulary()
		*/
		public virtual ILits GetVocabulary()
		{
			return this.lits;
		}

		protected internal UnitPropagationListener solver;

		protected internal Learner learner;

		public virtual void SetUnitPropagationListener(UnitPropagationListener s)
		{
			this.solver = s;
		}

		public virtual void SetLearner(Learner learner)
		{
			this.learner = learner;
		}

		public virtual void Reset()
		{
		}

		public virtual void LearnConstraint(Constr constr)
		{
			this.learner.Learn(constr);
		}

		/*
		* (non-Javadoc)
		*
		* @see
		* org.sat4j.minisat.core.DataStructureFactory#createCardinalityConstraint
		* (org.sat4j.specs.VecInt, int)
		*/
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual Constr CreateCardinalityConstraint(IVecInt literals, int degree)
		{
			throw new NotSupportedException();
		}

		public virtual Constr CreateUnregisteredCardinalityConstraint(IVecInt literals, int degree)
		{
			throw new NotSupportedException();
		}

		public abstract Constr CreateClause(IVecInt arg1);

		public abstract Constr CreateUnregisteredClause(IVecInt arg1);
	}
}
