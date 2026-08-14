using UnityEngine;

namespace GothmogToolkit.Tools.Grids
{
    public class GridPositioner : MonoBehaviour
    {
        public Vector3 GetPosition() => transform.localPosition;

        public void SetPosition(Vector3 vector3)
        {
            transform.localPosition = vector3;
        }
    }
}