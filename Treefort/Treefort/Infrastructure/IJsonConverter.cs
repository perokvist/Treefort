using System;

namespace Treefort.Infrastructure
{
    public interface IJsonConverter
    {
        object DeserializeObject(string json, Type type);
        string SerializeObject(object value);
    }
}