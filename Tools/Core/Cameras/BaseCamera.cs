using UnityEngine;

namespace Tools.Core.Cameras
{
    [RequireComponent(typeof(Camera))]
    public abstract class BaseCamera : MonoBehaviour
    {
        private Camera _camera;

        public Camera Camera
        {
            get
            {
                if (!_camera)
                    _camera = GetComponent<Camera>();
                return _camera;
            }
        }

        public static implicit operator Camera(BaseCamera camera) => camera.Camera;
    }
}
