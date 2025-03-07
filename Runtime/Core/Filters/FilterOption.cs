using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.IL2CPP.CompilerServices;

namespace AxeEngine
{
    [BurstCompile]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public readonly struct FilterOption
    {
        private readonly Type[] _withTypes;
        private readonly Type[] _withAnyTypes;
        private readonly Type[] _withoutTypes;
        private readonly HashSet<Type> _copyBuffer;

        /// <summary>
        /// Default FilterOption with capacity 7
        /// </summary>
        public static FilterOption Empty => new(7);

        /// <summary>
        /// Use for create new FilterOption with custom capacity
        /// </summary>
        /// <param name="capacity">capacity for types</param>
        public FilterOption(int capacity)
        {
            _copyBuffer = new HashSet<Type>();
            _withTypes = new Type[capacity];
            _withAnyTypes = new Type[capacity];
            _withoutTypes = new Type[capacity];
        }

        private int GetCount(Type[] array)
        {
            var freeIndex = Array.IndexOf(array, null);
            return freeIndex == -1 ? array.Length : freeIndex;
        }

        public bool IsEquals(FilterOption other)
        {
            var otherWithTypes = other._withTypes;
            _copyBuffer.Clear();
            foreach (var type in _withTypes)
            {
                _copyBuffer.Add(type);
            }

            foreach (var type in otherWithTypes)
            {
                _copyBuffer.Add(type);
            }

            var lessArray = GetCount(_withTypes) > GetCount(otherWithTypes) ? otherWithTypes : _withTypes;
            _copyBuffer.ExceptWith(lessArray);
            if (_withTypes.Length != otherWithTypes.Length || _copyBuffer.Count > 0) return false;

            var otherWithAnyTypes = other._withAnyTypes;
            _copyBuffer.Clear();
            foreach (var type in _withAnyTypes)
            {
                _copyBuffer.Add(type);
            }

            foreach (var type in otherWithAnyTypes)
            {
                _copyBuffer.Add(type);
            }

            lessArray = GetCount(_withAnyTypes) > GetCount(otherWithAnyTypes) ? otherWithAnyTypes : _withAnyTypes;
            _copyBuffer.ExceptWith(lessArray);
            if (_withAnyTypes.Length != otherWithAnyTypes.Length || _copyBuffer.Count > 0) return false;

            var otherWithoutTypes = other._withoutTypes;
            _copyBuffer.Clear();
            foreach (var type in _withoutTypes)
            {
                _copyBuffer.Add(type);
            }

            foreach (var type in otherWithoutTypes)
            {
                _copyBuffer.Add(type);
            }

            lessArray = GetCount(_withoutTypes) > GetCount(otherWithoutTypes) ? otherWithoutTypes : _withoutTypes;
            _copyBuffer.ExceptWith(lessArray);
            if (_withoutTypes.Length != otherWithoutTypes.Length || _copyBuffer.Count > 0) return false;

            return true;
        }

        public bool IsValid(IActor actor)
        {
            var properties = actor.GetAllProperties();
            var types = new Type[properties.Count];
            var i = 0;
            foreach (var property in properties)
            {
                types[i++] = property;
            }

            var isInvalid = false;
            foreach (var withType in _withTypes)
            {
                if (withType == null)
                {
                    continue;
                }

                if (Array.IndexOf(types, withType) == -1)
                {
                    isInvalid = true;
                    break;
                }
            }

            if (isInvalid) return false;
            isInvalid = _withAnyTypes[0] != null;
            foreach (var withAnyType in _withAnyTypes)
            {
                if (Array.IndexOf(types, withAnyType) > -1)
                {
                    isInvalid = false;
                    break;
                }
            }

            if (isInvalid) return false;
            foreach (var withoutType in _withoutTypes)
            {
                if (Array.IndexOf(types, withoutType) > -1)
                {
                    isInvalid = true;
                    break;
                }
            }

            return !isInvalid;
        }

        public FilterOption With<T>() where T : struct
        {
            var type = typeof(T);
            AddWithType(type);

            return this;
        }

        public FilterOption With<T, T1>() where T : struct
        {
            AddWithType(typeof(T));
            AddWithType(typeof(T1));

            return this;
        }

        public FilterOption With<T, T1, T2>() where T : struct
        {
            AddWithType(typeof(T));
            AddWithType(typeof(T1));
            AddWithType(typeof(T2));

            return this;
        }

        public FilterOption With<T, T1, T2, T3>() where T : struct
        {
            AddWithType(typeof(T));
            AddWithType(typeof(T1));
            AddWithType(typeof(T2));
            AddWithType(typeof(T3));

            return this;
        }

        public FilterOption With<T, T1, T2, T3, T4>() where T : struct
        {
            AddWithType(typeof(T));
            AddWithType(typeof(T1));
            AddWithType(typeof(T2));
            AddWithType(typeof(T3));
            AddWithType(typeof(T4));

            return this;
        }

        public FilterOption With<T, T1, T2, T3, T4, T5>() where T : struct
        {
            AddWithType(typeof(T));
            AddWithType(typeof(T1));
            AddWithType(typeof(T2));
            AddWithType(typeof(T3));
            AddWithType(typeof(T4));
            AddWithType(typeof(T5));

            return this;
        }

        public FilterOption With<T, T1, T2, T3, T4, T5, T6>() where T : struct
        {
            AddWithType(typeof(T));
            AddWithType(typeof(T1));
            AddWithType(typeof(T2));
            AddWithType(typeof(T3));
            AddWithType(typeof(T4));
            AddWithType(typeof(T5));
            AddWithType(typeof(T6));

            return this;
        }

        public FilterOption With<T, T1, T2, T3, T4, T5, T6, T7>() where T : struct
        {
            AddWithType(typeof(T));
            AddWithType(typeof(T1));
            AddWithType(typeof(T2));
            AddWithType(typeof(T3));
            AddWithType(typeof(T4));
            AddWithType(typeof(T5));
            AddWithType(typeof(T6));
            AddWithType(typeof(T7));

            return this;
        }

        public FilterOption WithAny<T, T1>() where T : struct
        {
            AddWithAnyType(typeof(T));
            AddWithAnyType(typeof(T1));

            return this;
        }

        public FilterOption WithAny<T, T1, T2>() where T : struct
        {
            AddWithAnyType(typeof(T));
            AddWithAnyType(typeof(T1));
            AddWithAnyType(typeof(T2));

            return this;
        }

        public FilterOption WithAny<T, T1, T2, T3>() where T : struct
        {
            AddWithAnyType(typeof(T));
            AddWithAnyType(typeof(T1));
            AddWithAnyType(typeof(T2));
            AddWithAnyType(typeof(T3));

            return this;
        }

        public FilterOption WithAny<T, T1, T2, T3, T4>() where T : struct
        {
            AddWithAnyType(typeof(T));
            AddWithAnyType(typeof(T1));
            AddWithAnyType(typeof(T2));
            AddWithAnyType(typeof(T3));
            AddWithAnyType(typeof(T4));

            return this;
        }

        public FilterOption WithAny<T, T1, T2, T3, T4, T5>() where T : struct
        {
            AddWithAnyType(typeof(T));
            AddWithAnyType(typeof(T1));
            AddWithAnyType(typeof(T2));
            AddWithAnyType(typeof(T3));
            AddWithAnyType(typeof(T4));
            AddWithAnyType(typeof(T5));

            return this;
        }

        public FilterOption WithAny<T, T1, T2, T3, T4, T5, T6>() where T : struct
        {
            AddWithAnyType(typeof(T));
            AddWithAnyType(typeof(T1));
            AddWithAnyType(typeof(T2));
            AddWithAnyType(typeof(T3));
            AddWithAnyType(typeof(T4));
            AddWithAnyType(typeof(T5));
            AddWithAnyType(typeof(T6));

            return this;
        }

        public FilterOption WithAny<T, T1, T2, T3, T4, T5, T6, T7>() where T : struct
        {
            AddWithAnyType(typeof(T));
            AddWithAnyType(typeof(T1));
            AddWithAnyType(typeof(T2));
            AddWithAnyType(typeof(T3));
            AddWithAnyType(typeof(T4));
            AddWithAnyType(typeof(T5));
            AddWithAnyType(typeof(T6));
            AddWithAnyType(typeof(T7));

            return this;
        }

        public FilterOption Without<T>() where T : struct
        {
            var type = typeof(T);
            AddWithoutType(type);

            return this;
        }

        public FilterOption Without<T, T1>() where T : struct
        {
            AddWithoutType(typeof(T));
            AddWithoutType(typeof(T1));

            return this;
        }

        public FilterOption Without<T, T1, T2>() where T : struct
        {
            AddWithoutType(typeof(T));
            AddWithoutType(typeof(T1));
            AddWithoutType(typeof(T2));

            return this;
        }

        public FilterOption Without<T, T1, T2, T3>() where T : struct
        {
            AddWithoutType(typeof(T));
            AddWithoutType(typeof(T1));
            AddWithoutType(typeof(T2));
            AddWithoutType(typeof(T3));

            return this;
        }

        public FilterOption Without<T, T1, T2, T3, T4>() where T : struct
        {
            AddWithoutType(typeof(T));
            AddWithoutType(typeof(T1));
            AddWithoutType(typeof(T2));
            AddWithoutType(typeof(T3));
            AddWithoutType(typeof(T4));

            return this;
        }

        public FilterOption Without<T, T1, T2, T3, T4, T5>() where T : struct
        {
            AddWithoutType(typeof(T));
            AddWithoutType(typeof(T1));
            AddWithoutType(typeof(T2));
            AddWithoutType(typeof(T3));
            AddWithoutType(typeof(T4));
            AddWithoutType(typeof(T5));

            return this;
        }

        public FilterOption Without<T, T1, T2, T3, T4, T5, T6>() where T : struct
        {
            AddWithoutType(typeof(T));
            AddWithoutType(typeof(T1));
            AddWithoutType(typeof(T2));
            AddWithoutType(typeof(T3));
            AddWithoutType(typeof(T4));
            AddWithoutType(typeof(T5));
            AddWithoutType(typeof(T6));

            return this;
        }

        public FilterOption Without<T, T1, T2, T3, T4, T5, T6, T7>() where T : struct
        {
            AddWithoutType(typeof(T));
            AddWithoutType(typeof(T1));
            AddWithoutType(typeof(T2));
            AddWithoutType(typeof(T3));
            AddWithoutType(typeof(T4));
            AddWithoutType(typeof(T5));
            AddWithoutType(typeof(T6));
            AddWithoutType(typeof(T7));

            return this;
        }

        private void AddWithType(Type type)
        {
            var freeIndex = Array.IndexOf(_withTypes, null);
            if (freeIndex == -1)
            {
                throw new Exception("Too many types. Create new FilterOption with more capacity.");
            }

            _withTypes[freeIndex] = type;
        }

        private void AddWithAnyType(Type type)
        {
            var freeIndex = Array.IndexOf(_withAnyTypes, null);
            if (freeIndex == -1)
            {
                throw new Exception("Too many types. Create new FilterOption with more capacity.");
            }

            _withAnyTypes[freeIndex] = type;
        }

        private void AddWithoutType(Type type)
        {
            var freeIndex = Array.IndexOf(_withoutTypes, null);
            if (freeIndex == -1)
            {
                throw new Exception("Too many types. Create new FilterOption with more capacity.");
            }

            _withoutTypes[freeIndex] = type;
        }
    }
}