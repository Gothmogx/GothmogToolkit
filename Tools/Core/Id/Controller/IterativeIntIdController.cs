namespace GothmogToolkit.Tools.Core.Id.Controller
{
	public class IterativeIntIdController : IdController<int>
	{ 
		protected override int GetNextId(int id) => ++id;
	}
}