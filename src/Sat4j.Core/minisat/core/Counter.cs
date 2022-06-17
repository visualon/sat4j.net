using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <since>2.1</since>
	public class Counter
	{
		private int value;

		public Counter()
			: this(1)
		{
		}

		public Counter(int initialValue)
		{
			this.value = initialValue;
		}

		public virtual void Inc()
		{
			this.value++;
		}

		/// <since>2.1</since>
		public virtual void Dec()
		{
			this.value--;
		}

		public override string ToString()
		{
			return this.value.ToString();
		}

		/// <returns>the value of the counter.</returns>
		/// <since>2.3.1</since>
		public virtual int GetValue()
		{
			return this.value;
		}
	}
}
