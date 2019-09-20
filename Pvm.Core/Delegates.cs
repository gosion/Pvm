using System.Collections.Generic;
using System.Threading.Tasks;
using Pvm.Core.Contexts;

namespace Pvm.Core
{
    public delegate Task ExecutionDelegate(ProcessContext context, WalkerContext token);

    public delegate Task<bool> PredicateDelegate(ProcessContext context, WalkerContext token);
}
