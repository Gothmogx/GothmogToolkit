#if ECS
using Unity.Entities;
using UnityEngine;

namespace GothmogToolkit.Tools.ECS
{
	public struct IdComponent : IComponentData
	{
		public uint Id;
		public IdComponent(uint id)
		{
			Id = id;
		}
		public bool IsValid => Id != 0;
	}

	public struct NewIdTag : IComponentData
	{
	}
	public class IdAuthoring : MonoBehaviour
	{
		[SerializeField] private uint _id;

		private class Baker : Baker<IdAuthoring>
		{
			public override void Bake(IdAuthoring authoring)
			{
				var entity = GetEntity(TransformUsageFlags.None);
				AddComponent(entity, new IdComponent(authoring._id));
				AddComponent(entity, new NewIdTag());
			}
		}

	}

#endif
}