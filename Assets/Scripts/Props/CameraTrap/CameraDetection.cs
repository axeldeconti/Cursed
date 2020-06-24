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
                AkSoundEngine.PostEvent("Play_CameraOnCharacter", gameObject);
                AkSoundEngine.SetState("LowPassCamera", "CameraOnChar");
            }
            if(collision.GetComponent<CharacterMovement>())
            {
                CharacterMovement character = collision.GetComponent<CharacterMovement>();
                character.DisableMovementImmediatly();
                character.InCameraVision = true;
                CharacterAttackManager characterAttacks = collision.GetComponent<CharacterAttackManager>();
                characterAttacks.InCameraVision = true;
                characterAttacks.AttacksUnlock = false;
                _refVfxDysfunction = character.GetComponent<VfxHandler>().Dysfunction(character.transform.position);
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.GetComponent<PlayerInputController>())
            {
                _exitCameraLight?.Raise();
                AkSoundEngine.PostEvent("Stop_CameraOnCharacter", gameObject);
                AkSoundEngine.SetState("LowPassCamera", "CameraOffChar");
            }
            if (collision.GetComponent<CharacterMovement>())
            {
                CharacterMovement character = collision.GetComponent<CharacterMovement>();
                character.ActiveMovementImmediatly();
                character.InCameraVision = false;
                CharacterAttackManager characterAttacks = collision.GetComponent<CharacterAttackManager>();
                characterAttacks.InCameraVision = false;
                characterAttacks.AttacksUnlock = true;
                Destroy(_refVfxDysfunction);
            }
        }
    }
}

