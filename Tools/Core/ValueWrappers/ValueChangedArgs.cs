namespace GothmogToolkit.Tools.Core.ValueWrappers
{
	public struct ValueChangedArgs<TType>
	{
		public TType OldValue { get; }
		public TType NewValue { get; }
	
		public ValueChangedArgs(TType oldValue, TType newValue)
		{
			OldValue = oldValue;
			NewValue = newValue;
		}
	}
}