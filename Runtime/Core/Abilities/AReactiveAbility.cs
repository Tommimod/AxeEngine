using System.Collections.Generic;
using Unity.Burst;

namespace AxeEngine
{
    [BurstCompile]
    public abstract class AReactiveAbility : IAbility
    {
        internal Trigger Trigger;
        public bool IsEnabled { get; set; }
        protected readonly World SharedWorld;

        protected AReactiveAbility(World world)
        {
            SharedWorld = world;
        }

        public abstract Trigger TriggerBy();
        public abstract bool IsCanExecute(IActor actor);
        public abstract void Execute(HashSet<IActor> actors);
    }
}