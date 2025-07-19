namespace GothmogToolkit.Tools.Core.Id.Controller
{
	public class IterativeUintIdController : IdController<uint>
	{ 
		protected override uint GetNextId(uint id) => ++id;
	}
}