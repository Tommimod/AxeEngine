using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Burst;
using UnityEngine;

namespace AxeEngine.Editor
{
    [BurstCompile]
    public class AxeEditorActor : MonoBehaviour
    {
        public Action<Type> OnPropertyChanged;

        [SerializeReference]
        public List<object> Properties = new();

        private IActor _actor;

        private void Awake()
        {
            _actor = WorldBridge.Shared.CreateActor();
            foreach (var obj in Properties)
            {
                _actor.RestorePropFromObject(obj);
            }

            if (!_actor.HasProp<UnityObject>())
            {
                var obj = new UnityObject { Value = gameObject };
                _actor.AddProp(ref obj);
                Properties.Add(obj);
            }

            _actor.OnPropertyAdded += OnActorChanged;
            _actor.OnPropertyRemoved += OnPropertyRemoved;
            _actor.OnPropertyReplaced += OnActorChanged;
        }

        private void OnDestroy()
        {
            _actor.OnPropertyAdded -= OnActorChanged;
            _actor.OnPropertyRemoved -= OnPropertyRemoved;
            _actor.OnPropertyReplaced -= OnActorChanged;
        }

        private void OnActorChanged(IActor actor, Type type)
        {
            Properties.Add(_actor.GetPropObject(type));
            OnPropertyChanged?.Invoke(type);
        }

        private void OnPropertyRemoved(IActor actor, Type type)
        {
             var obj = Properties.FirstOrDefault(x => x.GetType() == type);
             if (obj == null)
             {
                 return;
             }

             Properties.Remove(obj);
             OnPropertyChanged?.Invoke(type);
        }

        public void Validate()
        {
            OnValidate();
        }

        protected virtual void OnValidate()
        {
            // Remove components that have become null due to serialization issues or other reason
            if (Properties != null)
            {
                for (var i = Properties.Count - 1; i >= 0; i--)
                {
                    if (Properties[i] == null)
                    {
                        Properties.RemoveAt(i);
                    }
                }
            }
        }
    }
}