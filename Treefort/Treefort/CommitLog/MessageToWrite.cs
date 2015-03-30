namespace Treefort.CommitLog
{
    public class MessageToWrite
    {
        public readonly string Key;
        public readonly byte[] Value;

        public MessageToWrite(string key, byte[] value)
        {
            Key = key;
            Value = value;
        }
    }
}