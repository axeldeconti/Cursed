using UnityEngine;
using Cursed.Character;

namespace Cursed.Props
{
    public class CameraDetection : MonoBehaviour
    {
        [SerializeField] private VoidEvent _enterCameraLight;
        [SerializeField] private VoidEvent _exitCameraLight;

        private GameObject _refVfxDysfunction;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if(collision.GetComponent<PlayerInputController>())
            {
                _enterCameraLight?.Raise();
            }
            if(collision.GetComponent<CharacterMovement>())
            {
                CharacterMovement character = collision.GetComponent<CharacterMovement>();
                character.DisableMovementImmediatly();
                _refVfxDysfunction = character.GetComponent<VfxHandler>().Dysfunction(character.transform.position);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerInputController>())
            {
                _exitCameraLight?.Raise();
            }
            if (collision.GetComponent<CharacterMovement>())
            {
                CharacterMovement character = collision.GetComponent<CharacterMovement>();
                character.ActiveMovementImmediatly();
                Destroy(_refVfxDysfunction);
            }
        }
    }
}

