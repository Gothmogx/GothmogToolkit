using Cysharp.Threading.Tasks;

namespace GothmogToolkit.Tools.Core.LoadingScreens.Scripts
{
	public interface ILoadingScreen
	{
		UniTask Show();
		UniTask Hide();
		void SetActive(bool active);
	}
}