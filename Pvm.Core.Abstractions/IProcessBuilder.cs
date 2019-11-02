using System.Collections.Generic;

namespace Pvm.Core.Abstractions
{
    public interface IProcessBuilder
    {
        FeatureCollection Features { get; }
        IProcessBuilder UseFeature(string key, IFeature feature);
        IProcess Build();
    }
}
