using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pvm.Core;
using Pvm.Core.Contexts;
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
                    int current = t.Get("price", 26);
                    c.Set("total", total + current);
                    return Task.CompletedTask;
                };
            });

            act4.AddIncomingTransition(trans3);
            act4.AddIncomingTransition(trans4);

            var initData = new Dictionary<string, object>();
            process.Dispatcher.CreateWalker(trans0, null);

            process.Start(initData);

            Assert.Null(process.Context.Get<string>("waiting"));
            Assert.Equal(26, process.Context.Get<int>("total"));
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
                                    int current = t.Get("price", 26);
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
            var initData = new Dictionary<string, object>();
            process.Start(initData);

            Assert.Null(process.Context.Get<string>("waiting"));
            Assert.Equal(26, process.Context.Get<int>("total"));
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
                                    int current = t.Get("price", 31);
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
                                    int current = t.Get("price", 26);
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
            var initData = new Dictionary<string, object>();
            process.Start(initData);

            Assert.Equal(26 + 31, process.Context.Get<int>("total"));
        }

        [Fact]
        public void Test_DynamicActivities()
        {
            var process = new ProcessBuilder()
                            .CreateActivity("act1")
                            .CreateExecution("act1", _ =>
                            {
                                return (c, t) =>
                                {
                                    // 获取需要动态分拆的数量
                                    int number = t.Get("itemcount", 1);
                                    Transition nextTranistion = t.Destination.OutgoingTransitions?[0];

                                    if (nextTranistion != null)
                                    {
                                        Transition dynamicTransition = null;

                                        // 第一个item使用原有的transition
                                        // 从第二个开始，添加transition并把
                                        for (int i = 2; i <= number; i++)
                                        {
                                            dynamicTransition = new Transition
                                            {
                                                Source = nextTranistion.Source,
                                                Destination = nextTranistion.Destination
                                            };

                                            nextTranistion.Source.AddOutgoingTransition(dynamicTransition);
                                            nextTranistion.Destination.AddIncomingTransition(dynamicTransition);
                                        }
                                    }

                                    return Task.CompletedTask;
                                };
                            })
                            .CreateActivity("act2")
                            .CreateExecution("act2", _ =>
                            {
                                return (c, t) =>
                                {
                                    Console.WriteLine(c.Get("accepted", 0));
                                    c.Set("accepted", c.Get("accepted", 0) + 1);
                                    return Task.CompletedTask;
                                };
                            })
                            .CreateTransition("act1", "act2")
                            .CreateActivity("act3")
                            .CreateTransition("act2", "act3")
                            .CreateActivity("act4")
                            .CreateTransition("act3", "act4")
                            .CreateActivity("act5")
                            .CreateTransition("act4", "act5")
                            .CreateExecution("act5", _ =>
                            {
                                return (c, t) =>
                                {
                                    // TransitionState[] availableStates = new TransitionState[] {
                                    //     TransitionState.Passed, TransitionState.Blocked
                                    // };

                                    // if (t.Destination.IncomingTransitions.Any(ts => availableStates.Contains(ts.State) == false))
                                    // {
                                    //     t.CurrentTransition.SetState(TransitionState.Blocked);
                                    // }

                                    int number = c.Get("itemcount", 1);
                                    int accepted = c.Get("accepted", 0);

                                    if (number > accepted)
                                    {
                                        t.CurrentTransition.SetState(TransitionState.Blocked);
                                    }

                                    return Task.CompletedTask;
                                };
                            })
                            .SetStart("act1")
                            .Build();
            var initData = new Dictionary<string, object>();
            int itemcount = 5;
            initData["itemcount"] = itemcount;
            process.Start(initData);

            Assert.Equal(itemcount, process.Context.Get("accepted", 0));
        }

        [Fact]
        public void Test_DynamicActivitiesWithRandomSunmary()
        {
            var process = new ProcessBuilder()
                            .CreateActivity("act1")
                            .CreateExecution("act1", _ =>
                            {
                                return (c, t) =>
                                {
                                    // 获取需要动态分拆的数量
                                    int number = t.Get("itemcount", 1);
                                    Transition nextTranistion = t.Destination.OutgoingTransitions?[0];

                                    if (nextTranistion != null)
                                    {
                                        Transition dynamicTransition = null;

                                        // 第一个item使用原有的transition
                                        // 从第二个开始，添加transition并把
                                        for (int i = 2; i <= number; i++)
                                        {
                                            dynamicTransition = new Transition
                                            {
                                                Source = nextTranistion.Source,
                                                Destination = nextTranistion.Destination
                                            };

                                            nextTranistion.Source.AddOutgoingTransition(dynamicTransition);
                                            nextTranistion.Destination.AddIncomingTransition(dynamicTransition);
                                        }
                                    }

                                    return Task.CompletedTask;
                                };
                            })
                            .CreateActivity("act2")
                            .CreateExecution("act2", _ =>
                            {
                                return (c, t) =>
                                {
                                    int need = t.Get("need", 10);
                                    int total = c.Get("accepted", 0);

                                    Console.WriteLine($"before: {total}");

                                    if (total >= need)
                                    {
                                        t.CurrentTransition.SetState(TransitionState.Blocked);
                                    }
                                    else
                                    {
                                        Random ran = new Random();
                                        int maxPickableCount = ran.Next(1, need);
                                        int acceptableCount = ran.Next(1, maxPickableCount);

                                        Console.WriteLine($"max: {maxPickableCount}");
                                        Console.WriteLine($"picked: {acceptableCount}");
                                        c.Set("accepted", total + acceptableCount);
                                    }

                                    Console.WriteLine();

                                    return Task.CompletedTask;
                                };
                            })
                            .CreateTransition("act1", "act2")
                            .CreateActivity("act3")
                            .CreateExecution("act3", _ => {
                                return (c, t) => {
                                    int passed = c.Get("passed", 0);
                                    passed += 1;
                                    Console.WriteLine($"passed: {passed}");
                                    c.Set("passed", passed);

                                    return Task.CompletedTask;
                                };
                            })
                            .CreateTransition("act2", "act3")
                            .SetStart("act1")
                            .Build();
            var initData = new Dictionary<string, object>();
            int itemcount = 20;
            int need = 10;
            initData["itemcount"] = itemcount;
            initData["need"] = need;
            process.Start(initData);

            Assert.True(need <= process.Context.Get("accepted", 0));

            // Result samples:

            // before: 0
            // max: 2
            // picked: 1

            // passed: 1
            // before: 1
            // max: 9
            // picked: 1

            // passed: 2
            // before: 2
            // max: 2
            // picked: 1

            // passed: 3
            // before: 3
            // max: 9
            // picked: 3

            // passed: 4
            // before: 6
            // max: 5
            // picked: 1

            // passed: 5
            // before: 7
            // max: 9
            // picked: 5

            // passed: 6
            // before: 12

            // before: 12

            // before: 12

            // before: 12

            // before: 12

            // before: 12

            // before: 12

            // before: 12

            // before: 12

            // before: 12

            // before: 12

            // before: 12

            // before: 12

            // before: 12
        }
    }
}
