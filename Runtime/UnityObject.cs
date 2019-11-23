using UnityEngine;

namespace Leopotam.Ecs.Hybrid {
	public sealed class UnityObject : IEcsAutoReset {
		public GameObject gameObject;

		public void Reset() {
			gameObject = null;
		}
	}
}