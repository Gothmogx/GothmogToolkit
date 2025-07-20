using System;
using System.Diagnostics.CodeAnalysis;

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

		public event Action<ValueChangedArgs<TType>> ValueChanged;

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

			ValueChanged?.Invoke(new ValueChangedArgs<TType>(oldValue, _value));
			return true;
		}

		public bool Set(TType newValue)
		{
			var previousValue = _value;
			if (_value.Equals(newValue)) return false;
			ValueChanged?.Invoke(new ValueChangedArgs<TType>(_value, newValue));
			_value = newValue;
			return true;
		}
		public bool Set([NotNull]Func<TType,TType> predicate)
		{
			return Set(predicate(Value));
		}
		
		protected virtual TType GetValue() => _value;
	}
}