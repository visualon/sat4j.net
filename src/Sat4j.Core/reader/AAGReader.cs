using System.IO;
using System.Text;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Org.Sat4j.Tools;
using Sharpen;

namespace Org.Sat4j.Reader
{
	/// <summary>Reader for the ASCII And Inverter Graph format defined by Armin Biere.</summary>
	/// <author>daniel</author>
	public class AAGReader : Org.Sat4j.Reader.Reader
	{
		private const int False = 0;

		private const int True = 1;

		private readonly GateTranslator solver;

		private int maxvarid;

		private int nbinputs;

		internal AAGReader(ISolver s)
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

		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <exception cref="System.IO.IOException"/>
		public override IProblem ParseInstance(InputStream @in)
		{
			EfficientScanner scanner = new EfficientScanner(@in);
			string prefix = scanner.Next();
			if (!"aag".Equals(prefix))
			{
				throw new ParseFormatException("AAG format only!");
			}
			this.maxvarid = scanner.NextInt();
			this.nbinputs = scanner.NextInt();
			int nblatches = scanner.NextInt();
			int nboutputs = scanner.NextInt();
			if (nboutputs > 1)
			{
				throw new ParseFormatException("CNF conversion allowed for single output circuit only!");
			}
			int nbands = scanner.NextInt();
			this.solver.NewVar(this.maxvarid + 1);
			this.solver.SetExpectedNumberOfClauses(3 * nbands + 2);
			ReadInput(this.nbinputs, scanner);
			System.Diagnostics.Debug.Assert(nblatches == 0);
			if (nboutputs > 0)
			{
				int output0 = ReadOutput(nboutputs, scanner);
				ReadAnd(nbands, output0, scanner);
			}
			return this.solver;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		private void ReadAnd(int nbands, int output0, EfficientScanner scanner)
		{
			for (int i = 0; i < nbands; i++)
			{
				int lhs = scanner.NextInt();
				int rhs0 = scanner.NextInt();
				int rhs1 = scanner.NextInt();
				this.solver.And(ToDimacs(lhs), ToDimacs(rhs0), ToDimacs(rhs1));
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

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		private int ReadOutput(int nboutputs, EfficientScanner scanner)
		{
			IVecInt outputs = new VecInt(nboutputs);
			for (int i = 0; i < nboutputs; i++)
			{
				outputs.Push(scanner.NextInt());
			}
			return outputs.Get(0);
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		private IVecInt ReadInput(int numberOfInputs, EfficientScanner scanner)
		{
			IVecInt inputs = new VecInt(numberOfInputs);
			for (int i = 0; i < numberOfInputs; i++)
			{
				inputs.Push(scanner.NextInt());
			}
			return inputs;
		}
	}
}
