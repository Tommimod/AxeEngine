namespace AxeEngine
{
    internal struct TemporaryPropertyLifeData
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
    }
}