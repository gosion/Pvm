using System;

namespace Pvm.Core
{
    public abstract class KeyedModel
    {
        public Guid Id { get; private set; }

        public KeyedModel(Guid? id = null)
        {
            this.Id = id ?? Guid.NewGuid();
        }
    }
}
