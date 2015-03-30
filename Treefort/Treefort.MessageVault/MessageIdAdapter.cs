using System;
using System.Diagnostics.Contracts;
using MessageVault;
using Treefort.CommitLog;

namespace Treefort.MessageVault
{
    public class MessageIdAdapter : IMessageId
    {
        private readonly MessageId _messageId;

        public MessageIdAdapter(MessageId messageId)
        {
            _messageId = messageId;
        }

        public int CompareTo(IMessageId other)
        {
            return _messageId.CompareTo(other);
        }

        public int CompareTo(object obj)
        {
            return _messageId.CompareTo(obj);
        }

        [Pure]
        public DateTime GetTimeUtc()
        {
            return _messageId.GetTimeUtc();
        }

        [Pure]
        public long GetOffset()
        {
            return _messageId.GetOffset();
        }

        [Pure]
        public int GetRand()
        {
            return _messageId.GetRand();
        }

        [Pure]
        public bool IsEmpty()
        {
            return _messageId.IsEmpty();
        }

        [Pure]
        public byte[] GetBytes()
        {
            return _messageId.GetBytes();
        }
    }
}