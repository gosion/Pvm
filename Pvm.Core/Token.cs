using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Pvm.Core.Abstractions;
using Pvm.Core.Extensions;

namespace Pvm.Core
{
    public sealed class Token : ICloneable
    {
        public IDictionary<string, object> Environment { get; private set; }
        public ProcessContext ProcessContext { get; private set; }

        public Token(ProcessContext processContext, IDictionary<string, object> environment = null)
        {
            this.ProcessContext = processContext;

            if (environment == null)
            {
                this.Environment = new ConcurrentDictionary<string, object>();
            }
            else
            {
                this.Environment = new ConcurrentDictionary<string, object>(environment);
            }

            this.Set("transitions", new List<Transition>());
        }

        public T Get<T>(string key, T defaultValue=default(T))
        {
            return this.Environment.Get<T>(key, defaultValue);
        }

        public T GetOrInit<T>(string key, T defaultValue=default(T))
        {
            return this.Environment.GetOrInit<T>(key, defaultValue);
        }

        public void Set(string key, object value)
        {
            this.Environment[key] = value;
        }

        public object Clone()
        {
            return new Token(this.ProcessContext, this.Environment);
        }

        public IList<Transition> Transitions => this.GetOrInit<IList<Transition>>("transitions");

        public Transition CurrentTransition => this.Transitions.LastOrDefault();

        public Node Destination => this.CurrentTransition.Destination;

        public Node Source => this.CurrentTransition.Source;
    }
}
