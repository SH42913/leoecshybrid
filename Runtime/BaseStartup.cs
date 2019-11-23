using UnityEngine;

namespace Leopotam.Ecs.Hybrid {
	public abstract class BaseStartup : MonoBehaviour {
		public EcsWorld world { get; private set; }
		public bool worldIsAlive => world != null;
		public bool isRunning = false;

		protected EcsSystems updateSystems;
		protected EcsSystems fixedUpdateSystems;

		#if UNITY_EDITOR
		protected EcsSystems gizmosSystems;
		#endif

		protected virtual void CreateWorld() {
			world = new EcsWorld();

			#if UNITY_EDITOR
			UnityIntegration.EcsWorldObserver.Create(world);
			#endif
		}

		protected virtual void CreateSystems() {
			updateSystems = new EcsSystems(world, "UPDATE");
			fixedUpdateSystems = new EcsSystems(world, "FIXED UPDATE");

			#if UNITY_EDITOR
			gizmosSystems = new EcsSystems(world, "GIZMOS");
			UnityIntegration.EcsSystemsObserver.Create(updateSystems);
			UnityIntegration.EcsSystemsObserver.Create(fixedUpdateSystems);
			UnityIntegration.EcsSystemsObserver.Create(gizmosSystems);
			#endif
		}

		protected virtual void FinalizeSystems() {
			updateSystems.Init();
			fixedUpdateSystems.Init();

			#if UNITY_EDITOR
			gizmosSystems.Init();
			#endif

			isRunning = true;
		}

		private void FixedUpdate() {
			if (!isRunning) return;
			fixedUpdateSystems.Run();
		}

		private void Update() {
			if (!isRunning) return;

			updateSystems.Run();
			world.EndFrame();
		}

		private void OnDisable() {
			updateSystems.Destroy();
			updateSystems = null;
			fixedUpdateSystems.Destroy();
			fixedUpdateSystems = null;

			#if UNITY_EDITOR
			gizmosSystems.Destroy();
			gizmosSystems = null;
			#endif

			world.Destroy();
			world = null;
		}

		#if UNITY_EDITOR
		private void OnDrawGizmos() {
			gizmosSystems?.Run();
		}
		#endif
	}
}