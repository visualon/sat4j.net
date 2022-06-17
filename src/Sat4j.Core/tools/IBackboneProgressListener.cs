using Sharpen;

namespace Org.Sat4j.Tools
{
	/// <summary>Allows to keep track of the advance of the backbone computation.</summary>
	/// <author>lonca</author>
	/// <since>2.3.6</since>
	public interface IBackboneProgressListener
	{
		
		void Start(int litsToTest);

		void InProgress(int processed, int initLitsToTest);

		void End(int nCallsToSolver);
	}

	public static class IBackboneProgressListenerConstants
	{
        private sealed class _IBackboneProgressListener_40 : IBackboneProgressListener
        {
            public _IBackboneProgressListener_40()
            {
            }

            public void Start(int litsToTest)
            {
            }

            public void InProgress(int processed, int initLitsToTest)
            {
            }

            public void End(int nCallsToSolver)
            {
            }
        }

        public static readonly IBackboneProgressListener Void = new _IBackboneProgressListener_40();
	}
}
