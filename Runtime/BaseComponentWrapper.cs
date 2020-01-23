using System;
using UnityEngine;

namespace Leopotam.Ecs.Hybrid {
	public abstract class BaseComponentWrapper : MonoBehaviour {
		protected bool connected;
		protected bool componentExists => hybridEntity && hybridEntity.worldIsAlive && hybridEntity.isAlive && connected;

		private HybridEntity hybridEntityValue;

		protected HybridEntity hybridEntity {
			get {
				if (hybridEntityValue) return hybridEntityValue;

				hybridEntityValue = GetComponent<HybridEntity>() ?? transform.parent.GetComponent<HybridEntity>();

				#if DEBUG
				if (!hybridEntityValue) {
					throw new Exception($"There is no any {nameof(HybridEntity)}!");
				}
				#endif

				return hybridEntityValue;
			}
		}

		protected void OnEnable() {
			#if UNITY_EDITOR
			ValidateComponentValues();
			#endif

			if (!hybridEntity.isAlive || connectedToEntity) return;

			var entity = hybridEntity.entity;
			AddToEntity(ref entity);
		}

		protected void OnDisable() {
			if (!componentExists) return;

			var entity = hybridEntity.entity;
			RemoveFromEntity(ref entity);
		}

		protected virtual void ValidateComponentValues() { }
		public abstract void AddToEntity(ref EcsEntity entity);
		public abstract void RemoveFromEntity(ref EcsEntity entity);

		public void MarkAsUpdated() {
			if (!componentExists) return;

			AddUpdatedComponent(hybridEntity.entity);
		}

		protected abstract void AddUpdatedComponent(EcsEntity entity);
	}
}