using NUnit.Framework;
using System;
using System.Collections.Generic;
using Treefort.Application;
using Treefort.Commanding;
using Treefort.Domain;
using Treefort.Events;
using Treefort.Infrastructure;
using Treefort.IntegrationTests.Structure;
using Treefort.Read;

namespace Treefort.IntegrationTests
{
    [TestFixture]
    public class ApplicationServerWithInMemoryConfiguration
    {
        private IApplicationServer _appServer;
        private TestProjection _projection;
        private TestReceptor _receptor;

        [SetUp]
        public void Setup()
        {
         
            _projection = new TestProjection();
            _receptor = new TestReceptor();
            _appServer = Configuration.InMemory(
                new List<Func<IEventStore, IEventPublisher, IApplicationService>>
                    {
                        (es, ep) => new TestApplicationService(ep),
                        (es, ep) => new ProcessApplicationService(es)
                    }, 
                () => new List<IProjection> { _projection},
                () => new List<IReceptor> { _receptor }
              );
        }
        
        [Test]
        public void ShouldPublishEventToProjection()
        {
            _appServer.DispatchAsync(new TestCommand()).Wait();
            Assert.IsTrue(_projection.Retrived);
        }

        [Test]
        public void ShouldPublishEventsToProjection()
        {
            _appServer.DispatchAsync(new TestCommand()).Wait();
                Assert.AreEqual(2, _projection.EventCount); 
        }
        

        [Test]
        public void ShouldPublishEventToReceptor()
        {
            _appServer.DispatchAsync(new TestCommand()).Wait();
            Assert.IsTrue(_receptor.Touched);
        }
        
    }
}
