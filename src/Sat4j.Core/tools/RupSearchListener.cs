using System.IO;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;
using Sharpen.Logging;

namespace Org.Sat4j.Tools
{
	/// <summary>Output an unsat proof using the reverse unit propagation (RUP) format.</summary>
	/// <author>daniel</author>
	/// <?/>
	/// <since>2.3.4</since>
	[System.Serializable]
	public class RupSearchListener<S> : SearchListenerAdapter<S>
		where S : ISolverService
	{
		private const long serialVersionUID = 1L;

		private TextWriter @out;

		private readonly FilePath file;

		public RupSearchListener(string filename)
		{
			file = new FilePath(filename);
		}

		public override void Init(S solverService)
		{
			try
			{
				@out = new TextWriter(new FileOutputStream(file));
			}
			catch (FileNotFoundException)
			{
				@out = System.Console.Out;
			}
		}

		public override void End(Lbool result)
		{
			if (result == Lbool.False)
			{
				@out.WriteLine("0");
				@out.Close();
			}
			else
			{
				@out.Close();
				if (!file.Delete())
				{
					Logger.GetLogger("org.sat4j.core").Info("Cannot delete file " + file.GetName());
				}
			}
		}

		public override void Learn(IConstr c)
		{
			PrintConstr(c);
		}

		public override void Delete(IConstr c)
		{
			@out.Write("d ");
			PrintConstr(c);
		}

		private void PrintConstr(IConstr c)
		{
			for (int i = 0; i < c.Size(); i++)
			{
				@out.Write(LiteralsUtils.ToDimacs(c.Get(i)));
				@out.Write(" ");
			}
			@out.WriteLine("0");
		}

		public override void LearnUnit(int p)
		{
			@out.Write(p);
			@out.WriteLine(" 0");
		}
	}
}
