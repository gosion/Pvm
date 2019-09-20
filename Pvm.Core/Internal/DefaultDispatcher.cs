using System.Collections.Generic;
using Pvm.Core.Contexts;

namespace Pvm.Core.Internal
{
    internal class DefaultDispatcher : IDispatcher
    {
        private IList<Walker> _walkers = new List<Walker>();

        public Walker NextWalker => _walkers.Count > 0 ? _walkers[0] : null;

        public void CreateWalker(Transition transition, WalkerContext token = null)
        {
            if (token == null)
            {
                token = new WalkerContext();
            }

            var walker = new Walker();
            token.Transitions.Add(transition);
            walker.SetToken(token);
            this._walkers.Add(walker);
        }

        public void Dispatch(Walker walker, ProcessContext context)
        {
            var (token, transitions) = walker.Walk(context);

            if (transitions?.Count > 0)
            {
                var isFirst = true;

                foreach (var t in transitions)
                {
                    if (isFirst)
                    {
                        isFirst = false;
                        // walker.SetNextTransition(t);
                        token.Transitions.Add(t);
                        walker.SetToken(token);
                    }
                    else
                    {
                        this.CreateWalker(t, null);
                    }
                }
            }
            else
            {
                this._walkers.Remove(walker);
            }
        }
    }
}
