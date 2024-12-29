using System;

namespace AxeEngine
{
    [AttributeUsage(AttributeTargets.Struct), Serializable]
    public class AxeProperty : Attribute
    {
        public string Path { get; private set; }
        public AxeProperty(string path = null) => Path = path;
    }
}