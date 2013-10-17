using System;

namespace Treefort.Infrastructure
{
    public class ConsoleLogger : ILogger
    {
        public void Info(string message)
        {
            Console.WriteLine(message);
        }
    }
}