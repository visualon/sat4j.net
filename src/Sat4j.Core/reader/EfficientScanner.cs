using System.IO;
using System.Text;
using Mono.Math;
using Sharpen;

namespace Org.Sat4j.Reader
{
	/// <summary>
	/// Efficient scanner based on the LecteurDimacs class written by Frederic
	/// Laihem.
	/// </summary>
	/// <remarks>
	/// Efficient scanner based on the LecteurDimacs class written by Frederic
	/// Laihem. It is much faster than Java Scanner class because it does not split
	/// the input file into strings but reads and interpret the input char by char.
	/// Furthermore, it is based on stream (char in ASCII) and not reader (word in
	/// UTF).
	/// </remarks>
	/// <author>laihem</author>
	/// <author>leberre</author>
	/// <since>2.1</since>
	[System.Serializable]
	public class EfficientScanner
	{
		private const long serialVersionUID = 1L;

		private const int TailleBuf = 16384;

		[System.NonSerialized]
		private readonly BufferedInputStream @in;

		private const char Eof = (char)-1;

		private readonly char commentChar;

		public EfficientScanner(InputStream input, char commentChar)
		{
			/* taille du buffer */
			/*
			* nomFichier repr?sente le nom du fichier ? lire
			*/
			this.@in = new BufferedInputStream(input, Org.Sat4j.Reader.EfficientScanner.TailleBuf);
			this.commentChar = commentChar;
		}

		public EfficientScanner(InputStream input)
			: this(input, 'c')
		{
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual void Close()
		{
			this.@in.Close();
		}

		/// <summary>Skip commented lines.</summary>
		/// <exception cref="System.IO.IOException"/>
		public virtual void SkipComments()
		{
			char currentChar;
			for (; ; )
			{
				currentChar = CurrentChar();
				if (currentChar != this.commentChar)
				{
					break;
				}
				SkipRestOfLine();
				if (currentChar == Eof)
				{
					break;
				}
			}
		}

		/// <summary>To get the next available integer.</summary>
		/// <returns/>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="ParseFormatException"/>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		public virtual int NextInt()
		{
			int val = 0;
			bool neg = false;
			char currentChar = SkipSpaces();
			if (currentChar == '-')
			{
				neg = true;
				currentChar = (char)this.@in.Read();
			}
			else
			{
				if (currentChar == '+')
				{
					currentChar = (char)this.@in.Read();
				}
				else
				{
					if (currentChar >= '0' && currentChar <= '9')
					{
						val = currentChar - '0';
						currentChar = (char)this.@in.Read();
					}
					else
					{
						throw new ParseFormatException("Unknown character " + currentChar);
					}
				}
			}
			/* on lit la suite du literal */
			while (currentChar >= '0' && currentChar <= '9')
			{
				val = val * 10 + currentChar - '0';
				currentChar = (char)this.@in.Read();
			}
			if (currentChar == '\r')
			{
				this.@in.Read();
			}
			// skip \r\n on windows.
			return neg ? -val : val;
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		public virtual BigInteger NextBigInteger()
		{
			StringBuilder stb = new StringBuilder();
			char currentChar = SkipSpaces();
			if (currentChar == '-')
			{
				stb.Append(currentChar);
				currentChar = (char)this.@in.Read();
			}
			else
			{
				if (currentChar == '+')
				{
					currentChar = (char)this.@in.Read();
				}
				else
				{
					if (currentChar >= '0' && currentChar <= '9')
					{
						stb.Append(currentChar);
						currentChar = (char)this.@in.Read();
					}
					else
					{
						throw new ParseFormatException("Unknown character " + currentChar);
					}
				}
			}
			while (currentChar >= '0' && currentChar <= '9')
			{
				stb.Append(currentChar);
				currentChar = (char)this.@in.Read();
			}
			return new BigInteger(stb.ToString());
		}

		/// <exception cref="ParseFormatException">never used in that method.</exception>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		public virtual string Next()
		{
			StringBuilder stb = new StringBuilder();
			char currentChar = SkipSpaces();
			while (currentChar != ' ' && currentChar != '\n')
			{
				stb.Append(currentChar);
				currentChar = (char)this.@in.Read();
			}
			return stb.ToString();
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual char SkipSpaces()
		{
			char car;
			do
			{
				car = (char)this.@in.Read();
			}
			while (car == ' ' || car == '\n');
			return car;
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual string NextLine()
		{
			StringBuilder stb = new StringBuilder();
			char car;
			do
			{
				car = (char)this.@in.Read();
				stb.Append(car);
			}
			while (car != '\n' && car != Eof);
			return stb.ToString();
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual void SkipRestOfLine()
		{
			char car;
			do
			{
				car = (char)this.@in.Read();
			}
			while (car != '\n' && car != Eof);
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual bool Eof()
		{
			return CurrentChar() == Eof;
		}

		/// <exception cref="System.IO.IOException"/>
		public virtual char CurrentChar()
		{
			this.@in.Mark(10);
			char car = (char)this.@in.Read();
			this.@in.Reset();
			return car;
		}
	}
}
