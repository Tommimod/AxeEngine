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

# Actors History
Open Window -> Actor History
Use filters to find needed event. Click on event to get details in console.
