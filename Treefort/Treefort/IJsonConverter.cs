using System;

namespace Treefort
{
    public interface IJsonConverter
    {
        object DeserializeObject(string json, Type type);
        string SerializeObject(object value);
    }
}