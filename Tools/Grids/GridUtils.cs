using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

#if LINQFASTER
using JM.LinqFaster;
#endif


namespace GothmogToolkit.Tools.Grids
{
	public abstract class GridUtils<TVector, TDirection> where TVector : struct where TDirection : struct
	{
		protected static GridUtils<TVector, TDirection> _instance;
		private Dictionary<TVector, TDirection> VectorToDirection { get; }
		protected (TDirection directionName, TVector directionValue)[] Directions { get; }
		public abstract TDirection UndefinedDirection { get; }

		protected GridUtils((TDirection directionName, TVector directionValue)[] directions)
		{
			var count = directions.Length;
			Directions = directions;
			VectorToDirection = new Dictionary<TVector, TDirection>(count);
			foreach (var pair in directions)
			{
				VectorToDirection[pair.directionValue] = pair.directionName;
			}
		}

		public TDirection GetDirection(TVector vector) => VectorToDirection[vector];

		public TVector GetDirectionVector(TDirection direction)
		{
			return Directions[GetDirectionIndex(direction)].directionValue;
		}

		protected abstract int GetDirectionIndex(TDirection direction);

		public abstract TVector GetPositionInDirection(TVector currIndex, TDirection direction, int steps = 1);
		public abstract TVector GetAdjacentPositionDirection(TVector source, TDirection direction);


		public IReadOnlyList<TVector> GetAdjacentPositions(TVector source)
		{
#if LINQFASTER
			return Directions
				.WhereF(x => !x.directionName.Equals(UndefinedDirection))
				.SelectF(direction => GetPositionInDirection(source, direction.directionName));
#else
			return Directions
				.Where(x => !x.directionName.Equals(UndefinedDirection))
				.Select(direction => GetPositionInDirection(source, direction.directionName)).ToList();
#endif
		}
	}

}