using System;
using UnityEngine;

namespace Leopotam.Ecs.Hybrid {
	public class HybridEntity : MonoBehaviour {
		public bool checkChildrenForComponents = true;
		public bool worldIsAlive => startup && startup.worldIsAlive;

		public EcsEntity entity => entityValue;
		private EcsEntity entityValue;

		public bool isAlive => !entityValue.IsNull() && entityValue.IsAlive();

		private BaseStartup startup;

		private void Awake() {
			startup = GetStartup();
		}

		protected virtual void OnEnable() {
			#if DEBUG
			startup = GetStartup();
			if (!startup) {
				throw new Exception($"There is no {nameof(BaseStartup)} on parents!");
			}
			#endif

			entityValue = CreateEntity();
			AddComponentsFromGameObject(gameObject);

			if (!checkChildrenForComponents) return;
			AddComponentsFromChildren();
		}

		protected virtual void OnDisable() {
			DestroyEntity();
		}

		protected virtual BaseStartup GetStartup() {
			return GetComponentInParent<BaseStartup>();
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