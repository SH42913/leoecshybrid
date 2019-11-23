using System;
using UnityEngine;

namespace Leopotam.Ecs.Hybrid {
	public class HybridEntity : MonoBehaviour {
		[SerializeField] private BaseStartup startup = null;
		public bool worldIsAlive => startup && startup.worldIsAlive;

		public bool checkChildrenForComponents = true;

		public EcsEntity entity => entityValue;
		private EcsEntity entityValue;

		public bool isAlive => !entityValue.IsNull() && entityValue.IsAlive();

		protected virtual void OnEnable() {
			entityValue = CreateEntity();
			AddComponentsFromGameObject(gameObject);

			if (!checkChildrenForComponents) return;
			AddComponentsFromChildren();
		}

		protected virtual void OnDisable() {
			DestroyEntity();
		}

		protected virtual void AddComponentsFromGameObject(GameObject go) {
			foreach (var wrapper in go.GetComponents<BaseComponentWrapper>()) {
				if (!wrapper.enabled) continue;

				wrapper.AddToEntity(ref entityValue);
			}
		}

		protected void AddComponentsFromChildren() {
			foreach (Transform child in transform) {
				AddComponentsFromGameObject(child.gameObject);
			}
		}

		protected virtual void DestroyEntity() {
			if (startup.world != null && entityValue.IsAlive()) {
				entityValue.Destroy();
			}

			entityValue = EcsEntity.Null;
		}

		protected virtual EcsEntity CreateEntity() {
			#if DEBUG
			if (!startup) {
				throw new Exception("Startup is not found!");
			} else if (!startup.worldIsAlive) {
				throw new Exception("Startup is not init!");
			} else if (!entityValue.IsNull()) {
				throw new Exception("Entity already created!");
			}
			#endif

			var newEntity = startup.world.NewEntityWith(out UnityObject unityObject, out NewHybridEntity _);
			unityObject.gameObject = gameObject;

			return newEntity;
		}
	}
}