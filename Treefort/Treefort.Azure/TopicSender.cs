﻿using System;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace Treefort.Azure
{
    public class TopicSender : IMessageSender
    {
        private readonly TopicClient _client;

        public TopicSender()
        {
            var factory = MessagingFactory.Create();
            _client = factory.CreateTopicClient("path");
        }

        public void Send(Func<BrokeredMessage> messageFactory)
        {
            _client.Send(messageFactory());
        }

        public Task SendAsync(Func<BrokeredMessage> messageFactory)
        {
            return _client.SendAsync(messageFactory());
        }

        public event EventHandler Retrying;
    }
}