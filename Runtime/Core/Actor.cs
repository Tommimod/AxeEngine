using System;
using System.Collections.Generic;
using Unity.Burst;

namespace AxeEngine
{
    [BurstCompile]
    public class Actor : IActor
    {
        public bool IsAlive { get; private set; }
        public int Id { get; }
        public Action<IActor, Type> OnPropertyAdded { get; set; }
        public Action<IActor, Type> OnPropertyReplaced { get; set; }
        public Action<IActor, Type> OnPropertyRemoved { get; set; }

        IReadOnlyList<Type> IActor.GetAllProperties() => _properties;

        internal readonly World _world;
        private readonly List<Type> _properties = new();

        internal Actor(World world, int id)
        {
            _world = world;
            Id = id;
        }

        void IActor.Restore()
        {
            IsAlive = true;
        }

        public bool HasProp<T>() where T : struct => _world.GetChunk<T>().Has(Id);
        public ref T GetProp<T>() where T : struct => ref _world.GetChunk<T>().Get(Id);

        public IActor SetPropertyEnabled<T>(bool isEnabled) where T : struct
        {
            if (isEnabled)
            {
                if (!HasProp<T>())
                {
                    AddProp<T>();
                }
            }
            else
            {
                if (HasProp<T>())
                {
                    RemoveProp<T>();
                }
            }

            return this;
        }

        public IActor AddProp<T>(ref T property) where T : struct
        {
            if (HasProp<T>())
            {
                return ReplaceProp(ref property);
            }

            AddPropInternal(ref property);
            OnPropertyAdded.Invoke(this, typeof(T));
            return this;
        }

        public IActor AddProp<T>(T property = default) where T : struct
        {
            return AddProp(ref property);
        }

        public IActor RestorePropFromObject(object property)
        {
            var type = property.GetType();
            _properties.Add(property.GetType());
            _world.GetChunkDynamic(type).Add(Id, property);
            OnPropertyAdded?.Invoke(this, type);
            return this;
        }

        public IActor ReplaceProp<T>(ref T property) where T : struct
        {
            if (HasProp<T>())
            {
                RemovePropInternal<T>();
            }

            AddPropInternal(ref property);
            OnPropertyReplaced.Invoke(this, typeof(T));
            return this;
        }

        public IActor ReplaceProp<T>(T property) where T : struct
        {
            return ReplaceProp(ref property);
        }

        public IActor RemoveProp<T>() where T : struct
        {
            RemovePropInternal<T>();
            OnPropertyRemoved.Invoke(this, typeof(T));
            return this;
        }

        public void Release()
        {
            _properties.Clear();
            IsAlive = false;
        }

        private void AddPropInternal<T>(ref T property) where T : struct
        {
            _properties.Add(typeof(T));
            _world.GetChunk<T>().Add(Id, ref property);
        }

        private void RemovePropInternal<T>() where T : struct
        {
            _properties.Remove(typeof(T));
            _world.GetChunk<T>().Remove(Id);
        }
    }
}