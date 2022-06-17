using Org.Sat4j.Specs;
using Sharpen;

namespace Org.Sat4j.Minisat.Core
{
	/// <summary>Strategy for cleaning the database of learned clauses.</summary>
	/// <author>leberre</author>
	public interface LearnedConstraintsDeletionStrategy
	{
		void Init();

		ConflictTimer GetTimer();

		/// <summary>
		/// Hook method called when the solver wants to reduce the set of learned
		/// clauses.
		/// </summary>
		/// <param name="learnedConstrs"/>
		void Reduce(IVec<Constr> learnedConstrs);

		/// <summary>
		/// Hook method called when a new clause has just been derived during
		/// conflict analysis.
		/// </summary>
		/// <param name="outLearnt"/>
		void OnClauseLearning(Constr outLearnt);

		/// <summary>Hook method called on constraints participating to the conflict analysis.</summary>
		/// <param name="reason"/>
		void OnConflictAnalysis(Constr reason);

		/// <summary>Hook method called when a unit clause is propagated thanks to from.</summary>
		/// <param name="from"/>
		void OnPropagation(Constr from);
	}
}
