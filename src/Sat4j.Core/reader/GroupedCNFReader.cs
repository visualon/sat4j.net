using System;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Reader
{
	[System.Serializable]
	public class GroupedCNFReader : DimacsReader
	{
		private const long serialVersionUID = 1L;

		private int numberOfComponents;

		private readonly IGroupSolver groupSolver;

		private int currentComponentIndex;

		public GroupedCNFReader(IGroupSolver solver)
			: base(solver, "gcnf")
		{
			this.groupSolver = solver;
		}

		/// <exception cref="System.IO.IOException">iff an IO occurs</exception>
		/// <exception cref="ParseFormatException">if the input stream does not comply with the DIMACS format.</exception>
		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		protected internal override void ReadProblemLine()
		{
			string line = this.scanner.NextLine();
			System.Diagnostics.Debug.Assert(line != null);
			line = Sharpen.Extensions.Trim(line);
			string[] tokens = line.Split("\\s+");
			if (tokens.Length < 5 || !"p".Equals(tokens[0]) || !this.formatString.Equals(tokens[1]))
			{
				throw new ParseFormatException("problem line expected (p " + this.formatString + " ...)");
			}
			int vars;
			// reads the max var id
			vars = System.Convert.ToInt32(tokens[2]);
			System.Diagnostics.Debug.Assert(vars > 0);
			this.solver.NewVar(vars);
			// reads the number of clauses
			this.expectedNbOfConstr = System.Convert.ToInt32(tokens[3]);
			System.Diagnostics.Debug.Assert(this.expectedNbOfConstr > 0);
			this.numberOfComponents = System.Convert.ToInt32(tokens[4]);
			this.solver.SetExpectedNumberOfClauses(this.expectedNbOfConstr);
		}

		/// <since>2.1</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		protected internal override bool HandleLine()
		{
			int lit;
			bool added = false;
			string component = this.scanner.Next();
			if (!component.StartsWith("{") || !component.EndsWith("}"))
			{
				throw new ParseFormatException("Component index required at the beginning of the clause");
			}
			this.currentComponentIndex = Sharpen.Extensions.ValueOf(Sharpen.Runtime.Substring(component, 1, component.Length - 1));
			if (this.currentComponentIndex < 0 || this.currentComponentIndex > this.numberOfComponents)
			{
				throw new ParseFormatException("wrong component index: " + this.currentComponentIndex);
			}
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

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <since>2.1</since>
		protected internal override void FlushConstraint()
		{
			try
			{
				if (this.currentComponentIndex == 0)
				{
					this.groupSolver.AddClause(this.literals);
				}
				else
				{
					this.groupSolver.AddClause(this.literals, this.currentComponentIndex);
				}
			}
			catch (ArgumentException)
			{
				if (IsVerbose())
				{
					System.Console.Error.WriteLine("c Skipping constraint " + this.literals);
				}
			}
		}
	}
}
