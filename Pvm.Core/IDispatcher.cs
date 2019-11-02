using System;
using System.Collections.Generic;
using Pvm.Core.Abstractions;

namespace Pvm.Core
{
    public interface IDispatcher
    {
        Walker NextWalker { get; }

        Walker CreateWalker(ProcessContext processContext, Transition transition, Token token = null);

        void Dispatch(IDictionary<string, object> data, Guid? id = null);

        Walker FindWaitingWalker(Guid id);
    }
}
