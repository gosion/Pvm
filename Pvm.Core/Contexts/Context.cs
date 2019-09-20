using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pvm.Core.Utils;

namespace Pvm.Core.Contexts
{
    public class Context
    {
        public IDictionary<string, object> Environment { get; private set; }

        public Context(IDictionary<string, object> environment = null)
        {
            this.Environment = environment ?? new ConcurrentDictionary<string, object>();
        }

        public static Task<Context> GetContextAsync(IDictionary<string, object> environment)
        {
            return Task.FromResult(new Context(environment));
        }

        public T Get<T>(string key, T defaultValue=default(T))
        {
            return this.Environment.GetValue<T>(key, defaultValue);
        }

        public T GetOrInit<T>(string key, T defaultValue=default(T))
        {
            if (this.Environment.ContainsKey(key) == false)
            {
                this.Set(key, defaultValue);
            }

            return this.Get<T>(key, defaultValue);
        }

        public void Set(string key, object value)
        {
            this.Environment[key] = value;
        }

        public void UpdateTransition(Transition transition)
        {
            // TODO: Updates transition state
        }

        public void Persist()
        {

        }
    }
}
