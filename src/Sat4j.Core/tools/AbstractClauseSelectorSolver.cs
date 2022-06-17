using System;
using System.Collections.Generic;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	[System.Serializable]
	public abstract class AbstractClauseSelectorSolver<T> : SolverDecorator<T>
		where T : ISolver
	{
		private const long serialVersionUID = 1L;

		private int lastCreatedVar;

		private bool pooledVarId = false;

		private interface SelectorState
		{
			/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
			bool IsSatisfiable(bool global);

			/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
			bool IsSatisfiable();

			/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
			bool IsSatisfiable(IVecInt assumps);

			/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
			bool IsSatisfiable(IVecInt assumps, bool global);
		}

		private sealed class _SelectorState_60 : AbstractClauseSelectorSolver<T>.SelectorState
		{
			public _SelectorState_60(AbstractClauseSelectorSolver<T> _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private IVecInt GetNegatedSelectors()
			{
				IVecInt assumps = new VecInt();
				foreach (int var in this._enclosing.GetAddedVars())
				{
					assumps.Push(-var);
				}
				return assumps;
			}

			/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
			public bool IsSatisfiable(bool global)
			{
				return this._enclosing.Decorated().IsSatisfiable(this.GetNegatedSelectors(), global);
			}

			/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
			public bool IsSatisfiable(IVecInt assumps, bool global)
			{
				IVecInt all = this.GetNegatedSelectors();
				assumps.CopyTo(all);
				return this._enclosing.Decorated().IsSatisfiable(all, global);
			}

			/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
			public bool IsSatisfiable()
			{
				return this._enclosing.Decorated().IsSatisfiable(this.GetNegatedSelectors());
			}

			/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
			public bool IsSatisfiable(IVecInt assumps)
			{
				IVecInt all = this.GetNegatedSelectors();
				assumps.CopyTo(all);
				return this._enclosing.Decorated().IsSatisfiable(all);
			}

			private readonly AbstractClauseSelectorSolver<T> _enclosing;
		}

		private readonly AbstractClauseSelectorSolver<T>.SelectorState external;

		private sealed class _SelectorState_93 : AbstractClauseSelectorSolver<T>.SelectorState
		{
			public _SelectorState_93(AbstractClauseSelectorSolver<T> _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
			public bool IsSatisfiable(bool global)
			{
				return this._enclosing.Decorated().IsSatisfiable(global);
			}

			/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
			public bool IsSatisfiable()
			{
				return this._enclosing.Decorated().IsSatisfiable();
			}

			/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
			public bool IsSatisfiable(IVecInt assumps)
			{
				return this._enclosing.Decorated().IsSatisfiable(assumps);
			}

			/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
			public bool IsSatisfiable(IVecInt assumps, bool global)
			{
				return this._enclosing.Decorated().IsSatisfiable(assumps, global);
			}

			private readonly AbstractClauseSelectorSolver<T> _enclosing;
		}

		private readonly AbstractClauseSelectorSolver<T>.SelectorState @internal;

		private AbstractClauseSelectorSolver<T>.SelectorState selectedState;

		public AbstractClauseSelectorSolver(T solver)
			: base(solver)
		{
			external = new _SelectorState_60(this);
			@internal = new _SelectorState_93(this);
			selectedState = external;
		}

		public abstract ICollection<int> GetAddedVars();

		/// <param name="literals"/>
		/// <returns/>
		/// <since>2.1</since>
		protected internal virtual int CreateNewVar(IVecInt literals)
		{
			for (IteratorInt it = literals.Iterator(); it.HasNext(); )
			{
				if (Math.Abs(it.Next()) > NextFreeVarId(false))
				{
					throw new InvalidOperationException("Please call newVar(int) before adding constraints!!!");
				}
			}
			if (this.pooledVarId)
			{
				this.pooledVarId = false;
				return this.lastCreatedVar;
			}
			this.lastCreatedVar = NextFreeVarId(true);
			return this.lastCreatedVar;
		}

		protected internal virtual void DiscardLastestVar()
		{
			this.pooledVarId = true;
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override bool IsSatisfiable(bool global)
		{
			return selectedState.IsSatisfiable(global);
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override bool IsSatisfiable(IVecInt assumps, bool global)
		{
			return selectedState.IsSatisfiable(assumps, global);
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override bool IsSatisfiable()
		{
			return selectedState.IsSatisfiable();
		}

		/// <exception cref="Org.Sat4j.Specs.TimeoutException"/>
		public override bool IsSatisfiable(IVecInt assumps)
		{
			return selectedState.IsSatisfiable(assumps);
		}

		/// <summary>
		/// In the internal state, the solver will allow the selector variables to be
		/// satisfied.
		/// </summary>
		/// <remarks>
		/// In the internal state, the solver will allow the selector variables to be
		/// satisfied. As such, the solver will answer "true" to isSatisfiable() on
		/// an UNSAT problem. it is the responsibility of the user to take into
		/// account the meaning of the selector variables.
		/// </remarks>
		public virtual void InternalState()
		{
			this.selectedState = @internal;
		}

		/// <summary>
		/// In external state, the solver will prevent the selector variables to be
		/// satisfied.
		/// </summary>
		/// <remarks>
		/// In external state, the solver will prevent the selector variables to be
		/// satisfied. As a consequence, from an external point of view, an UNSAT
		/// problem will answer "false" to isSatisfiable().
		/// </remarks>
		public virtual void ExternalState()
		{
			this.selectedState = external;
		}
	}
}
