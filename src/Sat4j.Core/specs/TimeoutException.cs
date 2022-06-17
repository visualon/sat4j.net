using System;
using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>
	/// Exception launched when the solver cannot solve a problem within its allowed
	/// time.
	/// </summary>
	/// <remarks>
	/// Exception launched when the solver cannot solve a problem within its allowed
	/// time. Note that the name of that exception is subject to change since a
	/// TimeoutException must also be launched by incomplete solvers to reply
	/// "Unknown".
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public class TimeoutException : Exception
	{
		private const long serialVersionUID = 1L;

		/// <summary>Constructor for TimeoutException.</summary>
		public TimeoutException()
			: base()
		{
		}

		/// <summary>Constructor for TimeoutException.</summary>
		/// <param name="message">the error message</param>
		public TimeoutException(string message)
			: base(message)
		{
		}

		/// <summary>Constructor for TimeoutException.</summary>
		/// <param name="message">the error message</param>
		/// <param name="cause">the cause of the exception</param>
		public TimeoutException(string message, Exception cause)
			: base(message, cause)
		{
		}

		/// <summary>Constructor for TimeoutException.</summary>
		/// <param name="cause">the cause of the exception</param>
		public TimeoutException(Exception cause)
			: base(cause)
		{
		}
	}
}
