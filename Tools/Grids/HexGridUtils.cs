using System.Collections.Generic;
using UnityEngine;

namespace GothmogToolkit.Tools.Grids
{
	public class HexGridUtils : GridUtils<Vector2Int, HexGridUtils.Direction>
	{
		public static GridUtils<Vector2Int, Direction> Instance
		{
			get
			{
				_instance ??= new HexGridUtils(new[]
				{
					(Direction.Undefined, new Vector2Int(0, 0)),
					(Direction.UpLeft, new Vector2Int(0, 1)),
					(Direction.UpRight, new Vector2Int(1, 1)),
					(Direction.Right, new Vector2Int(1, 0)),
					(Direction.DownRight, new Vector2Int(1, -1)),
					(Direction.DownLeft, new Vector2Int(-1, -1)),
					(Direction.Left, new Vector2Int(-1, 0)),
				});
				return _instance;
			}
		}

		public override Direction UndefinedDirection => Direction.Undefined;

		private HexGridUtils((Direction directionName, Vector2Int directionValue)[] directions) : base(directions)
		{
		}

		protected override int GetDirectionIndex(Direction direction) => (int)direction;

		public override Vector2Int GetPositionInDirection(Vector2Int currIndex, Direction direction, int steps = 1)
		{
			var result = currIndex;
			for (var i = 0; i < steps; i++)
			{
				var directionVector = GetAdjacentPositionDirection(currIndex, direction);
				result += directionVector;
			}

			return result;
		}

		public override Vector2Int GetAdjacentPositionDirection(Vector2Int source, Direction direction)
		{
			return direction switch
			{
				Direction.Undefined => Vector2Int.zero,
				Direction.UpLeft => source.y % 2 == 0 ? new Vector2Int(-1, 1) : new Vector2Int(0, 1),
				Direction.UpRight => source.y % 2 == 0 ? new Vector2Int(0, 1) : new Vector2Int(1, 1),
				Direction.Right => Vector2Int.right,
				Direction.DownRight => source.y % 2 == 0 ? new Vector2Int(0, -1) : new Vector2Int(1, -1),
				Direction.DownLeft => source.y % 2 == 0 ? new Vector2Int(-1, -1) : new Vector2Int(0, -1),
				Direction.Left => Vector2Int.left,
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