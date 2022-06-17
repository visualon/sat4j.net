using Mono.Math;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>Utility class to easily feed a SAT solver using logical gates.</summary>
	/// <author>leberre</author>
	[System.Serializable]
	public class GateTranslator : SolverDecorator<ISolver>
	{
		private const long serialVersionUID = 1L;

		public GateTranslator(ISolver solver)
			: base(solver)
		{
		}

		/// <summary>translate <code>y &lt;=&gt; FALSE</code> into a clause.</summary>
		/// <param name="y">a variable to falsify</param>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException">iff a trivial inconsistency is found.</exception>
		/// <since>2.1</since>
		public virtual IConstr GateFalse(int y)
		{
			IVecInt clause = new VecInt(2);
			clause.Push(-y);
			return ProcessClause(clause);
		}

		/// <summary>translate <code>y &lt;=&gt; TRUE</code> into a clause.</summary>
		/// <param name="y">a variable to verify</param>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <since>2.1</since>
		public virtual IConstr GateTrue(int y)
		{
			IVecInt clause = new VecInt(2);
			clause.Push(y);
			return ProcessClause(clause);
		}

		/// <summary>translate <code>y &lt;=&gt; if x1 then x2 else x3</code> into clauses.</summary>
		/// <param name="y"/>
		/// <param name="x1">the selector variable</param>
		/// <param name="x2"/>
		/// <param name="x3"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <since>2.1</since>
		public virtual IConstr[] Ite(int y, int x1, int x2, int x3)
		{
			IConstr[] constrs = new IConstr[6];
			IVecInt clause = new VecInt(5);
			// y &lt;=&gt; (x1 -> x2) and (not x1 -> x3)
			// y -> (x1 -> x2) and (not x1 -> x3)
			clause.Push(-y).Push(-x1).Push(x2);
			constrs[0] = ProcessClause(clause);
			clause.Clear();
			clause.Push(-y).Push(x1).Push(x3);
			constrs[1] = ProcessClause(clause);
			// y <- (x1 -> x2) and (not x1 -> x3)
			// not(x1 -> x2) or not(not x1 -> x3) or y
			// x1 and not x2 or not x1 and not x3 or y
			// (x1 and not x2) or ((not x1 or y) and (not x3 or y))
			// (x1 or not x1 or y) and (not x2 or not x1 or y) and (x1 or not x3 or
			// y) and (not x2 or not x3 or y)
			// not x1 or not x2 or y and x1 or not x3 or y and not x2 or not x3 or y
			clause.Clear();
			clause.Push(-x1).Push(-x2).Push(y);
			constrs[2] = ProcessClause(clause);
			clause.Clear();
			clause.Push(x1).Push(-x3).Push(y);
			constrs[3] = ProcessClause(clause);
			clause.Clear();
			clause.Push(-x2).Push(-x3).Push(y);
			constrs[4] = ProcessClause(clause);
			// taken from Niklas Een et al SAT 2007 paper
			// Adding the following redundant clause will improve unit propagation
			// y -> x2 or x3
			clause.Clear();
			clause.Push(-y).Push(x2).Push(x3);
			constrs[5] = ProcessClause(clause);
			return constrs;
		}

		/// <summary>translate <code>y &lt;=&gt; (x1 =&gt; x2)</code></summary>
		/// <param name="y"/>
		/// <param name="x1">the selector variable</param>
		/// <param name="x2"/>
		/// <returns/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <since>2.3.6</since>
		public virtual IConstr[] It(int y, int x1, int x2)
		{
			IConstr[] constrs = new IConstr[3];
			IVecInt clause = new VecInt(5);
			// y &lt;=&gt; (x1 -> x2)
			// y -> (x1 -> x2)
			clause.Push(-y).Push(-x1).Push(x2);
			constrs[0] = ProcessClause(clause);
			clause.Clear();
			// y <- (x1 -> x2)
			// not(x1 -> x2) or y
			// x1 and not x2 or y
			// (x1 or y) and (not x2 or y)
			clause.Push(x1).Push(y);
			constrs[1] = ProcessClause(clause);
			clause.Clear();
			clause.Push(-x2).Push(y);
			constrs[2] = ProcessClause(clause);
			return constrs;
		}

		/// <summary>Translate <code>y &lt;=&gt; x1 /\ x2 /\ ...</summary>
		/// <remarks>Translate <code>y &lt;=&gt; x1 /\ x2 /\ ... /\ xn</code> into clauses.</remarks>
		/// <param name="y"/>
		/// <param name="literals">the x1 ... xn literals.</param>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <since>2.1</since>
		public virtual IConstr[] And(int y, IVecInt literals)
		{
			// y &lt;=&gt; AND x1 ... xn
			IConstr[] constrs = new IConstr[literals.Size() + 1];
			// y <= x1 .. xn
			IVecInt clause = new VecInt(literals.Size() + 2);
			clause.Push(y);
			for (int i = 0; i < literals.Size(); i++)
			{
				clause.Push(-literals.Get(i));
			}
			constrs[0] = ProcessClause(clause);
			clause.Clear();
			for (int i_1 = 0; i_1 < literals.Size(); i_1++)
			{
				// y => xi
				clause.Clear();
				clause.Push(-y);
				clause.Push(literals.Get(i_1));
				constrs[i_1 + 1] = ProcessClause(clause);
			}
			return constrs;
		}

		/// <summary>Translate <code>y &lt;=&gt; x1 /\ x2</code></summary>
		/// <param name="y"/>
		/// <param name="x1"/>
		/// <param name="x2"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <since>2.1</since>
		public virtual IConstr[] And(int y, int x1, int x2)
		{
			IVecInt clause = new VecInt(4);
			IConstr[] constrs = new IConstr[3];
			clause.Push(-y);
			clause.Push(x1);
			constrs[0] = AddClause(clause);
			clause.Clear();
			clause.Push(-y);
			clause.Push(x2);
			constrs[1] = AddClause(clause);
			clause.Clear();
			clause.Push(y);
			clause.Push(-x1);
			clause.Push(-x2);
			constrs[2] = AddClause(clause);
			return constrs;
		}

		/// <summary>translate <code>y &lt;=&gt; x1 \/ x2 \/ ...</summary>
		/// <remarks>translate <code>y &lt;=&gt; x1 \/ x2 \/ ... \/ xn</code> into clauses.</remarks>
		/// <param name="y"/>
		/// <param name="literals"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <since>2.1</since>
		public virtual IConstr[] Or(int y, IVecInt literals)
		{
			// y &lt;=&gt; OR x1 x2 ...xn
			// y => x1 x2 ... xn
			IConstr[] constrs = new IConstr[literals.Size() + 1];
			IVecInt clause = new VecInt(literals.Size() + 2);
			literals.CopyTo(clause);
			clause.Push(-y);
			constrs[0] = ProcessClause(clause);
			clause.Clear();
			for (int i = 0; i < literals.Size(); i++)
			{
				// xi => y
				clause.Clear();
				clause.Push(y);
				clause.Push(-literals.Get(i));
				constrs[i + 1] = ProcessClause(clause);
			}
			return constrs;
		}

		/// <summary>translate <code>y &lt;= x1 \/ x2 \/ ...</summary>
		/// <remarks>translate <code>y &lt;= x1 \/ x2 \/ ... \/ xn</code> into clauses.</remarks>
		/// <param name="y"/>
		/// <param name="literals"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <since>2.1</since>
		public virtual IConstr[] HalfOr(int y, IVecInt literals)
		{
			IConstr[] constrs = new IConstr[literals.Size()];
			IVecInt clause = new VecInt(literals.Size() + 2);
			for (int i = 0; i < literals.Size(); i++)
			{
				// xi => y
				clause.Clear();
				clause.Push(y);
				clause.Push(-literals.Get(i));
				constrs[i] = ProcessClause(clause);
			}
			return constrs;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		private IConstr ProcessClause(IVecInt clause)
		{
			return AddClause(clause);
		}

		/// <summary>Translate <code>y &lt;=&gt; not x</code> into clauses.</summary>
		/// <param name="y"/>
		/// <param name="x"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <since>2.1</since>
		public virtual IConstr[] Not(int y, int x)
		{
			IConstr[] constrs = new IConstr[2];
			IVecInt clause = new VecInt(3);
			// y &lt;=&gt; not x
			// y => not x = not y or not x
			clause.Push(-y).Push(-x);
			constrs[0] = ProcessClause(clause);
			// y <= not x = y or x
			clause.Clear();
			clause.Push(y).Push(x);
			constrs[1] = ProcessClause(clause);
			return constrs;
		}

		/// <summary>translate <code>y &lt;=&gt; x1 xor x2 xor ...</summary>
		/// <remarks>translate <code>y &lt;=&gt; x1 xor x2 xor ... xor xn</code> into clauses.</remarks>
		/// <param name="y"/>
		/// <param name="literals"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <since>2.1</since>
		public virtual IConstr[] Xor(int y, IVecInt literals)
		{
			literals.Push(-y);
			int[] f = new int[literals.Size()];
			literals.CopyTo(f);
			IVec<IConstr> vconstrs = new Vec<IConstr>();
			Xor2Clause(f, 0, false, vconstrs);
			IConstr[] constrs = new IConstr[vconstrs.Size()];
			vconstrs.CopyTo(constrs);
			return constrs;
		}

		/// <summary>
		/// translate
		/// <code>y &lt;=&gt; (x1 &lt;=&gt; x2 &lt;=&gt; ...
		/// </summary>
		/// <remarks>
		/// translate
		/// <code>y &lt;=&gt; (x1 &lt;=&gt; x2 &lt;=&gt; ... &lt;=&gt; xn)</code>
		/// into clauses.
		/// </remarks>
		/// <param name="y"/>
		/// <param name="literals"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <since>2.1</since>
		public virtual IConstr[] Iff(int y, IVecInt literals)
		{
			literals.Push(y);
			int[] f = new int[literals.Size()];
			literals.CopyTo(f);
			IVec<IConstr> vconstrs = new Vec<IConstr>();
			Iff2Clause(f, 0, false, vconstrs);
			IConstr[] constrs = new IConstr[vconstrs.Size()];
			vconstrs.CopyTo(constrs);
			return constrs;
		}

		/// <since>2.2</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual void Xor(int x, int a, int b)
		{
			IVecInt clause = new VecInt(3);
			clause.Push(-a).Push(b).Push(x);
			ProcessClause(clause);
			clause.Clear();
			clause.Push(a).Push(-b).Push(x);
			ProcessClause(clause);
			clause.Clear();
			clause.Push(-a).Push(-b).Push(-x);
			ProcessClause(clause);
			clause.Clear();
			clause.Push(a).Push(b).Push(-x);
			ProcessClause(clause);
			clause.Clear();
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		private void Xor2Clause(int[] f, int prefix, bool negation, IVec<IConstr> constrs)
		{
			if (prefix == f.Length - 1)
			{
				IVecInt clause = new VecInt(f.Length + 1);
				for (int i = 0; i < f.Length - 1; ++i)
				{
					clause.Push(f[i]);
				}
				clause.Push(f[f.Length - 1] * (negation ? -1 : 1));
				constrs.Push(ProcessClause(clause));
				return;
			}
			if (negation)
			{
				f[prefix] = -f[prefix];
				Xor2Clause(f, prefix + 1, false, constrs);
				f[prefix] = -f[prefix];
				Xor2Clause(f, prefix + 1, true, constrs);
			}
			else
			{
				Xor2Clause(f, prefix + 1, false, constrs);
				f[prefix] = -f[prefix];
				Xor2Clause(f, prefix + 1, true, constrs);
				f[prefix] = -f[prefix];
			}
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		private void Iff2Clause(int[] f, int prefix, bool negation, IVec<IConstr> constrs)
		{
			if (prefix == f.Length - 1)
			{
				IVecInt clause = new VecInt(f.Length + 1);
				for (int i = 0; i < f.Length - 1; ++i)
				{
					clause.Push(f[i]);
				}
				clause.Push(f[f.Length - 1] * (negation ? -1 : 1));
				ProcessClause(clause);
				return;
			}
			if (negation)
			{
				Iff2Clause(f, prefix + 1, false, constrs);
				f[prefix] = -f[prefix];
				Iff2Clause(f, prefix + 1, true, constrs);
				f[prefix] = -f[prefix];
			}
			else
			{
				f[prefix] = -f[prefix];
				Iff2Clause(f, prefix + 1, false, constrs);
				f[prefix] = -f[prefix];
				Iff2Clause(f, prefix + 1, true, constrs);
			}
		}

		// From Een and Sorensson JSAT 2006 paper
		/// <since>2.2</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual void FullAdderSum(int x, int a, int b, int c)
		{
			IVecInt clause = new VecInt(4);
			// -a /\ -b /\ -c -> -x
			clause.Push(a).Push(b).Push(c).Push(-x);
			ProcessClause(clause);
			clause.Clear();
			// -a /\ b /\ c -> -x
			clause.Push(a).Push(-b).Push(-c).Push(-x);
			ProcessClause(clause);
			clause.Clear();
			clause.Push(-a).Push(b).Push(-c).Push(-x);
			ProcessClause(clause);
			clause.Clear();
			clause.Push(-a).Push(-b).Push(c).Push(-x);
			ProcessClause(clause);
			clause.Clear();
			clause.Push(-a).Push(-b).Push(-c).Push(x);
			ProcessClause(clause);
			clause.Clear();
			clause.Push(-a).Push(b).Push(c).Push(x);
			ProcessClause(clause);
			clause.Clear();
			clause.Push(a).Push(-b).Push(c).Push(x);
			ProcessClause(clause);
			clause.Clear();
			clause.Push(a).Push(b).Push(-c).Push(x);
			ProcessClause(clause);
			clause.Clear();
		}

		/// <since>2.2</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual void FullAdderCarry(int x, int a, int b, int c)
		{
			IVecInt clause = new VecInt(3);
			clause.Push(-b).Push(-c).Push(x);
			ProcessClause(clause);
			clause.Clear();
			clause.Push(-a).Push(-c).Push(x);
			ProcessClause(clause);
			clause.Clear();
			clause.Push(-a).Push(-b).Push(x);
			ProcessClause(clause);
			clause.Clear();
			clause.Push(b).Push(c).Push(-x);
			ProcessClause(clause);
			clause.Clear();
			clause.Push(a).Push(c).Push(-x);
			ProcessClause(clause);
			clause.Clear();
			clause.Push(a).Push(b).Push(-x);
			ProcessClause(clause);
			clause.Clear();
		}

		/// <since>2.2</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual void AdditionalFullAdderConstraints(int xcarry, int xsum, int a, int b, int c)
		{
			IVecInt clause = new VecInt(3);
			clause.Push(-xcarry).Push(-xsum).Push(a);
			ProcessClause(clause);
			clause.Push(-xcarry).Push(-xsum).Push(b);
			ProcessClause(clause);
			clause.Push(-xcarry).Push(-xsum).Push(c);
			ProcessClause(clause);
			clause.Push(xcarry).Push(xsum).Push(-a);
			ProcessClause(clause);
			clause.Push(xcarry).Push(xsum).Push(-b);
			ProcessClause(clause);
			clause.Push(xcarry).Push(xsum).Push(-c);
			ProcessClause(clause);
		}

		/// <since>2.2</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual void HalfAdderSum(int x, int a, int b)
		{
			Xor(x, a, b);
		}

		/// <since>2.2</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual void HalfAdderCarry(int x, int a, int b)
		{
			And(x, a, b);
		}

		/// <summary>
		/// Translate an optimization function into constraints and provides the
		/// binary literals in results.
		/// </summary>
		/// <remarks>
		/// Translate an optimization function into constraints and provides the
		/// binary literals in results. Works only when the value of the objective
		/// function is positive.
		/// </remarks>
		/// <since>2.2</since>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public virtual void OptimisationFunction(IVecInt literals, IVec<BigInteger> coefs, IVecInt result)
		{
			IVec<IVecInt> buckets = new Vec<IVecInt>();
			IVecInt bucket;
			// filling the buckets
			for (int i = 0; i < literals.Size(); i++)
			{
				int p = literals.Get(i);
				BigInteger a = coefs.Get(i);
				for (int j = 0; j < a.BitLength(); j++)
				{
					bucket = CreateIfNull(buckets, j);
					if (a.TestBit(j))
					{
						bucket.Push(p);
					}
				}
			}
			// creating the adder
			int x;
			int y;
			int z;
			int sum;
			int carry;
			for (int i_1 = 0; i_1 < buckets.Size(); i_1++)
			{
				bucket = buckets.Get(i_1);
				while (bucket.Size() >= 3)
				{
					x = bucket.Get(0);
					y = bucket.Get(1);
					z = bucket.Get(2);
					bucket.Remove(x);
					bucket.Remove(y);
					bucket.Remove(z);
					sum = NextFreeVarId(true);
					carry = NextFreeVarId(true);
					FullAdderSum(sum, x, y, z);
					FullAdderCarry(carry, x, y, z);
					AdditionalFullAdderConstraints(carry, sum, x, y, z);
					bucket.Push(sum);
					CreateIfNull(buckets, i_1 + 1).Push(carry);
				}
				while (bucket.Size() == 2)
				{
					x = bucket.Get(0);
					y = bucket.Get(1);
					bucket.Remove(x);
					bucket.Remove(y);
					sum = NextFreeVarId(true);
					carry = NextFreeVarId(true);
					HalfAdderSum(sum, x, y);
					HalfAdderCarry(carry, x, y);
					bucket.Push(sum);
					CreateIfNull(buckets, i_1 + 1).Push(carry);
				}
				System.Diagnostics.Debug.Assert(bucket.Size() == 1);
				result.Push(bucket.Last());
				bucket.Pop();
				System.Diagnostics.Debug.Assert(bucket.IsEmpty());
			}
		}

		/// <summary>Create a new bucket if it does not exists.</summary>
		/// <remarks>
		/// Create a new bucket if it does not exists. Works only because the buckets
		/// are explored in increasing order.
		/// </remarks>
		/// <param name="buckets"/>
		/// <param name="i"/>
		/// <returns/>
		private IVecInt CreateIfNull(IVec<IVecInt> buckets, int i)
		{
			IVecInt bucket = buckets.Get(i);
			if (bucket == null)
			{
				bucket = new VecInt();
				buckets.Push(bucket);
				System.Diagnostics.Debug.Assert(buckets.Get(i) == bucket);
			}
			return bucket;
		}
	}
}
