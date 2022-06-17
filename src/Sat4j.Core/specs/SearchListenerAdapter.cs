using Sharpen;

namespace Org.Sat4j.Specs
{
	[System.Serializable]
	public abstract class SearchListenerAdapter<S> : SearchListener<S>
		where S : ISolverService
	{
		private const long serialVersionUID = 1L;

		public virtual void Init(S solverService)
		{
		}

		public virtual void Assuming(int p)
		{
		}

		public virtual void Propagating(int p)
		{
		}

		public virtual void Enqueueing(int p, IConstr reason)
		{
		}

		public virtual void Backtracking(int p)
		{
		}

		public virtual void Adding(int p)
		{
		}

		public virtual void Learn(IConstr c)
		{
		}

		public virtual void LearnUnit(int p)
		{
		}

		public virtual void Delete(IConstr c)
		{
		}

		public virtual void ConflictFound(IConstr confl, int dlevel, int trailLevel)
		{
		}

		public virtual void ConflictFound(int p)
		{
		}

		public virtual void SolutionFound(int[] model, RandomAccessModel lazyModel)
		{
		}

		public virtual void BeginLoop()
		{
		}

		public virtual void Start()
		{
		}

		public virtual void End(Lbool result)
		{
		}

		public virtual void Restarting()
		{
		}

		public virtual void Backjump(int backjumpLevel)
		{
		}

		public virtual void Cleaning()
		{
		}
	}
}
