using System.Collections.Generic;
using Pvm.Core.Abstractions;

namespace Pvm.Core.Internal
{
    internal sealed class DefaultProcessContext : ProcessContext
    {
        private FeatureCollection _features;
        private IDictionary<string, object> _scope;

        public DefaultProcessContext()
        {
            this._features = new FeatureCollection();
            this._scope = new Dictionary<string, object>();
        }

        public override FeatureCollection Features => this._features;

        public override IDictionary<string, object> Scope => this._scope;
    }
}
