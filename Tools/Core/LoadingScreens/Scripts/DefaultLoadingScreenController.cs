#if VCONTAINER && UNITASK
using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer.Unity;

namespace GothmogToolkit.Tools.Core.LoadingScreens.Scripts
{
	public class DefaultLoadingScreenController : MonoBehaviour, ILoadingScreenController, IInitializable
	{
		[SerializeField] private Camera _camera;
		[SerializeField] private LayerMask _loadingLayers;
		[SerializeField] private BlackScreenFading _defaultLoadingScreen;

		private ILoadingScreen _activeLoadingScreen;

		public void Initialize()
		{
			_camera.cullingMask = _loadingLayers;
			_camera.gameObject.SetActive(false);
			_defaultLoadingScreen.SetActive(false);
		}
		
		public UniTask Show(ILoadingScreen loadingScreen)
		{
			_activeLoadingScreen = loadingScreen;
			return Show();
		}

		public UniTask Show()
		{
			_activeLoadingScreen ??= _defaultLoadingScreen;
			try
			{
				_camera.gameObject.SetActive(true);
				return _activeLoadingScreen.Show();
			}
			catch (OperationCanceledException e) { }
			catch (Exception e)
			{
				Debug.LogError("Failed to show loading screen");
				Debug.LogException(e);
				_activeLoadingScreen = null;
				_camera.gameObject.SetActive(false);
			}
			return UniTask.CompletedTask;
		}
		public async UniTask Hide()
		{
			try
			{
				await _activeLoadingScreen.Hide();
				_camera.gameObject.SetActive(true);
			}
			catch (OperationCanceledException e) { }
			catch (Exception e)
			{
				Debug.LogError("Failed to hide loading screen");
				Debug.LogException(e);
				_activeLoadingScreen = null;
				_camera.gameObject.SetActive(false);
			}
		}
	}
}
#endif