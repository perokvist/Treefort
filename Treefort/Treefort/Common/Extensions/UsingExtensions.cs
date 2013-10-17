using System;
using System.Threading.Tasks;

namespace Treefort.Common.Extensions
{
    public static class Async
    {
        public static Task Using<T>(
            this T self, Action<T> action)
            where T : IDisposable
        {
            var disposable = self;

            return Task.Factory.StartNew(() => action(disposable))
                .ContinueWith(task =>
                {
                    if (!ReferenceEquals(disposable, null))
                    {
                        disposable.Dispose();
                    }
                    return task;
                });
        }

        public static Task Using<T>(
            this T self, Func<T, Task> taskFunc)
            where T : IDisposable
        {
            T instance = self;

            return taskFunc(instance)
                .ContinueWith(task =>
                {
                    if (!ReferenceEquals(instance, null))
                    {
                        instance.Dispose();
                    }
                    return task;
                });
        }

        public static Task<TResult> Using<T, TResult>(
            this T self, Func<T, Task<TResult>> taskFunc)
            where T : IDisposable
        {
            T instance = self;

            return taskFunc(instance)
                .ContinueWith(task =>
                {
                    if (!ReferenceEquals(instance, null))
                    {
                        instance.Dispose();
                    }
                    return task.Result;
                });
        }
    }
}