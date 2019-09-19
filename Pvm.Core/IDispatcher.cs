using System.Collections.Generic;

namespace Pvm.Core
{
    public interface IDispatcher
    {
        Walker NextWalker { get; }

        void CreateWalker(Transition transition, IDictionary<string, object> token);

        void Dispatch(Walker walker, Context context);
    }
}
