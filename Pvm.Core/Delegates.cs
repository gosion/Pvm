using System.Threading.Tasks;
using Pvm.Core.Abstractions;

namespace Pvm.Core
{
    public delegate Task ExecutionDelegate(Token token);

    public delegate Task<bool> PredicateDelegate(Token token);
}
