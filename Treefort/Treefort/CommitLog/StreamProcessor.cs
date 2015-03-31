using System;
using System.CodeDom;
using System.Threading;
using System.Threading.Tasks;
using Treefort.Events;

namespace Treefort.CommitLog
{
    public class StreamProcessor
    {
        private readonly ICommitLogClient _client;
        private readonly string _stream;
        private readonly ICheckpointWriter _checkpointWriter;
        private readonly Func<byte[], IEvent> _deserializer;
        private readonly Action<string> _infoLogger;

        public StreamProcessor(ICommitLogClient client, string stream,
            ICheckpointWriter checkpointWriter, Func<byte[], IEvent> deserializer, Action<string> infoLogger)
        {
            _client = client;
            _stream = stream;
            _checkpointWriter = checkpointWriter;
            _deserializer = deserializer;
            _infoLogger = infoLogger;
        }

        public void Run(CancellationToken token, Action<IEvent> dispatcher)
        {
            Func<IEvent, Task> adapter = @event =>
            {
                dispatcher(@event);
                return Task.FromResult(0);
            };

            Run(token, adapter, exception => { throw exception; }, () => { });
        }

        public void Run(CancellationToken token, IObserver<IEvent> observer)
        {
            Run(token, e => observer.OnNext(e), observer.OnError, observer.OnCompleted);
        }

        public void Run(CancellationToken token, Func<IEvent, Task> dispatcher)
        {
            Run(token, dispatcher, exception => { throw exception; }, () => { });
        }

        private async void Run(CancellationToken token, Func<IEvent, Task> dispatcher, Action<Exception> onError, Action onCompleted)
        {
            var current = _checkpointWriter.GetOrInitPosition();
            var reader = await _client.GetMessageReaderAsync(_stream);

            Task.Factory.StartNew(() => FetchMessages(token, dispatcher, onError, onCompleted, reader, current)
            , TaskCreationOptions.LongRunning);
        }

        private void Run(CancellationToken token, Action<IEvent> dispatcher, Action<Exception> onError, Action onComplete)
        {
            Func<IEvent, Task> adapter = @event =>
            {
                dispatcher(@event);
                return Task.FromResult(0);
            };

            Run(token, adapter, onError, onComplete);
        }


        private async void FetchMessages(CancellationToken token, Func<IEvent, Task> dispatcher, Action<Exception> onError, Action onCompleted, IMessageReader reader, long from)
        {
            var current = from;

            while (!token.IsCancellationRequested)
            {
                IMessageResult result;
                try
                {
                    result = await reader.GetMessagesAsync(token, current, 500);
                }
                catch (Exception e)
                {
                    onError(e);
                    token.WaitHandle.WaitOne(1000 * 20);
                    continue;
                }

                if (result.HasMessages())
                {
                    foreach (var message in result.Messages)
                    {
                        _infoLogger(string.Format("Received message {1} on stream {0}", _stream, message));
                        IEvent @event;
                        try
                        {
                            @event = _deserializer(message.Value);
                        }
                        catch (Exception e)
                        {
                            onError(e);
                            continue;
                        }
                        await dispatcher(@event);
                    }
                    current = result.NextOffset;
                    _checkpointWriter.Update(current);
                }
            }

            onCompleted();

        }
    }
}