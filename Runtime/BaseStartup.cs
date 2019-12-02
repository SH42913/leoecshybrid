using System;
using UnityEngine;

namespace Leopotam.Ecs.Hybrid {
	public abstract class BaseStartup : MonoBehaviour {
		public EcsWorld world { get; private set; }
		public bool worldIsAlive => world != null;
		public bool isRunning = false;

		protected EcsSystems fixedUpdateSystems;
		protected EcsSystems updateSystems;
		protected EcsSystems lateUpdateSystems;

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
			fixedUpdateSystems = new EcsSystems(world, "FIXED UPDATE");
			updateSystems = new EcsSystems(world, "UPDATE");
			lateUpdateSystems = new EcsSystems(world, "LATE UPDATE");

			#if UNITY_EDITOR
			gizmosSystems = new EcsSystems(world, "GIZMOS");
			UnityIntegration.EcsSystemsObserver.Create(fixedUpdateSystems);
			UnityIntegration.EcsSystemsObserver.Create(updateSystems);
			UnityIntegration.EcsSystemsObserver.Create(lateUpdateSystems);
			UnityIntegration.EcsSystemsObserver.Create(gizmosSystems);
			#endif
		}

		protected virtual void FinalizeSystems() {
			fixedUpdateSystems.ProcessInjects();
			updateSystems.ProcessInjects();
			lateUpdateSystems.ProcessInjects();

			#if UNITY_EDITOR
			gizmosSystems.ProcessInjects();
			#endif

			fixedUpdateSystems.Init();
			updateSystems.Init();
			lateUpdateSystems.Init();

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
		}

		private void LateUpdate() {
			if (!isRunning) return;

			lateUpdateSystems.Run();
			world.EndFrame();
		}

		private void OnDisable() {
			fixedUpdateSystems.Destroy();
			fixedUpdateSystems = null;
			updateSystems.Destroy();
			updateSystems = null;
			lateUpdateSystems.Destroy();
			lateUpdateSystems = null;

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