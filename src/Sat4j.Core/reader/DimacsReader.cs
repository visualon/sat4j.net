using System;
using System.IO;
using System.Text;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Reader
{
	/// <summary>Very simple Dimacs file parser.</summary>
	/// <remarks>
	/// Very simple Dimacs file parser. Allow solvers to read the constraints from a
	/// Dimacs formatted file. It should be used that way:
	/// <pre>
	/// DimacsReader solver = new DimacsReader(SolverFactory.OneSolver());
	/// solver.readInstance(&quot;mybench.cnf&quot;);
	/// if (solver.isSatisfiable()) {
	/// // SAT case
	/// } else {
	/// // UNSAT case
	/// }
	/// </pre>
	/// That parser is not used for efficiency reasons. It will be updated with Java
	/// 1.5 scanner feature.
	/// </remarks>
	/// <version>1.0</version>
	/// <author>dlb</author>
	/// <author>or</author>
	[System.Serializable]
	public class DimacsReader : Org.Sat4j.Reader.Reader
	{
		private const long serialVersionUID = 1L;

		protected internal int expectedNbOfConstr;

		protected internal readonly ISolver solver;

		private bool checkConstrNb = true;

		protected internal readonly string formatString;

		/// <since>2.1</since>
		protected internal EfficientScanner scanner;

		public DimacsReader(ISolver solver)
			: this(solver, "cnf")
		{
		}

		public DimacsReader(ISolver solver, string format)
		{
			// as announced on the p cnf line
			this.solver = solver;
			this.formatString = format;
		}

		public virtual void DisableNumberOfConstraintCheck()
		{
			this.checkConstrNb = false;
		}

		/// <summary>Skip comments at the beginning of the input stream.</summary>
		/// <exception cref="System.IO.IOException">if an IO problem occurs.</exception>
		/// <since>2.1</since>
		protected internal virtual void SkipComments()
		{
			this.scanner.SkipComments();
		}

		/// <exception cref="System.IO.IOException">iff an IO occurs</exception>
		/// <exception cref="ParseFormatException">if the input stream does not comply with the DIMACS format.</exception>
		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		protected internal virtual void ReadProblemLine()
		{
			string line = Sharpen.Extensions.Trim(this.scanner.NextLine());
			if (line == null)
			{
				throw new ParseFormatException("premature end of file: <p cnf ...> expected");
			}
			string[] tokens = line.Split("\\s+");
			if (tokens.Length < 4 || !"p".Equals(tokens[0]) || !this.formatString.Equals(tokens[1]))
			{
				throw new ParseFormatException("problem line expected (p cnf ...)");
			}
			int vars;
			// reads the max var id
			vars = System.Convert.ToInt32(tokens[2]);
			System.Diagnostics.Debug.Assert(vars > 0);
			this.solver.NewVar(vars);
			// reads the number of clauses
			this.expectedNbOfConstr = System.Convert.ToInt32(tokens[3]);
			System.Diagnostics.Debug.Assert(this.expectedNbOfConstr > 0);
			this.solver.SetExpectedNumberOfClauses(this.expectedNbOfConstr);
		}

		/// <since>2.1</since>
		protected internal IVecInt literals = new VecInt();

		/// <exception cref="System.IO.IOException">iff an IO problems occurs</exception>
		/// <exception cref="ParseFormatException">if the input stream does not comply with the DIMACS format.</exception>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException">si le probl?me est trivialement inconsistant.</exception>
		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		protected internal virtual void ReadConstrs()
		{
			int realNbOfConstr = 0;
			this.literals.Clear();
			bool needToContinue = true;
			while (needToContinue)
			{
				bool added = false;
				if (this.scanner.Eof())
				{
					// end of file
					if (this.literals.Size() > 0)
					{
						// no 0 end the last clause
						FlushConstraint();
						added = true;
					}
					needToContinue = false;
				}
				else
				{
					if (this.scanner.CurrentChar() == 'c')
					{
						// ignore comment line
						this.scanner.SkipRestOfLine();
						continue;
					}
					if (this.scanner.CurrentChar() == '%' && this.expectedNbOfConstr == realNbOfConstr)
					{
						if (this.solver.IsVerbose())
						{
							System.Console.Out.WriteLine("Ignoring the rest of the file (SATLIB format");
						}
						break;
					}
					added = HandleLine();
				}
				if (added)
				{
					realNbOfConstr++;
				}
			}
			if (this.checkConstrNb && this.expectedNbOfConstr != realNbOfConstr)
			{
				throw new ParseFormatException("wrong nbclauses parameter. Found " + realNbOfConstr + ", " + this.expectedNbOfConstr + " expected");
			}
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <since>2.1</since>
		protected internal virtual void FlushConstraint()
		{
			try
			{
				this.solver.AddClause(this.literals);
			}
			catch (ArgumentException)
			{
				if (IsVerbose())
				{
					System.Console.Error.WriteLine("c Skipping constraint " + this.literals);
				}
			}
		}

		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		protected internal virtual bool HandleLine()
		{
			int lit;
			bool added = false;
			while (!this.scanner.Eof())
			{
				lit = this.scanner.NextInt();
				if (lit == 0)
				{
					if (this.literals.Size() > 0)
					{
						FlushConstraint();
						this.literals.Clear();
						added = true;
					}
					break;
				}
				this.literals.Push(lit);
			}
			return added;
		}

		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <exception cref="System.IO.IOException"/>
		public override IProblem ParseInstance(InputStream @in)
		{
			this.scanner = new EfficientScanner(@in);
			return ParseInstance();
		}

		/// <param name="in">the input stream</param>
		/// <exception cref="ParseFormatException">if the input stream does not comply with the DIMACS format.</exception>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException">si le probl?me est trivialement inconsitant</exception>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		private IProblem ParseInstance()
		{
			this.solver.Reset();
			try
			{
				SkipComments();
				ReadProblemLine();
				ReadConstrs();
				this.scanner.Close();
				return this.solver;
			}
			catch (IOException e)
			{
				throw new ParseFormatException(e);
			}
			catch (FormatException)
			{
				throw new ParseFormatException("integer value expected ");
			}
		}

		public override string Decode(int[] model)
		{
			StringBuilder stb = new StringBuilder();
			foreach (int element in model)
			{
				stb.Append(element);
				stb.Append(" ");
			}
			stb.Append("0");
			return stb.ToString();
		}

		public override void Decode(int[] model, PrintWriter @out)
		{
			foreach (int element in model)
			{
				@out.Write(element);
				@out.Write(" ");
			}
			@out.Write("0");
		}

		protected internal virtual ISolver GetSolver()
		{
			return this.solver;
		}
	}
}
