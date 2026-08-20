#if UNITASK
using System;
using System.Collections;
using System.Threading;
using Cysharp.Threading.Tasks;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

namespace GothmogToolkit.Tools.UI.Windows.PlayMode.Tests
{
	public sealed class WindowDestructionTests
	{
		[UnityTest]
		public IEnumerator DestroyedWindow_CancelsPushAndCleansStack()
		{
			var gameObject = new GameObject("Window", typeof(CanvasGroup));
			var window = gameObject.AddComponent<DestructionTestWindow>();
			var stack = new WindowStack();
			var pushTask = stack.PushAsync(window, CancellationToken.None);
			Exception observedException = null;

			UnityEngine.Object.Destroy(gameObject);
			yield return null;
			yield return pushTask.ToCoroutine(
				_ => { },
				exception => observedException = exception);

			Assert.That(
				observedException,
				Is.TypeOf<OperationCanceledException>(),
				observedException?.ToString());
			Assert.That(stack.Count, Is.Zero);
		}
	}

	internal enum DestructionTestResult
	{
		Dismissed
	}

	internal sealed class DestructionTestWindow : WindowBase<DestructionTestResult>
	{
		protected override DestructionTestResult DismissResult => DestructionTestResult.Dismissed;
	}
}
#endif
