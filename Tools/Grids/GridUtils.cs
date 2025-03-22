using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace GothmogToolkit.Tools.Grids
{
	public abstract class GridUtils<TVector, TDirection> where TVector : struct where TDirection : struct
	{
		protected static GridUtils<TVector, TDirection> _instance;
		protected abstract List<TVector> Directions { get; }
		protected abstract List<TDirection> DirectionNames { get; }
		protected abstract Dictionary<TVector, TDirection> VectorToDirection { get; }
		public abstract TDirection UndefinedDirection { get; }

		public TDirection GetDirection(TVector vector) => VectorToDirection[vector];

		public TVector GetDirectionVector(TDirection direction)
		{
			return Directions[GetDirectionIndex(direction)];
		}

		protected abstract int GetDirectionIndex(TDirection direction);

		public abstract TVector GetPositionInDirection(TVector currIndex, TDirection direction, int steps = 1);
		public abstract TVector GetAdjacentPositionDirection(TVector source, TDirection direction);


		public IReadOnlyList<TVector> GetAdjacentPositions(TVector source)
		{
#if LINQFASTER
			return DirectionNames.WhereF(x =>  !x.Equals(UndefinedDirection))
				.SelectF(direction => GetIndexInDirection(source, direction));
#else
			return DirectionNames.Where(x => !x.Equals(UndefinedDirection))
				.Select(direction => GetPositionInDirection(source, direction)).ToList();
#endif
		}

		public IReadOnlyList<(TDirection, TVector)> GetAdjacentPositionsWithDirections(TVector source)
		{
#if LINQFASTER
			return HexGridUtils.DirectionNames.WhereF(x => x != UndefinedDirection)
				.SelectF(direction => (direction, GetIndexInDirection(source, direction)));
#else
			return DirectionNames.Where(x => !x.Equals(UndefinedDirection))
				.Select(direction => (direction, GetPositionInDirection(source, direction))).ToList();
#endif
		}
	}
}