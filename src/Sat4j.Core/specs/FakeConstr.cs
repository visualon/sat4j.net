using System;
using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>Fake constraint which is used typically as a NullObject design pattern.</summary>
	/// <author>leberre</author>
	public sealed class FakeConstr : IConstr
	{
		private const string FakeIConstrMsg = "Fake IConstr";

		private static readonly IConstr instance = new Org.Sat4j.Specs.FakeConstr();

		private FakeConstr()
		{
		}

		// to prevent instantiation
		public static IConstr Instance()
		{
			return instance;
		}

		public int Size()
		{
			throw new NotSupportedException(FakeIConstrMsg);
		}

		public bool Learnt()
		{
			throw new NotSupportedException(FakeIConstrMsg);
		}

		public double GetActivity()
		{
			throw new NotSupportedException(FakeIConstrMsg);
		}

		public int Get(int i)
		{
			throw new NotSupportedException(FakeIConstrMsg);
		}

		public bool CanBePropagatedMultipleTimes()
		{
			throw new NotSupportedException(FakeIConstrMsg);
		}

		public string ToString(VarMapper mapper)
		{
			return FakeIConstrMsg;
		}
	}
}
