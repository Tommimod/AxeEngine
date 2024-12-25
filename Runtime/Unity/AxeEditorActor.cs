using System.Collections.Generic;
using Unity.Burst;
using UnityEngine;

namespace AxeEngine.Editor
{
    [BurstCompile]
    public class AxeEditorActor : MonoBehaviour
    {
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