using System.Collections.Generic;
using System.IO;
using System.Text;
using Org.Sat4j.Core;
using Org.Sat4j.Specs;
using Org.Sat4j.Tools;
using Sharpen;

namespace Org.Sat4j.Reader
{
	/// <summary>Dimacs Reader written by Frederic Laihem.</summary>
	/// <remarks>
	/// Dimacs Reader written by Frederic Laihem. It is much faster than DimacsReader
	/// because it does not split the input file into strings but reads and interpret
	/// the input char by char. Hence, it is a bit more difficult to read and to
	/// modify than DimacsReader.
	/// This reader is used during the SAT Competitions.
	/// </remarks>
	/// <author>laihem</author>
	/// <author>leberre</author>
	[System.Serializable]
	public class LecteurDimacs : Org.Sat4j.Reader.Reader
	{
		private const long serialVersionUID = 1L;

		private const int TailleBuf = 16384;

		private ISolver s;

		[System.NonSerialized]
		private BufferedInputStream @in;

		private int nbVars = -1;

		private int nbClauses = -1;

		private const char Eof = (char)-1;

		private readonly bool pminimal = false;

		public LecteurDimacs(ISolver s)
		{
			/* taille du buffer */
			/* nombre de literaux dans le fichier */
			/*
			* nomFichier repr?sente le nom du fichier ? lire
			*/
			this.s = s;
		}

		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <exception cref="System.IO.IOException"/>
		public sealed override IProblem ParseInstance(InputStream input)
		{
			mapping = null;
			this.@in = new BufferedInputStream(input, Org.Sat4j.Reader.LecteurDimacs.TailleBuf);
			this.s.Reset();
			PasserCommentaire();
			if (this.nbVars < 0)
			{
				throw new ParseFormatException("DIMACS error: wrong max number of variables");
			}
			this.s.NewVar(this.nbVars);
			this.s.SetExpectedNumberOfClauses(this.nbClauses);
			char car = PasserEspaces();
			if (this.nbClauses > 0)
			{
				if (car == Eof)
				{
					throw new ParseFormatException("DIMACS error: the clauses are missing");
				}
				AjouterClauses(car);
			}
			input.Close();
			return this.s;
		}

		/// <summary>on passe les commentaires et on lit le nombre de literaux</summary>
		/// <exception cref="ParseFormatException"/>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		private char PasserCommentaire()
		{
			char car;
			for (; ; )
			{
				car = PasserEspaces();
				if (car == 'p')
				{
					CheckFormat();
					car = LectureNombreLiteraux();
				}
				if (car != 'c' && car != 'p')
				{
					break;
				}
				/* fin des commentaires */
				car = ManageCommentLine();
				/* on passe la ligne de commentaire */
				if (car == Eof)
				{
					break;
				}
			}
			return car;
		}

		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		private void CheckFormat()
		{
			// check if the format is really cnf
			try
			{
				char car = PasserEspaces();
				if ('c' != car || 'n' != @in.Read() || 'f' != @in.Read())
				{
					throw new ParseFormatException("Expecting file in cnf format.");
				}
			}
			catch (IOException e)
			{
				throw new ParseFormatException(e);
			}
		}

		/// <summary>lit le nombre repr?sentant le nombre de literaux</summary>
		/// <exception cref="System.IO.IOException"/>
		private char LectureNombreLiteraux()
		{
			char car = NextChiffre();
			/* on lit le prchain chiffre */
			if (car != Eof)
			{
				this.nbVars = car - '0';
				for (; ; )
				{
					/*
					* on lit le chiffre repr?sentant le nombre de literaux
					*/
					car = (char)this.@in.Read();
					if (car < '0' || car > '9')
					{
						break;
					}
					this.nbVars = 10 * this.nbVars + car - '0';
				}
				car = NextChiffre();
				this.nbClauses = car - '0';
				for (; ; )
				{
					/*
					* on lit le chiffre repr?sentant le nombre de literaux
					*/
					car = (char)this.@in.Read();
					if (car < '0' || car > '9')
					{
						break;
					}
					this.nbClauses = 10 * this.nbClauses + car - '0';
				}
				if (car != '\n' && car != Eof)
				{
					NextLine();
				}
			}
			/* on lit la fin de la ligne */
			return car;
		}

		/// <summary>lit les clauses et les ajoute dans le vecteur donn? en param?tre</summary>
		/// <exception cref="ParseFormatException"/>
		/// <exception cref="System.IO.IOException"/>
		/// <exception cref="Org.Sat4j.Specs.ContradictionException"/>
		/// <exception cref="Org.Sat4j.Reader.ParseFormatException"/>
		private void AjouterClauses(char car)
		{
			IVecInt lit = new VecInt();
			int val = 0;
			bool neg = false;
			for (; ; )
			{
				/* on lit le signe du literal */
				if (car == 'c')
				{
					ManageCommentLine();
					car = (char)this.@in.Read();
					continue;
				}
				else
				{
					if (car == '-')
					{
						neg = true;
						car = (char)this.@in.Read();
					}
					else
					{
						if (car == '+')
						{
							car = (char)this.@in.Read();
						}
						else
						{
							/* on le 1er chiffre du literal */
							if (car >= '0' && car <= '9')
							{
								val = car - '0';
								car = (char)this.@in.Read();
							}
							else
							{
								throw new ParseFormatException("Unknown character " + car);
							}
						}
					}
				}
				/* on lit la suite du literal */
				while (car >= '0' && car <= '9')
				{
					val = val * 10 + car - '0';
					car = (char)this.@in.Read();
				}
				if (val == 0)
				{
					// on a lu toute la clause
					this.s.AddClause(lit);
					lit.Clear();
				}
				else
				{
					/* on ajoute le literal au vecteur */
					// s.newVar(val-1);
					lit.Push(neg ? -val : val);
					neg = false;
					val = 0;
				}
				/* on reinitialise les variables */
				if (car != Eof)
				{
					car = PasserEspaces();
				}
				if (car == Eof)
				{
					if (!lit.IsEmpty())
					{
						this.s.AddClause(lit);
					}
					break;
				}
			}
		}

		/* on a lu tout le fichier */
		/// <summary>passe tout les caract?res d'espacement (espace ou \n)</summary>
		/// <exception cref="System.IO.IOException"/>
		private char PasserEspaces()
		{
			char car;
			do
			{
				car = (char)this.@in.Read();
			}
			while (car == ' ' || car == '\n');
			return car;
		}

		/// <summary>passe tout les caract?res jusqu? rencontrer une fin de la ligne</summary>
		/// <exception cref="System.IO.IOException"/>
		private char NextLine()
		{
			char car;
			do
			{
				car = (char)this.@in.Read();
			}
			while (car != '\n' && car != Eof);
			return car;
		}

		private IDictionary<int, string> mapping;

		/// <exception cref="System.IO.IOException"/>
		private char ManageCommentLine()
		{
			char car;
			StringBuilder stb = new StringBuilder();
			do
			{
				car = (char)this.@in.Read();
				stb.Append(car);
			}
			while (car != '\n' && car != Eof);
			string str = Sharpen.Extensions.Trim(stb.ToString());
			if (str.StartsWith("pmin"))
			{
				string[] tokens = str.Split(" ");
				IVecInt p = new VecInt(tokens.Length - 2);
				for (int i = 1; i < tokens.Length - 1; i++)
				{
					p.Push(System.Convert.ToInt32(tokens[i]));
				}
				s = new Minimal4InclusionModel(s, p);
				System.Console.Out.WriteLine("c computing p-minimal model for p=" + p);
			}
			else
			{
				if (IsUsingMapping())
				{
					string[] values = str.Split("=");
					if (values.Length == 2)
					{
						if (mapping == null)
						{
							mapping = new Dictionary<int, string>();
						}
						mapping[Sharpen.Extensions.ValueOf(Sharpen.Extensions.Trim(values[0]))] = Sharpen.Extensions.Trim(values[1]);
					}
				}
			}
			return car;
		}

		/// <summary>passe tout les caract?re jusqu'? rencontrer un chiffre</summary>
		/// <exception cref="System.IO.IOException"/>
		private char NextChiffre()
		{
			char car;
			do
			{
				car = (char)this.@in.Read();
			}
			while (car < '0' || car > '9' && car != Eof);
			return car;
		}

		public override string Decode(int[] model)
		{
			StringBuilder stb = new StringBuilder();
			foreach (int element in model)
			{
				stb.Append(element);
				stb.Append(" ");
			}
			stb.Append("0");
			return stb.ToString();
		}

		public override void Decode(int[] model, PrintWriter @out)
		{
			if (IsUsingMapping() && HasAMapping())
			{
				DecodeWithMapping(model, @out);
			}
			else
			{
				foreach (int element in model)
				{
					@out.Write(element);
					@out.Write(" ");
				}
				@out.Write("0");
			}
		}

		private void DecodeWithMapping(int[] model, PrintWriter @out)
		{
			string mapped;
			foreach (int element in model)
			{
				if (element > 0)
				{
					mapped = mapping[element];
					if (mapped == null)
					{
						@out.Write(element);
					}
					else
					{
						@out.Write(mapped);
					}
					@out.Write(" ");
				}
			}
			@out.Write("0");
		}

		public override bool HasAMapping()
		{
			return mapping != null;
		}

		public override IDictionary<int, string> GetMapping()
		{
			return mapping;
		}
	}
}
