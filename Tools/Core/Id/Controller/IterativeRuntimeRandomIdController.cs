using System;

namespace GothmogToolkit.Tools.Core.Id.Controller
{
	public class IterativeRuntimeRandomIdController : IdController<int>
	{
		protected virtual int Seed => 3512565;
		private Random _random;

		private Random Random
		{
			get
			{
				_random ??= new Random(Seed);
				return _random;
			}
		}

		protected override int GetNextId(int id) => Random.Next(1, int.MaxValue - 1);

	}
}