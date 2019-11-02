# Process Virtual Machine (PVM)
> Note: This is just a toy. Please DO NOT use it in your product(S)!!!

This is a pratice to abstract and implement the process transition ideas.

This pratice includes terminologies:
* Activity

  The node which has own execution(s) to perform, generally, make output based on the input.

* Transition

  The route that connect the Activities and Events

* Event

  Something that "happens" during the course of Process

* Walker

  An instance that keep the token and pass through the Activities "walk" along the Transitions.

* Dispatcher

  A walker manager who arranges the tasks and manages walkers lifecycle.

* Token

  A passport that stores someting important for Walker to pass the Activities



## Features Blueprint

- [x] Sequence
- [ ] Fork
- [x] Condition
- [ ] Cycle
- [x] Pause & Continue
- [ ] Grouping
- [ ] Clone
- [ ] Persistence & Wakeup
- [ ] Load from configuration file
- [ ] Flow chart
- [ ] Clustering
- [ ] Messaging (Email?)
- [ ] Scheduling
- [ ] Rule engine