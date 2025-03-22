namespace GothmogToolkit.Tools.Core.Id
{
	public class IterativeUintIdController : IdController<uint>
	{ 
		protected override uint GetNextId(uint id) => ++id;
	}
}