using System.Collections.Generic;
using UnityEngine;

namespace GothmogToolkit.Tools.Grids
{
	public class CrossGridUtils : GridUtils<CrossGridUtils.Direction>
	{
		public static GridUtils<Direction> Instance
		{
			get
			{
				_instance ??= new CrossGridUtils();
				return _instance;
			}
		}

		public override Direction UndefinedDirection => Direction.Undefined;

		public override List<Vector2Int> Directions { get; } = new()
		{
			new Vector2Int(0, 0),
			new Vector2Int(-1, 0),
			new Vector2Int(0, 1),
			new Vector2Int(1, 0),
			new Vector2Int(0, -1),
		};

		protected override Dictionary<Vector2Int, Direction> VectorToDirection { get; } = new()
		{
			{ new Vector2Int(0, 0), Direction.Undefined },
			{ new Vector2Int(-1, 0), Direction.Left },
			{ new Vector2Int(0, 1), Direction.Up },
			{ new Vector2Int(1, 0), Direction.Right },
			{ new Vector2Int(0, -1), Direction.Down },
		};

		public override List<Direction> DirectionNames { get; } = new()
		{
			Direction.Undefined,
			Direction.Left,
			Direction.Up,
			Direction.Right,
			Direction.Down,
		};

		protected override int GetDirectionIndex(Direction direction) => (int)direction;

		public override Vector2Int GetAdjacentDirection(Vector2Int source, Direction direction)
		{
			return direction switch
			{
				Direction.Undefined => Vector2Int.zero,
				Direction.Left => new Vector2Int(-1, 0),
				Direction.Up => new Vector2Int(0, 1),
				Direction.Right => new Vector2Int(1, 0),
				Direction.Down => new Vector2Int(0, -1),
				_ => source
			};
		}

		public enum Direction
		{
			Undefined,
			Left,
			Up,
			Right,
			Down
		}
	}
}