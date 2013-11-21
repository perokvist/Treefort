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

- Enevelope "builder" cms -> envelope -> Done. BuildMessage
- OnMessage should guarantee events persisted - Both guaranteed in same Task, not optional but ok, 1st itr.
- OnMessage should guarantee events "handeled" - Both EventListener and Receptors Async, so OnMesage Wais for task to complete.
- Command Retry
- Integration test for Azure
- MetaDataProvider
- JsonConverter -> TextSerializer



