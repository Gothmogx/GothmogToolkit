using UnityEngine;

namespace GothmogToolkit.Tools.Helpers.Extensions
{
	public static class VectorExtensions
	{
		public static Vector2Int ToVector2Int(this Vector3Int vector3Int) => new(vector3Int.x, vector3Int.y);
		public static Vector3Int ToVector3Int(this Vector2Int vector2Int) => new(vector2Int.x, vector2Int.y);
	}
}