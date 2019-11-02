using System;
using Pvm.Core.Abstractions;

namespace Pvm.Core.Features.Logs
{
    public static class ProcessBuilderExtensions
    {
        public static IProcessBuilder UseLog(this IProcessBuilder builder, Action<LogFeature> action = null)
        {
            var feature = new LogFeature();
            action(feature);
            builder.UseFeature("log", feature);

            return builder;
        }
    }
}
