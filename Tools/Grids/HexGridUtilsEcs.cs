using System;
using Unity.Mathematics;

namespace GothmogToolkit.Tools.Grids
{
	public class HexGridUtilsEcs : GridUtils<int2, HexGridUtilsEcs.Direction>
	{
		public static GridUtils<int2, Direction> Instance
		{
			get
			{
				_instance ??= new HexGridUtilsEcs(new[]
				{
					(Direction.Undefined, new int2(0, 0)),
					(Direction.UpLeft, new int2(0, 1)),
					(Direction.UpRight, new int2(1, 1)),
					(Direction.Right, new int2(1, 0)),
					(Direction.DownRight, new int2(1, -1)),
					(Direction.DownLeft, new int2(-1, -1)),
					(Direction.Left, new int2(-1, 0)),
				});
				return _instance;
			}
		}

		public override Direction UndefinedDirection => Direction.Undefined;

		private HexGridUtilsEcs((Direction name, int2 value)[] directions) : base(directions)
		{
		}

		protected override int GetDirectionIndex(Direction direction) => (int)direction;

		public override int2 GetPositionInDirection(int2 currIndex, Direction direction, int steps = 1)
		{
			var result = currIndex;
			for (var i = 0; i < steps; i++)
			{
				var directionVector = GetAdjacentPositionDirection(currIndex, direction);
				result += directionVector;
			}

			return result;
		}

		public override int2 GetAdjacentPositionDirection(int2 source, Direction direction)
		{
			return direction switch
			{
				Direction.Undefined => int2.zero,
				Direction.UpLeft => source.y % 2 == 0 ? new int2(-1, 1) : new int2(0, 1),
				Direction.UpRight => source.y % 2 == 0 ? new int2(0, 1) : new int2(1, 1),
				Direction.Right => new int2(1, 0),
				Direction.DownRight => source.y % 2 == 0 ? new int2(0, -1) : new int2(1, -1),
				Direction.DownLeft => source.y % 2 == 0 ? new int2(-1, -1) : new int2(0, -1),
				Direction.Left => new int2(-1, 0),
				_ => source
			};
		}

		public enum Direction
		{
			Undefined,
			UpLeft,
			UpRight,
			Right,
			DownRight,
			DownLeft,
			Left
		}
	}
}