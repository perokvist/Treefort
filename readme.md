#Treefort

Small infrastructure parts.

## ApplicationServer

When using Treefort in a non distributed scenario, like in app, the ApplicationServer is your starting point.
An ApplicationServer is an InMemoryCommandBus and CommandDispatcher (implements ICommandBus and ICommandDispatcher).
To create an ApplicationServer you need a dispatcher and a logger. The following code shows a simple InMemory scenario.



        var logger = new ConsoleLogger();
        var store = new InMemoryEventStore(() => new InMemoryEventStream());

        var dispatcher = new Dispatcher<ICommand, Task>();
        
        dispatcher.Register<ChangeNameCommand>(command => 
			ApplicationService.UpdateAsync<CarAggregate, CarState>
			(state => new CarAggregate(state), store, command, aggregate => aggregate.ChangeName(command.NewName)));

        var server = new ApplicationServer(dispatcher.Dispatch, logger);


##Receptors

Treefort support receptors through publishing events via an EventPublisher. The EventPublisher could publish to several EventListeners, one of the specific for recptors.

            var logger = new ConsoleLogger();
            var dispatcher = new Dispatcher<ICommand, Task>();
            var server = new ApplicationServer(dispatcher.Dispatch, logger);
            var receptorListener = new ReceptorListener(Enumerable.Empty<IReceptor>(), server);

            var store = new PublishingEventStore(new InMemoryEventStore(() => new InMemoryEventStream()), new EventPublisher(new[] { receptorListener }, logger));
            
            dispatcher.Register<ChangeNameCommand>(command => ApplicationService.UpdateAsync<CarAggregate, CarState>(state => new CarAggregate(state), store, command,
                aggregate => aggregate.ChangeName(command.NewName)));


Here we create an PublishingEventStore decorator, this enables publishing events to IEventListener, one such listener is the ReceptorListener. Is the code above no receptors is given to the listener.
Creation of a Receptor is simple.
	
	public interface IReceptor 
	    {
	        Task<ICommand> HandleAsync(IEvent @event);
	    }

##Process Managers

It is possible to implement process manager through the IReceptor interface. Same persistence helpers apply here, then the process manages state is eventsourced as well.

##Application Services

The are many was of using application services.

      dispatcher.Register<ChangeNameCommand>(command => 
			ApplicationService.UpdateAsync<CarAggregate, CarState>
			(state => new CarAggregate(state), store, command, aggregate => aggregate.ChangeName(command.NewName)));

If you like a class implementation for each application service there are to helper classes. *StatelessApplicationService* and *StatefulApplicationService*. Both have some helper method for publishing/updating events/state.

###Stateless

    public class TestApplicationService : StatelessApplicationService, IApplicationService
    {
        public TestApplicationService(IEventPublisher eventPublisher) : base(eventPublisher)
        {}

        public Task HandleAsync(Commanding.ICommand command)
        {
            return When((dynamic)command);
        }

        public void When(TestCommand command)
        {
            Do(action =>
            {
                action(command,new TestEvent());
                action(command,new TestEventTwo());
                return Task.FromResult(new object());
            });  
        }
	}


###Stateful

    public class ProcessApplicationService : StatefulApplicationService<TestAggregate, TestState>, 
        IApplicationService
    {
        public ProcessApplicationService(IEventStore eventStore)
            : base(eventStore, state => new TestAggregate(state))
        {
        }
        
        public Task HandleAsync(ICommand command)
        {
            return When((dynamic) command);
        }
	}

###Rx

Instead of using the PublishingEventStore, you could use the ObservableEventStore. This way you could subscribe to events from the eventstore. Another way is to create an eventlistener that publishes events to Subject.
If you are using EventStore (geteventstore) use their subscriptions model (you could sprikle that with Rx as well).
You could use the PublishEventStore together with ObservableEventStore, just create ObservableEventStore with PublishEventStore.

#TODO

- Enevelope "builder" cms -> envelope -> Done. BuildMessage
- OnMessage should guarantee events persisted - Both guaranteed in same Task, not optional but ok, 1st itr.
- OnMessage should guarantee events "handeled" - Both EventListener and Receptors Async, so OnMessage Wais for task to complete.
- Command Retry
- Integration test for Azure
- MetaDataProvider
- JsonConverter -> TextSerializer



