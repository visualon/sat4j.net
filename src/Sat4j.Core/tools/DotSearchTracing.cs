using System;
using System.Collections.Generic;
using System.IO;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Sharpen;
using Sharpen.Logging;

namespace Org.Sat4j.Tools
{
	/// <summary>Class allowing to express the search as a tree in the dot language.</summary>
	/// <remarks>
	/// Class allowing to express the search as a tree in the dot language. The
	/// resulting file can be viewed in a tool like
	/// <a href="http://www.graphviz.org/">Graphviz</a>
	/// To use only on small benchmarks.
	/// Note that also does not make sense to use such a listener on a distributed or
	/// remote solver.
	/// </remarks>
	/// <author>daniel</author>
	/// <since>2.2</since>
	[System.Serializable]
	public class DotSearchTracing<T> : SearchListenerAdapter<ISolverService>, VarMapper
	{
		private const long serialVersionUID = 1L;

		private readonly Vec<string> stack;

		private string currentNodeName = null;

		[System.NonSerialized]
		private TextWriter @out;

		private bool assertive = false;

		private IDictionary<int, T> mapping;

		/// <since>2.1</since>
		public DotSearchTracing(string fileNameToSave)
		{
			this.stack = new Vec<string>();
			try
			{
				this.@out = new FileWriter(fileNameToSave);
			}
			catch (IOException)
			{
				System.Console.Error.WriteLine("Problem when created file.");
			}
		}

		public virtual void SetMapping(IDictionary<int, T> mapping)
		{
			this.mapping = mapping;
		}

		public virtual string Map(int dimacs)
		{
			if (this.mapping != null)
			{
				int var = Math.Abs(dimacs);
				T t = this.mapping[var];
				if (t != null)
				{
					if (dimacs > 0)
					{
						return t.ToString();
					}
					return "-" + t.ToString();
				}
			}
			return Sharpen.Extensions.ToString(dimacs);
		}

		public sealed override void Assuming(int p)
		{
			int absP = Math.Abs(p);
			string newName;
			if (this.currentNodeName == null)
			{
				newName = Sharpen.Extensions.ToString(absP);
				this.stack.Push(newName);
				SaveLine(LineTab("\"" + newName + "\"" + "[label=\"" + Map(p) + "\", shape=circle, color=blue, style=filled]"));
			}
			else
			{
				newName = this.currentNodeName;
				this.stack.Push(newName);
				SaveLine(LineTab("\"" + newName + "\"" + "[label=\"" + Map(p) + "\", shape=circle, color=blue, style=filled]"));
			}
			this.currentNodeName = newName;
		}

		/// <since>2.1</since>
		public sealed override void Propagating(int p)
		{
			string newName = this.currentNodeName + "." + p + "." + this.assertive;
			if (this.currentNodeName == null)
			{
				SaveLine(LineTab("\"null\" [label=\"\", shape=point]"));
			}
			string couleur = this.assertive ? "orange" : "green";
			SaveLine(LineTab("\"" + newName + "\"" + "[label=\"" + Map(p) + "\",shape=point, color=black]"));
			SaveLine(LineTab("\"" + this.currentNodeName + "\"" + " -- " + "\"" + newName + "\"" + "[label=" + "\" " + Map(p) + "\", fontcolor =" + couleur + ", color = " + couleur + ", style = bold]"));
			this.currentNodeName = newName;
			this.assertive = false;
		}

		public sealed override void Enqueueing(int p, IConstr reason)
		{
			if (reason != null)
			{
				string newName = this.currentNodeName + "." + p + ".propagated." + this.assertive;
				SaveLine(LineTab("\"" + newName + "\"" + "[label=\"" + Map(p) + "\",shape=point, color=black]"));
				SaveLine(LineTab("\"" + this.currentNodeName + "\"" + " -- " + "\"" + newName + "\"" + "[label=" + "\" " + Map(p) + "\", fontcolor = gray, color = gray, style = bold]"));
				string reasonName = newName + ".reason";
				SaveLine(LineTab("\"" + reasonName + "\" [label=\"" + reason.ToString(this) + "\", shape=box, color=\"gray\", style=dotted]"));
				SaveLine("\"" + reasonName + "\"" + "--" + "\"" + this.currentNodeName + "\"" + "[label=\"\", color=gray, style=dotted]");
				this.currentNodeName = newName;
			}
		}

		public sealed override void Backtracking(int p)
		{
			string temp = this.stack.Last();
			this.stack.Pop();
			SaveLine("\"" + temp + "\"" + "--" + "\"" + this.currentNodeName + "\"" + "[label=\"\", color=red, style=dotted]");
			this.currentNodeName = temp;
		}

		public sealed override void Adding(int p)
		{
			this.assertive = true;
		}

		/// <since>2.1</since>
		public sealed override void Learn(IConstr constr)
		{
			string learned = this.currentNodeName + "_learned";
			SaveLine(LineTab("\"" + learned + "\" [label=\"" + constr.ToString(this) + "\", shape=box, color=\"orange\", style=dotted]"));
			SaveLine("\"" + learned + "\"" + "--" + "\"" + this.currentNodeName + "\"" + "[label=\"\", color=orange, style=dotted]");
		}

		public sealed override void Delete(IConstr c)
		{
		}

		/// <since>2.1</since>
		public sealed override void ConflictFound(IConstr confl, int dlevel, int trailLevel)
		{
			SaveLine(LineTab("\"" + this.currentNodeName + "\" [label=\"" + confl.ToString(this) + "\", shape=box, color=\"red\", style=filled]"));
		}

		/// <since>2.1</since>
		public sealed override void ConflictFound(int p)
		{
			SaveLine(LineTab("\"" + this.currentNodeName + "\" [label=\"\", shape=box, color=\"red\", style=filled]"));
		}

		public sealed override void SolutionFound(int[] model, RandomAccessModel lazyModel)
		{
			SaveLine(LineTab("\"" + this.currentNodeName + "\" [label=\"\", shape=box, color=\"green\", style=filled]"));
		}

		public sealed override void BeginLoop()
		{
		}

		public sealed override void Start()
		{
			SaveLine("graph G {");
		}

		/// <since>2.1</since>
		public sealed override void End(Lbool result)
		{
			SaveLine("}");
		}

		private string LineTab(string line)
		{
			return "\t" + line;
		}

		private void SaveLine(string line)
		{
			try
			{
				this.@out.Write(line + '\n');
				if ("}".Equals(line))
				{
					this.@out.Close();
				}
			}
			catch (IOException e)
			{
				Logger.GetLogger("org.sat4j.core").Log(Level.Info, "Something went wrong when saving dot file", e);
			}
		}

		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="System.TypeLoadException"/>
		private void ReadObject(ObjectInputStream stream)
		{
			// if the solver is serialized, out is linked to stdout
			stream.DefaultReadObject();
			this.@out = new PrintWriter(System.Console.Out);
		}
	}
}
