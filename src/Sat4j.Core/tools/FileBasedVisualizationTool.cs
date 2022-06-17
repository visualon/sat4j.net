using System.IO;
using Sharpen;

namespace Org.Sat4j.Tools
{
	[System.Serializable]
	public class FileBasedVisualizationTool : IVisualizationTool
	{
		private string filename;

		private TextWriter @out;

		public FileBasedVisualizationTool(string filename)
		{
			this.filename = filename;
			UpdateWriter();
		}

		public virtual void UpdateWriter()
		{
			try
			{
				this.@out = new TextWriter(new FileOutputStream(this.filename + ".dat"));
			}
			catch (FileNotFoundException)
			{
				this.@out = System.Console.Out;
			}
		}

		public virtual string GetFilename()
		{
			return this.filename;
		}

		public virtual void SetFilename(string filename)
		{
			this.filename = filename;
		}

		public virtual void AddPoint(double x, double y)
		{
			this.@out.WriteLine(x + "\t" + y);
		}

		public virtual void AddInvisiblePoint(double x, double y)
		{
			this.@out.WriteLine("#" + x + "\t" + "1/0");
		}

		public virtual void Init()
		{
			UpdateWriter();
		}

		public virtual void End()
		{
			this.@out.Close();
		}
	}
}
