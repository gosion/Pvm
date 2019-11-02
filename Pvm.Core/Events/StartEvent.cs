using System;
using System.Collections.Generic;
using System.Linq;
using Pvm.Core.Abstractions;
using Pvm.Core.Abstractions.Features;

namespace Pvm.Core.Events
{
    public sealed class StartEvent : Node, IEvent
    {
        public new IList<Transition> IncomingTransitions => null;

        public StartEvent(string name, Guid? id = null) : base(name, id) { }

        public new void AddIncomingTransition(Transition transition)
        {
            throw new NotImplementedException();
        }

        public override IList<Transition> execute(Token token)
        {
            var feature = token.ProcessContext.Features.Get<IFeature>("log");
            if (feature is ILogFeature logger)
            {
                logger.Info("Start Event {0} occurs", this.Id);
            }

            return this.OutgoingTransitions.Where(gt => gt.Validate(token)).ToList();
        }
    }
}
