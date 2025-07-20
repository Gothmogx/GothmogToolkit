using System;

namespace GothmogToolkit.Tools.Core.ValueWrappers
{
	/// <summary>
	/// Values wrapper with OnValueChanged support
	/// </summary>
	/// <typeparam name="TType"></typeparam>
	public interface IValueWrapper<TType>
	{
		TType Value { get; set; }
		event Action<ValueChangedArgs<TType>> ValueChanged;
	}
}