using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pvm.Core.Contexts;

namespace Pvm.Core
{
    public class Activity : Node
    {
        private IList<Func<ExecutionDelegate, ExecutionDelegate>> _executions =
            new List<Func<ExecutionDelegate, ExecutionDelegate>>();

        public string Name { get; private set; }

        public Activity(string name, Guid? id = null) : base(id)
        {
            this.Name = name;
        }

        public void AddExecution(Func<ExecutionDelegate, ExecutionDelegate> execution)
        {
            this._executions.Add(execution);
        }

        public (WalkerContext, IList<Transition>) execute(
            ProcessContext context, WalkerContext token)
        {
            ExecutionDelegate next = (c, t) => Task.CompletedTask;

            foreach (var exec in this._executions)
            {
                next = exec(next);
            }

            next(context, token).Wait();

            return (token, this.OutgoingTransitions);
        }
    }
}
