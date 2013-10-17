using System;
using System.Collections.Generic;
using NUnit.Framework;
using Treefort.Application;
using Treefort.Azure;
using Treefort.Domain;
using Treefort.Events;
using Treefort.IntegrationTests.Structure;
using Treefort.Read;
using Configuration = Treefort.Azure.Configuration;

namespace Treefort.IntegrationTests
{
    public class Worker
    {

        private TestReceptor _receptor;
        private IProcessor _commandProcessor;

        [SetUp]
        public void Setup()
        {

            _receptor = new TestReceptor();
            _commandProcessor = Configuration.CommandProcessorInMemory(
                new List<Func<IEventStore, IEventPublisher, IApplicationService>>
                    {
                        (es, ep) => new TestApplicationService(ep),
                        (es, ep) => new ProcessApplicationService(es)
                    }
              );
        }

        [Test]
        public void name()
        {
            //Arrange
            _commandProcessor.Start();

            //Act

            //Assert
        } 
    }
}