using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security;
using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Core
{
	/// <summary>
	/// A solver factory is responsible for providing prebuilt solvers to the end
	/// user.
	/// </summary>
	/// <author>bourgeois</author>
	[System.Serializable]
	public abstract class ASolverFactory<T>
		where T : ISolver
	{
		private const long serialVersionUID = 1L;

		/// <summary>
		/// This methods returns names of solvers to be used with the method
		/// getSolverByName().
		/// </summary>
		/// <returns>
		/// an array containing the names of all the solvers available in the
		/// library.
		/// </returns>
		/// <seealso cref="ASolverFactory{T}.CreateSolverByName(string)"/>
		public virtual string[] SolverNames()
		{
			IList<string> l = new AList<string>();
			MethodInfo[] solvers = Sharpen.Runtime.GetDeclaredMethods(this.GetType());
			foreach (MethodInfo solver in solvers)
			{
				if (Sharpen.Runtime.GetParameterTypes(solver).Length == 0 && solver.Name.StartsWith("new"))
				{
					//$NON-NLS-1$
					l.Add(Sharpen.Runtime.Substring(solver.Name, 3));
				}
			}
			string[] names = new string[l.Count];
			Sharpen.Collections.ToArray(l, names);
			return names;
		}

		/// <summary>create a solver from its String name.</summary>
		/// <remarks>
		/// create a solver from its String name. the solvername Xxxx must map one of
		/// the newXxxx methods.
		/// </remarks>
		/// <param name="solvername">the name of the solver</param>
		/// <returns>
		/// an ISolver built using newSolvername. <code>null</code> if the
		/// solvername doesn't map one of the method of the factory.
		/// </returns>
		public virtual T CreateSolverByName(string solvername)
		{
			try
			{
				Type[] paramtypes = new Type[] {  };
				MethodInfo m = this.GetType().GetMethod("new" + solvername, paramtypes);
				//$NON-NLS-1$
				return (T)m.Invoke(null, (object[])null);
			}
			catch (SecurityException e)
			{
				System.Console.Error.WriteLine(e.GetLocalizedMessage());
			}
			catch (ArgumentException e)
			{
				System.Console.Error.WriteLine(e.GetLocalizedMessage());
			}
			catch (MissingMethodException e)
			{
				System.Console.Error.WriteLine(e.GetLocalizedMessage());
			}
			catch (MemberAccessException e)
			{
				System.Console.Error.WriteLine(e.GetLocalizedMessage());
			}
			catch (TargetInvocationException e)
			{
				System.Console.Error.WriteLine(e.GetLocalizedMessage());
			}
			return null;
		}

		/// <summary>To obtain the default solver of the library.</summary>
		/// <remarks>
		/// To obtain the default solver of the library. The solver is suitable to
		/// solve huge SAT benchmarks. It should reflect state-of-the-art SAT
		/// technologies.
		/// For solving small/easy SAT benchmarks, use lightSolver() instead.
		/// </remarks>
		/// <returns>a solver from the factory</returns>
		/// <seealso cref="ASolverFactory{T}.LightSolver()"/>
		public abstract T DefaultSolver();

		/// <summary>
		/// To obtain a solver that is suitable for solving many small instances of
		/// SAT problems.
		/// </summary>
		/// <remarks>
		/// To obtain a solver that is suitable for solving many small instances of
		/// SAT problems.
		/// The solver is not using sophisticated but costly reasoning and avoids to
		/// allocate too much memory.
		/// For solving bigger SAT benchmarks, use defaultSolver() instead.
		/// </remarks>
		/// <returns>a solver from the factory</returns>
		/// <seealso cref="ASolverFactory{T}.DefaultSolver()"/>
		public abstract T LightSolver();
	}
}
