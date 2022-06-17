using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>Utility interface to catch objects with logging capability (able to log).</summary>
	/// <remarks>
	/// Utility interface to catch objects with logging capability (able to log).
	/// The interface supersedes the former org.sat4j.minisat.core.ICDCLLogger
	/// introduced in release 2.3.2.
	/// </remarks>
	/// <author>sroussel</author>
	/// <since>2.3.3</since>
	public interface ILogAble
	{
		private sealed class _ILogAble_42 : ILogAble
		{
			public _ILogAble_42()
			{
			}

			public void Log(string message)
			{
				System.Console.Out.WriteLine(message);
			}
		}

		void Log(string message);
	}

	public static class ILogAbleConstants
	{
		public const ILogAble Console = new _ILogAble_42();
	}
}
