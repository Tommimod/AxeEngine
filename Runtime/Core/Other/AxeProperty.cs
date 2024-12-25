using System;

namespace AxeEngine
{
    [AttributeUsage(AttributeTargets.Struct), Serializable]
    public class AxeProperty : Attribute
    {
        public bool IgnoreInInspector { get; private set; }

        public AxeProperty(bool ignoreInInspector = false) => IgnoreInInspector = ignoreInInspector;
    }
}