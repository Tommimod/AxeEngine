using System;

namespace AxeEngine
{
    internal struct TemporaryPropertyLifeData : IEquatable<TemporaryPropertyLifeData>
    {
        public readonly IActor Actor;
        public readonly object PropertyObject;
        public int LifecycleCount;

        private bool _createdNow;

        public TemporaryPropertyLifeData(IActor actor, object propertyObject, int lifecycleCount)
        {
            Actor = actor;
            PropertyObject = propertyObject;
            _createdNow = true;
            LifecycleCount = lifecycleCount;
        }

        public void Decrement()
        {
            if (_createdNow)
            {
                _createdNow = false;
                Actor.RestorePropFromObject(PropertyObject);
                return;
            }

            LifecycleCount--;
        }

        public bool Equals(TemporaryPropertyLifeData other)
        {
            return Equals(Actor, other.Actor) && Equals(PropertyObject, other.PropertyObject) && LifecycleCount == other.LifecycleCount && _createdNow == other._createdNow;
        }

        public override bool Equals(object obj)
        {
            return obj is TemporaryPropertyLifeData other && Equals(other);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Actor, PropertyObject, LifecycleCount, _createdNow);
        }
    }
}