﻿using System;
using Treefort.Events;

namespace Treefort.IntegrationTests.Structure
{
    public class TestEventTwo : IEvent
    {
        public Guid CorrelationId { get; set; }
        public Guid SourceId { get; private set; }
    }
}