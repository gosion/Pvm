using System;

namespace Pvm.Core
{
    public interface IProcessBuilder
    {
        Process Build();
        IProcessBuilder CreateActivity(string name);
        IProcessBuilder CreateTransition(string source, string destination);
        IProcessBuilder SetStart(string name);
        IProcessBuilder CreateExecution(string activityName, Func<ExecutionDelegate, ExecutionDelegate> execution);
    }
}
