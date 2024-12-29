# AxeEngine
AxeEngine - modern ECS-framework for Unity.  
Not ECS, but APA. **Actor-Property-Ability**

# Installation
1. Download source code
2. Double click by AxeEnginePackage.unitypackage  
*Download in package manager from git URL is not supported, because package can't see Assembly-Charp, which is required for reflections*

# Features
- support any structs as property
- no codegen
- integration with Unity (setup property in inspector, actors history)

# Examples
## Base api
```
    public struct ExampleFloatProperty
    {
        public float Value;
    }

    public struct ExampleProperty
    {
    }

    public void Initialize()
    {
        var world = WorldBridge.Shared; //get world instance
        var actor = world.CreateActor(); //actor will create from pool

        if (actor.HasProp<ExampleFloatProperty>()) //we can check if actor has property
        {
            return;
        }

        actor.AddProp(new ExampleFloatProperty { Value = 2 }); //add property with value
        actor.AddProp<ExampleProperty>(); //add property without value

        var value = actor.GetProp<ExampleFloatProperty>().Value; //get property

        actor.ReplaceProp(new ExampleFloatProperty { Value = 3 }); //replace property

        actor.RemoveProp<ExampleFloatProperty>(); //remove property
        
        world.DestroyActor(actorByLink); //actor will return to pool
    }
```
## Advanced api
### Working with big structs
```
  public struct ExampleBigProperty //some big struct
    {
        public float W;
        public int I;
        public DateTime D;
        public Action A;
        public Dictionary<int, int> X;
        //and so on
    }

    public void Initialize()
    {
        var world = WorldBridge.Shared; //get world instance
        var actor = world.CreateActor();

        var prop = new ExampleBigProperty();
        actor.AddProp(ref prop); //use ref when working with big structs
        ref var value = ref actor.GetRefProp<ExampleBigProperty>(); //you can get struct by ref
    }
```
### How find actors by properties
```
    public struct ExampleInitialAbility : IInitializeAbility
    {
        public bool IsEnabled { get; set; }

        public void Initialize(World world)
        {
            //FilterOption.Empty - initialized struct with empty arrays
            var filterOptions = FilterOption.Empty.With<Test, FloatTest, BoolTest>(); //With - mandatory properties
            var filter = world.GetFilter(ref filterOptions);

            var filterOptions2 = FilterOption.Empty.With<BoolTest>().WithAny<Test, FloatTest>(); //WithAny - At least one of the properties must be
            filter = world.GetFilter(ref filterOptions);

            var filterOptions3 = FilterOption.Empty.WithAny<Test, FloatTest>().Without<BoolTest>(); //Without - These properties should not be
            filter = world.GetFilter(ref filterOptions3);

            var actors = filter.Get(); //return HashSet<IActor>
        }
    }
```
>[!NOTE]
> Filters are cached in World and fetched by match from FilterOptions. This means you don't need to cache the filters yourself 
## How save link to actor
```
    public struct ActorLink
    {
        public int Id;
    }

    public void Initialize()
    {
        var world = WorldBridge.Shared;
        var actorOne = world.CreateActor();
        var actorTwo = world.CreateActor();

        var actorLink = new ActorLink { Id = actorOne.Id }; // actors have public Id field
        actorTwo.AddProp(actorLink); //save to property

        // do some work

        var actorByLink = world.GetActorById(actorLink.Id); // get actor by link via World API
        world.DestroyActor(actorByLink); //destroy actor for example
    }
```
# Unity Integration
1. On game object add AxeEditorActor component
2. for display properties in inspector add [AxeProperty] attribute. Also add [Serializable] attribute if some fields not visible

![image](https://github.com/user-attachments/assets/b36a951a-8557-4e38-b9db-00c76b8164eb)


# Actors History
Open Window -> Actor History
Use filters to find needed event. Click on event to get details in console.

![image](https://github.com/user-attachments/assets/2c3868da-d4c8-4e72-a90f-7dad6b4ac8f1)
# Abilities
### Initialize ability
```
    [BurstCompile]
    public struct ExampleInitializeAbility: IInitializeAbility
    {
        public bool IsEnabled { get; set; }
        public void Initialize(World world)
        {
            //some code
        }
    }
```
### Update ability
```
    [BurstCompile]
    public struct ExampleUpdateAbility: IUpdateAbility
    {
        public bool IsEnabled { get; set; }
        public void Update(World world)
        {
            //some code
        }
    }
```
### Fixed update ability
```
    [BurstCompile]
    public struct ExampleFixedUpdateAbility: IFixedUpdateAbility
    {
        public bool IsEnabled { get; set; }
        public void FixedUpdate(World world)
        {
            //some code
        }
    }
```
### Tear down ability
```
    [BurstCompile]
    public struct ExampleTearDownAbility: ITearDownAbility
    {
        public bool IsEnabled { get; set; }
        public void TearDown(World world)
        {
            //some code
        }
    }
```
> [!TIP]
> Add [BurstCompile] attribute to all struct abilities for best performance
### Reactive ability
```
    public struct ExampleIntProperty
    {
        public int Value;
    }

    public struct ExampleFloatProperty
    {
        public float Value;
    }

    public struct ExampleBoolProperty
    {
        public bool Value;
    }

    public class ExampleReactiveAbility : AReactiveAbility
    {
        public ExampleReactiveAbility(World world) : base(world)
        {
        }

        public override Trigger TriggerBy()
        {
            return new Trigger().Added<ExampleIntProperty>().Removed<ExampleFloatProperty>().Replaced<ExampleBoolProperty>();
            //As with FilterOptions, you can specify multiple types in a row, separated by commas
            //For the trigger to work, it is enough to perform at least one condition
            //Trigger collect actors in one frame. After WorldAbilityManager.PerformTearDown all collected actors will be removed
        }

        //Additional validation before execution
        public override bool IsCanExecute(IActor actor)
        {
            return actor.HasProp<ExampleIntProperty>() && actor.GetProp<ExampleIntProperty>().Value > 2;
        }

        //HashSet with actors which passed TriggerBy and IsCanExecute
        public override void Execute(HashSet<IActor> actors)
        {
            foreach (var actor in actors)
            {
                // do something
            }
        }
    }
```
### How run abilities
```
    public class ExampleBehaviour : MonoBehaviour
    {
        private void Awake()
        {
            var world = WorldBridge.Shared;
            world.AbilityManager.AddAbility(new ExampleInitialAbility()); //add ability via AbilityManager
            world.AbilityManager.AddAbility(new ExampleReactiveAbility(world));
        }

        private void Start()
        {
            WorldBridge.Shared.AbilityManager.PerformInitialization();
        }

        private void Update()
        {
            WorldBridge.Shared.AbilityManager.PerformUpdate();
        }

        private void FixedUpdate()
        {
            WorldBridge.Shared.AbilityManager.PerformFixedUpdate();
        }

        private void LateUpdate()
        {
            WorldBridge.Shared.AbilityManager.PerformTearDown();
        }

        private void OnDestroy()
        {
            WorldBridge.Shared.AbilityManager.PerformTearDown();
        }
    }
```