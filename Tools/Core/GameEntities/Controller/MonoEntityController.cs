using System.Collections.Generic;
using Game.Domains.Common.Scripts.Core.Id;
using UnityEngine;

namespace Game.Domains.Common.Scripts.Contoller
{
	public class MonoEntityController<TEntity> : MonoBehaviour, IEntityController<TEntity> where TEntity : IHasId
	{
		private readonly EntityController<TEntity> _internalController = new();
		public List<TEntity> Entities => _internalController.Entities;
		public TEntity Get(long id) => _internalController.Get(id);
		public bool TryGet(long id, out TEntity entity) => _internalController.TryGet(id, out entity);
		public virtual void Register(TEntity entity) => _internalController.Register(entity);

	}
}