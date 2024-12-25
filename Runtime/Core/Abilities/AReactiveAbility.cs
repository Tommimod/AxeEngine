using System.Collections.Generic;
using Unity.Burst;

namespace AxeEngine
{
    [BurstCompile]
    public abstract class AReactiveAbility : IAbility
    {
        internal Filter Filter;
        public bool IsEnabled { get; set; }
        protected readonly World ShaderWorld;

        protected AReactiveAbility(World world)
        {
            ShaderWorld = world;
        }

        public abstract Filter FilterBy();
        public abstract bool IsCanExecute(IActor actor);
        public abstract void Execute(HashSet<IActor> actors);
    }
}