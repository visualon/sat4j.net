using System;
using Sharpen;

namespace Org.Sat4j.Reader
{
	/// <summary>Exception launched when there is a problem during parsing.</summary>
	/// <author>leberre</author>
	/// <seealso cref="Reader"/>
	[System.Serializable]
	public class ParseFormatException : Exception
	{
		public const string ParsingError = "Parsing Error";

		private const long serialVersionUID = 1L;

		/// <summary>Constructor for ParseFormatException.</summary>
		public ParseFormatException()
			: base(ParsingError)
		{
		}

		/// <summary>Constructor for ParseFormatException.</summary>
		/// <param name="message">the error message</param>
		public ParseFormatException(string message)
			: base(ParsingError + message)
		{
		}

		/// <summary>Constructor for ParseFormatException.</summary>
		/// <param name="message">the error message</param>
		/// <param name="cause">the cause of the exception</param>
		public ParseFormatException(string message, Exception cause)
			: base(ParsingError + message, cause)
		{
		}

		/// <summary>Constructor for ParseFormatException.</summary>
		/// <param name="cause">the cause of the exception</param>
		public ParseFormatException(Exception cause)
			: base(cause.Message, cause)
		{
		}
	}
}
