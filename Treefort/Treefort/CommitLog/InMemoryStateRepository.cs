using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Treefort.Domain;

namespace Treefort.CommitLog
{
    public class InMemoryStateRepository<T> : IStateRepository<T>
        where T : IState, new()
    {
        private readonly IDictionary<Guid, T> _states;

        public InMemoryStateRepository()
        {
            _states = new ConcurrentDictionary<Guid, T>();
        }

        public Task<T> GetOrCreateAsync(Guid id)
        {
            return Task.Run(() =>
            {
                var key = id;

                if (!_states.ContainsKey(key))
                {
                    _states.Add(key, new T() { AggregateId = id });
                }

                return _states[key];
            });
        }

        public Task InsertOrUpdateAsync(T state)
        {
            return Task.Run(() =>
            {
                var key = state.AggregateId;

                if (!_states.ContainsKey(key))
                    _states.Add(key, state);
                else
                    _states[key] = state;
            });
        }

        public Task<IEnumerable<T>> All()
        {
            return Task.FromResult<IEnumerable<T>>(_states.Values);
        }
    }
}