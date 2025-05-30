using System;
using System.Collections.Generic;
using Unity.IL2CPP.CompilerServices;

namespace AxeEngine
{
    [Il2CppSetOption(Option.NullChecks, false)]
    [Il2CppSetOption(Option.DivideByZeroChecks, false)]
    public class Filter
    {
        private FilterOption _options;
        private readonly World _world;
        private readonly HashSet<IActor> _actors = new();
        private IActor[] _bufferArray = Array.Empty<IActor>();

        private bool _isBuild;

        internal Filter(World world)
        {
            _world = world;
        }

        internal ref FilterOption GetOptions()
        {
            return ref _options;
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
            var isValid = _options.IsValid(actor);
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
        public IActor[] GetCopy()
        {
            if (_bufferArray.Length != _actors.Count)
            {
                Array.Resize(ref _bufferArray, _actors.Count);
            }

            _actors.CopyTo(_bufferArray);
            return _bufferArray;
        }

        /// <summary>
        /// Apply FilterOption to filter. If filter already build return itself without changes
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        internal Filter Build(ref FilterOption options)
        {
            if (_isBuild)
            {
                return this;
            }

            _options = options;
            var allActors = _world.GetAllActors();
            foreach (var actor in allActors)
            {
                var isValid = _options.IsValid(actor);
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