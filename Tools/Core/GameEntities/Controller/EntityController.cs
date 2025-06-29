using System;
using System.Collections.Generic;
using Game.Domains.Common.Scripts.Core.Id;

namespace Game.Domains.Common.Scripts.Contoller
{
	public interface IEntityController<TEntity> where TEntity: IHasId
	{
		List<TEntity> Entities { get; }
		TEntity Get(long id);
		bool TryGet(long id, out TEntity entity);
		public event Action<TEntity> EntityRegistered;
	}

	public class EntityController<TEntity> : IEntityController<TEntity> where TEntity : IHasId
	{
		private readonly Dictionary<long, TEntity> _entitiesDictionary = new();
		public List<TEntity> Entities { get; } = new();
		public TEntity Get(long id) => _entitiesDictionary.GetValueOrDefault(id);
		public event Action<TEntity> EntityRegistered;

		public virtual void Register(TEntity entity)
		{
			if (!_entitiesDictionary.TryAdd(entity.Id, entity))
				throw new ArgumentException($"Entity already exists in the dictionary");
			Entities.Add(entity);
			EntityRegistered?.Invoke(entity);
		}

		public bool TryGet(long id, out TEntity entity)
		{
			return _entitiesDictionary.TryGetValue(id, out entity);
		}
	}
}