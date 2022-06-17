using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>Solver used to write down a CNF into a String.</summary>
	/// <remarks>
	/// Solver used to write down a CNF into a String.
	/// It is especially useful compared to the DimacsOutputSolver because the number
	/// of clauses does not need to be known in advance.
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public class DimacsStringSolver : AbstractOutputSolver, IGroupSolver
	{
		private const long serialVersionUID = 1L;

		private StringBuilder @out;

		private int firstCharPos;

		private readonly int initBuilderSize;

		private int maxvarid = 0;

		public DimacsStringSolver()
			: this(16)
		{
		}

		public DimacsStringSolver(int initSize)
		{
			this.@out = new StringBuilder(initSize);
			this.initBuilderSize = initSize;
		}

		public virtual StringBuilder GetOut()
		{
			return this.@out;
		}

		public override int NewVar()
		{
			return 0;
		}

		public override int NewVar(int howmany)
		{
			SetNbVars(howmany);
			return howmany;
		}

		protected internal virtual void SetNbVars(int howmany)
		{
			this.nbvars = howmany;
			this.maxvarid = howmany;
		}

		public override void SetExpectedNumberOfClauses(int nb)
		{
			this.@out.Append(" ");
			this.@out.Append(nb);
			this.nbclauses = nb;
			this.fixedNbClauses = true;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override IConstr AddClause(IVecInt literals)
		{
			if (this.firstConstr)
			{
				if (!this.fixedNbClauses)
				{
					this.firstCharPos = 0;
					this.@out.Append("                    ");
					this.@out.Append("\n");
					this.nbclauses = 0;
				}
				this.firstConstr = false;
			}
			if (!this.fixedNbClauses)
			{
				this.nbclauses++;
			}
			for (IteratorInt iterator = literals.Iterator(); iterator.HasNext(); )
			{
				this.@out.Append(iterator.Next()).Append(" ");
			}
			this.@out.Append("0\n");
			return null;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override IConstr AddAtMost(IVecInt literals, int degree)
		{
			if (degree > 1)
			{
				throw new NotSupportedException("Not a clausal problem! degree " + degree);
			}
			System.Diagnostics.Debug.Assert(degree == 1);
			if (this.firstConstr)
			{
				this.firstCharPos = 0;
				this.@out.Append("                    ");
				this.@out.Append("\n");
				this.nbclauses = 0;
				this.firstConstr = false;
			}
			for (int i = 0; i <= literals.Size(); i++)
			{
				for (int j = i + 1; j < literals.Size(); j++)
				{
					if (!this.fixedNbClauses)
					{
						this.nbclauses++;
					}
					this.@out.Append(-literals.Get(i));
					this.@out.Append(" ");
					this.@out.Append(-literals.Get(j));
					this.@out.Append(" 0\n");
				}
			}
			return null;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override IConstr AddExactly(IVecInt literals, int n)
		{
			if (n > 1)
			{
				throw new NotSupportedException("Not a clausal problem! degree " + n);
			}
			System.Diagnostics.Debug.Assert(n == 1);
			AddAtMost(literals, n);
			AddAtLeast(literals, n);
			return null;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override IConstr AddAtLeast(IVecInt literals, int degree)
		{
			if (degree > 1)
			{
				throw new NotSupportedException("Not a clausal problem! degree " + degree);
			}
			System.Diagnostics.Debug.Assert(degree == 1);
			return AddClause(literals);
		}

		public override void Reset()
		{
			this.fixedNbClauses = false;
			this.firstConstr = true;
			this.@out = new StringBuilder(this.initBuilderSize);
			this.maxvarid = 0;
		}

		public override string ToString(string prefix)
		{
			return "Dimacs output solver";
		}

		public override int NConstraints()
		{
			return this.nbclauses;
		}

		public override int NVars()
		{
			return this.maxvarid;
		}

		public override string ToString()
		{
			this.@out.Insert(this.firstCharPos, "p cnf " + this.maxvarid + " " + this.nbclauses);
			return this.@out.ToString();
		}

		/// <since>2.1</since>
		public override int NextFreeVarId(bool reserve)
		{
			if (reserve)
			{
				return ++this.maxvarid;
			}
			return this.maxvarid + 1;
		}

		/// <since>2.3.1</since>
		public override int[] ModelWithInternalVariables()
		{
			throw new NotSupportedException();
		}

		public override int RealNumberOfVariables()
		{
			return this.maxvarid;
		}

		public override void RegisterLiteral(int p)
		{
			throw new NotSupportedException();
		}

		/// <since>2.3.2</since>
		public override bool PrimeImplicant(int p)
		{
			throw new NotSupportedException();
		}

		/// <since>2.3.3</since>
		public override void PrintStat(PrintWriter @out)
		{
		}

		/// <since>2.3.3</since>
		public override void PrintInfos(PrintWriter @out)
		{
			@out.WriteLine(ToString());
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddClause(IVecInt literals, int desc)
		{
			this.@out.Append(desc + "> ");
			for (IteratorInt iterator = literals.Iterator(); iterator.HasNext(); )
			{
				this.@out.Append(iterator.Next() + " ");
			}
			this.@out.Append("0\n");
			return null;
		}

		public virtual ICollection<int> GetAddedVars()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override void AddAllClauses(IVec<IVecInt> clauses)
		{
			for (IEnumerator<IVecInt> it = clauses.Iterator(); it.HasNext(); )
			{
				AddClause(it.Next());
			}
		}

		public override IConstr AddParity(IVecInt literals, bool even)
		{
			throw new NotSupportedException("Not implemented yet!");
		}
	}
}
