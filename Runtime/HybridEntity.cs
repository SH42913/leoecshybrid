using System;
using UnityEngine;

namespace Leopotam.Ecs.Hybrid {
	public class HybridEntity : MonoBehaviour {
		public bool createEntityOnEnable = true;
		public bool checkChildrenForComponents = true;

		public bool worldIsAlive => startup && startup.worldIsAlive;
		private BaseStartup startup;

		public EcsEntity entity => entityValue;
		public bool entityIsAlive => !entityValue.IsNull() && entityValue.IsAlive();
		private EcsEntity entityValue;

		private void Awake() {
			startup = GetStartup();
		}

		protected virtual void OnEnable() {
			if (!createEntityOnEnable) return;

			entityValue = CreateEntity();
			FillEntityWithComponents();
		}

		protected virtual void OnDisable() {
			DestroyEntity();
		}

		public void AttachToEntity(EcsEntity parentEntity) {
			#if DEBUG
			if (!entity.IsNull()) {
				throw new Exception($"{nameof(HybridEntity)} already attached to {nameof(EcsEntity)}");
			}
			#endif

			entityValue = parentEntity;
			FillEntityWithComponents();
		}

		protected virtual BaseStartup GetStartup() {
			return GetComponentInParent<BaseStartup>();
		}

		protected virtual EcsEntity CreateEntity() {
			#if DEBUG
			startup = GetStartup();
			if (!startup) {
				throw new Exception($"There is no {nameof(BaseStartup)} on parents!");
			} else if (!startup.worldIsAlive) {
				throw new Exception($"{startup.GetType().Name} is not init!");
			} else if (!entityValue.IsNull()) {
				throw new Exception("Entity already created!");
			}
			#endif

			var newEntity = startup.world.NewEntityWith(out UnityObject unityObject, out NewHybridEntityEvent _);
			unityObject.gameObject = gameObject;
			return newEntity;
		}

		protected void FillEntityWithComponents() {
			AddComponentsFromGameObject(gameObject);

			if (!checkChildrenForComponents) return;
			AddComponentsFromChildren();
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
			if (worldIsAlive && entityIsAlive) {
				entityValue.Destroy();
			}

			entityValue = EcsEntity.Null;
		}
	}

	public sealed class NewHybridEntityEvent : IEcsOneFrame { }

	public sealed class UnityObject : IEcsAutoReset {
		public GameObject gameObject;

		public void Reset() {
			gameObject = null;
		}
	}
}