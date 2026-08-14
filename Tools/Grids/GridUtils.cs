using System.Collections.Generic;
#if LINQFASTER
using JM.LinqFaster;
#endif


namespace GothmogToolkit.Tools.Grids
{
	public abstract class GridUtils<TVector, TDirection> where TVector : struct where TDirection : struct
	{
		protected static GridUtils<TVector, TDirection> _instance;
		private Dictionary<TVector, TDirection> VectorToDirection { get; }
		protected (TDirection name, TVector value)[] Directions { get; }
		public abstract TDirection UndefinedDirection { get; }

		protected GridUtils((TDirection name, TVector value)[] directions)
		{
			var count = directions.Length;
			Directions = directions;
			VectorToDirection = new Dictionary<TVector, TDirection>(count);
			foreach (var pair in directions)
			{
				VectorToDirection[pair.value] = pair.name;
			}
		}

		public TDirection GetDirection(TVector vector) => VectorToDirection[vector];

		public TVector GetDirectionVector(TDirection direction)
		{
			return Directions[GetDirectionIndex(direction)].value;
		}

		protected abstract int GetDirectionIndex(TDirection direction);

		public abstract TVector GetPositionInDirection(TVector currIndex, TDirection direction, int steps = 1);
		public abstract TVector GetAdjacentPositionDirection(TVector source, TDirection direction);
		
		public IReadOnlyList<TVector> GetAdjacentPositions(TVector source)
		{
#if LINQFASTER
			return Directions
				.WhereF(x => !x.name.Equals(UndefinedDirection))
				.SelectF(direction => GetPositionInDirection(source, direction.name));
#else
			return Directions
				.Where(x => !x.name.Equals(UndefinedDirection))
				.Select(direction => GetPositionInDirection(source, direction.name)).ToList();
#endif
		}

		public IReadOnlyList<(TDirection name, TVector value)> GetAdjacentDirectionPositionPairs(
			TVector source)
		{
#if LINQFASTER
			return Directions
				.WhereF(x => !x.name.Equals(UndefinedDirection))
				.SelectF(
					direction => (direction.name, GetPositionInDirection(source, direction.name)));
#else
			return Directions
				.Where(x => !x.name.Equals(UndefinedDirection))
				.Select(direction => (direction.name, GetPositionInDirection(source, direction.name)))
				.ToList();
#endif
		}
	}
}
