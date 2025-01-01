using System.Collections.Generic;

namespace AxeEngine
{
    public class Filter
    {
        public FilterOption Options { get; private set; }

        private readonly World _world;
        private readonly HashSet<IActor> _actors = new();

        private bool _isBuild;

        internal Filter(World world)
        {
            _world = world;
        }

        internal void OnActorReleased(IActor actor)
        {
            if (_actors.Contains(actor))
            {
                _actors.Remove(actor);
            }
        }

        internal void OnActorChanged(IActor actor)
        {
            var isValid = Options.IsValid(actor);
            if (isValid)
            {
                _actors.Add(actor);
            }
            else
            {
                if (_actors.Contains(actor))
                {
                    _actors.Remove(actor);
                }
            }
        }

        public HashSet<IActor> Get()
        {
            return _actors;
        }

        public Filter Build(ref FilterOption options)
        {
            if (_isBuild)
            {
                return this;
            }

            Options = options;
            var allActors = _world.GetAllActors();
            foreach (var actor in allActors)
            {
                var isValid = Options.IsValid(actor);
                if (!isValid)
                {
                    continue;
                }

                _actors.Add(actor);
            }

            _isBuild = true;
            return this;
        }
    }
}