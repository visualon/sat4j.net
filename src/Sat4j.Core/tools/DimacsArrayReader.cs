using System.Text;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>Very simple Dimacs array reader.</summary>
	/// <remarks>
	/// Very simple Dimacs array reader. Allow solvers to read the constraints from
	/// arrays that effectively contain Dimacs formatted lines (without the
	/// terminating 0).
	/// Adaptation of org.sat4j.reader.DimacsReader.
	/// </remarks>
	/// <author>dlb</author>
	/// <author>fuhs</author>
	[System.Serializable]
	public class DimacsArrayReader
	{
		private const long serialVersionUID = 1L;

		protected internal readonly ISolver solver;

		public DimacsArrayReader(ISolver solver)
		{
			this.solver = solver;
		}

		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		protected internal virtual bool HandleConstr(int gateType, int output, int[] inputs)
		{
			IVecInt literals = new VecInt(inputs);
			this.solver.AddClause(literals);
			return true;
		}

		/// <param name="gateType">
		/// gateType[i] is the type of gate i according to the Extended
		/// Dimacs specs; ignored in DimacsArrayReader, but important for
		/// inheriting classes
		/// </param>
		/// <param name="outputs">
		/// outputs[i] is the number of the output; ignored in
		/// DimacsArrayReader
		/// </param>
		/// <param name="inputs">
		/// inputs[i] contains the clauses in DimacsArrayReader; an
		/// overriding class might have it contain the inputs of the
		/// current gate
		/// </param>
		/// <param name="maxVar">the maximum number of assigned ids</param>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException">si le probleme est trivialement inconsitant</exception>
		public virtual ISolver ParseInstance(int[] gateType, int[] outputs, int[][] inputs, int maxVar)
		{
			this.solver.Reset();
			this.solver.NewVar(maxVar);
			this.solver.SetExpectedNumberOfClauses(outputs.Length);
			for (int i = 0; i < outputs.Length; ++i)
			{
				HandleConstr(gateType[i], outputs[i], inputs[i]);
			}
			return this.solver;
		}

		public virtual string Decode(int[] model)
		{
			StringBuilder stb = new StringBuilder(4 * model.Length);
			foreach (int element in model)
			{
				stb.Append(element);
				stb.Append(" ");
			}
			stb.Append("0");
			return stb.ToString();
		}

		protected internal virtual ISolver GetSolver()
		{
			return this.solver;
		}
	}
}
