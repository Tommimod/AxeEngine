using System;
using System.Collections.Generic;

namespace AxeEngine
{
    public static class PropertyTypeMapper
    {
        private static readonly Dictionary<Type, int> _typeToIndex = new();
        private static readonly List<Type> _indexToType = new();
        private static int _nextIndex;

        public static int GetComponentIndex(Type type)
        {
            if (_typeToIndex.TryGetValue(type, out var index))
            {
                return index;
            }

            index = _nextIndex++;
            _typeToIndex[type] = index;
            _indexToType.Add(type);
            return index;
        }

        public static int GetMaskArrayIndex(int componentIndex)
        {
            return componentIndex / 64;
        }

        public static int GetMaskBitIndex(int componentIndex)
        {
            return componentIndex % 64;
        }
    }
}