using System;
using System.Collections.Generic;
using Unity.IL2CPP.CompilerServices;

namespace AxeEngine
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class Trigger
    {
        private readonly HashSet<IActor> _validActors = new();

        private Type[] _shouldBeAddedTypes = new Type[7];
        private Type[] _shouldBeReplacedTypes = new Type[7];
        private Type[] _shouldBeRemovedTypes = new Type[7];

        public bool IsEquals(Trigger other)
        {
            var otherWithTypes = other._shouldBeAddedTypes;
            var copyWithTypes = new HashSet<Type>(_shouldBeAddedTypes);
            copyWithTypes.ExceptWith(otherWithTypes);
            if (_shouldBeAddedTypes.Length != otherWithTypes.Length || copyWithTypes.Count > 0) return false;

            var otherWithAnyTypes = other._shouldBeReplacedTypes;
            var copyWithAnyTypes = new HashSet<Type>(_shouldBeReplacedTypes);
            copyWithAnyTypes.ExceptWith(otherWithAnyTypes);
            if (_shouldBeReplacedTypes.Length != otherWithAnyTypes.Length || copyWithAnyTypes.Count > 0) return false;

            var otherWithoutTypes = other._shouldBeRemovedTypes;
            var copyWithoutTypes = new HashSet<Type>(_shouldBeRemovedTypes);
            copyWithoutTypes.ExceptWith(otherWithoutTypes);
            if (_shouldBeRemovedTypes.Length != otherWithoutTypes.Length || copyWithoutTypes.Count > 0) return false;

            return true;
        }

        internal void ValidateForTrigger(IActor actor, Type type, TriggerAction action)
        {
            if (actor == null)
            {
                return;
            }

            switch (action)
            {
                case TriggerAction.Added:
                {
                    foreach (var shouldBeAddedType in _shouldBeAddedTypes)
                    {
                        if (shouldBeAddedType != type)
                        {
                            continue;
                        }

                        _validActors.Add(actor);
                        return;
                    }

                    break;
                }
                case TriggerAction.Replaced:
                {
                    foreach (var shouldBeReplacedType in _shouldBeReplacedTypes)
                    {
                        if (shouldBeReplacedType != type)
                        {
                            continue;
                        }

                        _validActors.Add(actor);
                        return;
                    }

                    break;
                }
                case TriggerAction.Removed:
                {
                    foreach (var shouldBeRemovedType in _shouldBeRemovedTypes)
                    {
                        if (shouldBeRemovedType != type)
                        {
                            continue;
                        }

                        _validActors.Add(actor);
                        return;
                    }

                    break;
                }
            }
        }

        internal void Clear()
        {
            _validActors.Clear();
        }

        internal HashSet<IActor> GetValidActors() => _validActors;

        public Trigger Added<T>() where T : struct
        {
            var type = typeof(T);
            MarkTypeShouldBeAdded(type);

            return this;
        }

        public Trigger Added<T, T1>() where T : struct
        {
            MarkTypeShouldBeAdded(typeof(T));
            MarkTypeShouldBeAdded(typeof(T1));

            return this;
        }

        public Trigger Added<T, T1, T2>() where T : struct
        {
            MarkTypeShouldBeAdded(typeof(T));
            MarkTypeShouldBeAdded(typeof(T1));
            MarkTypeShouldBeAdded(typeof(T2));

            return this;
        }

        public Trigger Added<T, T1, T2, T3>() where T : struct
        {
            MarkTypeShouldBeAdded(typeof(T));
            MarkTypeShouldBeAdded(typeof(T1));
            MarkTypeShouldBeAdded(typeof(T2));
            MarkTypeShouldBeAdded(typeof(T3));

            return this;
        }

        public Trigger Added<T, T1, T2, T3, T4>() where T : struct
        {
            MarkTypeShouldBeAdded(typeof(T));
            MarkTypeShouldBeAdded(typeof(T1));
            MarkTypeShouldBeAdded(typeof(T2));
            MarkTypeShouldBeAdded(typeof(T3));
            MarkTypeShouldBeAdded(typeof(T4));

            return this;
        }

        public Trigger Added<T, T1, T2, T3, T4, T5>() where T : struct
        {
            MarkTypeShouldBeAdded(typeof(T));
            MarkTypeShouldBeAdded(typeof(T1));
            MarkTypeShouldBeAdded(typeof(T2));
            MarkTypeShouldBeAdded(typeof(T3));
            MarkTypeShouldBeAdded(typeof(T4));
            MarkTypeShouldBeAdded(typeof(T5));

            return this;
        }

        public Trigger Added<T, T1, T2, T3, T4, T5, T6>() where T : struct
        {
            MarkTypeShouldBeAdded(typeof(T));
            MarkTypeShouldBeAdded(typeof(T1));
            MarkTypeShouldBeAdded(typeof(T2));
            MarkTypeShouldBeAdded(typeof(T3));
            MarkTypeShouldBeAdded(typeof(T4));
            MarkTypeShouldBeAdded(typeof(T5));
            MarkTypeShouldBeAdded(typeof(T6));

            return this;
        }

        public Trigger Added<T, T1, T2, T3, T4, T5, T6, T7>() where T : struct
        {
            MarkTypeShouldBeAdded(typeof(T));
            MarkTypeShouldBeAdded(typeof(T1));
            MarkTypeShouldBeAdded(typeof(T2));
            MarkTypeShouldBeAdded(typeof(T3));
            MarkTypeShouldBeAdded(typeof(T4));
            MarkTypeShouldBeAdded(typeof(T5));
            MarkTypeShouldBeAdded(typeof(T6));
            MarkTypeShouldBeAdded(typeof(T7));

            return this;
        }

        public Trigger Replaced<T>() where T : struct
        {
            MarkTypeShouldBeReplaced(typeof(T));

            return this;
        }

        public Trigger Replaced<T, T1>() where T : struct
        {
            MarkTypeShouldBeReplaced(typeof(T));
            MarkTypeShouldBeReplaced(typeof(T1));

            return this;
        }

        public Trigger Replaced<T, T1, T2>() where T : struct
        {
            MarkTypeShouldBeReplaced(typeof(T));
            MarkTypeShouldBeReplaced(typeof(T1));
            MarkTypeShouldBeReplaced(typeof(T2));

            return this;
        }

        public Trigger Replaced<T, T1, T2, T3>() where T : struct
        {
            MarkTypeShouldBeReplaced(typeof(T));
            MarkTypeShouldBeReplaced(typeof(T1));
            MarkTypeShouldBeReplaced(typeof(T2));
            MarkTypeShouldBeReplaced(typeof(T3));

            return this;
        }

        public Trigger Replaced<T, T1, T2, T3, T4>() where T : struct
        {
            MarkTypeShouldBeReplaced(typeof(T));
            MarkTypeShouldBeReplaced(typeof(T1));
            MarkTypeShouldBeReplaced(typeof(T2));
            MarkTypeShouldBeReplaced(typeof(T3));
            MarkTypeShouldBeReplaced(typeof(T4));

            return this;
        }

        public Trigger Replaced<T, T1, T2, T3, T4, T5>() where T : struct
        {
            MarkTypeShouldBeReplaced(typeof(T));
            MarkTypeShouldBeReplaced(typeof(T1));
            MarkTypeShouldBeReplaced(typeof(T2));
            MarkTypeShouldBeReplaced(typeof(T3));
            MarkTypeShouldBeReplaced(typeof(T4));
            MarkTypeShouldBeReplaced(typeof(T5));

            return this;
        }

        public Trigger Replaced<T, T1, T2, T3, T4, T5, T6>() where T : struct
        {
            MarkTypeShouldBeReplaced(typeof(T));
            MarkTypeShouldBeReplaced(typeof(T1));
            MarkTypeShouldBeReplaced(typeof(T2));
            MarkTypeShouldBeReplaced(typeof(T3));
            MarkTypeShouldBeReplaced(typeof(T4));
            MarkTypeShouldBeReplaced(typeof(T5));
            MarkTypeShouldBeReplaced(typeof(T6));

            return this;
        }

        public Trigger Replaced<T, T1, T2, T3, T4, T5, T6, T7>() where T : struct
        {
            MarkTypeShouldBeReplaced(typeof(T));
            MarkTypeShouldBeReplaced(typeof(T1));
            MarkTypeShouldBeReplaced(typeof(T2));
            MarkTypeShouldBeReplaced(typeof(T3));
            MarkTypeShouldBeReplaced(typeof(T4));
            MarkTypeShouldBeReplaced(typeof(T5));
            MarkTypeShouldBeReplaced(typeof(T6));
            MarkTypeShouldBeReplaced(typeof(T7));

            return this;
        }

        public Trigger Removed<T>() where T : struct
        {
            var type = typeof(T);
            MarkTypeShouldBeRemoved(type);

            return this;
        }

        public Trigger Removed<T, T1>() where T : struct
        {
            MarkTypeShouldBeRemoved(typeof(T));
            MarkTypeShouldBeRemoved(typeof(T1));

            return this;
        }

        public Trigger Removed<T, T1, T2>() where T : struct
        {
            MarkTypeShouldBeRemoved(typeof(T));
            MarkTypeShouldBeRemoved(typeof(T1));
            MarkTypeShouldBeRemoved(typeof(T2));

            return this;
        }

        public Trigger Removed<T, T1, T2, T3>() where T : struct
        {
            MarkTypeShouldBeRemoved(typeof(T));
            MarkTypeShouldBeRemoved(typeof(T1));
            MarkTypeShouldBeRemoved(typeof(T2));
            MarkTypeShouldBeRemoved(typeof(T3));

            return this;
        }

        public Trigger Removed<T, T1, T2, T3, T4>() where T : struct
        {
            MarkTypeShouldBeRemoved(typeof(T));
            MarkTypeShouldBeRemoved(typeof(T1));
            MarkTypeShouldBeRemoved(typeof(T2));
            MarkTypeShouldBeRemoved(typeof(T3));
            MarkTypeShouldBeRemoved(typeof(T4));

            return this;
        }

        public Trigger Removed<T, T1, T2, T3, T4, T5>() where T : struct
        {
            MarkTypeShouldBeRemoved(typeof(T));
            MarkTypeShouldBeRemoved(typeof(T1));
            MarkTypeShouldBeRemoved(typeof(T2));
            MarkTypeShouldBeRemoved(typeof(T3));
            MarkTypeShouldBeRemoved(typeof(T4));
            MarkTypeShouldBeRemoved(typeof(T5));

            return this;
        }

        public Trigger Removed<T, T1, T2, T3, T4, T5, T6>() where T : struct
        {
            MarkTypeShouldBeRemoved(typeof(T));
            MarkTypeShouldBeRemoved(typeof(T1));
            MarkTypeShouldBeRemoved(typeof(T2));
            MarkTypeShouldBeRemoved(typeof(T3));
            MarkTypeShouldBeRemoved(typeof(T4));
            MarkTypeShouldBeRemoved(typeof(T5));
            MarkTypeShouldBeRemoved(typeof(T6));

            return this;
        }

        public Trigger Removed<T, T1, T2, T3, T4, T5, T6, T7>() where T : struct
        {
            MarkTypeShouldBeRemoved(typeof(T));
            MarkTypeShouldBeRemoved(typeof(T1));
            MarkTypeShouldBeRemoved(typeof(T2));
            MarkTypeShouldBeRemoved(typeof(T3));
            MarkTypeShouldBeRemoved(typeof(T4));
            MarkTypeShouldBeRemoved(typeof(T5));
            MarkTypeShouldBeRemoved(typeof(T6));
            MarkTypeShouldBeRemoved(typeof(T7));

            return this;
        }

        public Trigger AnyAction<T>() where T : struct
        {
            Added<T>();
            Replaced<T>();
            Removed<T>();

            return this;
        }

        public Trigger AnyAction<T, T1>() where T : struct
        {
            Added<T, T1>();
            Replaced<T, T1>();
            Removed<T, T1>();

            return this;
        }

        public Trigger AnyAction<T, T1, T2>() where T : struct
        {
            Added<T, T1, T2>();
            Replaced<T, T1, T2>();
            Removed<T, T1, T2>();

            return this;
        }

        public Trigger AnyAction<T, T1, T2, T3>() where T : struct
        {
            Added<T, T1, T2, T3>();
            Replaced<T, T1, T2, T3>();
            Removed<T, T1, T2, T3>();

            return this;
        }

        public Trigger AnyAction<T, T1, T2, T3, T4>() where T : struct
        {
            Added<T, T1, T2, T3, T4>();
            Replaced<T, T1, T2, T3, T4>();
            Removed<T, T1, T2, T3, T4>();

            return this;
        }

        public Trigger AnyAction<T, T1, T2, T3, T4, T5>() where T : struct
        {
            Added<T, T1, T2, T3, T4, T5>();
            Replaced<T, T1, T2, T3, T4, T5>();
            Removed<T, T1, T2, T3, T4, T5>();

            return this;
        }

        public Trigger AnyAction<T, T1, T2, T3, T4, T5, T6>() where T : struct
        {
            Added<T, T1, T2, T3, T4, T5, T6>();
            Replaced<T, T1, T2, T3, T4, T5, T6>();
            Removed<T, T1, T2, T3, T4, T5, T6>();

            return this;
        }

        public Trigger AnyAction<T, T1, T2, T3, T4, T5, T6, T7>() where T : struct
        {
            Added<T, T1, T2, T3, T4, T5, T6, T7>();
            Replaced<T, T1, T2, T3, T4, T5, T6, T7>();
            Removed<T, T1, T2, T3, T4, T5, T6, T7>();

            return this;
        }

        private void MarkTypeShouldBeAdded(Type type)
        {
            var freeIndex = Array.IndexOf(_shouldBeAddedTypes, null);
            if (freeIndex == -1)
            {
                Array.Resize(ref _shouldBeAddedTypes, _shouldBeAddedTypes.Length * 2);
                freeIndex = Array.IndexOf(_shouldBeAddedTypes, null);
            }

            _shouldBeAddedTypes[freeIndex] = type;
        }

        private void MarkTypeShouldBeReplaced(Type type)
        {
            var freeIndex = Array.IndexOf(_shouldBeReplacedTypes, null);
            if (freeIndex == -1)
            {
                Array.Resize(ref _shouldBeReplacedTypes, _shouldBeReplacedTypes.Length * 2);
                freeIndex = Array.IndexOf(_shouldBeReplacedTypes, null);
            }

            _shouldBeReplacedTypes[freeIndex] = type;
        }

        private void MarkTypeShouldBeRemoved(Type type)
        {
            var freeIndex = Array.IndexOf(_shouldBeRemovedTypes, null);
            if (freeIndex == -1)
            {
                Array.Resize(ref _shouldBeRemovedTypes, _shouldBeRemovedTypes.Length * 2);
                freeIndex = Array.IndexOf(_shouldBeRemovedTypes, null);
            }

            _shouldBeRemovedTypes[freeIndex] = type;
        }
    }
}