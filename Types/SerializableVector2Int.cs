using System;
using UnityEngine;

namespace GothmogToolkit.Types
{
	///Lightweight version of Unity Vector2Int
	[Serializable]
	public struct SerializableVector2Int : IEquatable<SerializableVector2Int>
	{
		public int x;
		public int y;

		public SerializableVector2Int(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
		public SerializableVector2Int(Vector2Int vector2Int)
		{
			x = vector2Int.x;
			y = vector2Int.y;
		}
		public static implicit operator SerializableVector2Int(Vector2Int vector) =>
			new SerializableVector2Int(vector);
		public static implicit operator Vector2Int(SerializableVector2Int vector) =>
			new Vector2Int(vector.x,vector.y);

		public bool Equals(SerializableVector2Int other) => x == other.x && y == other.y;

		public override bool Equals(object obj) => obj is SerializableVector2Int other && Equals(other);

		public override int GetHashCode() => HashCode.Combine(x, y);
	}
}