using System;
using UnityEngine;

namespace GothmogToolkit.Types
{
	///Lightweight version of Unity Vector2
	[Serializable]
	public struct SerializableVector2 : IEquatable<SerializableVector2>
	{
		public float x;
		public float y;

		public SerializableVector2(int x, int y)
		{
			this.x = x;
			this.y = y;
		}
		public SerializableVector2(Vector2 vector2)
		{
			x = vector2.x;
			y = vector2.y;
		}
		public static implicit operator SerializableVector2(Vector2 vector) =>
			new SerializableVector2(vector);
		public static implicit operator Vector2(SerializableVector2 vector) =>
			new Vector2(vector.x,vector.y);

		public bool Equals(SerializableVector2 other) => x == other.x && y == other.y;

		public override bool Equals(object obj) => obj is SerializableVector2 other && Equals(other);

		public override int GetHashCode() => HashCode.Combine(x, y);
	}
}