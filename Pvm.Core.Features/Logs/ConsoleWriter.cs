using System;
using System.Threading.Tasks;

namespace Pvm.Core.Features.Logs
{
    public sealed class ConsoleWriter : ILogWriter
    {
        public async Task Write(string message)
        {
            Console.Write(message);
            await Task.Yield();
        }
    }
}
