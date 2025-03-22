using System.Collections.Generic;
using UnityEngine;
#if LINQFASTER
using JM.LinqFaster
#endif

namespace GothmogToolkit.Tools.Grids
{
	public class HexGridUtils : GridUtils<Vector2Int, HexGridUtils.Direction>
	{
		public static GridUtils<Vector2Int, Direction> Instance
		{
			get
			{
				_instance ??= new HexGridUtils();
				return _instance;
			}
		}

		public override Direction UndefinedDirection => Direction.Undefined;

		protected override List<Vector2Int> Directions { get; } = new()
		{
			new Vector2Int(0, 0),
			new Vector2Int(0, 1),
			new Vector2Int(1, 1),
			new Vector2Int(1, 0),
			new Vector2Int(1, -1),
			new Vector2Int(-1, -1),
			new Vector2Int(-1, 0),
		};

		protected override List<Direction> DirectionNames { get; } = new()
		{
			Direction.Undefined,
			Direction.UpLeft,
			Direction.UpRight,
			Direction.Right,
			Direction.DownRight,
			Direction.DownLeft,
			Direction.Left
		};

		protected override Dictionary<Vector2Int, Direction> VectorToDirection { get; } = new()
		{
			{ new Vector2Int(0, 0), Direction.Undefined },
			{ new Vector2Int(0, 1), Direction.UpLeft },
			{ new Vector2Int(1, 1), Direction.UpRight },
			{ new Vector2Int(1, 0), Direction.Right },
			{ new Vector2Int(1, -1), Direction.DownRight },
			{ new Vector2Int(-1, -1), Direction.DownLeft },
			{ new Vector2Int(-1, 0), Direction.Left },
		};

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