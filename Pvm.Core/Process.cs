using System;
using System.Collections.Generic;
using Pvm.Core.Abstractions;
using Pvm.Core.Extensions;
using Pvm.Core.Internal;

namespace Pvm.Core
{
    public sealed class Process : KeyedModel, IProcess
    {
        public ProcessContext ProcessContext { get; private set; }
        public IDispatcher Dispatcher { get; private set; }


        public Process(Guid? id = null, IDispatcher dispatcher = null)
            : base(id)
        {
            this.ProcessContext = new DefaultProcessContext();
            this.Dispatcher = dispatcher ?? new DefaultDispatcher();
        }

        public void Start(IDictionary<string, object> data)
        {
            this.doProceed(data);
        }

        public void Pause() {}

        public void Proceed(Guid id, IDictionary<string, object> data)
        {
            this.doProceed(data, id);
        }

        public void Stop() {}

        private void doProceed(IDictionary<string, object> data, Guid? id = null)
        {
            this.Dispatcher.Dispatch(data, id);
        }
    }
}
