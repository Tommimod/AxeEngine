using System;
using System.Collections.Generic;
using Unity.Burst;
using Unity.IL2CPP.CompilerServices;

namespace AxeEngine
{
    [BurstCompile]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public struct FilterOption
    {
        private readonly string _name;
        private HashSet<Type> _withTypes;
        private HashSet<Type> _withAnyTypes;
        private HashSet<Type> _withoutTypes;

        private void Initialize()
        {
            if (_withTypes == null)
            {
                _withTypes = new HashSet<Type>();
                _withAnyTypes = new HashSet<Type>();
                _withoutTypes = new HashSet<Type>();
            }
        }

        public bool IsEquals(FilterOption other)
        {
            Initialize();
            var otherWithTypes = other._withTypes;
            var copyWithTypes = new HashSet<Type>(_withTypes);
            copyWithTypes.ExceptWith(otherWithTypes);
            if (_withTypes.Count != otherWithTypes.Count || copyWithTypes.Count > 0) return false;

            var otherWithAnyTypes = other._withAnyTypes;
            var copyWithAnyTypes = new HashSet<Type>(_withAnyTypes);
            copyWithAnyTypes.ExceptWith(otherWithAnyTypes);
            if (_withAnyTypes.Count != otherWithAnyTypes.Count || copyWithAnyTypes.Count > 0) return false;

            var otherWithoutTypes = other._withoutTypes;
            var copyWithoutTypes = new HashSet<Type>(_withoutTypes);
            copyWithoutTypes.ExceptWith(otherWithoutTypes);
            if (_withoutTypes.Count != otherWithoutTypes.Count || copyWithoutTypes.Count > 0) return false;

            return true;
        }

        public bool IsValid(IActor actor)
        {
            Initialize();
            var properties = actor.GetAllProperties();
            var types = new Type[properties.Count];
            var i = 0;
            foreach (var property in properties)
            {
                types[i++] = property;
            }

            bool needSkip = false;
            foreach (var withType in _withTypes)
            {
                if (Array.IndexOf(types, withType) == -1)
                {
                    needSkip = true;
                    break;
                }
            }

            if (needSkip) return false;
            needSkip = _withAnyTypes.Count > 0;
            foreach (var withAnyType in _withAnyTypes)
            {
                if (Array.IndexOf(types, withAnyType) > -1)
                {
                    needSkip = false;
                    break;
                }
            }

            if (needSkip) return false;
            foreach (var withoutType in _withoutTypes)
            {
                if (Array.IndexOf(types, withoutType) > -1)
                {
                    needSkip = true;
                    break;
                }
            }

            return !needSkip;
        }

        public FilterOption With<T>() where T : struct
        {
            Initialize();
            var type = typeof(T);
            _withTypes.Add(type);

            return this;
        }

        public FilterOption With<T, T1>() where T : struct
        {
            Initialize();
            _withTypes.Add(typeof(T));
            _withTypes.Add(typeof(T1));

            return this;
        }

        public FilterOption With<T, T1, T2>() where T : struct
        {
            Initialize();
            _withTypes.Add(typeof(T));
            _withTypes.Add(typeof(T1));
            _withTypes.Add(typeof(T2));

            return this;
        }

        public FilterOption With<T, T1, T2, T3>() where T : struct
        {
            Initialize();
            _withTypes.Add(typeof(T));
            _withTypes.Add(typeof(T1));
            _withTypes.Add(typeof(T2));
            _withTypes.Add(typeof(T3));

            return this;
        }

        public FilterOption With<T, T1, T2, T3, T4>() where T : struct
        {
            Initialize();
            _withTypes.Add(typeof(T));
            _withTypes.Add(typeof(T1));
            _withTypes.Add(typeof(T2));
            _withTypes.Add(typeof(T3));
            _withTypes.Add(typeof(T4));

            return this;
        }

        public FilterOption With<T, T1, T2, T3, T4, T5>() where T : struct
        {
            Initialize();
            _withTypes.Add(typeof(T));
            _withTypes.Add(typeof(T1));
            _withTypes.Add(typeof(T2));
            _withTypes.Add(typeof(T3));
            _withTypes.Add(typeof(T4));
            _withTypes.Add(typeof(T5));

            return this;
        }

        public FilterOption With<T, T1, T2, T3, T4, T5, T6>() where T : struct
        {
            Initialize();
            _withTypes.Add(typeof(T));
            _withTypes.Add(typeof(T1));
            _withTypes.Add(typeof(T2));
            _withTypes.Add(typeof(T3));
            _withTypes.Add(typeof(T4));
            _withTypes.Add(typeof(T5));
            _withTypes.Add(typeof(T6));

            return this;
        }

        public FilterOption With<T, T1, T2, T3, T4, T5, T6, T7>() where T : struct
        {
            Initialize();
            _withTypes.Add(typeof(T));
            _withTypes.Add(typeof(T1));
            _withTypes.Add(typeof(T2));
            _withTypes.Add(typeof(T3));
            _withTypes.Add(typeof(T4));
            _withTypes.Add(typeof(T5));
            _withTypes.Add(typeof(T6));
            _withTypes.Add(typeof(T7));

            return this;
        }

        public FilterOption WithAny<T>() where T : struct
        {
            Initialize();
            var type = typeof(T);
            _withAnyTypes.Add(type);

            return this;
        }

        public FilterOption WithAny<T, T1>() where T : struct
        {
            Initialize();
            _withAnyTypes.Add(typeof(T));
            _withAnyTypes.Add(typeof(T1));

            return this;
        }

        public FilterOption WithAny<T, T1, T2>() where T : struct
        {
            Initialize();
            _withAnyTypes.Add(typeof(T));
            _withAnyTypes.Add(typeof(T1));
            _withAnyTypes.Add(typeof(T2));

            return this;
        }

        public FilterOption WithAny<T, T1, T2, T3>() where T : struct
        {
            Initialize();
            _withAnyTypes.Add(typeof(T));
            _withAnyTypes.Add(typeof(T1));
            _withAnyTypes.Add(typeof(T2));
            _withAnyTypes.Add(typeof(T3));

            return this;
        }

        public FilterOption WithAny<T, T1, T2, T3, T4>() where T : struct
        {
            Initialize();
            _withAnyTypes.Add(typeof(T));
            _withAnyTypes.Add(typeof(T1));
            _withAnyTypes.Add(typeof(T2));
            _withAnyTypes.Add(typeof(T3));
            _withAnyTypes.Add(typeof(T4));

            return this;
        }

        public FilterOption WithAny<T, T1, T2, T3, T4, T5>() where T : struct
        {
            Initialize();
            _withAnyTypes.Add(typeof(T));
            _withAnyTypes.Add(typeof(T1));
            _withAnyTypes.Add(typeof(T2));
            _withAnyTypes.Add(typeof(T3));
            _withAnyTypes.Add(typeof(T4));
            _withAnyTypes.Add(typeof(T5));

            return this;
        }

        public FilterOption WithAny<T, T1, T2, T3, T4, T5, T6>() where T : struct
        {
            Initialize();
            _withAnyTypes.Add(typeof(T));
            _withAnyTypes.Add(typeof(T1));
            _withAnyTypes.Add(typeof(T2));
            _withAnyTypes.Add(typeof(T3));
            _withAnyTypes.Add(typeof(T4));
            _withAnyTypes.Add(typeof(T5));
            _withAnyTypes.Add(typeof(T6));

            return this;
        }

        public FilterOption WithAny<T, T1, T2, T3, T4, T5, T6, T7>() where T : struct
        {
            Initialize();
            _withAnyTypes.Add(typeof(T));
            _withAnyTypes.Add(typeof(T1));
            _withAnyTypes.Add(typeof(T2));
            _withAnyTypes.Add(typeof(T3));
            _withAnyTypes.Add(typeof(T4));
            _withAnyTypes.Add(typeof(T5));
            _withAnyTypes.Add(typeof(T6));
            _withAnyTypes.Add(typeof(T7));

            return this;
        }

        public FilterOption Without<T>() where T : struct
        {
            Initialize();
            var type = typeof(T);
            _withoutTypes.Add(type);

            return this;
        }

        public FilterOption Without<T, T1>() where T : struct
        {
            Initialize();
            _withoutTypes.Add(typeof(T));
            _withoutTypes.Add(typeof(T1));

            return this;
        }

        public FilterOption Without<T, T1, T2>() where T : struct
        {
            Initialize();
            _withoutTypes.Add(typeof(T));
            _withoutTypes.Add(typeof(T1));
            _withoutTypes.Add(typeof(T2));

            return this;
        }

        public FilterOption Without<T, T1, T2, T3>() where T : struct
        {
            Initialize();
            _withoutTypes.Add(typeof(T));
            _withoutTypes.Add(typeof(T1));
            _withoutTypes.Add(typeof(T2));
            _withoutTypes.Add(typeof(T3));

            return this;
        }

        public FilterOption Without<T, T1, T2, T3, T4>() where T : struct
        {
            Initialize();
            _withoutTypes.Add(typeof(T));
            _withoutTypes.Add(typeof(T1));
            _withoutTypes.Add(typeof(T2));
            _withoutTypes.Add(typeof(T3));
            _withoutTypes.Add(typeof(T4));

            return this;
        }

        public FilterOption Without<T, T1, T2, T3, T4, T5>() where T : struct
        {
            Initialize();
            _withoutTypes.Add(typeof(T));
            _withoutTypes.Add(typeof(T1));
            _withoutTypes.Add(typeof(T2));
            _withoutTypes.Add(typeof(T3));
            _withoutTypes.Add(typeof(T4));
            _withoutTypes.Add(typeof(T5));

            return this;
        }

        public FilterOption Without<T, T1, T2, T3, T4, T5, T6>() where T : struct
        {
            Initialize();
            _withoutTypes.Add(typeof(T));
            _withoutTypes.Add(typeof(T1));
            _withoutTypes.Add(typeof(T2));
            _withoutTypes.Add(typeof(T3));
            _withoutTypes.Add(typeof(T4));
            _withoutTypes.Add(typeof(T5));
            _withoutTypes.Add(typeof(T6));

            return this;
        }

        public FilterOption Without<T, T1, T2, T3, T4, T5, T6, T7>() where T : struct
        {
            Initialize();
            _withoutTypes.Add(typeof(T));
            _withoutTypes.Add(typeof(T1));
            _withoutTypes.Add(typeof(T2));
            _withoutTypes.Add(typeof(T3));
            _withoutTypes.Add(typeof(T4));
            _withoutTypes.Add(typeof(T5));
            _withoutTypes.Add(typeof(T6));
            _withoutTypes.Add(typeof(T7));

            return this;
        }
    }
}