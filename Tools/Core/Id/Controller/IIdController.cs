namespace GothmogToolkit.Tools.Core.Id
{
	public interface IIdController<T>
	{
		public bool Register(IIdContainer<T> source);
	}
}