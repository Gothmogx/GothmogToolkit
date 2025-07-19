namespace GothmogToolkit.Tools.Core.Id.Controller
{
	public class IterativeRuntimeIntIdController : IdController<int>
	{ 
		protected override int GetNextId(int id) => ++id;
	}
}