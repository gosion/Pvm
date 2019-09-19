using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pvm.Core.Utils;

namespace Pvm.Core.Storages
{
    public sealed class InMemoryStorage<T> : IStorage<T> where T : KeyedModel
    {
        IDictionary<string, T> _dict = new ConcurrentDictionary<string, T>();

        public Task<T> GetSync(Guid id)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            T result = this._dict.GetValue(id.ToString());

            return Task.FromResult(result);
        }

        public Task AddAsync(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return Task.Run(() => {
                if (this._dict.ContainsKey(model.Id.ToString()))
                {
                    throw new Exception($"Model (id: {model.Id}) already exists.");
                }

                this._dict[model.Id.ToString()] = model;
            });
        }

        public Task SaveAsync(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return Task.Run(() => {
                if (this._dict.ContainsKey(model.Id.ToString()) == false)
                {
                    throw new Exception($"Model (id: {model.Id} not found.");
                }

                this._dict[model.Id.ToString()] = model;
            });
        }

        public Task RemoveAsync(T model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }

            return Task.Run(() => {
                if (this._dict.ContainsKey(model.Id.ToString()) == false)
                {
                    throw new Exception($"Model (id: {model.Id} not found.");
                }

                this._dict.Remove(model.Id.ToString());
            });
        }
    }
}
