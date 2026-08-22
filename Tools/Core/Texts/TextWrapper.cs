#if TextMeshPro
using TMPro;
using UnityEngine;
#endif

namespace Tools.Core.Texts
{
#if TextMeshPro
    [RequireComponent(typeof(TMP_Text))]
    public class TextWrapper : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public string Text
        {
            get => _text.text;
            set => _text.text = value;
        }

        public Color Color
        {
            get => _text.color;
            set => _text.color = value;
        }

        private void Reset()
        {
            _text = GetComponent<TMP_Text>();
            if (!_text)
                Debug.LogError("No TMP_Text component attached to this UiManager");
        }

        public Vector2 GetPreferredSizeDelta(string text)
        {
            var baseSize = _text.GetPreferredValues(text, width: 400, height: float.PositiveInfinity);
            baseSize.x += _text.margin.x + _text.margin.z;
            baseSize.y += _text.margin.y + _text.margin.w;
            return baseSize;
        }

        public void SetSizeDelta(Vector2 dimensions)
        {
            _text.rectTransform.sizeDelta = new Vector2(dimensions.x, dimensions.y);
        }
    }
#else
     public class TextWrapper : MonoBehaviour
    {
    }
#endif
}
