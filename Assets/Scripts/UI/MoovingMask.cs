using UnityEngine;

namespace Cursed.UI
{
    public class MoovingMask : MonoBehaviour
    {
        private RectTransform _maskTransform = null;
        private RectTransform _barTransform = null;
        private float _maxPos = 0;
        private float _currentPos = 1;

        private void Start()
        {
            _maskTransform = (RectTransform)transform;
            _barTransform = (RectTransform)transform.GetChild(0).transform;
            _maxPos = transform.localPosition.x - 16;
        }

        public void SetPos(float pos)
        {
            _maskTransform.anchoredPosition = new Vector2(- 1 * (1 - pos) * _maxPos, _maskTransform.anchoredPosition.y);
            _barTransform.anchoredPosition = new Vector2((1 - pos) * _maxPos, _barTransform.anchoredPosition.y);
            _currentPos = pos;
        }

        public float Pos => _currentPos;
    }
}