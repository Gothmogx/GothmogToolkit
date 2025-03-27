using Game.Domains.Common.Scripts.Abstraction.View.View;
using Game.Domains.Common.Scripts.Core.Id;
using Game.Domains.Session.View.Scripts.Grid;
using UnityEngine;

namespace Game.Domains.Session.View.Scripts.Other
{
	public abstract class EntityView : MonoBehaviour, IHasId
	{
		private IViewTransform _transform;

		public int Id { get; private set; }

		public IViewTransform Transform
		{
			get
			{
				_transform ??= new UnityViewTransform(transform);
				return _transform;
			}
		}

		protected virtual void Awake()
		{
		}

		public void SetId(int id)
		{
			Id = id;
		}
	}
}