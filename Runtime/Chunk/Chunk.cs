using System;
using Unity.IL2CPP.CompilerServices;
using UnityEngine;

namespace AxeEngine
{
    [Serializable]
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    internal class Chunk<T> : IChunk where T : struct
    {
        private readonly T[] _defaultProperties = new T[1];
        private T[] _properties;
        private int[] _actorToIndex;

        public Chunk()
        {
            _defaultProperties[0] = default;
            _properties = new T[128];
            _actorToIndex = new int[128];
            Array.Fill(_actorToIndex, -1);
        }

        public void Add(int actorId, ref T property)
        {
            if (_actorToIndex.Length <= actorId) Resize(actorId * 2);

            var index = Array.IndexOf(_actorToIndex, -1);
            _properties[index] = property;
            _actorToIndex[index] = actorId;
        }

        public void Remove(int actorId)
        {
            var index = Array.IndexOf(_actorToIndex, actorId);
            if (index == -1) return;

            _properties[index] = default;
            _actorToIndex[index] = -1;
        }

        public void Add(int actorId, object property)
        {
            if (property is T typedComponent)
            {
                Add(actorId, ref typedComponent);
            }
            else
            {
                throw new InvalidCastException($"Invalid property type: {property.GetType()}");
            }
        }

        object IChunk.Get(int actorId)
        {
            return Get(actorId);
        }

        public ref T Get(int actorId)
        {
            var index = Array.IndexOf(_actorToIndex, actorId);
            if (index == -1)
            {
                Debug.LogError(new NullReferenceException($"Property {typeof(T).Name} not found for entity {actorId}"));
                return ref _defaultProperties[0];
            }

            return ref _properties[index];
        }

        public bool Has(int actorId) => Array.IndexOf(_actorToIndex, actorId) != -1;

        private void Resize(int newSize)
        {
            var size = _properties.Length;
            Array.Resize(ref _properties, newSize);
            Array.Resize(ref _actorToIndex, newSize);
            Array.Fill(_actorToIndex, -1, size, newSize - size);
        }
    }
}