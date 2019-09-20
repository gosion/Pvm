using System;
using System.Collections.Generic;
using Pvm.Core.Contexts;
using Pvm.Core.Internal;
using Pvm.Core.Utils;

namespace Pvm.Core
{
    public class Process : KeyedModel
    {
        // public IList<Activity> Activities { get; private set; }
        // public IList<Transition> Transitions { get; private set; }
        private IStorage<Process> _storage;

        public IDispatcher Dispatcher { get; private set; }
        public ProcessContext Context { get; private set; }

        public Process(Guid? id = null, IDispatcher dispatcher = null, IStorage<Process> storage = null)
            : base(id)
        {
            this.Dispatcher = dispatcher ?? new DefaultDispatcher();
            this.Context = new ProcessContext();
            this._storage = storage;
        }

        public void Start(IDictionary<string, object> initData)
        {
            var walker = this.Dispatcher.NextWalker;
            walker.Token.Environment.Merge(initData);

            while (walker != null)
            {
                this.Dispatcher.Dispatch(walker, this.Context);
                walker = this.Dispatcher.NextWalker;
            }
        }

        public void Pause() {}

        public void Proceed() {}

        public void Stop() {}

        public void Persist()
        {
            this._storage.SaveAsync(this);
        }
    }
}
