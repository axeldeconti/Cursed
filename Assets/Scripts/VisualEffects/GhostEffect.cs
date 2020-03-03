using UnityEngine;

namespace Cursed.VisualEffect
{
    public class GhostEffect : MonoBehaviour
    {
        public GameObject _ghost;

        [SerializeField] private FloatReference _ghostDelayInterval;
        [SerializeField] private FloatReference _timeGhostVisible;
        private float _ghostDelaySeconde;

        private void Start()
        {
            _ghostDelaySeconde = _ghostDelayInterval;
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
                _ghostDelaySeconde = _ghostDelayInterval;
                Destroy(currentGhost, _timeGhostVisible);
            }
        }
    }
}
