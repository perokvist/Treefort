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

        public ICommand When(IEvent @event)
        {
            Touched = true;
            return On((dynamic) @event);
        }

        public ICommand On(dynamic @event)
        {
            return null;
        }

        public ICommand On(TestEvent @event)
        {
            return new TestCommandTwo();
        }
    }
}
