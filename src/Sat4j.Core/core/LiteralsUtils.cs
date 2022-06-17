using Sharpen;

namespace Org.Sat4j.Core
{
	/// <summary>Utility methods to avoid using bit manipulation inside code.</summary>
	/// <remarks>
	/// Utility methods to avoid using bit manipulation inside code. One should use
	/// Java 1.5 import static feature to use it without class qualification inside
	/// the code.
	/// In the DIMACS format, the literals are represented by signed integers, 0
	/// denoting the end of the clause. In the solver, the literals are represented
	/// by positive integers, in order to use them as index in arrays for instance.
	/// <pre>
	/// int p : a literal (p&gt;1)
	/// p &circ; 1 : the negation of the literal
	/// p &gt;&gt; 1 : the DIMACS number representing the variable.
	/// int v : a DIMACS variable (v&gt;0)
	/// v &lt;&lt; 1 : a positive literal for that variable in the solver.
	/// v &lt;&lt; 1 &circ; 1 : a negative literal for that variable.
	/// </pre>
	/// </remarks>
	/// <author>leberre</author>
	public sealed class LiteralsUtils
	{
		private LiteralsUtils()
		{
		}

		// no instance supposed to be created.
		/// <summary>Returns the variable associated to the literal</summary>
		/// <param name="p">a literal in internal representation</param>
		/// <returns>the Dimacs variable associated to that literal.</returns>
		public static int Var(int p)
		{
			System.Diagnostics.Debug.Assert(p > 1);
			return p >> 1;
		}

		/// <summary>Returns the opposite literal.</summary>
		/// <param name="p">a literal in internal representation</param>
		/// <returns>the opposite literal in internal representation</returns>
		public static int Neg(int p)
		{
			return p ^ 1;
		}

		/// <summary>Returns the positive literal associated with a variable.</summary>
		/// <param name="var">a variable in Dimacs format</param>
		/// <returns>
		/// the positive literal associated with this variable in internal
		/// representation
		/// </returns>
		public static int PosLit(int var)
		{
			return var << 1;
		}

		/// <summary>Returns the negative literal associated with a variable.</summary>
		/// <param name="var">a variable in Dimacs format</param>
		/// <returns>
		/// the negative literal associated with this variable in internal
		/// representation
		/// </returns>
		public static int NegLit(int var)
		{
			return var << 1 ^ 1;
		}

		/// <summary>
		/// decode the internal representation of a literal in internal
		/// representation into Dimacs format.
		/// </summary>
		/// <param name="p">the literal in internal representation</param>
		/// <returns>the literal in dimacs representation</returns>
		public static int ToDimacs(int p)
		{
			return ((p & 1) == 0 ? 1 : -1) * (p >> 1);
		}

		/// <summary>
		/// encode the classical Dimacs representation (negated integers for negated
		/// literals) into the internal format.
		/// </summary>
		/// <param name="x">the literal in Dimacs format</param>
		/// <returns>the literal in internal format.</returns>
		/// <since>2.2</since>
		public static int ToInternal(int x)
		{
			return x < 0 ? -x << 1 ^ 1 : x << 1;
		}
	}
}
