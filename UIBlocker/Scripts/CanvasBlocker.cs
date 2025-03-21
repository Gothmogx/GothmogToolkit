using UnityEngine;

namespace GothmogToolkit.UIBlocker
{
	[RequireComponent(typeof(CanvasGroup))]
	public class CanvasBlocker : MonoBehaviour
	{
		[SerializeField] private CanvasGroup _canvasGroup;
		
		private IUIBlockerController _blockUIEventsHandler;

#if VCONTAINER 
		[VContainer.Inject]
#elif ZENJECT
		[Zenject.Inject]
#endif
		public void Construct(IUIBlockerController blockUIEventsHandler)
		{
			_blockUIEventsHandler = blockUIEventsHandler;
		}

		private void Awake()
		{
			_canvasGroup ??= GetComponent<CanvasGroup>();

			if (!_canvasGroup)
				return;

			_blockUIEventsHandler.BlockedChanged += OnBlockedChanged;
			SetBlocked(_blockUIEventsHandler.IsBlocked);
		}

		private void OnDestroy()
		{
			_blockUIEventsHandler.BlockedChanged -= OnBlockedChanged;
		}

		private void OnBlockedChanged(bool isBlocked)
		{
			SetBlocked(isBlocked);
		}

		private void SetBlocked(bool blocked)
		{
			if (_canvasGroup)
				_canvasGroup.interactable = !blocked;
		}
	}
}