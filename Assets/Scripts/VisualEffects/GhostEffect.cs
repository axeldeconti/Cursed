using UnityEngine;

namespace Cursed.VisualEffect
{
    public class GhostEffect : MonoBehaviour
    {
        public GameObject _ghostDash;
        public GameObject _ghostDivekick;

        [SerializeField] private FloatReference _ghostDelayInterval;
        [SerializeField] private FloatReference _timeGhostDashVisible;
        [SerializeField] private FloatReference _timeGhostDivekickVisible;
        private float _ghostDashDelaySeconde;
        private float _ghostDivekickDelaySeconde;

        private void Start()
        {
            _ghostDashDelaySeconde = _ghostDelayInterval;
            _ghostDivekickDelaySeconde = _ghostDelayInterval;
        }
        public void GhostDashEffect()
        {
            if (_ghostDashDelaySeconde > 0)
            {
                _ghostDashDelaySeconde -= Time.deltaTime;
            }
            else
            {
                //Generate a ghost
                GameObject currentGhost = Instantiate(_ghostDash, transform.position, transform.rotation);
                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                currentGhost.transform.localScale = this.transform.localScale;
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                _ghostDashDelaySeconde = _ghostDelayInterval;
                Destroy(currentGhost, _timeGhostDashVisible);
            }
        }

        public void GhostDivekickEffect()
        {
            if (_ghostDivekickDelaySeconde > 0)
            {
                _ghostDivekickDelaySeconde -= Time.deltaTime;
            }
            else
            {
                //Generate a ghost
                GameObject currentGhost = Instantiate(_ghostDivekick, transform.position, transform.rotation);
                Sprite currentSprite = GetComponent<SpriteRenderer>().sprite;
                currentGhost.transform.localScale = this.transform.localScale;
                currentGhost.GetComponent<SpriteRenderer>().sprite = currentSprite;
                _ghostDivekickDelaySeconde = _ghostDelayInterval;
                Destroy(currentGhost, _timeGhostDivekickVisible);
            }
        }
    }
}
