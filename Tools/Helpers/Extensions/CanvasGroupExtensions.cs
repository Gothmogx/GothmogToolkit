using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace GothmogToolkit.Tools.Helpers.Extensions
{
    public static class CanvasGroupExtensions
    {
        private static readonly Dictionary<CanvasGroup, CancellationTokenSource> CanvasGroupToTokenSource = new();
        private const float MinAllowedDuration = 0.001f;

        #region CanvasGroup

        private static async UniTask AnimateCanvasAlpha(this CanvasGroup canvasGroup, float targetAlpha, float duration,
            bool unscaledTime = true,
            CancellationToken parentToken = default)
        {
            if (CanvasGroupToTokenSource.TryGetValue(canvasGroup, out var existingCts))
            {
                existingCts.CancelAndDispose();
            }

            if (duration <= MinAllowedDuration || Mathf.Approximately(canvasGroup.alpha, targetAlpha))
            {
                canvasGroup.alpha = targetAlpha;
                return;
            }

            var cts = CancellationTokenSource.CreateLinkedTokenSource(
                parentToken,
                canvasGroup.GetCancellationTokenOnDestroy()
            );

            CanvasGroupToTokenSource[canvasGroup] = cts;
            var token = cts.Token;

            var progress = 0f;
            var speed = 1 / duration;
            var startAlpha = canvasGroup.alpha;
            try
            {
                while (progress < 1)
                {
                    token.ThrowIfCancellationRequested();
                    var deltaTime = unscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    progress = Mathf.MoveTowards(progress, 1f, speed * deltaTime);
                    canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, progress);
                    await UniTask.NextFrame(cts.Token);
                }
            }
            finally
            {
                if (CanvasGroupToTokenSource.TryGetValue(canvasGroup, out var currentCts) && currentCts == cts)
                {
                    CanvasGroupToTokenSource.Remove(canvasGroup);
                    cts.Dispose();
                }
            }
        }

        public static async UniTask Hide(this CanvasGroup canvasGroup, float duration, bool unscaledTime = true,
            bool disable = false,
            CancellationToken token = default)
        {
            if (!canvasGroup)
                return;

            try
            {
                await canvasGroup.AnimateCanvasAlpha(0f, duration, unscaledTime, token);
            }
            catch (OperationCanceledException)
            {
                return;
            }

            if (disable)
                canvasGroup.SetActiveSafe(false);
        }

        public static async UniTask Show(this CanvasGroup canvasGroup, float duration, bool unscaledTime = true,
            CancellationToken token = default)
        {
            if (!canvasGroup)
                return;

            canvasGroup.SetActiveSafe(true);
            try
            {
                await canvasGroup.AnimateCanvasAlpha(1f, duration, unscaledTime, token);
            }
            catch (OperationCanceledException)
            {
            }
        }

        #endregion
    }
}