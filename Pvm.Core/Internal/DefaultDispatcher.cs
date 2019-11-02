using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Pvm.Core.Abstractions;
using Pvm.Core.Extensions;

namespace Pvm.Core.Internal
{
    internal class DefaultDispatcher : IDispatcher
    {
        private IList<Walker> _walkers = new List<Walker>();

        public Walker NextWalker => this._walkers.FirstOrDefault(w => w.Token.CurrentTransition.State != TransitionState.Waiting);

        public Walker CreateWalker(ProcessContext processContext, Transition transition, Token token = null)
        {
            if (token == null)
            {
                token = new Token(processContext);
            }

            var walker = new Walker();
            var transitions = token.GetOrInit("transitions", new List<Transition>());
            transitions.Add(transition);
            walker.SetToken(token);
            this._walkers.Add(walker);

            return walker;
        }

        public void Dispatch(IDictionary<string, object> data, Guid? id = null)
        {
            Walker walker;

            if (id.HasValue)
            {
                walker = this.FindWaitingWalker(id.Value);
            }
            else
            {
                walker = this.NextWalker;
            }

            if (walker != null)
            {
                walker.Token.Environment.Merge(data);
            }

            while (walker != null)
            {
                this.dispatch(walker);
                walker = this.NextWalker;
            }
        }

        private void dispatch(Walker walker)
        {
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
                    }
                    else
                    {
                        this.CreateWalker(walker.Token.ProcessContext, t, (walker.Token.Clone() as Token));
                    }
                }
            }
            else if(walker.Token.CurrentTransition.State != TransitionState.Waiting)
            {
                this._walkers.Remove(walker);
            }
        }

        public Walker FindWaitingWalker(Guid id)
        {
            return this._walkers.FirstOrDefault(w => w.Token.CurrentTransition.Id == id);
        }
    }
}
