using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using Tools.Helpers.Easings;
using UnityEngine;

namespace Tools.Presentation.ProgressBar.Scripts
{
    public class BaseProgressBarPresenter : MonoBehaviour
    {
        [SerializeField] private RectTransform _parentRect;
        [SerializeField] private RectTransform _progressBarRect;
        [SerializeField] private float _updateDuration = 0.2f;
        [SerializeField] private EasingType _easeIn = EasingType.Linear;
        [SerializeField] private EasingType _easeOut = EasingType.Cubic;
        [SerializeField] [Range(0, 1)] private float _currentProgress;
        private CancellationTokenSource _cts;
        private Func<float> _getProgress;

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            if (!_parentRect || !_progressBarRect)
                return;

            _currentProgress = Mathf.Clamp01(_currentProgress);
            UpdateProgressBarSize();
        }

        public virtual void Initialize(Func<float> getProgress)
        {
            _getProgress = getProgress;
        }

        public void UpdateView(bool instant = false)
        {
            if (_getProgress == null)
                throw new InvalidOperationException($"{nameof(_getProgress)} function is not initialized.");

            _cts?.Cancel();

            OnUpdateProgressStarted();
            var targetProgress = Mathf.Clamp01(_getProgress?.Invoke() ?? 0);

            if (instant)
            {
                _currentProgress = targetProgress;
                UpdateProgressBarSize();
                OnProgressChanged();
                OnProgressApplied();
                return;
            }

            var cancellationTokenSource =
                CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

            _cts = cancellationTokenSource;

            UpdateProgressAsync(targetProgress, cancellationTokenSource).Forget();
        }

        private async UniTask UpdateProgressAsync(float targetProgress,
            CancellationTokenSource cts)
        {
            var cancellationToken = cts.Token;
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                var startTime = Time.unscaledTime;
                var endTime = startTime + _updateDuration;
                var startProgress = _currentProgress;

                while (Time.unscaledTime < endTime)
                {
                    var progress = (Time.unscaledTime - startTime) / _updateDuration;
                    var easeProgress = Easing.EaseInOut(progress, _easeIn, _easeOut);
                    _currentProgress = Mathf.Lerp(startProgress, targetProgress, easeProgress);
                    OnProgressChanged();
                    UpdateProgressBarSize();
                    await UniTask.WaitForEndOfFrame(cancellationToken);
                }

                _currentProgress = targetProgress;
                UpdateProgressBarSize();
                OnProgressApplied();
            }
            catch (OperationCanceledException)
                when (cancellationToken.IsCancellationRequested)
            {
                OnProgressCancelled();
            }
            finally
            {
                if (ReferenceEquals(_cts, cts))
                {
                    _cts = null;
                }

                cts.Dispose();
            }
        }

        protected virtual void OnProgressCancelled()
        {
        }

        protected virtual void OnProgressApplied()
        {
        }

        protected virtual void OnProgressChanged()
        {
        }

        protected virtual void OnUpdateProgressStarted()
        {
        }

        private void UpdateProgressBarSize()
        {
            _progressBarRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,
                _currentProgress * _parentRect.rect.width);
        }
    }
}
