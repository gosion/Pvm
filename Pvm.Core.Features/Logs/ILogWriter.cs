using System.Threading.Tasks;

namespace Pvm.Core.Features.Logs
{
    public interface ILogWriter
    {
        Task Write(string message);
    }
}
