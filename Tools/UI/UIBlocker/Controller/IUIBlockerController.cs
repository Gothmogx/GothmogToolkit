using System;

namespace GothmogToolkit.Tools.UI.UIBlocker
{
	/// <summary>
	/// Handles blocking ui events
	/// </summary>
	public interface IUIBlockerController
	{
		bool IsBlocked { get; }
		event Action<bool> BlockedChanged;

		/// <returns>true if blocking succeed</returns>
		bool TryBlock();

		/// <returns>true if unblocking succeed</returns>
		bool TryUnblock();
	}
}