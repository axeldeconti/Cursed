using UnityEngine;

namespace Cursed.Character
{
    public class CharacterSnap : MonoBehaviour
    {
        private CharacterMovement _character;


        private void Awake()
        {
            _character = GetComponentInParent<CharacterMovement>();
            //this.transform.position = Vector3.zero;
        }

        private void Update()
        {
            switch (_character.State)
            {
                case CharacterMovementState.Run:
                    this.transform.localPosition = new Vector3(.1f, this.transform.localPosition.y, this.transform.localPosition.z);
                    break;
                case CharacterMovementState.Idle:
                    this.transform.localPosition = new Vector3(0f, this.transform.localPosition.y, this.transform.localPosition.z);
                    break;
                case CharacterMovementState.WallRun:
                    this.transform.localPosition = new Vector3(-.3f, this.transform.localPosition.y, this.transform.localPosition.z);
                    break;

                default:
                    this.transform.localPosition = new Vector3(0f, this.transform.localPosition.y, this.transform.localPosition.z);
                    break;
            }
        }
    }
}

