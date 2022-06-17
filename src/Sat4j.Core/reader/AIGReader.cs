using System.IO;
using System.Text;
using Org.Sat4j.Specs;
using Org.Sat4j.Tools;
using Sharpen;

namespace Org.Sat4j.Reader
{
	/// <summary>Reader for the Binary And Inverter Graph format defined by Armin Biere.</summary>
	/// <author>daniel</author>
	public class AIGReader : Org.Sat4j.Reader.Reader
	{
		private const int False = 0;

		private const int True = 1;

		private readonly GateTranslator solver;

		private int maxvarid;

		private int nbinputs;

		internal AIGReader(ISolver s)
		{
			this.solver = new GateTranslator(s);
		}

		public override string Decode(int[] model)
		{
			StringBuilder stb = new StringBuilder();
			for (int i = 0; i < this.nbinputs; i++)
			{
				stb.Append(model[i] > 0 ? 1 : 0);
			}
			return stb.ToString();
		}

		public override void Decode(int[] model, PrintWriter @out)
		{
			for (int i = 0; i < this.nbinputs; i++)
			{
				@out.Write(model[i] > 0 ? 1 : 0);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		internal virtual int ParseInt(InputStream @in, char expected)
		{
			int res;
			int ch;
			ch = @in.Read();
			if (ch < '0' || ch > '9')
			{
				throw new ParseFormatException("expected digit");
			}
			res = ch - '0';
			while ((ch = @in.Read()) >= '0' && ch <= '9')
			{
				res = 10 * res + ch - '0';
			}
			if (ch != expected)
			{
				throw new ParseFormatException("unexpected character");
			}
			return res;
		}

		/*
		* (non-Javadoc)
		*
		* @see org.sat4j.reader.Reader#parseInstance(java.io.InputStream)
		*/
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <exception cref="System.IO.IOException"/>
		public override IProblem ParseInstance(InputStream @in)
		{
			if (@in.Read() != 'a' || @in.Read() != 'i' || @in.Read() != 'g' || @in.Read() != ' ')
			{
				throw new ParseFormatException("AIG format only!");
			}
			this.maxvarid = ParseInt(@in, ' ');
			this.nbinputs = ParseInt(@in, ' ');
			int nblatches = ParseInt(@in, ' ');
			if (nblatches > 0)
			{
				throw new ParseFormatException("CNF conversion cannot handle latches!");
			}
			int nboutputs = ParseInt(@in, ' ');
			if (nboutputs > 1)
			{
				throw new ParseFormatException("CNF conversion allowed for single output circuit only!");
			}
			int nbands = ParseInt(@in, '\n');
			this.solver.NewVar(this.maxvarid + 1);
			this.solver.SetExpectedNumberOfClauses(3 * nbands + 2);
			if (nboutputs > 0)
			{
				System.Diagnostics.Debug.Assert(nboutputs == 1);
				int output0 = ParseInt(@in, '\n');
				ReadAnd(nbands, output0, @in, 2 * (this.nbinputs + 1));
			}
			return this.solver;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		internal static int SafeGet(InputStream @in)
		{
			int ch = @in.Read();
			if (ch == -1)
			{
				throw new ParseFormatException("AIG Error, EOF met too early");
			}
			return ch;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		internal static int Decode(InputStream @in)
		{
			int x = 0;
			int i = 0;
			int ch;
			while (((ch = SafeGet(@in)) & unchecked((int)(0x80))) > 0)
			{
				System.Console.Out.WriteLine("=>" + ch);
				x |= (ch & unchecked((int)(0x7f))) << 7 * i++;
			}
			return x | ch << 7 * i;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		private void ReadAnd(int nbands, int output0, InputStream @in, int startid)
		{
			int lhs = startid;
			for (int i = 0; i < nbands; i++)
			{
				int delta0 = Decode(@in);
				int delta1 = Decode(@in);
				int rhs0 = lhs - delta0;
				int rhs1 = rhs0 - delta1;
				this.solver.And(ToDimacs(lhs), ToDimacs(rhs0), ToDimacs(rhs1));
				lhs += 2;
			}
			this.solver.GateTrue(this.maxvarid + 1);
			this.solver.GateTrue(ToDimacs(output0));
		}

		private int ToDimacs(int v)
		{
			if (v == False)
			{
				return -(this.maxvarid + 1);
			}
			if (v == True)
			{
				return this.maxvarid + 1;
			}
			int var = v >> 1;
			if ((v & 1) == 0)
			{
				return var;
			}
			return -var;
		}
	}
}
