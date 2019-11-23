using System;
using System.Runtime.CompilerServices;

namespace Leopotam.Ecs.Hybrid {
	public static class EntityExtensions {
		[MethodImpl (MethodImplOptions.AggressiveInlining)]
		public static void MarkAsUpdated<T>(this EcsEntity entity) where T : class, new() {
			#if DEBUG
			if (entity.Get<T>() == null) {
				throw new Exception($"Entity has no {typeof(T).Name} and can be marked as updated!");
			}
			#endif

			entity.Set<Updated<T>>();
		}
	}
}