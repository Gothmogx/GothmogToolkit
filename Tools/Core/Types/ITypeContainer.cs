namespace GothmogToolkit.Tools.Core.Types
{
	public interface ITypeContainer<out T>
	{
		T Type { get; }
	}
}