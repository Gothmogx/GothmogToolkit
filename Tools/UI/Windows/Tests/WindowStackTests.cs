#if UNITASK
using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;

namespace GothmogToolkit.Tools.UI.Windows.Tests
{
	public class WindowStackTests
	{
		private readonly List<GameObject> _gameObjects = new();

		[TearDown]
		public void TearDown()
		{
			foreach (var gameObject in _gameObjects)
			{
				if (gameObject != null)
				{
					UnityEngine.Object.DestroyImmediate(gameObject);
				}
			}

			_gameObjects.Clear();
		}

		[Test]
		public void PushAsync_ReturnsTypedResultAfterHideCompletes()
		{
			var stack = new WindowStack();
			var window = CreateWindow("Window");
			window.PauseHide();

			var pushTask = stack.PushAsync(window, CancellationToken.None);
			Assert.That(window.TryClose(TestResult.Accepted), Is.True);
			Assert.That(pushTask.Status.IsCompleted(), Is.False);

			window.CompleteHide();

			Assert.That(pushTask.GetAwaiter().GetResult(), Is.EqualTo(TestResult.Accepted));
			Assert.That(window.State, Is.EqualTo(WindowState.Closed));
			Assert.That(window.gameObject.activeSelf, Is.False);
		}

		[Test]
		public void NestedPush_DisablesCoveredWindowAndRestoresItAfterClose()
		{
			var stack = new WindowStack();
			var lowerWindow = CreateWindow("Lower");
			var upperWindow = CreateWindow("Upper");

			var lowerTask = stack.PushAsync(lowerWindow, CancellationToken.None);
			var upperTask = stack.PushAsync(upperWindow, CancellationToken.None);

			Assert.That(lowerWindow.gameObject.activeSelf, Is.True);
			Assert.That(lowerWindow.IsInteractable, Is.False);
			Assert.That(upperWindow.IsInteractable, Is.True);

			Assert.That(upperWindow.TryClose(TestResult.Accepted), Is.True);
			Assert.That(upperTask.GetAwaiter().GetResult(), Is.EqualTo(TestResult.Accepted));
			Assert.That(lowerWindow.IsInteractable, Is.True);

			lowerWindow.TryClose(TestResult.Accepted);
			lowerTask.GetAwaiter().GetResult();
		}

		[Test]
		public void DismissTopAsync_UsesWindowDismissResult()
		{
			var stack = new WindowStack();
			var window = CreateWindow("Window");
			var pushTask = stack.PushAsync(window, CancellationToken.None);

			var dismissed = stack.DismissTopAsync(CancellationToken.None)
				.GetAwaiter()
				.GetResult();

			Assert.That(dismissed, Is.True);
			Assert.That(pushTask.GetAwaiter().GetResult(), Is.EqualTo(TestResult.Dismissed));
		}

		[Test]
		public void PushAsync_AllowsMultipleInstancesOfSameType()
		{
			var stack = new WindowStack();
			var first = CreateWindow("First");
			var second = CreateWindow("Second");
			var firstTask = stack.PushAsync(first, CancellationToken.None);
			var secondTask = stack.PushAsync(second, CancellationToken.None);

			second.TryClose(TestResult.Accepted);
			secondTask.GetAwaiter().GetResult();
			first.TryClose(TestResult.Accepted);
			firstTask.GetAwaiter().GetResult();

			Assert.That(stack.Count, Is.Zero);
		}

		[Test]
		public void PushAsync_RejectsDuplicateAndCrossStackOwnership()
		{
			var firstStack = new WindowStack();
			var secondStack = new WindowStack();
			var window = CreateWindow("Window");
			var pushTask = firstStack.PushAsync(window, CancellationToken.None);

			Assert.That(
				() => firstStack.PushAsync(window, CancellationToken.None).GetAwaiter().GetResult(),
				Throws.TypeOf<InvalidOperationException>());
			Assert.That(
				() => secondStack.PushAsync(window, CancellationToken.None).GetAwaiter().GetResult(),
				Throws.TypeOf<InvalidOperationException>());

			window.TryClose(TestResult.Accepted);
			pushTask.GetAwaiter().GetResult();
		}

		[Test]
		public void CloseDuringOpening_IsQueuedAndFirstResultWins()
		{
			var stack = new WindowStack();
			var window = CreateWindow("Window");
			window.PauseShow();
			var pushTask = stack.PushAsync(window, CancellationToken.None);

			Assert.That(window.State, Is.EqualTo(WindowState.Opening));
			Assert.That(window.TryClose(TestResult.Accepted), Is.True);
			Assert.That(window.TryClose(TestResult.Rejected), Is.False);

			window.CompleteShow();

			Assert.That(pushTask.GetAwaiter().GetResult(), Is.EqualTo(TestResult.Accepted));
		}

		[Test]
		public void Cancellation_CleansStackAndRestoresCoveredWindow()
		{
			var stack = new WindowStack();
			var lowerWindow = CreateWindow("Lower");
			var upperWindow = CreateWindow("Upper");
			var lowerTask = stack.PushAsync(lowerWindow, CancellationToken.None);
			using (var cancellationTokenSource = new CancellationTokenSource())
			{
				var upperTask = stack.PushAsync(upperWindow, cancellationTokenSource.Token);
				cancellationTokenSource.Cancel();

				Assert.That(
					() => upperTask.GetAwaiter().GetResult(),
					Throws.TypeOf<OperationCanceledException>());
			}

			Assert.That(stack.Count, Is.EqualTo(1));
			Assert.That(lowerWindow.IsInteractable, Is.True);
			lowerWindow.TryClose(TestResult.Accepted);
			lowerTask.GetAwaiter().GetResult();
		}

		[Test]
		public void ShowFailure_CleansStackAndRestoresCoveredWindow()
		{
			var stack = new WindowStack();
			var lowerWindow = CreateWindow("Lower");
			var failingWindow = CreateWindow("Failing");
			failingWindow.ThrowOnShow = true;
			var lowerTask = stack.PushAsync(lowerWindow, CancellationToken.None);

			Assert.That(
				() => stack.PushAsync(failingWindow, CancellationToken.None).GetAwaiter().GetResult(),
				Throws.TypeOf<TestWindowException>());
			Assert.That(stack.Count, Is.EqualTo(1));
			Assert.That(lowerWindow.IsInteractable, Is.True);

			lowerWindow.TryClose(TestResult.Accepted);
			lowerTask.GetAwaiter().GetResult();
		}

		[Test]
		public void HideFailure_CleansStackAndPropagatesFailure()
		{
			var stack = new WindowStack();
			var window = CreateWindow("Window");
			window.ThrowOnHide = true;
			var pushTask = stack.PushAsync(window, CancellationToken.None);
			window.TryClose(TestResult.Accepted);

			Assert.That(
				() => pushTask.GetAwaiter().GetResult(),
				Throws.TypeOf<TestWindowException>());
			Assert.That(stack.Count, Is.Zero);
			Assert.That(window.State, Is.EqualTo(WindowState.Closed));
		}

		[Test]
		public void MissingCanvasGroup_ThrowsConfigurationError()
		{
			var gameObject = new GameObject("Misconfigured Window");
			_gameObjects.Add(gameObject);
			var window = gameObject.AddComponent<TestWindow>();
			var stack = new WindowStack();

			Assert.That(
				() => stack.PushAsync(window, CancellationToken.None).GetAwaiter().GetResult(),
				Throws.TypeOf<InvalidOperationException>());
		}

		private TestWindow CreateWindow(string name)
		{
			var gameObject = new GameObject(name, typeof(CanvasGroup));
			_gameObjects.Add(gameObject);
			return gameObject.AddComponent<TestWindow>();
		}
	}

	internal enum TestResult
	{
		Accepted,
		Rejected,
		Dismissed
	}

	internal sealed class TestWindowException : Exception
	{
	}

	internal sealed class TestWindow : WindowBase<TestResult>
	{
		private UniTaskCompletionSource _showCompletionSource;
		private UniTaskCompletionSource _hideCompletionSource;

		public bool ThrowOnShow { get; set; }
		public bool ThrowOnHide { get; set; }

		protected override TestResult DismissResult => TestResult.Dismissed;

		public void PauseShow()
		{
			_showCompletionSource = new UniTaskCompletionSource();
		}

		public void CompleteShow()
		{
			_showCompletionSource.TrySetResult();
		}

		public void PauseHide()
		{
			_hideCompletionSource = new UniTaskCompletionSource();
		}

		public void CompleteHide()
		{
			_hideCompletionSource.TrySetResult();
		}

		protected override UniTask PlayShowAnimationAsync(CancellationToken cancellationToken)
		{
			if (ThrowOnShow)
			{
				throw new TestWindowException();
			}

			return _showCompletionSource?.Task ?? base.PlayShowAnimationAsync(cancellationToken);
		}

		protected override UniTask PlayHideAnimationAsync(CancellationToken cancellationToken)
		{
			if (ThrowOnHide)
			{
				throw new TestWindowException();
			}

			return _hideCompletionSource?.Task ?? base.PlayHideAnimationAsync(cancellationToken);
		}
	}
}
#endif
