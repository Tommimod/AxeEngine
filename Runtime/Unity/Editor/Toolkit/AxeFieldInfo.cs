using System;

namespace AxeEngine.Editor.Toolkit
{
    public struct AxeFieldInfo
    {
        public string Name { get; private set; }
        public Type FieldType { get; private set; }
        public object Value { get; private set; }

        public AxeFieldInfo(string name, Type fieldType, object value)
        {
            Name = name;
            FieldType = fieldType;
            Value = value;
        }
    }
}