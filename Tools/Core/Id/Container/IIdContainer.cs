namespace GothmogToolkit.Tools.Core.Id
{
	public interface IIdContainer<T> 
	{
		T Id { get; }
		void SetId(T id);
		bool IsSet() => !Id.Equals(default);
	}
}