using System.Collections;
using UnityEngine;
using Cursed.Props;

namespace Cursed.UI
{
    public class MapUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _mapObject;
        [SerializeField] private Animator _mapAnimator;
        private bool _mapActive;

        private void Start()
        {
            _mapActive = false;
            //_mapObject.SetActive(_mapActive);

            MapSpot spot = GetComponentInParent<MapSpot>();
            if (spot != null)
                spot._mapInteractionTriggered += () => ToggleMapActive();
        }

        public void DeactiveMap()
        {
            _mapActive = true;
            ToggleMapActive();
        }

        public void ToggleMapActive()
        {
            _mapActive = !_mapActive;

            if (_mapActive)
            {
                _mapAnimator.SetBool("Open", true);
                _mapAnimator.SetBool("Close", false);
            }
            else
            {
                _mapAnimator.SetBool("Open", false);
                _mapAnimator.SetBool("Close", true);
            }
        }
    }
}
