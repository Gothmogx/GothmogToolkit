using UnityEngine;

namespace Tools.Core.Canvases
{
    [RequireComponent(typeof(Canvas))]
    public class BaseCanvas : MonoBehaviour
    {
        private Canvas _canvas;

        public Canvas Canvas
        {
            get
            {
                if (!_canvas)
                    _canvas = GetComponent<Canvas>();
                return _canvas;
            }
        }

        public void SetCamera(Camera canvasCamera) => Canvas.worldCamera = canvasCamera;
        public static implicit operator Canvas (BaseCanvas canvas) => canvas.Canvas;
    }
}
