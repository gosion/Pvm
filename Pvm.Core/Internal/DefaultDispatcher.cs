using System.Collections.Generic;

namespace Pvm.Core.Internal
{
    internal class DefaultDispatcher : IDispatcher
    {
        private IList<Walker> _walkers = new List<Walker>();

        public Walker NextWalker => _walkers.Count > 0 ? _walkers[0] : null;

        public void CreateWalker(Transition transition, IDictionary<string, object> token)
        {
            var walker = new Walker();
            walker.SetNextTransition(transition);
            walker.SetToken(token);
            this._walkers.Add(walker);
        }

        public void Dispatch(Walker walker, Context context)
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
                        walker.SetNextTransition(t);
                        walker.SetToken(token);
                    }
                    else
                    {
                        this.CreateWalker(t, token);
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
