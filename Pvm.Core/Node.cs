using System;
using System.Collections.Generic;

namespace Pvm.Core
{
    public class Node : KeyedModel
    {
        public IList<Transition> IncomingTransitions { get; private set; }
        public IList<Transition> OutgoingTransitions { get; private set; }

        public Node(Guid? id = null) : base(id)
        {
            this.IncomingTransitions = new List<Transition>();
            this.OutgoingTransitions = new List<Transition>();
        }

        public void AddIncomingTransition(Transition transition)
        {
            this.IncomingTransitions.Add(transition);
        }

        public void AddOutgoingTransition(Transition transition)
        {
            this.OutgoingTransitions.Add(transition);
        }
    }
}
