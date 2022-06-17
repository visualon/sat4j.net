using System;
using Org.Sat4j.Specs;

namespace Org.Sat4j.Tools.Encoding
{
    public class EncodingStrategyAdapter
    {
        internal IConstr AddAtLeastOne<T>(T t, IVecInt literals) where T : ISolver
        {
            throw new NotImplementedException();
        }

        internal IConstr AddAtLeast<T>(T t, IVecInt literals, int k) where T : ISolver
        {
            throw new NotImplementedException();
        }

        internal IConstr AddAtMostOne<T>(T t, IVecInt literals) where T : ISolver
        {
            throw new NotImplementedException();
        }

        internal IConstr AddAtMost<T>(T t, IVecInt literals, int k) where T : ISolver
        {
            throw new NotImplementedException();
        }

        internal IConstr AddExactlyOne<T>(T t, IVecInt literals) where T : ISolver
        {
            throw new NotImplementedException();
        }

        internal IConstr AddExactly<T>(T t, IVecInt literals, int k) where T : ISolver
        {
            throw new NotImplementedException();
        }
    }
}
