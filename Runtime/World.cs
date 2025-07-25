using System;
using System.Collections.Generic;
using UnityEngine.Pool;

namespace AxeEngine
{
#if AXE_ENGINE_ENABLE_STATIC
    public static class WorldBridge
    {
        public static World Shared => _world ??= new World();
        private static World _world;

        public static void Reset() => _world = new World();
    }
#endif

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
        private readonly HashSet<Trigger> _triggers = new();
        private readonly Dictionary<Type, object> _componentStorage = new();
        private readonly WorldAbilityManager _abilityManager;
        private ObjectPool<IActor> _objectPool;
        private readonly List<TemporaryPropertyLifeData> _temporaryProperties = new();
        private TemporaryPropertyLifeData[] _temporaryPropertiesBuffer = new TemporaryPropertyLifeData[64];
        private int _lastId = 1;

        public World()
        {
            _objectPool = new ObjectPool<IActor>(OnCreateFromPull, OnGetFromPull, OnReleaseToPull);
            _abilityManager = new WorldAbilityManager(this);
            _abilityManager.CycleFinished += AbilitiesCycleFinished;
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
                ref var options = ref filter.GetOptions();
                if (options.IsEquals(filterOption))
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

            return null;
        }

        internal void AddTrigger(Trigger trigger) => _triggers.Add(trigger);

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

        private void AbilitiesCycleFinished()
        {
            ClearTriggers();
            if (_temporaryProperties.Count == 0)
            {
                return;
            }

            if (_temporaryPropertiesBuffer.Length < _temporaryProperties.Count)
            {
                Array.Resize(ref _temporaryPropertiesBuffer, _temporaryProperties.Count);
            }

            var count = _temporaryProperties.Count;
            _temporaryProperties.CopyTo(_temporaryPropertiesBuffer);
            var offset = 0;
            for (var i = 0; i < count; i++)
            {
                if (_temporaryPropertiesBuffer[i].Actor == null)
                {
                    continue;
                }

                _temporaryPropertiesBuffer[i].Decrement();
                _temporaryProperties[i - offset] = _temporaryPropertiesBuffer[i];
                if (_temporaryPropertiesBuffer[i].LifecycleCount > 0)
                {
                    continue;
                }

                offset++;
                _temporaryProperties.Remove(_temporaryPropertiesBuffer[i]);
                _temporaryPropertiesBuffer[i].Actor.RemovePropInternal(_temporaryPropertiesBuffer[i].PropertyObject);
                _temporaryPropertiesBuffer[i] = default;
            }
        }

        private void ClearTriggers()
        {
            foreach (var trigger in _triggers)
            {
                trigger.Clear();
            }
        }

        private IActor OnCreateFromPull()
        {
            if (_lastId.Equals(int.MaxValue))
            {
                _lastId = 1;
            }

            return new Actor(this, _lastId++);
        }

        private void OnActorAddTemporaryProperty(IActor actor, object actorProperty, int lifecyclesCount)
        {
            _temporaryProperties.Add(new TemporaryPropertyLifeData(actor, actorProperty, lifecyclesCount));
        }

        private void OnGetFromPull(IActor actor)
        {
            actor.Restore();
            _actors.Add(actor);
            actor.OnPropertyAdded += OnActorAddProperty;
            actor.OnPropertyReplaced += OnActorReplaceProperty;
            actor.OnPropertyRemoved += OnActorRemoveProperty;
            actor.OnTemporaryPropertyAdded += OnActorAddTemporaryProperty;

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
            actor.OnTemporaryPropertyAdded -= OnActorAddTemporaryProperty;

            OnActorDestroyed?.Invoke(actor);
        }


        private void OnActorAddProperty(IActor actor, Type actorProperty)
        {
            foreach (var filter in _filters)
            {
                filter.OnActorChanged(actor);
            }

            foreach (var trigger in _triggers)
            {
                trigger.ValidateForTrigger(actor, actorProperty, TriggerAction.Added);
            }
        }

        private void OnActorReplaceProperty(IActor actor, Type actorProperty)
        {
            foreach (var trigger in _triggers)
            {
                trigger.ValidateForTrigger(actor, actorProperty, TriggerAction.Replaced);
            }
        }

        private void OnActorRemoveProperty(IActor actor, Type actorProperty)
        {
            foreach (var filter in _filters)
            {
                filter.OnActorChanged(actor);
            }

            foreach (var trigger in _triggers)
            {
                trigger.ValidateForTrigger(actor, actorProperty, TriggerAction.Removed);
            }
        }

        public void Dispose()
        {
            _abilityManager.CycleFinished -= AbilitiesCycleFinished;
            _abilityManager?.Dispose();
            _objectPool?.Dispose();
            _actors.Clear();
            _filters.Clear();
            _objectPool = null;
        }
    }
}