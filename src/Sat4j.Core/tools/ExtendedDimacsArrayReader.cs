using System;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>
	/// Reader for the Extended Dimacs format proposed by Fahiem Bacchus and Toby
	/// Walsh in array representation (without the terminating 0).
	/// </summary>
	/// <remarks>
	/// Reader for the Extended Dimacs format proposed by Fahiem Bacchus and Toby
	/// Walsh in array representation (without the terminating 0).
	/// Adaptation of org.sat4j.reader.ExtendedDimacsReader.
	/// </remarks>
	/// <author>leberre</author>
	/// <author>fuhs</author>
	[System.Serializable]
	public class ExtendedDimacsArrayReader : DimacsArrayReader
	{
		public const int False = 1;

		public const int True = 2;

		public const int Not = 3;

		public const int And = 4;

		public const int Nand = 5;

		public const int Or = 6;

		public const int Nor = 7;

		public const int Xor = 8;

		public const int Xnor = 9;

		public const int Implies = 10;

		public const int Iff = 11;

		public const int Ifthenelse = 12;

		public const int Atleast = 13;

		public const int Atmost = 14;

		public const int Count = 15;

		private const long serialVersionUID = 1L;

		private readonly GateTranslator gater;

		public ExtendedDimacsArrayReader(ISolver solver)
			: base(solver)
		{
			this.gater = new GateTranslator(solver);
		}

		/// <summary>Handles a single constraint (constraint == Extended Dimacs circuit gate).</summary>
		/// <param name="gateType">the type of the gate in question</param>
		/// <param name="output">the number of the output of the gate in question</param>
		/// <param name="inputs">
		/// the numbers of the inputs of the gates in question; the array
		/// must have the corresponding length for the gate type unless
		/// arbitrary lengths are allowed (i.e., 0 for TRUE and FALSE, 1
		/// for NOT, or 3 for ITE)
		/// </param>
		/// <returns>true</returns>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		protected internal override bool HandleConstr(int gateType, int output, int[] inputs)
		{
			IVecInt literals;
			switch (gateType)
			{
				case False:
				{
					System.Diagnostics.Debug.Assert(inputs.Length == 0);
					this.gater.GateFalse(output);
					break;
				}

				case True:
				{
					System.Diagnostics.Debug.Assert(inputs.Length == 0);
					this.gater.GateTrue(output);
					break;
				}

				case Or:
				{
					literals = new VecInt(inputs);
					this.gater.Or(output, literals);
					break;
				}

				case Not:
				{
					System.Diagnostics.Debug.Assert(inputs.Length == 1);
					this.gater.Not(output, inputs[0]);
					break;
				}

				case And:
				{
					literals = new VecInt(inputs);
					this.gater.And(output, literals);
					break;
				}

				case Xor:
				{
					literals = new VecInt(inputs);
					this.gater.Xor(output, literals);
					break;
				}

				case Iff:
				{
					literals = new VecInt(inputs);
					this.gater.Iff(output, literals);
					break;
				}

				case Ifthenelse:
				{
					System.Diagnostics.Debug.Assert(inputs.Length == 3);
					this.gater.Ite(output, inputs[0], inputs[1], inputs[2]);
					break;
				}

				default:
				{
					throw new NotSupportedException("Gate type " + gateType + " not handled yet");
				}
			}
			return true;
		}
	}
}
