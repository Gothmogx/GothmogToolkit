using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using GothmogToolkit.Tools.Helpers.Extensions;
using UnityEngine;

namespace GothmogToolkit.Tools.Core.LoadingScreens.Scripts
{
	[RequireComponent(typeof(CanvasGroup))]
	public class BlackScreenFading : MonoBehaviour, ILoadingScreen
	{
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private float _showDuration = 0.2f;
		[SerializeField] private float _hideDuration = 0.5f;

		private CancellationTokenSource _cts;

		public async UniTask Show()
		{
			SetActive(true);
			CreateCancellationToken();

			SetActive(true);
			_canvasGroup.blocksRaycasts = true;
			await AnimateCanvasAlpha(1f, _showDuration, _cts.Token);
		}

		private async UniTask AnimateCanvasAlpha(float targetAlpha, float duration, CancellationToken token)
		{
			var progress = 0f;
			var speed = 1 / duration;
			var startAlpha = _canvasGroup.alpha;
			while (progress < 1 && !token.IsCancellationRequested)
			{
				progress = Mathf.MoveTowards(progress, 1f, speed * Time.deltaTime);
				_canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
				await UniTask.WaitForEndOfFrame(token);
			}
		}

		public async UniTask Hide()
		{
			CreateCancellationToken();

			_canvasGroup.blocksRaycasts = false;
			await AnimateCanvasAlpha(0f, _hideDuration, _cts.Token);

			SetActive(false);
		}

		public void SetActive(bool active) => gameObject.SetActive(active);
		private void CreateCancellationToken()
		{
			_cts.CancelAndDispose();
			_cts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
		}
	}
}