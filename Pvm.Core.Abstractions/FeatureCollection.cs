using System.Collections;
using System.Collections.Generic;

namespace Pvm.Core.Abstractions
{
    public sealed class FeatureCollection : IEnumerable
    {
        public IDictionary<string, IFeature> Items { get; private set; } = new Dictionary<string, IFeature>();

        public IEnumerator GetEnumerator()
        {
            foreach (var item in this.Items)
            {
                yield return item;
            }
        }

        public T Get<T>(string key) where T : IFeature
        {
            IFeature target = null;
            if (this.Items.TryGetValue(key, out target))
            {
                return (T)target;
            }

            return default(T);
        }

        public void Set<T>(string key, T value) where T : IFeature
        {
            this.Items[key] = value;
        }
    }
}
