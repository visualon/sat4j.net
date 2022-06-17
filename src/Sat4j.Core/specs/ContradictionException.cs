using System;
using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>That exception is launched whenever a trivial contradiction is found (e.g.</summary>
	/// <remarks>
	/// That exception is launched whenever a trivial contradiction is found (e.g.
	/// null clause).
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public class ContradictionException : Exception
	{
		private const long serialVersionUID = 1L;

		public ContradictionException()
			: base()
		{
		}

		/// <param name="message">un message</param>
		public ContradictionException(string message)
			: base(message)
		{
		}

		/// <param name="cause">la cause de l'exception</param>
		public ContradictionException(Exception cause)
			: base(cause)
		{
		}

		/// <param name="message">un message</param>
		/// <param name="cause">une cause</param>
		public ContradictionException(string message, Exception cause)
			: base(message, cause)
		{
		}
	}
}
