using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Treefort.Commanding;
using Treefort.Domain;
using Treefort.Events;

namespace Treefort.IntegrationTests.Structure
{
    public class TestReceptor : IReceptor
    {
        public bool Touched { get; set; }

        public Task<ICommand> HandleAsync(IEvent @event)
        {
            Touched = true;
            return Task.FromResult<ICommand>(null);
        }

    }
}
