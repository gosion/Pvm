using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pvm.Core;
using Pvm.Core.Utils;
using Xunit;

namespace Pvm.Tests
{
    public class UnitTest1
    {
        [Fact]
        public void Test_ManuallyCreateProcess()
        {
            var process = new Process();

            var act1 = new Activity("act1");
            var act2 = new Activity("act2");
            var act3 = new Activity("act3");
            var act4 = new Activity("act4");

            var trans0 = new Transition();
            trans0.Destination = act1;
            act1.AddIncomingTransition(trans0);

            var trans1 = new Transition();
            trans1.Source = act1;
            trans1.Destination = act2;
            act1.AddOutgoingTransition(trans1);

            var trans2 = new Transition();
            trans2.Source = act1;
            trans2.Destination = act3;
            act1.AddOutgoingTransition(trans2);

            var trans3 = new Transition();
            trans3.Source = act2;
            trans3.Destination = act4;
            act2.AddIncomingTransition(trans1);
            act2.AddOutgoingTransition(trans3);

            var trans4 = new Transition();
            trans4.Source = act3;
            trans4.Destination = act4;
            act3.AddIncomingTransition(trans2);
            act3.AddOutgoingTransition(trans4);

            act2.AddExecution(_ =>
            {
                return (c, t) =>
                {
                    c.Set("state", "waiting");
                    return Task.CompletedTask;
                };
            });

            act3.AddExecution(_ =>
            {
                return (c, t) =>
                {
                    int total = c.Get("total", 0);
                    int current = t.GetValue("price", 26);
                    c.Set("total", total + current);
                    return Task.CompletedTask;
                };
            });

            act4.AddIncomingTransition(trans3);
            act4.AddIncomingTransition(trans4);

            var token = new Dictionary<string, object>();
            process.Dispatcher.CreateWalker(trans0, token);

            process.Start(token);

            this.assertion(process);
        }

        [Fact]
        public void Test_UseProcessBuilder()
        {
            var process = new ProcessBuilder()
                            .CreateActivity("act1")
                            .CreateActivity("act2")
                            .CreateExecution("act2", _ =>
                            {
                                return (c, t) =>
                                {
                                    c.Set("state", "waiting");
                                    return Task.CompletedTask;
                                };
                            })
                            .CreateTransition("act1", "act2")
                            .CreateActivity("act3")
                            .CreateExecution("act3", _ =>
                            {
                                return (c, t) =>
                                {
                                    int total = c.Get("total", 0);
                                    int current = t.GetValue("price", 26);
                                    c.Set("total", total + current);
                                    return Task.CompletedTask;
                                };
                            })
                            .CreateTransition("act1", "act3")
                            .CreateActivity("act4")
                            .CreateTransition("act2", "act4")
                            .CreateTransition("act3", "act4")
                            .SetStart("act1")
                            .Build();
            var token = new Dictionary<string, object>();
            process.Start(token);

            this.assertion(process);
        }

        [Fact]
        public void Test_Execution()
        {
            var process = new ProcessBuilder()
                            .CreateActivity("act1")
                            .CreateActivity("act2")
                            .CreateExecution("act2", _ =>
                            {
                                return (c, t) =>
                                {
                                    int total = c.Get("total", 0);
                                    int current = t.GetValue("price", 31);
                                    c.Set("total", total + current);
                                    return Task.CompletedTask;
                                };
                            })
                            .CreateTransition("act1", "act2")
                            .CreateActivity("act3")
                            .CreateExecution("act3", _ =>
                            {
                                return (c, t) =>
                                {
                                    int total = c.Get("total", 0);
                                    int current = t.GetValue("price", 26);
                                    c.Set("total", total + current);
                                    return Task.CompletedTask;
                                };
                            })
                            .CreateTransition("act1", "act3")
                            .CreateActivity("act4")
                            .CreateTransition("act2", "act4")
                            .CreateTransition("act3", "act4")
                            .SetStart("act1")
                            .Build();
            var token = new Dictionary<string, object>();
            process.Start(token);

            this.assertion(process);
        }

        private void assertion(Process process)
        {
            Assert.Null(process.Context.Get<string>("state"));
            Assert.Equal(26 + 31, process.Context.Get<int>("total"));
        }
    }
}
