using Treefort.CommitLog;

namespace Treefort.MessageVault
{
    public class CheckpointWriterAdapter : ICheckpointWriter
    {
        private readonly global::MessageVault.ICheckpointWriter _checkpointWriter;

        public CheckpointWriterAdapter(global::MessageVault.ICheckpointWriter checkpointWriter)
        {
            _checkpointWriter = checkpointWriter;
        }

        public void Dispose()
        {
            _checkpointWriter.Dispose();
        }

        public long GetOrInitPosition()
        {
            return _checkpointWriter.GetOrInitPosition();
        }

        public void Update(long position)
        {
            _checkpointWriter.Update(position);
        }
    }
}