using System;
using System.Text;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Implementation of a queue.</summary>
	/// <remarks>
	/// Implementation of a queue.
	/// Formerly used in the solver to maintain unit literals for unit propagation.
	/// No longer used currently.
	/// </remarks>
	/// <author>leberre</author>
	[System.Serializable]
	public sealed class IntQueue
	{
		private const long serialVersionUID = 1L;

		private const int InitialQueueCapacity = 10;

		/// <summary>Add an element to the queue.</summary>
		/// <remarks>
		/// Add an element to the queue. The queue is supposed to be large enough for
		/// that!
		/// </remarks>
		/// <param name="x">the element to add</param>
		public void Insert(int x)
		{
			// ensure(size + 1);
			System.Diagnostics.Debug.Assert(this.size < this.myarray.Length);
			this.myarray[this.size++] = x;
		}

		/// <summary>returns the nexdt element in the queue.</summary>
		/// <remarks>
		/// returns the nexdt element in the queue. Unexpected results if the queue
		/// is empty!
		/// </remarks>
		/// <returns>the firsst element on the queue</returns>
		public int Dequeue()
		{
			System.Diagnostics.Debug.Assert(this.first < this.size);
			return this.myarray[this.first++];
		}

		/// <summary>Vide la queue</summary>
		public void Clear()
		{
			this.size = 0;
			this.first = 0;
		}

		/// <summary>Pour connaitre la taille de la queue.</summary>
		/// <returns>le nombre d'elements restant dans la queue</returns>
		public int Size()
		{
			return this.size - this.first;
		}

		/// <summary>Utilisee pour accroitre dynamiquement la taille de la queue.</summary>
		/// <param name="nsize">la taille maximale de la queue</param>
		public void Ensure(int nsize)
		{
			if (nsize >= this.myarray.Length)
			{
				int[] narray = new int[Math.Max(nsize, this.size * 2)];
				System.Array.Copy(this.myarray, 0, narray, 0, this.size);
				this.myarray = narray;
			}
		}

		public override string ToString()
		{
			StringBuilder stb = new StringBuilder();
			stb.Append(">");
			//$NON-NLS-1$
			for (int i = this.first; i < this.size - 1; i++)
			{
				stb.Append(this.myarray[i]);
				stb.Append(" ");
			}
			//$NON-NLS-1$
			if (this.first != this.size)
			{
				stb.Append(this.myarray[this.size - 1]);
			}
			stb.Append("<");
			//$NON-NLS-1$
			return stb.ToString();
		}

		private int[] myarray = new int[InitialQueueCapacity];

		private int size = 0;

		private int first = 0;
	}
}
