using System;
using System.Collections.Generic;
using Pvm.Core.Abstractions;

namespace Pvm.Core
{
    public abstract class Node : KeyedModel
    {
        public string Name { get; private set; }
        public IList<Transition> IncomingTransitions { get; private set; }
        public IList<Transition> OutgoingTransitions { get; private set; }

        public Node(string name, Guid? id = null) : base(id)
        {
            this.Name = name;
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

        public abstract IList<Transition> execute(Token token);
    }
}
