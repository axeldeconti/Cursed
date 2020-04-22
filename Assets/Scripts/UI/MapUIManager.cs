using UnityEngine;

namespace Cursed.UI
{
    public class MapUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _mapObject;
        private bool _mapActive;

        private void Start()
        {
            _mapActive = false;
            _mapObject.SetActive(_mapActive);
        }

        public void ToggleMapActive()
        {
            _mapActive = !_mapActive;
            _mapObject.SetActive(_mapActive);
        }
    }
}
