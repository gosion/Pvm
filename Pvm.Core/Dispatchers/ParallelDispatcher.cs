using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pvm.Core.Abstractions;
using Pvm.Core.Extensions;

namespace Pvm.Core.Dispatchers
{
    public sealed class ParallelDispatcher : IDispatcher
    {
        private class StatableWalker : Walker
        {
            public int State { get; set; } = 0;
        }

        private IList<StatableWalker> _walkers = new List<StatableWalker>();
        private static object lockObj = new object();
        public Walker NextWalker => this._walkers.FirstOrDefault(w => w.Token.CurrentTransition.State != TransitionState.Waiting);

        public Walker CreateWalker(ProcessContext processContext, Transition transition, Token token = null)
        {
            if (token == null)
            {
                token = new Token(processContext);
            }

            var walker = new StatableWalker();
            var transitions = token.GetOrInit("transitions", new List<Transition>());
            transitions.Add(transition);
            walker.SetToken(token);

            lock(lockObj)
            {
                this._walkers.Add(walker);
            }

            return walker;
        }

        public void Dispatch(IDictionary<string, object> data, Guid? id = null)
        {
            System.IO.File.AppendAllLines("logs/test.log", new string[] {this._walkers.Count.ToString()});
            Walker walker;

            if (id.HasValue)
            {
                walker = this.FindWaitingWalker(id.Value);
                walker.Token.CurrentTransition.SetState(TransitionState.Pending);
            }
            else
            {
                walker = this.NextWalker;
            }

            if (walker != null)
            {
                walker.Token.Environment.Merge(data);
            }

            while (this._walkers.Any(w => w.Token.CurrentTransition.State != TransitionState.Waiting))
            {
                foreach (var w in this._walkers.Where(w => w.Token.CurrentTransition.State != TransitionState.Waiting).ToList())
                {
                    if (w.State == 0)
                    {
                        this.dispatch(w).Wait();
                    }
                }
            }
        }

        private async Task dispatch(StatableWalker walker)
        {
            System.IO.File.AppendAllLines("logs/test.log", new string[] {"dispatch start"});

            walker.State = 1;
            await Task.Yield();

            System.IO.File.AppendAllLines("logs/test.log", new string[] {"dispatch run"});

            var transitions = walker.Walk();

            if (transitions?.Count > 0)
            {
                var isFirst = true;

                foreach (var t in transitions)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        walker.Token.Transitions.Add(t);
                        walker.State = 0;
                        System.IO.File.AppendAllLines("logs/test.log", new string[] {"use original walker"});
                    }
                    else
                    {
                        this.CreateWalker(walker.Token.ProcessContext, t, (walker.Token.Clone() as Token));
                        System.IO.File.AppendAllLines("logs/test.log", new string[] {"create new walker"});
                    }
                }
            }
            else if(walker.Token.CurrentTransition.State == TransitionState.Waiting)
            {
                walker.State = 0;
            }
            else
            {
                System.IO.File.AppendAllLines("logs/test.log", new string[] {"remove walker"});
                lock(lockObj)
                {
                    this._walkers.Remove(walker);
                }
            }
        }

        public Walker FindWaitingWalker(Guid id)
        {
            return this._walkers.FirstOrDefault(w => w.Token.CurrentTransition.Id == id);
        }
    }
}
