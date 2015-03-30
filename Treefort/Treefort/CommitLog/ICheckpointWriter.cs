using System;

namespace Treefort.CommitLog
{
   	public interface ICheckpointWriter : IDisposable{
		long GetOrInitPosition();
		void Update(long position);
	}
}