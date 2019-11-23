namespace Leopotam.Ecs.Hybrid {
	public interface ICanCopyData<in T> where T : class {
		void Copy(T other);
	}
}