using System;
using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

namespace AxeEngine.Editor
{
    [BurstCompile]
    public class AxeEditorActor : MonoBehaviour
    {
        #if AXE_ENGINE_ENABLE_STATIC
        public int ActorId => _actor.Id;
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

            _actor.OnPropertyAdded += OnActorChanged;
            _actor.OnPropertyRemoved += OnPropertyRemoved;
            _actor.OnPropertyReplaced += OnActorChanged;

            if (!_actor.HasProp<UnityObject>())
            {
                var obj = new UnityObject { Value = gameObject };
                _actor.AddProp(ref obj);
            }
        }

        private void OnDestroy()
        {
            _actor.OnPropertyAdded -= OnActorChanged;
            _actor.OnPropertyRemoved -= OnPropertyRemoved;
            _actor.OnPropertyReplaced -= OnActorChanged;
        }

        private void OnActorChanged(IActor actor, Type type)
        {
            Properties.RemoveAll(x => x.GetType() == type);
            Properties.Add(_actor.GetPropObject(type));
            OnPropertyChanged?.Invoke(type);
        }

        private void OnPropertyRemoved(IActor actor, Type type)
        {
            Properties.RemoveAll(x => x.GetType() == type);
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
#endif
    }
}