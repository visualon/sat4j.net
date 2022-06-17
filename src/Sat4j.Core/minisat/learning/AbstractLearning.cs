using Org.Sat4j.Minisat.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Learning
{
	/// <summary>An abstract learning strategy.</summary>
	/// <remarks>
	/// An abstract learning strategy.
	/// The Variable Activity Listener is expected to be set thanks to the
	/// appropriate setter method before using it.
	/// It was not possible to set it in the constructor.
	/// </remarks>
	/// <author>daniel</author>
	[System.Serializable]
	internal abstract class AbstractLearning<D> : LearningStrategy<D>
		where D : DataStructureFactory
	{
		private const long serialVersionUID = 1L;

		private VarActivityListener val;

		public virtual void SetVarActivityListener(VarActivityListener s)
		{
			this.val = s;
		}

		public virtual void SetSolver(Solver<D> s)
		{
			this.val = s;
		}

		public void ClaBumpActivity(Constr reason)
		{
			for (int i = 0; i < reason.Size(); i++)
			{
				int q = reason.Get(i);
				System.Diagnostics.Debug.Assert(q > 1);
				this.val.VarBumpActivity(q);
			}
		}

		public virtual void Init()
		{
		}

		public abstract void Learns(Constr arg1);
	}
}
