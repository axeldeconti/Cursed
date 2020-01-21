using UnityEngine;
using UnityEngine.UI;

namespace Cursed.UI
{
    public class CutBarTemplate : MonoBehaviour
    {
        private float _timer;
        private Image _barImage;

        private void Awake()
        {
            _barImage = GetComponent<Image>();
            _timer = 1f;
        }

        private void Update()
        {
            _timer -= Time.deltaTime;
            if(_timer < 0)
            {
                float shrinkSpeed = .5f;
                _barImage.fillAmount -= shrinkSpeed * Time.deltaTime;
                if (_barImage.fillAmount <= 0)
                    Destroy(this.gameObject);
            }
        }
    }
}

