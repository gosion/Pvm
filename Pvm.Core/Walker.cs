using System;
using System.Collections.Generic;

namespace Pvm.Core
{
    public class Walker
    {
        public Guid Id { get; private set; }

        public Transition CurrentTransition { get; private set; }
        public Transition NextTransition { get; private set; }
        public IDictionary<string, object> Token { get; private set; }

        public Walker()
        {
            this.Id = Guid.NewGuid();
        }

        public void SetNextTransition(Transition transition)
        {
            if (transition == null)
            {
                throw new ArgumentNullException(nameof(transition));
            }

            this.NextTransition = transition;
        }

        public void SetToken(IDictionary<string, object> token)
        {
            this.Token = token;
        }

        public (IDictionary<string, object>, IList<Transition>) Walk(Context context)
        {
            if (this.NextTransition?.Destination != null)
            {
                this.CurrentTransition = this.NextTransition;
                return this.CurrentTransition.Destination.execute(context, this.Token);
            }

            return (null, null);
        }
    }
}
