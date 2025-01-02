using System.Collections.Generic;

namespace AxeEngine
{
    public class Filter
    {
        /// <summary>
        /// Applied filter options
        /// </summary>
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

        /// <summary>
        /// Return collected actors by references
        /// </summary>
        /// <returns></returns>
        public HashSet<IActor> Get()
        {
            return _actors;
        }

        /// <summary>
        /// Return collected actors by values
        /// </summary>
        /// <returns></returns>
        public IEnumerable<IActor> GetCopy()
        {
            var array = new IActor[_actors.Count];
            _actors.CopyTo(array);
            return array;
        }

        /// <summary>
        /// Apply FilterOption to filter. If filter already build return itself without changes
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
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