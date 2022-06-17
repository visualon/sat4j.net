using System;
using System.IO;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Reader
{
	/// <summary>Simple JSON reader for clauses and cardinality constraints.</summary>
	/// <remarks>
	/// Simple JSON reader for clauses and cardinality constraints.
	/// Clauses are represented as an array of Dimacs literals (non zero integers).
	/// Cardinality constraints are represented like a clause for its left hand side,
	/// a comparator (a string) and a number.
	/// <code>[[-1,-2,-3],[[1,-2,3],'&gt;',2],[4,-3,6]]</code> for instance
	/// represents three constraints, two clauses and the cardinality constraint
	/// <code>x1 + not x2 + x3 &gt; 2</code>.
	/// </remarks>
	/// <author>leberre</author>
	/// <?/>
	/// <since>2.3.3</since>
	public class JSONReader<S> : Org.Sat4j.Reader.Reader
		where S : ISolver
	{
		protected internal readonly S solver;

		public const string Clause = "(\\[(-?(\\d+)(,-?(\\d+))*)?\\])";

		public const string Card = "(\\[" + Clause + ",'[=<>]=?',-?\\d+\\])";

		public readonly string constraint;

		public readonly string formula;

		private static readonly Sharpen.Pattern ClausePattern = Sharpen.Pattern.Compile(Clause);

		private static readonly Sharpen.Pattern CardPattern = Sharpen.Pattern.Compile(Card);

		private readonly Sharpen.Pattern constraintPattern;

		public JSONReader(S solver)
		{
			this.solver = solver;
			constraint = ConstraintRegexp();
			formula = "^\\[(" + constraint + "(," + constraint + ")*)?\\]$";
			constraintPattern = Sharpen.Pattern.Compile(constraint);
		}

		protected internal virtual string ConstraintRegexp()
		{
			return "(" + Clause + "|" + Card + ")";
		}

		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		private void HandleConstraint(string constraint)
		{
			if (CardPattern.Matcher(constraint).Matches())
			{
				HandleCard(constraint);
			}
			else
			{
				if (ClausePattern.Matcher(constraint).Matches())
				{
					HandleClause(constraint);
				}
				else
				{
					HandleNotHandled(constraint);
				}
			}
		}

		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		protected internal virtual void HandleNotHandled(string constraint)
		{
			throw new ParseFormatException("Unknown constraint: " + constraint);
		}

		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		private void HandleClause(string constraint)
		{
			solver.AddClause(GetLiterals(constraint));
		}

		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		protected internal virtual IVecInt GetLiterals(string constraint)
		{
			string trimmed = Sharpen.Extensions.Trim(constraint);
			trimmed = Sharpen.Runtime.Substring(trimmed, 1, trimmed.Length - 1);
			string[] literals = trimmed.Split(",");
			IVecInt clause = new VecInt();
			foreach (string literal in literals)
			{
				if (literal.Length > 0)
				{
					clause.Push(Sharpen.Extensions.ValueOf(Sharpen.Extensions.Trim(literal)));
				}
			}
			return clause;
		}

		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		protected internal virtual void HandleCard(string constraint)
		{
			string trimmed = Sharpen.Extensions.Trim(constraint);
			trimmed = Sharpen.Runtime.Substring(trimmed, 1, trimmed.Length - 1);
			Matcher matcher = ClausePattern.Matcher(trimmed);
			if (matcher.Find())
			{
				IVecInt clause = GetLiterals(matcher.Group());
				trimmed = matcher.ReplaceFirst(string.Empty);
				string[] str = trimmed.Split(",");
				int degree = Sharpen.Extensions.ValueOf(str[2]);
				string comparator = Sharpen.Runtime.Substring(str[1], 1, str[1].Length - 1);
				if ("=".Equals(comparator) || ("==".Equals(comparator)))
				{
					solver.AddExactly(clause, degree);
				}
				else
				{
					if ("<=".Equals(comparator))
					{
						solver.AddAtMost(clause, degree);
					}
					else
					{
						if ("<".Equals(comparator))
						{
							solver.AddAtMost(clause, degree - 1);
						}
						else
						{
							if (">=".Equals(comparator))
							{
								solver.AddAtLeast(clause, degree);
							}
							else
							{
								if (">".Equals(comparator))
								{
									solver.AddAtLeast(clause, degree + 1);
								}
							}
						}
					}
				}
			}
		}

		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <exception cref="System.IO.IOException"/>
		public override IProblem ParseInstance(InputStream @in)
		{
			StringWriter @out = new StringWriter();
			BufferedReader reader = new BufferedReader(new InputStreamReader(@in));
			string line;
			while ((line = reader.ReadLine()) != null)
			{
				@out.Append(line);
			}
			return ParseString(@out.ToString());
		}

		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual ISolver ParseString(string json)
		{
			string trimmed = Sharpen.Extensions.Trim(json);
			if (!trimmed.Matches(formula))
			{
				throw new ParseFormatException("Wrong input " + json);
			}
			Matcher matcher = constraintPattern.Matcher(trimmed);
			while (matcher.Find())
			{
				HandleConstraint(matcher.Group());
			}
			return solver;
		}

		[Obsolete]
		public override string Decode(int[] model)
		{
			return "[" + new VecInt(model) + "]";
		}

		public override void Decode(int[] model, PrintWriter @out)
		{
			@out.Write("[");
			@out.Write(new VecInt(model));
			@out.Write("]");
		}
	}
}
