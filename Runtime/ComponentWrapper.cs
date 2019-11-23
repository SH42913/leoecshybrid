using UnityEngine;

namespace Leopotam.Ecs.Hybrid {
	public abstract class ComponentWrapper<T> : BaseComponentWrapper where T : class, ICanCopyData<T>, new() {
		[SerializeField] [WrappedComponent] private T value;
		private T startValue;

		public T component => value;

		public override void AddToEntity(ref EcsEntity entity) {
			startValue = value;
			value = entity.Set<T>();
			connected = true;

			if (startValue != null) {
				startValue.Copy(value);
				MarkAsUpdated();
			}
		}

		public override void RemoveFromEntity(ref EcsEntity entity) {
			if (startValue != null) {
				value.Copy(startValue);
				value = startValue;
			}

			entity.Unset<T>();
			connected = false;
		}

		public override void MarkAsUpdated() {
			if (startValue == null || !componentExists) return;

			hybridEntity.entity.MarkAsUpdated<T>();
		}
	}
}