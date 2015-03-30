namespace Treefort.CommitLog
{
    public class Message
    {
        public readonly IMessageId Id;
        public readonly string Key;
        public readonly byte[] Value;

        public Message(IMessageId id, string key, byte[] value)
        {
            Id = id;
            Key = key;
            Value = value;
        }
    }
}