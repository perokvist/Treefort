#Treefort

Small infrastructure parts.
Ick! , yes infrastructure libraries. 

## ApplicationServer

The simplest usage is via the ApplicationServer, created by configuration.
 ```
 var appServer = Configuration.InMemory(...);
 await appServer.DispatchAsync(new TestCommand());
 ```
 Configuration supports a range of different options.

 ```
var projection = new TestProjection();
var receptor = new TestReceptor();
var appServer = Configuration.InMemory(
    new List<Func<IEventStore, IEventPublisher, IApplicationService>>
        {
            (es, ep) => new TestApplicationService(ep)
        }, 
    () => new List<IProjection> { projection},
    () => new List<IReceptor> { receptor }
    );

 ```

#TODO

- InMemory CommandBus - Done AppServer impl.
- Azure CommandBus - Done 1st it.
- EventBus - Done 1st it.
- No Projections in Azure Config - Done.
- Enevelope "builder" cms -> envelope -> Done. BuildMessage
- EventProcessor - Done 1st it.
- OnMessage should guarantee events persisted, but not dispatched 1st scope.
- OnMessage should guarantee events "handeled" but not receportor command dispatchement 1st scope.
- Command Retry
- Integration test for Azure
- MetaDataProvider
- JsonConverter -> TextSerializer
- Remove / Make AppService "functional" return events