using System.Collections;
using UnityEngine;

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
        }

        public void DeactiveMap()
        {
            _mapObject.SetActive(false);
        }

        public void ToggleMapActive()
        {
            _mapActive = !_mapActive;

            if(_mapActive)
                _mapObject.SetActive(_mapActive);
            else
            {
                _mapAnimator.SetTrigger("Close");
                StartCoroutine(WaitForActive(_mapObject, _mapActive, _mapAnimator.GetCurrentAnimatorClipInfo(0).Length));
            }
        }

        IEnumerator WaitForActive(GameObject go, bool active, float delay)
        {
            yield return new WaitForSeconds(delay);
            Debug.Log("Deactive");
            go.SetActive(active);
        }
    }
}
