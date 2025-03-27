namespace GothmogToolkit.Tools.Core.Id.Controller
{
	public interface IIdController<T>
	{
		public bool Register(IIdContainer<T> source);
	}
}