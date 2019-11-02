using System.Collections.Generic;

namespace Pvm.Core.Abstractions
{
    public abstract class ProcessContext
    {
        public abstract FeatureCollection Features { get; }
        public abstract IDictionary<string, object> Scope { get; }
    }
}
