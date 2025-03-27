using UnityEngine;

namespace Game.Domains.Common.Scripts.Abstraction.View.View
{
	public interface IViewTransform
	{
		Vector3 Position { get; }
		void SetPosition(Vector3 position);
	}
}