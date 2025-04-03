using System.Collections.Generic;
using UnityEngine;

namespace GothmogToolkit.Tools.Grids
{
	public class Grid8DUtils : GridUtils<Vector2Int, Grid8DUtils.Direction>
	{
		public static GridUtils<Vector2Int, Direction> Instance
		{
			get
			{
				_instance ??= new Grid8DUtils(new[]
				{
					(Direction.Undefined, new Vector2Int(0, 0)),
					(Direction.Left, new Vector2Int(-1, 0)),
					(Direction.UpLeft, new Vector2Int(-1, 1)),
					(Direction.Up, new Vector2Int(0, 1)),
					(Direction.UpRight, new Vector2Int(1, 1)),
					(Direction.Right, new Vector2Int(1, 0)),
					(Direction.DownRight, new Vector2Int(1, -1)),
					(Direction.Down, new Vector2Int(0, -1)),
					(Direction.DownRight, new Vector2Int(-1, -1)),
				});
				return _instance;
			}
		}

		public override Direction UndefinedDirection => Direction.Undefined;

		private Grid8DUtils((Direction directionName, Vector2Int directionValue)[] directions) : base(directions)
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
			=> Directions[(int)direction].directionValue;

		public enum Direction
		{
			Undefined,
			Left,
			UpLeft,
			Up,
			UpRight,
			Right,
			DownRight,
			Down,
			DownLeft,
		}
	}
}