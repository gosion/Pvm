using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pvm.Core
{
    public delegate Task ExecutionDelegate(Context context, IDictionary<string, object> token);
}
