using System;
using System.Runtime.CompilerServices;

namespace Leopotam.Ecs.Hybrid {
	public sealed class Updated<T> : IEcsOneFrame, IEcsIgnoreInFilter { }

	public static class EcsUpdatedExtensions {
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void MarkAsUpdated<T>(this EcsEntity entity) where T : class, new() {
			#if DEBUG
			if (entity.Get<T>() == null) {
				throw new Exception($"Entity has no {typeof(T).Name} and can not be marked as updated!");
			}
			#endif

			entity.Set<Updated<T>>();
		}
	}
}