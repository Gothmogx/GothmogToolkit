using System;

namespace GothmogToolkit.UIBlocker
{
	public class UIBlockerController : IUIBlockerController
	{
		public bool IsBlocked { private set; get; }
		private bool CanBeBlocked => !IsBlocked;
		private bool CanBeUnblocked => IsBlocked;

		public event Action<bool> BlockedChanged;

		public bool TryBlock()
		{
			if (!CanBeBlocked)
				return false;
			SetBlocked(true);
			return true;
		}

		public bool TryUnblock()
		{
			if (!CanBeUnblocked)
				return false;
			SetBlocked(false);
			return true;
		}

		private void SetBlocked(bool isBlocked)
		{
			if (isBlocked == IsBlocked)
				return;
			IsBlocked = isBlocked;
			BlockedChanged?.Invoke(IsBlocked);
		}
	}
}