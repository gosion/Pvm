using System;
using System.Collections.Generic;
using Pvm.Core.Abstractions;
using Pvm.Core.Abstractions.Features;

namespace Pvm.Core.Events
{
    public sealed class EndEvent : Node, IEvent
    {
        public new IList<Transition> OutgoingTransitions => null;

        public EndEvent(string name, Guid? id = null) : base(name, id) { }

        public new void AddOutgoingTransition(Transition transition)
        {
            throw new NotImplementedException();
        }

        public override IList<Transition> execute(Token token)
        {
            var feature = token.ProcessContext.Features.Get<IFeature>("log");
            if (feature is ILogFeature logger)
            {
                logger.Info("End Event {0} occurs.", this.Id);
            }

            return null;
        }
    }
}
