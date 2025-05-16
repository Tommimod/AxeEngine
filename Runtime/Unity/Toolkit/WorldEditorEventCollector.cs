#if UNITY_EDITOR && AXE_ENGINE_ENABLE_STATIC
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using JetBrains.Annotations;
using UnityEditor;
using UnityEngine;

namespace AxeEngine.Editor
{
    [InitializeOnLoad]
    public class WorldEditorEventCollector
    {
        public static event Action<ActorEvent> OnActorEvent;
        public static IEnumerable<ActorEvent> ActorEvents => _actorEvents;

        private static WorldEditorEventCollector _instance;
        private static StringBuilder _stringBuilder;
        private static readonly List<ActorEvent> _actorEvents = new();

        static WorldEditorEventCollector()
        {
            EditorApplication.playModeStateChanged += ModeStateChanged;
        }

        private static void ModeStateChanged(PlayModeStateChange state)
        {
            switch (state)
            {
                case PlayModeStateChange.EnteredPlayMode:
                    Dispose();
                    Initialize();
                    break;
            }
        }

        private static void Initialize()
        {
            _stringBuilder = new StringBuilder();
            var world = WorldBridge.Shared;
            var actors = world.GetAllActors();
            foreach (var actor in actors)
            {
                OnEntityCreated(actor);
            }

            world.OnActorCreated += OnEntityCreated;
            world.OnActorDestroyed += OnEntityDestroyed;

            OnActorEvent += SaveEvent;
        }

        private static void Dispose()
        {
            var world = WorldBridge.Shared;
            world.OnActorCreated -= OnEntityCreated;
            world.OnActorDestroyed -= OnEntityDestroyed;

            OnActorEvent -= SaveEvent;
            _actorEvents.Clear();
        }

        private static void SaveEvent(ActorEvent data)
        {
            if (_actorEvents.Count + 1 >= int.MaxValue)
            {
                _actorEvents.RemoveAt(0);
            }

            _actorEvents.Add(data);
        }

        private static void OnEntityCreated(IActor actor)
        {
            actor.OnPropertyAdded += OnPropertyAdded;
            actor.OnPropertyRemoved += OnPropertyRemoved;
            actor.OnPropertyReplaced += OnPropertyReplaced;
        }

        private static void OnEntityDestroyed(IActor actor)
        {
            actor.OnPropertyAdded -= OnPropertyAdded;
            actor.OnPropertyRemoved -= OnPropertyRemoved;
            actor.OnPropertyReplaced -= OnPropertyReplaced;
        }

        private static void OnPropertyAdded(IActor actor, Type type)
        {
            var id = actor.Id;
            var gameObject = TryGetGameObject(actor);
            var gameObjectName = gameObject != null ? $"{gameObject.name}({id})" : id.ToString();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                object value = null;
                try
                {
                    value = field.GetValue(type);
                }
                catch
                {
                    value = "Cannot get value";
                }

                _stringBuilder.AppendFormat("{0} = {1}\n", field.Name, value);
            }

            OnActorEvent?.Invoke(new ActorEvent(ActorEvent.Type.Added, actor.Id, GetNameAfterLastDot(type.Name), _stringBuilder.ToString(), gameObjectName,
                DateTime.Now, Environment.StackTrace));
            _stringBuilder.Clear();
        }

        private static void OnPropertyReplaced(IActor actor, Type type)
        {
            var id = actor.Id;
            var gameObject = TryGetGameObject(actor);
            var gameObjectName = gameObject != null ? $"{gameObject.name}({id})" : id.ToString();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                object value = null;
                try
                {
                    value = field.GetValue(type);
                }
                catch
                {
                    value = "Cannot get value";
                }

                _stringBuilder.AppendFormat("{0} = {1}\n", field.Name, value);
            }

            OnActorEvent?.Invoke(new ActorEvent(ActorEvent.Type.Replaced, actor.Id, GetNameAfterLastDot(type.Name), _stringBuilder.ToString(), gameObjectName,
                DateTime.Now, Environment.StackTrace));
            _stringBuilder.Clear();
        }

        private static void OnPropertyRemoved(IActor actor, Type type)
        {
            var id = actor.Id;
            var gameObject = TryGetGameObject(actor);
            var gameObjectName = gameObject != null ? $"{gameObject.name}({id})" : id.ToString();
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                object value = null;
                try
                {
                    value = field.GetValue(type);
                }
                catch
                {
                    value = "Cannot get value";
                }

                _stringBuilder.AppendFormat("{0} = {1}\n", field.Name, value);
            }

            OnActorEvent?.Invoke(new ActorEvent(ActorEvent.Type.Removed, actor.Id, GetNameAfterLastDot(type.Name), _stringBuilder.ToString(), gameObjectName,
                DateTime.Now, Environment.StackTrace));
            _stringBuilder.Clear();
        }

        [CanBeNull]
        private static GameObject TryGetGameObject(IActor actor)
        {
            return actor.HasProp<UnityObject>() ? actor.GetProp<UnityObject>().Value : null;
        }

        private static string GetNameAfterLastDot(string nameOfType)
        {
            return nameOfType.Split('.').Last();
        }
    }
}
#endif