using System;
using Org.Sat4j.Core;
using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Constraints.Cnf
{
	/// <author>laihem</author>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class Lits : ILits
	{
		private const int DefaultInitSize = 128;

		private const long serialVersionUID = 1L;

		private bool[] pool = new bool[1];

		private int realnVars = 0;

		private IVec<Propagatable>[] watches = new IVec[0];

		private int[] level = new int[0];

		private Constr[] reason = new Constr[0];

		private int maxvarid = 0;

		private IVec<Undoable>[] undos = new IVec[0];

		private bool[] falsified = new bool[0];

		public Lits()
		{
			Init(DefaultInitSize);
		}

		public void Init(int nvar)
		{
			if (nvar < this.pool.Length)
			{
				return;
			}
			System.Diagnostics.Debug.Assert(nvar >= 0);
			// let some space for unused 0 indexer.
			int nvars = nvar + 1;
			bool[] npool = new bool[nvars];
			System.Array.Copy(this.pool, 0, npool, 0, this.pool.Length);
			this.pool = npool;
			int[] nlevel = new int[nvars];
			System.Array.Copy(this.level, 0, nlevel, 0, this.level.Length);
			this.level = nlevel;
			IVec<Propagatable>[] nwatches = new IVec[2 * nvars];
			System.Array.Copy(this.watches, 0, nwatches, 0, this.watches.Length);
			this.watches = nwatches;
			IVec<Undoable>[] nundos = new IVec[nvars];
			System.Array.Copy(this.undos, 0, nundos, 0, this.undos.Length);
			this.undos = nundos;
			Constr[] nreason = new Constr[nvars];
			System.Array.Copy(this.reason, 0, nreason, 0, this.reason.Length);
			this.reason = nreason;
			bool[] newFalsified = new bool[2 * nvars];
			System.Array.Copy(this.falsified, 0, newFalsified, 0, this.falsified.Length);
			this.falsified = newFalsified;
		}

		public int GetFromPool(int x)
		{
			int var = Math.Abs(x);
			if (var >= this.pool.Length)
			{
				Init(Math.Max(var, this.pool.Length << 1));
			}
			System.Diagnostics.Debug.Assert(var < this.pool.Length);
			if (var > this.maxvarid)
			{
				this.maxvarid = var;
			}
			int lit = LiteralsUtils.ToInternal(x);
			System.Diagnostics.Debug.Assert(lit > 1);
			if (!this.pool[var])
			{
				this.realnVars++;
				this.pool[var] = true;
				this.watches[var << 1] = new Vec<Propagatable>();
				this.watches[var << 1 | 1] = new Vec<Propagatable>();
				this.undos[var] = new Vec<Undoable>();
				this.level[var] = -1;
				this.falsified[var << 1] = false;
				// because truthValue[var] is
				// UNDEFINED
				this.falsified[var << 1 | 1] = false;
			}
			// because truthValue[var] is
			// UNDEFINED
			return lit;
		}

		public bool BelongsToPool(int x)
		{
			System.Diagnostics.Debug.Assert(x > 0);
			if (x >= this.pool.Length)
			{
				return false;
			}
			return this.pool[x];
		}

		public void ResetPool()
		{
			for (int i = 0; i < this.pool.Length; i++)
			{
				if (this.pool[i])
				{
					Reset(i << 1);
				}
			}
			this.maxvarid = 0;
			this.realnVars = 0;
		}

		public void EnsurePool(int howmany)
		{
			if (howmany >= this.pool.Length)
			{
				Init(Math.Max(howmany, this.pool.Length << 1));
			}
			if (this.maxvarid < howmany)
			{
				this.maxvarid = howmany;
			}
		}

		public void Unassign(int lit)
		{
			System.Diagnostics.Debug.Assert(this.falsified[lit] || this.falsified[lit ^ 1]);
			this.falsified[lit] = false;
			this.falsified[lit ^ 1] = false;
		}

		public void Satisfies(int lit)
		{
			System.Diagnostics.Debug.Assert(!this.falsified[lit] && !this.falsified[lit ^ 1]);
			this.falsified[lit] = false;
			this.falsified[lit ^ 1] = true;
		}

		public void Forgets(int var)
		{
			this.falsified[var << 1] = true;
			this.falsified[var << 1 ^ 1] = true;
		}

		public bool IsSatisfied(int lit)
		{
			return this.falsified[lit ^ 1];
		}

		public bool IsFalsified(int lit)
		{
			return this.falsified[lit];
		}

		public bool IsUnassigned(int lit)
		{
			return !this.falsified[lit] && !this.falsified[lit ^ 1];
		}

		public string ValueToString(int lit)
		{
			if (IsUnassigned(lit))
			{
				return "?";
			}
			//$NON-NLS-1$
			if (IsSatisfied(lit))
			{
				return "T";
			}
			//$NON-NLS-1$
			return "F";
		}

		//$NON-NLS-1$
		public int NVars()
		{
			// return pool.length - 1;
			return this.maxvarid;
		}

		public int Not(int lit)
		{
			return lit ^ 1;
		}

		public static string ToString(int lit)
		{
			return ((lit & 1) == 0 ? string.Empty : "-") + (lit >> 1);
		}

		//$NON-NLS-1$//$NON-NLS-2$
		public static string ToStringX(int lit)
		{
			return ((lit & 1) == 0 ? "+" : "-") + "x" + (lit >> 1);
		}

		//$NON-NLS-1$//$NON-NLS-2$
		public void Reset(int lit)
		{
			this.watches[lit].Clear();
			this.watches[lit ^ 1].Clear();
			this.level[lit >> 1] = -1;
			this.reason[lit >> 1] = null;
			this.undos[lit >> 1].Clear();
			this.falsified[lit] = false;
			this.falsified[lit ^ 1] = false;
			this.pool[lit >> 1] = false;
		}

		public int GetLevel(int lit)
		{
			return this.level[lit >> 1];
		}

		public void SetLevel(int lit, int l)
		{
			this.level[lit >> 1] = l;
		}

		public Constr GetReason(int lit)
		{
			return this.reason[lit >> 1];
		}

		public void SetReason(int lit, Constr r)
		{
			this.reason[lit >> 1] = r;
		}

		public IVec<Undoable> Undos(int lit)
		{
			return this.undos[lit >> 1];
		}

		public void Watch(int lit, Propagatable c)
		{
			this.watches[lit].Push(c);
		}

		public IVec<Propagatable> Watches(int lit)
		{
			return this.watches[lit];
		}

		public bool IsImplied(int lit)
		{
			int var = lit >> 1;
			System.Diagnostics.Debug.Assert(this.reason[var] == null || this.falsified[lit] || this.falsified[lit ^ 1]);
			// a literal is implied if it is a unit clause, ie
			// propagated without reason at decision level 0.
			return this.pool[var] && (this.reason[var] != null || this.level[var] == 0);
		}

		public int RealnVars()
		{
			return this.realnVars;
		}

		/// <summary>To get the capacity of the current vocabulary.</summary>
		/// <returns>
		/// the total number of variables that can be managed by the
		/// vocabulary.
		/// </returns>
		protected internal int Capacity()
		{
			return this.pool.Length - 1;
		}

		/// <since>2.1</since>
		public int NextFreeVarId(bool reserve)
		{
			if (reserve)
			{
				EnsurePool(this.maxvarid + 1);
				// ensure pool changes maxvarid
				return this.maxvarid;
			}
			return this.maxvarid + 1;
		}
	}
}
