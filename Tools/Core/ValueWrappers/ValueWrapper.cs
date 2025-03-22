using System;

namespace GothmogToolkit.Tools.Core.ValueWrappers
{
	[Serializable]
	public class ValueWrapper<TType> : IValueWrapper<TType>
	{
		protected TType _value;
		public virtual TType Value
		{
			get => GetValue();
			set => SetValue(value);
		}

		public event Action<ValueChangedEventArgs<TType>> ValueChanged;

		/// <summary>
		/// Returns true if value has changed
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		protected virtual bool SetValue(TType value)
		{
			if (value.Equals(_value))
				return false;

			var oldValue = _value;
			_value = value;

			ValueChanged?.Invoke(new ValueChangedEventArgs<TType>(oldValue, _value));
			return true;
		}

		protected virtual TType GetValue() => _value;
	}
}