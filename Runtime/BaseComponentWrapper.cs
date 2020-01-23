using System;
using UnityEngine;

namespace Leopotam.Ecs.Hybrid {
	public abstract class BaseComponentWrapper : MonoBehaviour {
		protected bool componentExists => hybridEntity != null && hybridEntity.isAlive && connectedToEntity;
		protected bool connectedToEntity;

		protected HybridEntity hybridEntity => hybridEntityValue == null ? hybridEntityValue : DetectHybridEntity();
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

		public void MarkAsUpdated() {
			if (!componentExists) return;

			AddUpdatedComponent(hybridEntity.entity);
		}

		protected abstract void AddUpdatedComponent(EcsEntity entity);

		private HybridEntity DetectHybridEntity() {
			hybridEntityValue = GetComponent<HybridEntity>() ?? transform.parent.GetComponent<HybridEntity>();

			#if DEBUG
			if (hybridEntityValue == null || !hybridEntityValue.checkChildrenForComponents) {
				throw new Exception($"There is no any {nameof(HybridEntity)}!");
			}
			#endif

			return hybridEntityValue;
		}
	}
}