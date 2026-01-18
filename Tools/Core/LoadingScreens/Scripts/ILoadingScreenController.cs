using Cysharp.Threading.Tasks;
using JetBrains.Annotations;

namespace GothmogToolkit.Tools.Core.LoadingScreens.Scripts
{
	public interface ILoadingScreenController
	{
		UniTask Show([CanBeNull] ILoadingScreen loadingScreen);
		UniTask Show();
		UniTask Hide();
	}
}