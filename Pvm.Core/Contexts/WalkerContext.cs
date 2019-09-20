using System.Collections.Generic;
using System.Linq;

namespace Pvm.Core.Contexts
{
    public sealed class WalkerContext : Context
    {
        public IList<Transition> Transitions => this.GetOrInit<IList<Transition>>("transitions", new List<Transition>());

        public Transition CurrentTransition => this.Transitions.LastOrDefault();

        public Activity Destination => this.CurrentTransition.Destination;

        public Activity Source => this.CurrentTransition.Source;
    }
}
