using UnityEngine;
using Cursed.Character;

namespace Cursed.UI
{
    public class EnemyHealthBarFlip : MonoBehaviour
    {
        private CharacterMovement _characterMovementParent;
        private RectTransform _rectTransform;
        private float _initialXScale;

        private void Awake()
        {
            _characterMovementParent = GetComponentInParent<CharacterMovement>();
            _rectTransform = GetComponent<RectTransform>();
            _initialXScale = _rectTransform.localScale.x;
        }

        private void Update()
        {
            UpdateScale();
        }

        private void UpdateScale()
        {
            if (_characterMovementParent == null)
                return;

            if(_characterMovementParent.Side == 1)
                _rectTransform.localScale = new Vector3(_initialXScale, _rectTransform.localScale.y, _rectTransform.localScale.z);
            else if(_characterMovementParent.Side == -1)
                _rectTransform.localScale = new Vector3(-_initialXScale, _rectTransform.localScale.y, _rectTransform.localScale.z);
        }
    }
}

