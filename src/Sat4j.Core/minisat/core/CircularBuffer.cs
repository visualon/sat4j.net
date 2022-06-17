using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>
	/// Create a circular buffer of a given capacity allowing to compute efficiently
	/// the mean of the values storied.
	/// </summary>
	/// <author>leberre</author>
	[System.Serializable]
	public class CircularBuffer
	{
		private const long serialVersionUID = 1L;

		private readonly int[] values;

		private int index = 0;

		private long sum = 0;

		private bool full = false;

		public CircularBuffer(int capacity)
		{
			this.values = new int[capacity];
		}

		public virtual void Push(int value)
		{
			if (!this.full)
			{
				this.values[this.index++] = value;
				this.sum += value;
				if (this.index == this.values.Length)
				{
					this.full = true;
					this.index = -1;
				}
				return;
			}
			this.index++;
			if (this.index == this.values.Length)
			{
				this.index = 0;
			}
			// buffer full, overwrite
			this.sum -= this.values[this.index];
			this.values[this.index] = value;
			this.sum += value;
		}

		public virtual long Average()
		{
			if (this.full)
			{
				return this.sum / this.values.Length;
			}
			if (this.index == 0)
			{
				return 0;
			}
			return this.sum / this.index;
		}

		public virtual void Clear()
		{
			this.index = 0;
			this.full = false;
			this.sum = 0;
		}

		public virtual bool IsFull()
		{
			return this.full;
		}
	}
}
