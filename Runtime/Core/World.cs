using System;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine.Pool;

namespace AxeEngine
{
    public static class WorldBridge
    {
        public static World Shared => _world ??= new World();
        private static World _world;
    }

    [BurstCompile]
    public class World : IDisposable
    {
        /// <summary>
        /// Manager class for registering abilities
        /// </summary>
        public WorldAbilityManager AbilityManager => _abilityManager;

        public Action<IActor> OnActorCreated { get; set; }
        public Action<IActor> OnActorDestroyed { get; set; }

        private readonly HashSet<IActor> _actors = new();
        private readonly HashSet<Filter> _filters = new();
        private readonly Dictionary<Type, object> _componentStorage = new();
        private readonly WorldAbilityManager _abilityManager;
        private ObjectPool<IActor> _objectPool;
        private int _lastId = 1;

        public World()
        {
            _objectPool = new ObjectPool<IActor>(OnCreateFromPull, OnGetFromPull, OnReleaseToPull);
            _abilityManager = new WorldAbilityManager(this);
        }

        public HashSet<IActor> GetAllActors() => _actors;

        /// <summary>
        /// Get filter by options
        /// </summary>
        /// <param name="filterOption">struct with filter options</param>
        /// <returns></returns>
        public Filter GetFilter(ref FilterOption filterOption)
        {
            foreach (var filter in _filters)
            {
                if (filter.Options.IsEquals(filterOption))
                {
                    return filter;
                }
            }

            var newFilter = new Filter(this);
            _filters.Add(newFilter);
            return newFilter.Build(ref filterOption);
        }

        /// <summary>
        /// Get or create actor from pool
        /// </summary>
        /// <returns></returns>
        public IActor CreateActor()
        {
            return _objectPool.Get();
        }

        /// <summary>
        /// Clear and return actor to pool
        /// </summary>
        /// <param name="actor">actor instance</param>
        public void DestroyActor(IActor actor)
        {
            _objectPool.Release(actor);
        }

        /// <summary>
        /// Return actor by actorId. If actor not found return NULL
        /// </summary>
        /// <param name="id">Actor id from instance</param>
        /// <returns></returns>
        public IActor GetActorById(int id)
        {
            foreach (var actor in _actors)
            {
                if (actor.Id == id)
                {
                    return actor;
                }
            }

            return default;
        }

        internal Chunk<T> GetChunk<T>() where T : struct
        {
            if (!_componentStorage.TryGetValue(typeof(T), out var storage))
            {
                storage = new Chunk<T>();
                _componentStorage[typeof(T)] = storage;
            }

            return (Chunk<T>)storage;
        }

        internal IChunk GetChunkDynamic(Type componentType)
        {
            if (!_componentStorage.TryGetValue(componentType, out var storage))
            {
                var chunkType = typeof(Chunk<>).MakeGenericType(componentType);
                storage = Activator.CreateInstance(chunkType);
                _componentStorage[componentType] = storage;
            }

            return (IChunk)storage;
        }

        private IActor OnCreateFromPull()
        {
            if (_lastId.Equals(int.MaxValue))
            {
                _lastId = 1;
            }

            return new Actor(this, _lastId++);
        }

        private void OnGetFromPull(IActor actor)
        {
            actor.Restore();
            _actors.Add(actor);
            actor.OnPropertyAdded += OnActorAddProperty;
            actor.OnPropertyReplaced += OnActorReplaceProperty;
            actor.OnPropertyRemoved += OnActorRemoveProperty;

            OnActorCreated?.Invoke(actor);
        }

        private void OnReleaseToPull(IActor actor)
        {
            foreach (var filter in _filters)
            {
                filter.OnActorReleased(actor);
            }

            _actors.Remove(actor);
            actor.Release();
            actor.OnPropertyAdded -= OnActorAddProperty;
            actor.OnPropertyReplaced -= OnActorReplaceProperty;
            actor.OnPropertyRemoved -= OnActorRemoveProperty;

            OnActorDestroyed?.Invoke(actor);
        }


        private void OnActorAddProperty(IActor actor, Type actorProperty)
        {
            foreach (var filter in _filters)
            {
                filter.OnActorChanged(actor);
            }
        }

        private void OnActorReplaceProperty(IActor actor, Type actorProperty)
        {
        }

        private void OnActorRemoveProperty(IActor actor, Type actorProperty)
        {
            foreach (var filter in _filters)
            {
                filter.OnActorChanged(actor);
            }
        }

        public void Dispose()
        {
            _objectPool?.Dispose();
            _actors.Clear();
            _filters.Clear();
            _objectPool = null;
        }
    }
}