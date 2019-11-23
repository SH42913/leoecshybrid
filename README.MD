# Leopotam ECS Hybrid
This is the Unity Hybrid extension to [Leopotam Entity Component System Framework](https://github.com/Leopotam/ecs).\
Hybrid ECS let you to connect scenes and prefabs of Unity with ECS in easy way. It's working like `Unity.Entities.Hybrid`.

You need to install [LeoECS](https://github.com/Leopotam/ecs) and [LeoECS Unity Integration](https://github.com/Leopotam/ecs-unityintegration) to proper work.

## Installation as unity module
This repository can be installed as unity module directly from git url. In this way new line should be added to `Packages/manifest.json`:
```
"com.leopotam.ecs": "https://github.com/Leopotam/ecs.git",
```
By default last released version will be used. If you need trunk / developing version then `develop` name of branch should be added after hash:
```
"com.leopotam.ecs": "https://github.com/Leopotam/ecs.git#develop",
```

## Content
- `HybridEntity` to represent Entity on scene/prefab.
- ECS One Frame Component `NewHybridEntity` to let you know about any new `HybridEntity`.
- `ComponentWrapper` to add/remove ECS components to HybridEntity. You also can override some methods.
- Inspector to `ComponentWrapper`. It will warning you when something went wrong.
- Interface `ICanCopyData`.\
It's mandatory interface to component in `ComponentWrapper`, you need to copy every field to other instance to proper work.
- ECS Component `UnityObject` to let you refer to GameObject of HybridEntity.
- ECS Component `Updated<>` to let you know when component changed from Inspector.\
You also can use it to reactive-like systems. Just make `EcsFilter<Updated<Component>>` and fire event with...
- Entity Extension `MarkAsUpdated<>`. It will simply add `Updated<>` component to entity.
- Abstract mono behaviour `BaseStartup`.\
You can inherit it to get simple interface for working in Unity.
```
    internal sealed class $STARTUP$ : BaseStartup 
    {
		private void OnEnable() {
			CreateWorld();
			CreateSystems();
			
			//fixedUpdateSystems.Add(new FixedUpdateSystem());
			//updateSystems.Add(new UpdateSystem());
			//gizmosSystems.Add(new GizmosSystem());

			//updateSystems.Inject(something);
			FinalizeSystems();
		}
	}
```

I also very recommend think "One startup - one scene" and decomposite your game to scenes!
