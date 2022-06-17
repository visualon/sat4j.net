using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Learning
{
	/// <summary>MiniSAT learning scheme.</summary>
	/// <remarks>
	/// MiniSAT learning scheme.
	/// The Data Structure Factory is expected to be set thanks to the appropriate
	/// setter method before using it.
	/// It was not possible to set it in the constructor.
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class MiniSATLearning<D> : AbstractLearning<D>
		where D : DataStructureFactory
	{
		private const long serialVersionUID = 1L;

		private DataStructureFactory dsf;

		public void SetDataStructureFactory(DataStructureFactory dsf)
		{
			this.dsf = dsf;
		}

		public override void SetSolver(Solver<D> s)
		{
			base.SetSolver(s);
			if (s != null)
			{
				this.dsf = s.GetDSFactory();
			}
		}

		public override void Learns(Constr constr)
		{
			// va contenir une nouvelle clause ou null si la clause est unitaire
			ClaBumpActivity(constr);
			this.dsf.LearnConstraint(constr);
		}

		/*
		* (non-Javadoc)
		*
		* @see java.lang.Object#toString()
		*/
		public override string ToString()
		{
			return "Learn all clauses as in MiniSAT";
		}
	}
}
