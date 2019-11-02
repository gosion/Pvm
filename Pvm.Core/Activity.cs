using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Pvm.Core.Abstractions;
using Pvm.Core.Abstractions.Features;

namespace Pvm.Core
{
    public class Activity : Node
    {
        private IList<Func<ExecutionDelegate, ExecutionDelegate>> _executions =
            new List<Func<ExecutionDelegate, ExecutionDelegate>>();

        public Activity(string name, Guid? id = null) : base(name, id) { }

        public void AddExecution(Func<ExecutionDelegate, ExecutionDelegate> execution)
        {
            this._executions.Add(execution);
        }

        public override IList<Transition> execute(Token token)
        {
            var feature = token.ProcessContext.Features.Get<IFeature>("log");
            var logger = feature as ILogFeature;

            logger?.Info("Activity {0}({1}) is ready to execute.", this.Name, this.Id);

            ExecutionDelegate next = ctx =>
            {
                ctx.CurrentTransition.SetState(TransitionState.Passed);
                return Task.CompletedTask;
            };

            foreach (var exec in this._executions.Reverse())
            {
                next = exec(next);
            }

            next(token).Wait();

            logger?.Info("Activity {0}({1}) finished ths executions.", this.Name, this.Id);

            if (token.CurrentTransition.State == TransitionState.Passed)
            {
                logger?.Info("Activity {0}({1}) passed.", this.Name, this.Id);
                return this.OutgoingTransitions.Where(gt => gt.Validate(token)).ToList();
            }
            else
            {
                logger?.Info("Activity {0}({1}) is waiting for user's feed back.", this.Name, this.Id);
                return null;
            }
        }
    }
}
