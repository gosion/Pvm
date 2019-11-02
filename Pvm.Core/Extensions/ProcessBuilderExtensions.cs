using System;
using System.Linq;
using Pvm.Core.Abstractions;
using Pvm.Core.Builders;
using Pvm.Core.Events;
using Pvm.Core.Internal;

namespace Pvm.Core.Extensions
{
    public static class ProcessBuilderExtensions
    {
        private static ProcessData GetProcessData(this IProcessBuilder builder)
        {
            return (builder as ProcessBuilder)?.ProcessData;
        }

        public static IProcessBuilder UseDispatcher(this IProcessBuilder builder, IDispatcher dispatcher)
        {
            builder.GetProcessData()?.SetDispatcher(dispatcher);

            return builder;
        }

        public static IProcessBuilder AddActivity(this IProcessBuilder builder, string name)
        {

            builder.GetProcessData()?.Nodes.Add(new Activity(name));

            return builder;
        }

        public static IProcessBuilder AddTransition(this IProcessBuilder builder, string source, string destination)
        {
            Node nodeSource = builder.findNode(source);
            Node nodeDestination = builder.findNode(destination);
            builder.AddTransition(nodeSource, nodeDestination);

            return builder;
        }

        public static Transition AddTransition(this IProcessBuilder builder, Node source, Node destination)
        {
            Transition transition = new Transition
            {
                Source = source,
                Destination = destination
            };

            builder.GetProcessData()?.Transitions.Add(transition);
            source.AddOutgoingTransition(transition);
            destination.AddIncomingTransition(transition);

            return transition;
        }

        public static IProcessBuilder AddExecution(
            this IProcessBuilder builder,
            string activityName,
            Func<ExecutionDelegate, ExecutionDelegate> execution)
        {
            (builder.findNode(activityName) as Activity)?.AddExecution(execution);

            return builder;
        }

        public static IProcessBuilder SetStart(this IProcessBuilder builder, string activityName, string eventName = null)
        {
            Node node = builder.findNode(activityName);

            if ((node is Activity) == false)
            {
                throw new Exception("Activity not found.");
            }

            if (string.IsNullOrEmpty(eventName))
            {
                eventName = Guid.NewGuid().ToString();
            }

            StartEvent start = builder.findNode(eventName) as StartEvent;

            if (start == null)
            {
                start = new StartEvent(eventName);
            }

            builder.AddTransition(start, node);

            /*
             * To follow walker dispatch logic and make it easy,
             * we add a virtual transition for starter.
             * This virtual transition MUST NOT be included in nodes list.
             */
            Transition virt = new Transition
            {
                Destination = start
            };

            builder.GetProcessData()?.SetStartTransition(virt);

            return builder;
        }

        public static IProcessBuilder PreBuild(this IProcessBuilder builder, ProcessContext processContext, IDispatcher dispatcher)
        {
            var process = builder.GetProcessData();
            if (process != null)
            {
                dispatcher.CreateWalker(processContext, process.StartTransition);

                EndEvent end = null;

                foreach (var node in process.Nodes.ToList())
                {
                    if (node is EndEvent)
                    {
                        continue;
                    }

                    if (node.OutgoingTransitions == null || node.OutgoingTransitions.Count == 0)
                    {
                        if (end == null)
                        {
                            end = new EndEvent(Guid.NewGuid().ToString());
                        }

                        builder.AddTransition(node, end);
                    }
                }

                if (end != null)
                {
                    process.Nodes.Add(end);
                }
            }

            return builder;
        }

        private static Node findNode(this IProcessBuilder builder, string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            return builder.GetProcessData()?.Nodes.FirstOrDefault(a => string.Compare(a.Name, name, true) == 0);
        }
    }
}
