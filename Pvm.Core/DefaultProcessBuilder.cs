using System;
using System.Collections.Generic;
using System.Linq;

namespace Pvm.Core
{
    public class ProcessBuilder : IProcessBuilder
    {
        public IList<Activity> Activities { get; private set; } = new List<Activity>();
        public IList<Transition> Transitions { get; private set; } = new List<Transition>();

        private Transition _startTransition = null;

        public Process Build()
        {
            var process = new Process();
            process.Dispatcher.CreateWalker(this._startTransition);

            return process;
        }

        public IProcessBuilder CreateActivity(string name)
        {
            this.Activities.Add(new Activity(name));

            return this;
        }

        public IProcessBuilder CreateTransition(string source, string destination)
        {
            Activity actSource = this.findActivity(source);
            Activity actDestination = this.findActivity(destination);
            Transition trans = new Transition {
                Source = actSource,
                Destination = actDestination
            };

            this.Transitions.Add(trans);
            actSource.AddOutgoingTransition(trans);
            actDestination.AddIncomingTransition(trans);

            return this;
        }

        public IProcessBuilder SetStart(string name)
        {
            Activity act = this.findActivity(name);

            if (act == null)
            {
                throw new Exception("Activity not found.");
            }

            Transition trans = new Transition();
            trans.Destination = act;
            act.AddIncomingTransition(trans);

            this._startTransition = trans;

            return this;
        }

        private Activity findActivity(string name)
        {
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }

            return this.Activities.FirstOrDefault(a => string.Compare(a.Name, name, true) == 0);
        }

        public IProcessBuilder CreateExecution(string activityName, Func<ExecutionDelegate, ExecutionDelegate> execution)
        {
            this.findActivity(activityName)?.AddExecution(execution);

            return this;
        }
    }
}
