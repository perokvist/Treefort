using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Treefort.Events;

namespace Treefort.CommitLog
{
    public static class Serialization
    {
        public static Func<IEvent, byte[]> Serializer()
        {
            return @event =>
            {
                using (var ms = new MemoryStream())
                {
                    var bf = new BinaryFormatter();
                    bf.Serialize(ms, @event);
                    var result = ms.ToArray();
                    return result;
                }
            };
        }

        public static Func<byte[], IEvent> Deserializer()
        {
            return bytes =>
            {
                using (var ms = new MemoryStream(bytes))
                {
                    var bf = new BinaryFormatter();
                    var @event = (IEvent)bf.Deserialize(ms);
                    return @event;
                }
            };
        }
    }
}