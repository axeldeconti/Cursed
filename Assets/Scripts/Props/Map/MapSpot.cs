using UnityEngine;
using UnityEngine.UI;
using Cursed.Character;

namespace Cursed.Props
{
    public class MapSpot : MonoBehaviour
    {
        [SerializeField] private Image _buttonImage;
        public event System.Action _mapInteractionTriggered;
        private bool _triggered;
        private PlayerInputController _playerInput = null;

        private void Start()
        {
            _buttonImage.enabled = false;
        }

        private void Update()
        {
            if (_playerInput != null)
            {
                if (_triggered && _playerInput.WorldInteraction)
                    _mapInteractionTriggered?.Invoke();
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _playerInput = collision.GetComponent<PlayerInputController>();
                _buttonImage.enabled = true;
                _triggered = true;
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("Player"))
            {
                _buttonImage.enabled = false;
                _triggered = false;
            }
        }
    }
}
