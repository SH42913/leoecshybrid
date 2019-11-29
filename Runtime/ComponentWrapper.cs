using UnityEngine;

namespace Leopotam.Ecs.Hybrid {
	public abstract class ComponentWrapper<T> : BaseComponentWrapper where T : class, ICanCopyData<T>, new() {
		[SerializeField] [WrappedComponent] private T value = new T();
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

		protected override void AddUpdatedComponent(EcsEntity entity) {
			if (startValue == null) return;
			entity.MarkAsUpdated<T>();
		}
	}
}