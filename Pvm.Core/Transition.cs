using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pvm.Core.Contexts;

namespace Pvm.Core
{
    public class Transition : KeyedModel
    {
        public Activity Source { get; set; }

        public Activity Destination { get; set; }

        public int Weight { get; set; }

        public TransitionState State { get; private set; }

        public IList<PredicateDelegate> _predicates = new List<PredicateDelegate>();

        private PredicateDelegate _defaultPredicate = (c, t) =>
        {
            if (t.CurrentTransition.State == TransitionState.Blocked)
            {
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        };

        public Transition(Guid? id = null) : base(id)
        {
            this.State = TransitionState.Pending;

            this._predicates.Add(this._defaultPredicate);
        }

        public void SetState(TransitionState state)
        {
            this.State = state;
        }

        public bool Validate(ProcessContext context, WalkerContext token)
        {
            // TODO: Should be asynchronous process
            foreach (var p in this._predicates)
            {
                if (p(context, token).Result == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
