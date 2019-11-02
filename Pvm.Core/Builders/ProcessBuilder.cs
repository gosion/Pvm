using System.Collections.Generic;
using Pvm.Core.Abstractions;
using Pvm.Core.Extensions;
using Pvm.Core.Internal;

namespace Pvm.Core.Builders
{
    public sealed class ProcessBuilder : IProcessBuilder
    {
        internal ProcessData ProcessData { get; private set; } = new ProcessData();
        public FeatureCollection Features { get; private set; } = new FeatureCollection();

        public IProcessBuilder UseFeature(string key, IFeature feature)
        {
            this.Features.Set(key, feature);
            return this;
        }

        public IProcess Build()
        {
            IProcess process = new Process(dispatcher: this.ProcessData.Dispatcher);

            foreach (KeyValuePair<string, IFeature> f in this.Features)
            {
                process.ProcessContext.Features.Set(f.Key, f.Value);
                process.ProcessContext.Features.Get<IFeature>(f.Key).Enable();
            }

            this.PreBuild(process.ProcessContext, (process as Process).Dispatcher);

            return process;
        }
    }
}
