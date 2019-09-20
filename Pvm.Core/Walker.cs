using System;
using System.Collections.Generic;
using Pvm.Core.Contexts;

namespace Pvm.Core
{
    public class Walker
    {
        public Guid Id { get; private set; }

        public WalkerContext Token { get; private set; }

        public Walker()
        {
            this.Id = Guid.NewGuid();
            this.Token = new WalkerContext();
        }

        public void SetToken(WalkerContext token)
        {
            this.Token = token;
        }

        public (WalkerContext, IList<Transition>) Walk(ProcessContext context)
        {
            if (this.Token.CurrentTransition?.Destination != null)
            {
                return this.Token.CurrentTransition.Destination.execute(context, this.Token);
            }

            return (null, null);
        }
    }
}
