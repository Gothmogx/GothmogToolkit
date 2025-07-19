using System;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GothmogToolkit.Tools.UI.UIBlocker
{
	public class RaycasterBlocker : MonoBehaviour
	{
		[SerializeField] private BaseRaycaster _raycaster;
		private IUIBlockerController _blockUIEventsHandler;

		#if VCONTAINER 
		[VContainer.Inject]
		#elif ZENJECT
		[Zenject.Inject]
		#endif
		public void Construct(IUIBlockerController blockUIEventsHandler) =>
			_blockUIEventsHandler = blockUIEventsHandler;

		private void Awake()
		{
			_raycaster ??= GetComponent<BaseRaycaster>();

			if (!_raycaster)
				return;

			_blockUIEventsHandler.BlockedChanged += OnBlockedChanged;
			SetBlocked(_blockUIEventsHandler.IsBlocked);
		}
		private void OnBlockedChanged(bool isBlocked) => SetBlocked(isBlocked);
		private void SetBlocked(bool blocked) => _raycaster.enabled = !blocked;

		private void OnDestroy()
		{
			_blockUIEventsHandler.BlockedChanged -= OnBlockedChanged;
		}
	}
}