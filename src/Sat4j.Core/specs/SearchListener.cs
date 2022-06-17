using Sharpen;

namespace Org.Sat4j.Specs
{
	/// <summary>Interface to the solver main steps.</summary>
	/// <remarks>
	/// Interface to the solver main steps. Useful for integrating search
	/// visualization or debugging.
	/// (that class moved from org.sat4j.minisat.core in earlier version of SAT4J).
	/// </remarks>
	/// <author>daniel</author>
	/// <since>2.1</since>
	public interface SearchListener<S>
		where S : ISolverService
	{
		/// <summary>Provide access to the solver's controllable interface.</summary>
		/// <param name="solverService">a way to safely control the solver.</param>
		/// <since>2.3.2</since>
		void Init(S solverService);

		/// <summary>decision variable</summary>
		/// <param name="p"/>
		void Assuming(int p);

		/// <summary>Unit propagation</summary>
		/// <param name="p"/>
		void Propagating(int p);

		/// <summary>Fixes the truth value of a variable before propagating it.</summary>
		/// <remarks>
		/// Fixes the truth value of a variable before propagating it. For all calls
		/// to enqueueing(p,_) there should be a call to propagating(p) unless a
		/// conflict is found.
		/// </remarks>
		/// <param name="p"/>
		/// <param name="reason">TODO</param>
		void Enqueueing(int p, IConstr reason);

		/// <summary>backtrack on a decision variable</summary>
		/// <param name="p"/>
		void Backtracking(int p);

		/// <summary>adding forced variable (conflict driven assignment)</summary>
		void Adding(int p);

		/// <summary>learning a new clause</summary>
		/// <param name="c"/>
		void Learn(IConstr c);

		/// <summary>learn a new unit clause (a literal)</summary>
		/// <param name="p">a literal in Dimacs format.</param>
		/// <since>2.3.4</since>
		void LearnUnit(int p);

		/// <summary>delete a clause</summary>
		void Delete(IConstr c);

		/// <summary>a conflict has been found.</summary>
		/// <param name="confl">TODO</param>
		/// <param name="dlevel">TODO</param>
		/// <param name="trailLevel">TODO</param>
		void ConflictFound(IConstr confl, int dlevel, int trailLevel);

		/// <summary>a conflict has been found while propagating values.</summary>
		/// <param name="p">the conflicting value.</param>
		void ConflictFound(int p);

		/// <summary>a solution is found.</summary>
		/// <param name="model">the model found</param>
		/// <param name="lazyModel">TODO</param>
		void SolutionFound(int[] model, RandomAccessModel lazyModel);

		/// <summary>starts a propagation</summary>
		void BeginLoop();

		/// <summary>Start the search.</summary>
		void Start();

		/// <summary>End the search.</summary>
		/// <param name="result">the result of the search.</param>
		void End(Lbool result);

		/// <summary>The solver restarts the search.</summary>
		void Restarting();

		/// <summary>The solver is asked to backjump to a given decision level.</summary>
		/// <param name="backjumpLevel"/>
		void Backjump(int backjumpLevel);

		/// <summary>The solver is going to delete some learned clauses.</summary>
		void Cleaning();
	}
}
