using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace Sharpen
{
    public static class Extensions
    {

        public static string GetMessage(this Exception self) => self.Message;


        public static bool HasNext<T>(this IEnumerator<T> self){
            return self.MoveNext();
        }

        public static T Next<T>(this IEnumerator<T> self) => self.Current;


        public static object GetProperty(this Hashtable self, object key) => self[key];


        public static void Printf(this TextWriter self, string fmt, params object[] args)
        {
            self.Write("TODO: {0}", fmt);
        }

        public static int GetAndDecrement(this AtomicInteger ai) => ai.DecrementAndGet();
    }
}
