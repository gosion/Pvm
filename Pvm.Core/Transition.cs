using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pvm.Core
{
    public sealed class Transition: KeyedModel
    {
        public Node Source { get; set; }
        public Node Destination { get; set; }
        public int Weight { get; set; }
        public TransitionState State { get; private set; }
        public IList<PredicateDelegate> _predicates = new List<PredicateDelegate>();

        private PredicateDelegate _defaultPredicate = token =>
        {
            if (token.CurrentTransition.State == TransitionState.Blocked)
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

        public bool Validate(Token token)
        {
            // TODO: Should be asynchronous process
            foreach (var p in this._predicates)
            {
                if (p(token).Result == false)
                {
                    return false;
                }
            }

            return true;
        }
    }
}
