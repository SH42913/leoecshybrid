using UnityEngine;

namespace Leopotam.Ecs.Hybrid {
	public abstract class ComponentWrapper<T> : BaseComponentWrapper where T : class, ICanCopyData<T>, new() {
		public T component => value;
		[SerializeField] [WrappedComponent] private T value = new T();
		private T startValue;

		public override void AddToEntity(ref EcsEntity entity) {
			startValue = value;
			value = entity.Set<T>();
			connectedToEntity = true;
			startValue?.Copy(value);
		}

		public override void RemoveFromEntity(ref EcsEntity entity) {
			if (startValue != null) {
				value.Copy(startValue);
				value = startValue;
			}

			entity.Unset<T>();
			connectedToEntity = false;
		}
	}

	public abstract class ReactiveComponentWrapper<T> : BaseReactiveComponentWrapper where T : class, ICanCopyData<T>, new() {
		public T component => value;
		[SerializeField] [WrappedComponent] private T value = new T();
		private T startValue;

		public override void AddToEntity(ref EcsEntity entity) {
			startValue = value;
			value = entity.Set<T>();
			connectedToEntity = true;

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
			connectedToEntity = false;
		}

		protected override void AddUpdatedComponent(EcsEntity entity) {
			entity.MarkAsUpdated<T>();
		}
	}

	public interface ICanCopyData<in T> where T : class {
		void Copy(T other);
	}

	public sealed class WrappedComponentAttribute : PropertyAttribute { }
}