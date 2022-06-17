using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Agregator for conflict timers (composite design pattern).</summary>
	/// <author>daniel</author>
	[System.Serializable]
	public class ConflictTimerContainer : ConflictTimer
	{
		private const long serialVersionUID = 1L;

		private readonly IVec<ConflictTimer> timers = new Vec<ConflictTimer>();

		internal virtual ConflictTimerContainer Add(ConflictTimer timer)
		{
			this.timers.Push(timer);
			return this;
		}

		internal virtual ConflictTimerContainer Remove(ConflictTimer timer)
		{
			this.timers.Remove(timer);
			return this;
		}

		public virtual void Reset()
		{
			for (int i = 0; i < this.timers.Size(); i++)
			{
				this.timers.Get(i).Reset();
			}
		}

		public virtual void NewConflict()
		{
			for (int i = 0; i < this.timers.Size(); i++)
			{
				this.timers.Get(i).NewConflict();
			}
		}
	}
}
