namespace GothmogToolkit.Tools.Core.ValueWrappers
{
	public struct ValueChangedEventArgs<TType>
	{
		public TType OldValue { get; }
		public TType NewValue { get; }
	
		public ValueChangedEventArgs(TType oldValue, TType newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}