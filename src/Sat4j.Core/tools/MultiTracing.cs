using System.Collections.Generic;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>Allow to feed the solver with several SearchListener.</summary>
	/// <author>leberre</author>
	[System.Serializable]
	public class MultiTracing<T> : SearchListener<T>
		where T : ISolverService
	{
		private const long serialVersionUID = 1L;

		private readonly ICollection<SearchListener<T>> listeners = new AList<SearchListener<T>>();

		public MultiTracing(params SearchListener<T>[] listeners)
		{
			Sharpen.Collections.AddAll(this.listeners, Arrays.AsList(listeners));
		}

		public MultiTracing(IList<SearchListener<T>> listenersList)
		{
			Sharpen.Collections.AddAll(this.listeners, listenersList);
		}

		public virtual void Assuming(int p)
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.Assuming(p);
			}
		}

		public virtual void Propagating(int p)
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.Propagating(p);
			}
		}

		public virtual void Enqueueing(int p, IConstr reason)
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.Enqueueing(p, reason);
			}
		}

		public virtual void Backtracking(int p)
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.Backtracking(p);
			}
		}

		public virtual void Adding(int p)
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.Adding(p);
			}
		}

		public virtual void Learn(IConstr c)
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.Learn(c);
			}
		}

		public virtual void LearnUnit(int p)
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.LearnUnit(p);
			}
		}

		public virtual void Delete(IConstr c)
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.Delete(c);
			}
		}

		public virtual void ConflictFound(IConstr confl, int dlevel, int trailLevel)
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.ConflictFound(confl, dlevel, trailLevel);
			}
		}

		public virtual void ConflictFound(int p)
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.ConflictFound(p);
			}
		}

		public virtual void SolutionFound(int[] model, RandomAccessModel lazyModel)
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.SolutionFound(model, lazyModel);
			}
		}

		public virtual void BeginLoop()
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.BeginLoop();
			}
		}

		public virtual void Start()
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.Start();
			}
		}

		public virtual void End(Lbool result)
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.End(result);
			}
		}

		public virtual void Restarting()
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.Restarting();
			}
		}

		public virtual void Backjump(int backjumpLevel)
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.Backjump(backjumpLevel);
			}
		}

		public virtual void Init(T solverService)
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.Init(solverService);
			}
		}

		public virtual void Cleaning()
		{
			foreach (SearchListener<T> sl in this.listeners)
			{
				sl.Cleaning();
			}
		}
	}
}
