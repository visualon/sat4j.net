using Sharpen;

namespace Org.Sat4j
{
	/// <summary>
	/// Enumeration allowing to manage easily exit code for the SAT and PB
	/// Competitions.
	/// </summary>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class ExitCode
	{
		public static readonly Org.Sat4j.ExitCode OptimumFound = new Org.Sat4j.ExitCode(30, "OPTIMUM FOUND");

		public static readonly Org.Sat4j.ExitCode UpperBound = new Org.Sat4j.ExitCode(30, "UPPER BOUND");

		public static readonly Org.Sat4j.ExitCode Satisfiable = new Org.Sat4j.ExitCode(10, "SATISFIABLE");

		public static readonly Org.Sat4j.ExitCode Unknown = new Org.Sat4j.ExitCode(0, "UNKNOWN");

		public static readonly Org.Sat4j.ExitCode Unsatisfiable = new Org.Sat4j.ExitCode(20, "UNSATISFIABLE");

		/// <summary>value of the exit code.</summary>
		private readonly int value;

		/// <summary>alternative textual representation of the exit code.</summary>
		private readonly string str;

		/// <summary>
		/// creates an exit code with a given value and an alternative textual
		/// representation.
		/// </summary>
		/// <param name="i">the value of the exit code</param>
		/// <param name="str">the alternative textual representation</param>
		private ExitCode(int i, string str)
		{
			this.value = i;
			this.str = str;
		}

		/// <returns>the exit code value</returns>
		public int Value()
		{
			return this.value;
		}

		/// <returns>
		/// the name of the enum or the alternative textual representation if
		/// any.
		/// </returns>
		public override string ToString()
		{
			return this.str;
		}
	}
}
