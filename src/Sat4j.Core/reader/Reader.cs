using System;
using System.Collections.Generic;
using System.IO;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Reader
{
	/// <summary>
	/// A reader is responsible to feed an ISolver from a text file and to convert
	/// the model found by the solver to a textual representation.
	/// </summary>
	/// <author>leberre</author>
	public abstract class Reader
	{
		/// <summary>This is the usual method to feed a solver with a benchmark.</summary>
		/// <param name="filename">
		/// the fully qualified name of the benchmark. The filename
		/// extension may by used to detect which type of benchmarks it is
		/// (SAT, OPB, MAXSAT, etc).
		/// </param>
		/// <returns>the problem to solve (an ISolver in fact).</returns>
		/// <exception cref="ParseFormatException">if an error occurs during parsing.</exception>
		/// <exception cref="System.IO.IOException">if an I/O error occurs.</exception>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException">if the problem is found trivially inconsistent.</exception>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		public virtual IProblem ParseInstance(string filename)
		{
			InputStream @in = null;
			try
			{
				@in = GetInputStreamFromFilename(filename);
				IProblem problem;
				problem = ParseInstance(@in);
				return problem;
			}
			catch (InvalidOperationException e)
			{
				if (e.InnerException is ContradictionException)
				{
					throw ((ContradictionException)e.InnerException);
				}
				else
				{
					throw;
				}
			}
			finally
			{
				if (@in != null)
				{
					@in.Close();
				}
			}
		}

		/// <summary>
		/// This method tries to get an input type from a file, whether it is
		/// compressed or not.
		/// </summary>
		/// <param name="filename">the file to read in</param>
		/// <returns>a corresponding input stream to read in the file</returns>
		/// <exception cref="System.IO.IOException">if an I/O exception occurs</exception>
		/// <exception cref="System.UriFormatException">if a provided URL is incorrect</exception>
		public static InputStream GetInputStreamFromFilename(string filename)
		{
			InputStream @in;
			if (filename.StartsWith("http://"))
			{
				@in = new Uri(filename).OpenStream();
			}
			else
			{
				@in = new FileInputStream(filename);
			}
			if (filename.EndsWith(".gz"))
			{
				@in = new GZIPInputStream(@in);
			}
			else
			{
				if (filename.EndsWith(".bz2"))
				{
					@in.Close();
					@in = Runtime.GetRuntime().Exec("bunzip2 -c " + filename).GetInputStream();
				}
				else
				{
					if (filename.EndsWith(".lzma"))
					{
						@in.Close();
						@in = Runtime.GetRuntime().Exec("lzma -d -c " + filename).GetInputStream();
					}
				}
			}
			return @in;
		}

		/// <summary>Read a file from a stream.</summary>
		/// <remarks>
		/// Read a file from a stream.
		/// It is important to note that benchmarks are usually encoded in ASCII, not
		/// UTF8. As such, the only reasonable way to feed a solver from a stream is
		/// to use a stream.
		/// </remarks>
		/// <param name="in">a stream containing the benchmark.</param>
		/// <returns>the problem to solve (an ISolver in fact).</returns>
		/// <exception cref="ParseFormatException">if an error occurs during parsing.</exception>
		/// <exception cref="System.IO.IOException">if an I/O error occurs.</exception>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException">if the problem is found trivially inconsistent.</exception>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		public abstract IProblem ParseInstance(InputStream @in);

		/// <summary>Read a file from a reader.</summary>
		/// <remarks>
		/// Read a file from a reader.
		/// Do not use that method, it is no longer supported.
		/// </remarks>
		/// <param name="in">a stream containing the benchmark.</param>
		/// <returns>the problem to solve (an ISolver in fact).</returns>
		/// <exception cref="ParseFormatException">if an error occurs during parsing.</exception>
		/// <exception cref="System.IO.IOException">if an I/O error occurs.</exception>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException">if the problem is found trivially inconsistent.</exception>
		/// <seealso cref="ParseInstance(System.IO.InputStream)"/>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		[Obsolete]
		public virtual IProblem ParseInstance(System.IO.StreamReader @in)
		{
			throw new NotSupportedException("Use #parseInstance(InputStream) instead");
		}

		/// <summary>Produce a model using the reader format.</summary>
		/// <remarks>
		/// Produce a model using the reader format.
		/// Note that the approach of building a string representation of the model
		/// may be quite inefficient when the model is rather large. For that reason,
		/// the preferred way to proceed is to directly output the textual
		/// representation in a specific PrintWriter using
		/// <see cref="Decode(int[], System.IO.PrintWriter)"/>
		/// </remarks>
		/// <param name="model">a model using the Dimacs format.</param>
		/// <returns>a human readable view of the model.</returns>
		/// <seealso cref="Decode(int[], System.IO.PrintWriter)"/>
		[Obsolete]
		public abstract string Decode(int[] model);

		/// <summary>Produce a model using the reader format on a provided printwriter.</summary>
		/// <param name="model">a model using the Dimacs format.</param>
		/// <param name="out">the place where to display the model</param>
		public abstract void Decode(int[] model, PrintWriter @out);

		public virtual bool IsVerbose()
		{
			return this.verbose;
		}

		public virtual void SetVerbosity(bool b)
		{
			this.verbose = b;
		}

		private bool verbose = false;

		private bool useMapping = false;

		/// <summary>Does the reader found a mapping.</summary>
		/// <returns>true iff the solver found a mapping in the problem</returns>
		/// <since>2.3.6</since>
		public virtual bool HasAMapping()
		{
			return false;
		}

		/// <summary>Returns the mapping found in the problem.</summary>
		/// <returns>the mapping varid-&gt;String found in the problem.</returns>
		public virtual IDictionary<int, string> GetMapping()
		{
			return null;
		}

		/// <summary>
		/// Check if the reader is going to use the mapping available in the file or
		/// not.
		/// </summary>
		/// <returns>true iff the solver will output a model in terms of mapping</returns>
		/// <since>2.3.6</since>
		public virtual bool IsUsingMapping()
		{
			return this.useMapping;
		}

		/// <summary>Change the behavior of the solver regarding the mapping</summary>
		/// <param name="b">
		/// set to true to display the solution in terms of mapped id and
		/// false to display dimacs variables.
		/// </param>
		/// <since>2.3.6</since>
		public virtual void SetUseMapping(bool b)
		{
			this.useMapping = b;
		}
	}
}
