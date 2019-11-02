using Pvm.Core.Abstractions;
using Pvm.Core.Abstractions.Features;

namespace Pvm.Core.Features.Logs
{
    public static class ProcessContextExtensions
    {
        public static ILogFeature GetLogger(this ProcessContext processContext)
        {
            return processContext.Features.Get<IFeature>("log") as ILogFeature;
        }
    }
}
