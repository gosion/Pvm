using System.Collections.Generic;

namespace Pvm.Core.Internal
{
    internal class ProcessData
    {
        public IDispatcher Dispatcher { get; private set; } = new DefaultDispatcher();
        public IList<Node> Nodes { get; private set; } = new List<Node>();
        public IList<Transition> Transitions { get; private set; } = new List<Transition>();
        public Transition StartTransition { get; private set; }

        public void SetStartTransition(Transition transition)
        {
            this.StartTransition = transition;
        }

        public void SetDispatcher(IDispatcher dispatcher)
        {
            this.Dispatcher = dispatcher;
        }
    }
}
