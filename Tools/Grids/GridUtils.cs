using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GothmogToolkit.Tools.Grids
{
	public abstract class GridUtils<TDirection> where TDirection : struct
	{
		protected static GridUtils<TDirection> _instance;
		public abstract TDirection UndefinedDirection { get; }

		public abstract List<Vector2Int> Directions { get; }

		public abstract List<TDirection> DirectionNames { get; }

		protected abstract Dictionary<Vector2Int, TDirection> VectorToDirection { get; }

		public TDirection GetDirection(Vector2Int vector) => VectorToDirection[vector];
		
		public Vector2Int GetDirection(TDirection direction)
		{
			return Directions[GetDirectionIndex(direction)];
		}

		protected abstract int GetDirectionIndex(TDirection direction);

		public Vector2Int GetIndexInDirection(Vector2Int currIndex, TDirection direction, int steps = 1)
		{
			var result = currIndex;
			for (var i = 0; i < steps; i++)
			{
				var directionVector = GetAdjacentDirection(currIndex, direction);
				result += directionVector;
			}

			return result;
		}

		public abstract Vector2Int GetAdjacentDirection(Vector2Int source, TDirection direction);


		public IReadOnlyList<Vector2Int> GetAdjacentPositions(Vector2Int source)
		{
#if LINQFASTER
			return DirectionNames.WhereF(x =>  !x.Equals(UndefinedDirection))
				.SelectF(direction => GetIndexInDirection(source, direction));
#else
			return DirectionNames.Where(x => !x.Equals(UndefinedDirection))
				.Select(direction => GetIndexInDirection(source, direction)).ToList();
#endif
		}

		public IReadOnlyList<(TDirection, Vector2Int)> GetAdjacentPositionsWithDirections(Vector2Int source)
		{
#if LINQFASTER
			return HexGridUtils.DirectionNames.WhereF(x => x != UndefinedDirection)
				.SelectF(direction => (direction, GetIndexInDirection(source, direction)));
#else
			return DirectionNames.Where(x => !x.Equals(UndefinedDirection))
				.Select(direction => (direction, GetIndexInDirection(source, direction))).ToList();
#endif
		}
	}
}