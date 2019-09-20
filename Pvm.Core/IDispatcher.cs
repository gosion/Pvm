using System.Collections.Generic;
using Pvm.Core.Contexts;

namespace Pvm.Core
{
    public interface IDispatcher
    {
        Walker NextWalker { get; }

        void CreateWalker(Transition transition, WalkerContext token = null);

        void Dispatch(Walker walker, ProcessContext context);
    }
}
