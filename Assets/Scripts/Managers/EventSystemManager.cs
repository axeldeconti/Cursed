using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Cursed.Managers
{
    public class EventSystemManager : MonoBehaviour
    {
        private EventSystem _eventSystem = null;

        [SerializeField] private FloatReference _joystickThreshold = null;

        private void Awake()
        {
            _eventSystem = GetComponent<EventSystem>();
        }

        private void Update()
        {
            float x = Input.GetAxis("Horizontal");
            float y = Input.GetAxis("Vertical");

            if(Mathf.Abs(x) >= _joystickThreshold || Mathf.Abs(y) >= _joystickThreshold)
            {
                if(_eventSystem.currentSelectedGameObject == null)
                {
                    _eventSystem.firstSelectedGameObject.GetComponent<Selectable>().Select();
                }
            }
        }
    }
}