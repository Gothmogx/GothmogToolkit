using System;
using UnityEngine;

namespace GothmogToolkit.Tools.Core.ValueWrappers
{
	public class ScriptableValueWrapperDecorator<TType> : ScriptableObject, IValueWrapper<TType>
	{
		[SerializeField] private ValueWrapper<TType> _wrapper;

		public TType Value
		{
			get => _wrapper.Value;
			set => _wrapper.Value = value;
		}

		public event Action<ValueChangedEventArgs<TType>> ValueChanged
		{
			add => _wrapper.ValueChanged += value;
			remove => _wrapper.ValueChanged -= value;
		}
	}
}