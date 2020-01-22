using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Cursed.UI
{
    public class UpdateFilledImageVisual : MonoBehaviour
    {
        private Image _filledBar;
        private Color _initialColor;
        private Sprite _initialSpriteImage;

        [SerializeField] private Color _healColor;
        [SerializeField] private Sprite _healSpriteImage;

        private void Awake()
        {
            _filledBar = GetComponent<Image>();
            _initialColor = _filledBar.color;
            _initialSpriteImage = _filledBar.sprite;
        }

        public void ChangeVisual()
        {
            _filledBar.color = _healColor;

            if (_healSpriteImage != null)
                _filledBar.sprite = _healSpriteImage;

            StartCoroutine(WaitForReset());
        }

        public void ResetToInitial()
        {
            _filledBar.color = _initialColor;
            _filledBar.sprite = _initialSpriteImage;
        }

        private IEnumerator WaitForReset()
        {
            yield return new WaitForSeconds(1f);
            ResetToInitial();
        }
    }
}
