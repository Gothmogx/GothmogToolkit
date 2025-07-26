#if ECS
using GothmogToolkit.Tools.ECS.SystemGroups;
using GothmogToolkit.Tools.Helpers.ECS;
using Unity.Collections;
using Unity.Entities;

namespace GothmogToolkit.Tools.ECS
{
	[UpdateInGroup(typeof(InGameInitializationSystemGroup))]
	public partial class IdSystem : SystemBase
	{
		private const int InitialCapacity = 1024;
		private NativeHashMap<uint, Entity> _entities;
		private uint _lastId;
		protected override void OnCreate()
		{
			base.OnCreate();
			_entities = new NativeHashMap<uint, Entity>(InitialCapacity, Allocator.Persistent);
		}
		protected override void OnDestroy()
		{
			base.OnDestroy();
			_entities.Dispose();
		}
		protected override void OnUpdate()
		{
			var ecb = new EntityCommandBuffer(Allocator.Temp);
			foreach (var (idComponent, newIdTag, entity) in SystemAPI.Query<RefRW<IdComponent>, RefRO<NewIdTag>>().WithEntityAccess())
			{
				if (TryAddId(idComponent.ValueRO, entity))
				{
					entity.RemoveComponent<NewIdTag>();
					continue;
				}

				var newId = ++_lastId;

				while (!_entities.TryAdd(newId, entity))
				{
					newId = ++_lastId;
				}
				_lastId = newId;

				idComponent.ValueRW.Id = newId;
				entity.RemoveComponent<NewIdTag>(ecb);
			}
			ecb.Playback(EntityManager);
			ecb.Dispose();
		}

		private bool TryAddId(IdComponent idComponent, Entity entity)
			=> idComponent.IsValid && _entities.TryAdd(idComponent.Id, entity);
	}
}
#endif