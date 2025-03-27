using Game.Domains.Common.Scripts.Abstraction.View.View;
using UnityEngine;

namespace Game.Domains.Session.View.Scripts.Grid
{ 
	public class UnityViewTransform : IViewTransform
	{
		private readonly Transform _transform;

		public UnityViewTransform(Transform transform)
		{
			_transform = transform;
		}

		public Vector3 Position => _transform.position;

		public void SetPosition(Vector3 position)
		{
			_transform.position = position;
		}
	}
}