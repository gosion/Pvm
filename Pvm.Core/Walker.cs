using System;
using System.Collections.Generic;

namespace Pvm.Core
{
    public class Walker
    {
        public Guid Id { get; private set; }

        public Token Token { get; private set; }

        public Walker()
        {
            this.Id = Guid.NewGuid();
        }

        public void SetToken(Token token)
        {
            this.Token = token;
        }

        public IList<Transition> Walk()
        {
            if (this.Token.CurrentTransition?.Destination != null)
            {
                return this.Token.CurrentTransition.Destination.execute(this.Token);
            }

            return null;
        }
    }
}
