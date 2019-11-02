using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pvm.Core.Abstractions;
using Pvm.Core.Builders;
using Pvm.Core.Extensions;
using Pvm.Core.Features.Logs;

namespace Pvm.Core.Tests
{
    internal class PauseAndContinute
    {
        public static IProcess GenProcess(bool enableLog = false, IDispatcher dispatcher = null, Action<LogFeature> action = null)
        {
            var builder = new ProcessBuilder();

            if (dispatcher != null)
            {
                builder.UseDispatcher(dispatcher);
            }

            builder.AddActivity("act1")
            .AddActivity("act2")
            .AddExecution("act2", next =>
            {
                return token =>
                {
                    int input = token.Get("user_input", 0);
                    if (input == 0)
                    {
                        token.CurrentTransition.SetState(TransitionState.Waiting);
                        token.ProcessContext.GetLogger()?.Info("I am waiting");
                        var waitingIds = token.ProcessContext.Scope.Get<IList<Guid>>(
                            "waiting_ids",
                            new List<Guid>());
                        waitingIds.Add(token.CurrentTransition.Id);
                        token.ProcessContext.Scope["waiting_ids"] = waitingIds;
                        return Task.CompletedTask;
                    }
                    else
                    {
                        int total = token.ProcessContext.Scope.Get("total", 0);
                        token.ProcessContext.Scope["total"] = total + input;
                        token.ProcessContext.GetLogger()?.Info(
                            "Current {0}, Total: {1}",
                            input,
                            token.ProcessContext.Scope.Get("total", 0));

                        return next(token);
                    }
                };
            })
            .AddTransition("act1", "act2")
            .AddActivity("act3")
            .AddExecution("act3", next =>
            {
                return token =>
                {
                    int total = token.ProcessContext.Scope.Get("total", 0);
                    int current = token.Get("price1", 0);
                    token.ProcessContext.Scope["total"] = total + current;
                    token.ProcessContext.GetLogger()?.Info(
                        "Current {0}, Total: {1}",
                        current,
                        token.ProcessContext.Scope.Get("total", 0).ToString());

                    return next(token);
                };
            })
            .AddExecution("act3", next =>
            {
                return token =>
                {
                    int total = token.ProcessContext.Scope.Get("total", 0);
                    int current = token.Get("price2", 0);
                    token.ProcessContext.Scope["total"] = total + current;
                    token.ProcessContext.GetLogger()?.Info(
                        "Current {0}, Total: {1}",
                        current,
                        token.ProcessContext.Scope.Get("total", 0).ToString());

                    return next(token);
                };
            })
            .AddTransition("act1", "act3")
            .AddActivity("act4")
            .AddTransition("act2", "act4")
            .AddTransition("act3", "act4")
            .SetStart("act1");
            if (enableLog)
            {
                builder.UseLog(action);
            }

            return builder.Build();
        }
    }
}
