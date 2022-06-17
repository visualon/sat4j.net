using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>Interface for engines able to derive unit clauses for the current problem.</summary>
	/// <author>daniel</author>
	/// <since>2.3.4</since>
	public interface UnitClauseProvider
	{
		private sealed class _UnitClauseProvider_41 : UnitClauseProvider
		{
			public _UnitClauseProvider_41()
			{
			}

			public void ProvideUnitClauses(UnitPropagationListener upl)
			{
			}
		}

		// do nothing
		void ProvideUnitClauses(UnitPropagationListener upl);
	}

	public static class UnitClauseProviderConstants
	{
		public const UnitClauseProvider Void = new _UnitClauseProvider_41();
	}
}
