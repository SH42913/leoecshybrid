# Leopotam ECS Hybrid
This is the Unity Hybrid extension to [Leopotam Entity Component System Framework](https://github.com/Leopotam/ecs).\
Hybrid ECS let you to connect scenes and prefabs of Unity with ECS in easy way. It's working like `Unity.Entities.Hybrid`.

You need to install [LeoECS](https://github.com/Leopotam/ecs) and [LeoECS Unity Integration](https://github.com/Leopotam/ecs-unityintegration) to proper work.

## Installation into Unity project
1. Read the instructions from the official Unity documentation: https://docs.unity3d.com/Manual/upm-dependencies.html#Git
2. Open up *manifest.json* inside the *Packages* directory in your Unity project using a text editor.
3. Under the dependencies section of this file, you should add the following line at the top: ```"com.leopotam.ecs-hybrid": "https://github.com/SH42913/leoecshybrid.git",```
4. It should be like this:
```
{
  "dependencies": {
    "com.leopotam.ecs-hybrid": "https://github.com/SH42913/leoecshybrid.git",
    ...
  }
}
```
5. Success! Start up Unity with your Unity project and you will see Leo ECS Hybrid in the Unity Package Manager.
6. If you want to update package to latest version, you should to remove *"lock"* section in *manifest.json*
7. Since Unity 2019.3 you can also add the package in Package Manager UI in Unity using the link to repo.

## Content
- `HybridEntity` to represent Entity on scene/prefab.
- ECS One Frame Component `NewHybridEntityEvent` to let you know about any new `HybridEntity`.
- `ComponentWrapper` to add/remove ECS components to HybridEntity. You also can override some methods.
- Inspector to `ComponentWrapper`. It will warning you when something went wrong.
- Interface `ICanCopyData`.\
It's mandatory interface to component in `ComponentWrapper`, you need to copy every field to other instance to proper work.
- ECS Component `UnityObject` to let you refer to GameObject of HybridEntity.
- ECS Component `Updated<>` to let you know when component changed from Inspector.\
You also can use it to reactive-like systems. Just make `EcsFilter<Updated<Component>>` and fire event with...
- Entity Extension `MarkAsUpdated<>`. It will simply add `Updated<>` component to entity.
- Abstract mono behaviour `BaseStartup`. You can inherit it to get simple interface for working in Unity. All ECS One Frame components will remove after `lateUpdateSystems` processing.
```
internal sealed class $STARTUP$ : BaseStartup 
{
    private void OnEnable() {
        CreateWorld();
        CreateSystems();
        
        //fixedUpdateSystems.Add(new FixedUpdateSystem());
        //updateSystems.Add(new UpdateSystem());
        //lateUpdateSystems.Add(new LateUpdateSystem());
        //gizmosSystems.Add(new GizmosSystem());

        //updateSystems.Inject(something);
        FinalizeSystems();
    }
}
```

I also very recommend to think "One startup - one scene" and to decompose your game to scenes!
