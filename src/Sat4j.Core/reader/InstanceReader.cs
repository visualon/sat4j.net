using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Reader
{
	/// <summary>
	/// An reader having the responsibility to choose the right reader according to
	/// the input.
	/// </summary>
	/// <author>leberre</author>
	public class InstanceReader : Org.Sat4j.Reader.Reader
	{
		private AAGReader aag;

		private AIGReader aig;

		private DimacsReader ezdimacs;

		private LecteurDimacs dimacs;

		private Org.Sat4j.Reader.Reader reader = null;

		private readonly ISolver solver;

		public InstanceReader(ISolver solver, Org.Sat4j.Reader.Reader reader)
		{
			this.solver = solver;
			this.reader = reader;
		}

		public InstanceReader(ISolver solver)
		{
			// dimacs = new DimacsReader(solver);
			this.solver = solver;
		}

		private Org.Sat4j.Reader.Reader GetDefaultSATReader()
		{
			if (this.dimacs == null)
			{
				this.dimacs = new LecteurDimacs(this.solver);
			}
			// new
			// LecteurDimacs(solver);
			return this.dimacs;
		}

		private Org.Sat4j.Reader.Reader GetEZSATReader()
		{
			if (this.ezdimacs == null)
			{
				this.ezdimacs = new DimacsReader(this.solver);
			}
			// new
			// LecteurDimacs(solver);
			return this.ezdimacs;
		}

		private Org.Sat4j.Reader.Reader GetAIGReader()
		{
			if (this.aig == null)
			{
				this.aig = new AIGReader(this.solver);
			}
			return this.aig;
		}

		private Org.Sat4j.Reader.Reader GetAAGReader()
		{
			if (this.aag == null)
			{
				this.aag = new AAGReader(this.solver);
			}
			return this.aag;
		}

		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		public override IProblem ParseInstance(string filename)
		{
			string fname;
			string prefix = string.Empty;
			if (filename.StartsWith("http://"))
			{
				filename = Sharpen.Runtime.Substring(filename, filename.LastIndexOf('/'), filename.Length - 1);
			}
			if (filename.IndexOf(':') != -1)
			{
				string[] parts = filename.Split(":", 2);
				filename = parts[1];
				prefix = parts[0].ToUpper(CultureInfo.CurrentCulture);
			}
			if (filename.EndsWith(".gz") || filename.EndsWith(".bz2"))
			{
				fname = Sharpen.Runtime.Substring(filename, 0, filename.LastIndexOf('.'));
			}
			else
			{
				fname = filename;
			}
			if (this.reader == null)
			{
				this.reader = HandleFileName(fname, prefix);
			}
			return this.reader.ParseInstance(filename);
		}

		protected internal virtual Org.Sat4j.Reader.Reader HandleFileName(string fname, string prefix)
		{
			if ("EZCNF".Equals(prefix))
			{
				return GetEZSATReader();
			}
			if (fname.EndsWith(".aag"))
			{
				return GetAAGReader();
			}
			if (fname.EndsWith(".aig"))
			{
				return GetAIGReader();
			}
			return GetDefaultSATReader();
		}

		[Obsolete]
		public override string Decode(int[] model)
		{
			return this.reader.Decode(model);
		}

		public override void Decode(int[] model, PrintWriter @out)
		{
			this.reader.Decode(model, @out);
		}

		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <exception cref="System.IO.IOException"/>
		public override IProblem ParseInstance(InputStream @in)
		{
			throw new NotSupportedException("Use a domain specific Reader (LecteurDimacs, AIGReader, etc.) for stream input ");
		}

		public override bool HasAMapping()
		{
			return this.reader.HasAMapping();
		}

		public override IDictionary<int, string> GetMapping()
		{
			return this.reader.GetMapping();
		}
	}
}
