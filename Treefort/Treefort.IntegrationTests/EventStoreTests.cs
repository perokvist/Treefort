using System;
using System.Collections.Generic;
using System.Net;
using EventStore.ClientAPI;
using Newtonsoft.Json;
using NUnit.Framework;
using Treefort.Events;
using Treefort.Infrastructure;
using Treefort.IntegrationTests.Structure;

namespace Treefort.IntegrationTests
{
    public class EventStoreTests
    {
        [Test]
        public void AppendAndLoad()
        {
            //Arrange
            var c = EventStoreConnection.Create(new IPEndPoint(IPAddress.Loopback, 1113));
            c.ConnectAsync().Wait();
            var store = new Treefort.EventStore.EventStore(c, new JsonConverter());
            var events = new List<IEvent> {new TestEvent {CorrelationId = Guid.NewGuid()}};
            var streamName = "test-" + Guid.NewGuid();
            //Act
            store.AppendAsync(streamName, -1, events).Wait();
            var stream = store.LoadEventStreamAsync(streamName).Result;
            //Assert
            Assert.AreEqual(1, stream.EventCount);
        }
    }

    public class JsonConverter : IJsonConverter
    {
        public object DeserializeObject(string json, System.Type type)
        {
            return JsonConvert.DeserializeObject(json, type);
        }

        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value);
        }
    }

}