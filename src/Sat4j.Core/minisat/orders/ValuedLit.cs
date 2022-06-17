using Sharpen;

namespace Org.Sat4j.Minisat.Orders
{
	/// <summary>Utility class used to order the literals according to a specific heuristics.</summary>
	internal sealed class ValuedLit : Comparable<Org.Sat4j.Minisat.Orders.ValuedLit>
	{
		internal readonly int id;

		internal readonly int count;

		internal ValuedLit(int id, int count)
		{
			this.id = id;
			this.count = count;
		}

		public int CompareTo(Org.Sat4j.Minisat.Orders.ValuedLit t)
		{
			if (this.count == 0)
			{
				return int.MaxValue;
			}
			if (t.count == 0)
			{
				return -1;
			}
			return this.count - t.count;
		}

		public override bool Equals(object o)
		{
			if (o == null)
			{
				return false;
			}
			if (o is Org.Sat4j.Minisat.Orders.ValuedLit)
			{
				return ((Org.Sat4j.Minisat.Orders.ValuedLit)o).count == this.count;
			}
			return false;
		}

		public override int GetHashCode()
		{
			return this.id;
		}

		public override string ToString()
		{
			return string.Empty + this.id + "(" + this.count + ")";
		}
		//$NON-NLS-1$//$NON-NLS-2$ //$NON-NLS-3$
	}
}
