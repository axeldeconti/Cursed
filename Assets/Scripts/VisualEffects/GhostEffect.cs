using UnityEngine;

namespace Cursed.VisualEffect
{
    public class GhostEffect : MonoBehaviour
    {
        public GameObject _ghost;

        [SerializeField] private FloatReference _ghostDelay;
        private float _ghostDelaySeconde;

        private void Start()
        {
            _ghostDelaySeconde = _ghostDelay;
        }
        public void GhostDashEffect()
        {
            if (_ghostDelaySeconde > 0)
            {
                _ghostDelaySeconde -= Time.deltaTime;
            }
            else
            {
                //Generate a ghost
                GameObject currentGhost = Instantiate(_ghost, transform.position, transform.rotation);
                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                currentGhost.transform.localScale = this.transform.localScale;
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                _ghostDelaySeconde = _ghostDelay;
                Destroy(currentGhost, 0.5f);
            }
        }
    }
}
