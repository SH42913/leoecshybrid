using System;
using UnityEngine;

namespace Leopotam.Ecs.Hybrid {
	public class HybridEntity : MonoBehaviour {
		public bool createEntityOnEnable = true;
		public bool checkChildrenForComponents = true;

		[Tooltip("You should to register NewHybridEntityEvent as OneFrame in any your EcsSystems before enable")]
		public bool sendNewHybridEntityEvent = false;

		private BaseStartup startup;

		public ref EcsEntity entity => ref entityValue;
		private EcsEntity entityValue;

		public bool isAlive => startup != null && startup.worldIsAlive && entityValue.IsAlive();

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
			if (!entityValue.IsNull()) {
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

			var newEntity = startup.world.NewEntityWith(out UnityObject unityObject);
			unityObject.transform = gameObject.transform;

			if (sendNewHybridEntityEvent) {
				newEntity.Set<NewHybridEntityEvent>();
			}

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
			if (isAlive) {
				entityValue.Destroy();
			}

			entityValue = EcsEntity.Null;
		}
	}

	public sealed class NewHybridEntityEvent { }

	public sealed class UnityObject : IEcsAutoReset {
		public Transform transform;

		public void Reset() {
			transform = null;
		}
	}
}