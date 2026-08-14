using GothmogToolkit.Tools.Helpers.Extensions;
using UnityEngine;

namespace GothmogToolkit.Tools.Grids
{
    public class GridHandler2D : MonoBehaviour
    {
        [SerializeField] private Grid _grid;
        [SerializeField] private int _gizmosGridSize = 10;
        [SerializeField] private float _gizmoSphereRadius = 0.1f;

        public Grid Grid => _grid;

        public Vector2Int GetGridPosition(Vector3 localPosition) =>
            _grid.LocalToCellInterpolated(localPosition).ToVector2Int();

        public Vector3 GetLocalPosition(Vector2Int gridPosition)
        {
            return _grid.CellToLocal(gridPosition.ToVector3Int());
        }

        private void OnDrawGizmosSelected()
        {
            for (var i = 0; i < _gizmosGridSize; i++)
            {
                for (var j = 0; j < _gizmosGridSize; j++)
                {
                    Gizmos.DrawSphere(_grid.transform.position + _grid.CellToLocal(new Vector3Int(i, j, 0)),
                        _gizmoSphereRadius);
                }
            }
        }

        public void UpdateChildPositions()
        {
            var children = GetComponentsInChildren<GridPositioner>();

            foreach (var child in children)
            {
                var position = _grid.LocalToCellInterpolated(child.GetPosition());
                var cellPosition = new Vector3Int(Mathf.RoundToInt(position.x), Mathf.RoundToInt(position.y), 0);
                child.SetPosition(_grid.CellToLocal(cellPosition));
            }
        }

        public Vector3 CellToLocal(Vector3Int vector3Int)
        {
            return _grid.CellToLocal(vector3Int);
        }
    }
}
