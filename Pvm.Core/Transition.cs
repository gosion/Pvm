using System;

namespace Pvm.Core
{
    public class Transition : KeyedModel
    {
        public Activity Source { get; set; }

        public Activity Destination { get; set; }

        public Transition(Guid? id = null) : base(id)
        {

        }
    }
}
