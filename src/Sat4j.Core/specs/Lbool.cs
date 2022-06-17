using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>
	/// That enumeration defines the possible truth value for a variable: satisfied,
	/// falsified or unknown/undefined.
	/// </summary>
	/// <remarks>
	/// That enumeration defines the possible truth value for a variable: satisfied,
	/// falsified or unknown/undefined.
	/// (that class moved from org.sat4j.minisat.core in earlier version of SAT4J).
	/// </remarks>
	/// <author>leberre</author>
	/// <since>2.1</since>
	public sealed class Lbool
	{
		public static readonly Org.Sat4j.Specs.Lbool False = new Org.Sat4j.Specs.Lbool("F");

		public static readonly Org.Sat4j.Specs.Lbool True = new Org.Sat4j.Specs.Lbool("T");

		public static readonly Org.Sat4j.Specs.Lbool Undefined = new Org.Sat4j.Specs.Lbool("U");

		static Lbool()
		{
			// usual boolean rules for negation
			False.opposite = True;
			True.opposite = False;
			Undefined.opposite = Undefined;
		}

		private Lbool(string symbol)
		{
			this.symbol = symbol;
		}

		/// <summary>boolean negation.</summary>
		/// <returns>Boolean negation. The negation of UNDEFINED is UNDEFINED.</returns>
		public Org.Sat4j.Specs.Lbool Not()
		{
			return this.opposite;
		}

		/// <summary>Textual representation for the truth value.</summary>
		/// <returns>"T","F" or "U"</returns>
		public override string ToString()
		{
			return this.symbol;
		}

		/// <summary>The symbol representing the truth value.</summary>
		private readonly string symbol;

		/// <summary>the opposite truth value.</summary>
		private Org.Sat4j.Specs.Lbool opposite;
	}
}
