﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Treefort.Commanding
{
    [ContractClass(typeof(CommandHandlerContracts<>))]
    public interface ICommandHandler<in TModel> where TModel : class
    {
        void Handle(TModel command);
    }

    public interface ICommandHandler
    {
        void Handle(object command);
    }

    [ContractClassFor(typeof(ICommandHandler<>))]
    internal abstract class CommandHandlerContracts<T> : ICommandHandler<T>
        where T : class
    {
        public void Handle(T command)
        {
            Contract.Requires<ArgumentNullException>(command != null);
            throw new NotImplementedException();
        }
    }
}
