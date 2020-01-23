using System;
using UnityEngine;

namespace Leopotam.Ecs.Hybrid {
	public abstract class BaseComponentWrapper : MonoBehaviour {
		protected bool componentExists => hybridEntity != null && hybridEntity.isAlive && connectedToEntity;
		protected bool connectedToEntity;

		protected HybridEntity hybridEntity => hybridEntityValue != null ? hybridEntityValue : DetectHybridEntity();
		private HybridEntity hybridEntityValue;

		protected void OnEnable() {
			#if UNITY_EDITOR
			ValidateComponentValues();
			#endif

			if (hybridEntity.isAlive && !connectedToEntity) {
				AddToEntity(ref hybridEntity.entity);
			}
		}

		protected void OnDisable() {
			if (componentExists) {
				RemoveFromEntity(ref hybridEntity.entity);
			}
		}

		protected virtual void ValidateComponentValues() { }
		public abstract void AddToEntity(ref EcsEntity entity);
		public abstract void RemoveFromEntity(ref EcsEntity entity);

		private HybridEntity DetectHybridEntity() {
			var foundInParent = false;
			hybridEntityValue = GetComponent<HybridEntity>();
			if (hybridEntityValue == null && transform.parent != null) {
				hybridEntityValue = transform.parent.GetComponent<HybridEntity>();
				foundInParent = true;
			}

			#if DEBUG
			if (hybridEntityValue == null || foundInParent && !hybridEntityValue.checkChildrenForComponents) {
				throw new Exception($"There is no any {nameof(HybridEntity)}!");
			}
			#endif

			return hybridEntityValue;
		}
	}

	public abstract class BaseReactiveComponentWrapper : BaseComponentWrapper {
		public void MarkAsUpdated() {
			if (componentExists) {
				AddUpdatedComponent(hybridEntity.entity);
			}
		}

		protected abstract void AddUpdatedComponent(EcsEntity entity);
	}
}