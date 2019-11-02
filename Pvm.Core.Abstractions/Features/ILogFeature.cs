using System.Threading.Tasks;

namespace Pvm.Core.Abstractions.Features
{
    public interface ILogFeature
    {
        Task Info(string message, params object[] parameters);
        Task Debug(string message, params object[] parameters);
        Task Warning(string message, params object[] parameters);
        Task Error(string message, params object[] parameters);
    }
}
