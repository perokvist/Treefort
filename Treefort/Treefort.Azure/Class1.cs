using System.Runtime.Serialization.Json;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treefort.Commanding;

namespace Treefort.Azure
{
    public class CommandBus : ICommandBus
    {
        public async void t()
        {
            string connectionString = "";
            string path = "";
            var client = QueueClient.CreateFromConnectionString(connectionString, path, ReceiveMode.PeekLock);
            await client.SendAsync(new BrokeredMessage("Message"));

            client.OnMessageAsync(message => Task.Run(() => Console.WriteLine(message.GetBody<string>())), new OnMessageOptions() {AutoComplete = true});

            //q.OnMessageAsync(message => message.c);
        }

        public void Send(Envelope<ICommand> command)
        {
            throw new NotImplementedException();
        }

        public void Send(IEnumerable<Envelope<ICommand>> commands)
        {
            throw new NotImplementedException();
        }
    }
}
