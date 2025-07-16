using System;
using System.Collections.Generic;

namespace AxeEngine
{
    public class WorldAbilityManager : IDisposable
    {
        public Action CycleFinished { get; set; }

        private readonly World _world;
        private readonly List<IAbility> _abilities = new();
        private readonly List<IInitializeAbility> _initializeAbilities = new();
        private readonly List<IUpdateAbility> _updateAbilities = new();
        private readonly List<IFixedUpdateAbility> _fixedUpdateAbilities = new();
        private readonly List<ITearDownAbility> _tearDownAbilities = new();
        private readonly List<AReactiveAbility> _reactiveAbilities = new();

        public WorldAbilityManager(World world)
        {
            _world = world;
        }

        public void AddAbility(IAbility ability)
        {
            _abilities.Add(ability);
            ability.IsEnabled = true;
            if (ability is IInitializeAbility initializeAbility)
            {
                _initializeAbilities.Add(initializeAbility);
            }

            if (ability is IUpdateAbility iUpdateAbility)
            {
                _updateAbilities.Add(iUpdateAbility);
            }

            if (ability is IFixedUpdateAbility iFixedUpdateAbility)
            {
                _fixedUpdateAbilities.Add(iFixedUpdateAbility);
            }

            if (ability is ITearDownAbility iTearDownAbility)
            {
                _tearDownAbilities.Add(iTearDownAbility);
            }

            if (ability is AReactiveAbility reactiveAbility)
            {
                _reactiveAbilities.Add(reactiveAbility);
                reactiveAbility.Trigger = reactiveAbility.TriggerBy();
                _world.AddTrigger(reactiveAbility.Trigger);
            }
        }

        public void RemoveAbility(IAbility ability)
        {
            if (!_abilities.Contains(ability))
            {
                return;
            }

            _abilities.Remove(ability);

            if (ability is IInitializeAbility initializeAbility)
            {
                _initializeAbilities.Remove(initializeAbility);
            }

            if (ability is IUpdateAbility iUpdateAbility)
            {
                _updateAbilities.Remove(iUpdateAbility);
            }

            if (ability is IFixedUpdateAbility iFixedUpdateAbility)
            {
                _fixedUpdateAbilities.Remove(iFixedUpdateAbility);
            }

            if (ability is ITearDownAbility iTearDownAbility)
            {
                _tearDownAbilities.Remove(iTearDownAbility);
            }

            if (ability is AReactiveAbility reactiveAbility)
            {
                _reactiveAbilities.Remove(reactiveAbility);
            }
        }

        public void RemoveAbility(Type abilityType)
        {
            _abilities.RemoveAll(x => x.GetType() == abilityType);
            _initializeAbilities.RemoveAll(x => x.GetType() == abilityType);
            _updateAbilities.RemoveAll(x => x.GetType() == abilityType);
            _fixedUpdateAbilities.RemoveAll(x => x.GetType() == abilityType);
            _tearDownAbilities.RemoveAll(x => x.GetType() == abilityType);
            _reactiveAbilities.RemoveAll(x => x.GetType() == abilityType);
        }

        public void SetActiveAbility(Type aType, bool isActive)
        {
            foreach (var aAbility in _abilities)
            {
                if (aAbility.GetType() == aType)
                {
                    aAbility.IsEnabled = isActive;
                }
            }
        }

        public void PerformInitialization()
        {
            foreach (var ability in _initializeAbilities)
            {
                if (!ability.IsEnabled)
                {
                    continue;
                }

                ability.Initialize(_world);
            }
        }

        public void PerformUpdate()
        {
            foreach (var ability in _updateAbilities)
            {
                if (!ability.IsEnabled)
                {
                    continue;
                }

                ability.Update(_world);
            }

            foreach (var reactiveAbility in _reactiveAbilities)
            {
                if (!reactiveAbility.IsEnabled)
                {
                    continue;
                }

                var actors = reactiveAbility.Trigger.GetValidActors(reactiveAbility.IsCanExecute);
                if (actors.Count == 0)
                {
                    continue;
                }

                reactiveAbility.Execute(actors);
            }
        }

        public void PerformFixedUpdate()
        {
            foreach (var ability in _fixedUpdateAbilities)
            {
                if (!ability.IsEnabled)
                {
                    continue;
                }

                ability.FixedUpdate(_world);
            }
        }

        public void PerformTearDown()
        {
            foreach (var ability in _tearDownAbilities)
            {
                if (!ability.IsEnabled)
                {
                    continue;
                }

                ability.TearDown(_world);
            }

            CycleFinished?.Invoke();
        }

        public void Dispose()
        {
            _abilities.Clear();
            _initializeAbilities.Clear();
            _updateAbilities.Clear();
            _fixedUpdateAbilities.Clear();
            _tearDownAbilities.Clear();
            _reactiveAbilities.Clear();
        }
    }
}