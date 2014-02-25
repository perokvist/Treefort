using System;
using System.Collections.Generic;

namespace Treefort.Application
{
    public class Dispatcher<TMessage, TResult>
    {
        private readonly Dictionary<Type, Func<TMessage, TResult>> _dictionary = new Dictionary<Type, Func<TMessage, TResult>>();



        public void Register<T>(Func<T, TResult> func) where T : TMessage
        {
            _dictionary.Add(typeof(T), x => func((T)x));
        }

        public TResult Dispatch(TMessage m)
        {
            Func<TMessage, TResult> handler;
            if (!_dictionary.TryGetValue(m.GetType(), out handler))
            {
                throw new Exception("cannot dispatch " + m.GetType());
            }
            return handler(m);
        }
    }
}