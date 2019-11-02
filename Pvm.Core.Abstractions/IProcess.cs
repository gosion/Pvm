using System;
using System.Collections.Generic;

namespace Pvm.Core.Abstractions
{
    public interface IProcess
    {
        ProcessContext ProcessContext { get; }

        void Start(IDictionary<string, object> data);
        void Proceed(Guid id, IDictionary<string, object> data);
    }
}
