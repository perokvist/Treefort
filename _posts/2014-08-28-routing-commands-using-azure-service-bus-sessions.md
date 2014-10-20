---
layout: default
title: Routing Commands using Azure Service Bus Sessions
---

### Introduction

In an earlier post a colleague wrote about [Message ordering](http://www.jayway.com/2013/12/20/message-ordering-on-windows-azure-service-bus-queues/) and FIFO options in Azure Service Bus Queues and Topics. In this post we're going to look at how to use Azure Service Bus Topic/Queue Sessions to help us route commands, in a scenario when using Service Bus as a Command Bus.

### Scenario
We're going to use a simplified domain of a Rock-Paper-Scissors game. The same domain used in earlier posts about [F# modeling](http://www.jayway.com/2014/01/13/exploring-f-through-modeling/).
In this domain we only have one Aggregate - the game aggregate. 

To state the obvious, if we were to unleash this game as a service/app, immediate success would follow. That would mean heavy load on our service.

One node would struggle to process all commands with a decent throughput.
If we add more nodes and let them compete in processing commands from the Topic/Queue, concurrency problems would occur with the aggregate state. One node could process a command out of order due to another node haven't yet processed the command before for that game.

We would like the same node to handle all commands for a given game - aggregate instance. How could we route all commands for a game instance to the same node?

### Service Bus Sessions
To solve this problem [message sessions](http://msdn.microsoft.com/en-us/library/microsoft.servicebus.messaging.messagesession.aspx) could be used. We're going to use a session to identify a game.
To send a message/command as part a session is as simple as setting the SessionId property on the BrokeredMessage.
Simplified;

    new BrokeredMessage(command) {SessionId = command.AggregateId};

The node processing the commands needs to start handling commands for a given session/game.
To do this the client starts accepting a session.

    var session = await _client.AcceptMessageSessionAsync();

When accepting the session we could specify what session we like to receive messages for, using an overload. But we don't know the session. Without session param the client would take the first session available.

We could also give a timespan as timeout when the node would release the session if no messages are received. This allows the node or another node to pick up the session when new messages comes in. *Note if you don't specify a TimeSpan, a default time out is set, 60 seconds)*

    var session = await _client.AcceptMessageSessionAsync(TimeSpan.FromMinutes(1));

Ok, great. But how could one node listen to several sessions? The node needs to process as many messages/commands as possible.

When not using sessions, it is easy to handle messages using the [message pump](http://msdn.microsoft.com/en-us/library/azure/dn198643.aspx).

    _client.OnMessageAsync(messageHandler, options);


Using sessions is similar, the message pump is on every session.

	var session = await _client.AcceptMessageSessionAsync(TimeSpan.FromMinutes(1));
    session.OnMessageAsync(messageHandler, options);
            
Ok, but that’s only one session, the node should process more than one game each minute (or one in total).
We could let the node handle a session per thread.

	private async Task StartSessionAsync(Func<BrokeredMessage, Task> messageHandler, OnMessageOptions options)
    {
            var session = await _client.AcceptMessageSessionAsync(TimeSpan.FromMinutes(1));
            session.OnMessageAsync(messageHandler, options);
            await Task.Factory.StartNew(() => StartSessionAsync(messageHandler, options));
    }

*Note to keep the sample simple, we don't catch the [Timeout Exception](http://msdn.microsoft.com/en-us/library/hh293141.aspx) from AcceptMessageSession in the example code, nor do we have any cancellation token*.

Here we start listening for new a session on a new thread when the first session is accepted.
If the node would go down, the session should be released, letting another node take over.

Ok, simplified and not complete.  But I hope if given some information on how session could be used for this kind of scenario.

	public static async Task StartSessionAsync(Func<Task<MessageSession>> clientAccept,  Func<BrokeredMessage, Task> messageHandler, Action<string> logger,  OnMessageOptions options, CancellationToken token)
        {
            if (!token.IsCancellationRequested)
            {
                try
                {
                    var session = await clientAccept();
                    logger(string.Format("Session accepted: {0} on {1}", session.SessionId, Thread.CurrentThread.ManagedThreadId));
                    session.OnMessageAsync(messageHandler, options);
                }
                catch (TimeoutException ex)
                {
                    logger(string.Format("Session timeout: {0}", ex.Message));
                }
                catch(Exception ex)
                {
                    logger(string.Format("Session exception: {0}", ex.Message));
                    throw;
                }

                 await Task.Run(() => StartSessionAsync(clientAccept, messageHandler, logger, options, token), token);
            }
        } 

### Conclusion
We have looked at a simple scenario in theory (aka the happy place) to explore session’s usage when using Azure Service Bus Queues or Topics in a Command Bus scenario.

Below you find some other interesting, related topics.
Maybe topics for future exploration.

Enjoy!

#### More Resources
There are a lot more on the topic of routing, partitioning and scaling.
The [Service Bus is now supported with Azure WebJobs](http://www.nuget.org/packages/Microsoft.Azure.Jobs.ServiceBus/0.3.1-beta), and web jobs support auto-scaling based on queue size.

If we were to model our Aggregates with [Orleans](http://research.microsoft.com/en-us/projects/orleans/) as a Grain, the routing and scaling would be handled for us in a scenario like this, where the a game instance would be the grain instance identity.

On the topic of high throughput, the latest addition to the Service Bus - [Event Hubs](http://azure.microsoft.com/en-us/services/event-hubs/) have some interesting features.





