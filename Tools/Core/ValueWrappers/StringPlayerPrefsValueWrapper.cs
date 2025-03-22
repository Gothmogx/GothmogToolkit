using UnityEngine;

namespace GothmogToolkit.Tools.Core.ValueWrappers
{
	public abstract class StringPlayerPrefsValueWrapper : ValueWrapper<string>
	{
		protected abstract string PlayerPrefsKey { get; }
		protected override bool SetValue(string value)
		{
			if (!base.SetValue(value)) 
				return false;
			
			PlayerPrefs.SetString(PlayerPrefsKey, value);
			return true;
		}
		protected override string GetValue()
		{
			_value = PlayerPrefs.GetString(PlayerPrefsKey, string.Empty);
			return _value;
		}
	}
}