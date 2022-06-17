using System;
using System.Collections.Generic;
using System.IO;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>Solver used to display in a writer the CNF instance in Dimacs format.</summary>
	/// <remarks>
	/// Solver used to display in a writer the CNF instance in Dimacs format.
	/// That solver is useful to produce CNF files to be used by third party solvers.
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public class DimacsOutputSolver : AbstractOutputSolver, IGroupSolver
	{
		private const long serialVersionUID = 1L;

		[System.NonSerialized]
		private PrintWriter @out;

		public DimacsOutputSolver()
			: this(new PrintWriter(System.Console.Out, true))
		{
		}

		public DimacsOutputSolver(PrintWriter pw)
		{
			this.@out = pw;
		}

		private void ReadObject(ObjectInputStream stream)
		{
			this.@out = new PrintWriter(System.Console.Out, true);
		}

		public override int NewVar()
		{
			return 0;
		}

		public override int NewVar(int howmany)
		{
			this.@out.Write("p cnf " + howmany);
			this.nbvars = howmany;
			return 0;
		}

		public override void SetExpectedNumberOfClauses(int nb)
		{
			this.@out.WriteLine(" " + nb);
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
					this.@out.WriteLine(" XXXXXX");
				}
				this.firstConstr = false;
			}
			for (IteratorInt iterator = literals.Iterator(); iterator.HasNext(); )
			{
				this.@out.Write(iterator.Next() + " ");
			}
			this.@out.WriteLine("0");
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
				if (!this.fixedNbClauses)
				{
					this.@out.WriteLine("XXXXXX");
				}
				this.firstConstr = false;
			}
			for (int i = 0; i <= literals.Size(); i++)
			{
				for (int j = i + 1; j < literals.Size(); j++)
				{
					this.@out.WriteLine(string.Empty + -literals.Get(i) + " " + -literals.Get(j) + " 0");
				}
			}
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

		public override void Reset()
		{
			this.fixedNbClauses = false;
			this.firstConstr = true;
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
			return this.nbvars;
		}

		/// <since>2.1</since>
		public override int NextFreeVarId(bool reserve)
		{
			if (reserve)
			{
				return ++this.nbvars;
			}
			return this.nbvars + 1;
		}

		/// <since>2.3.1</since>
		public override int[] ModelWithInternalVariables()
		{
			throw new NotSupportedException();
		}

		/// <since>2.3.1</since>
		public override int RealNumberOfVariables()
		{
			return this.nbvars;
		}

		/// <since>2.3.1</since>
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
			throw new NotSupportedException();
		}

		/// <since>2.3.3</since>
		public override void PrintInfos(PrintWriter @out)
		{
			throw new NotSupportedException();
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual IConstr AddClause(IVecInt literals, int desc)
		{
			this.@out.Write(desc + "> ");
			for (IteratorInt iterator = literals.Iterator(); iterator.HasNext(); )
			{
				this.@out.Write(iterator.Next() + " ");
			}
			this.@out.WriteLine("0");
			return null;
		}

		public virtual ICollection<int> GetAddedVars()
		{
			throw new NotSupportedException("Not implemented yet!");
		}

		public override IConstr AddParity(IVecInt literals, bool even)
		{
			throw new NotSupportedException("Not implemented yet!");
		}
	}
}
