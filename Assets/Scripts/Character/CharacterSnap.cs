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
            if (_character.Side == 1 && _character.State == CharacterMovementState.Run)
            {
                this.transform.localPosition = new Vector3(.1f, this.transform.localPosition.y, this.transform.localPosition.z);
                Debug.Log(this.transform.localPosition);    
            }
            else if (_character.Side == -1 && _character.State == CharacterMovementState.Run)
                this.transform.localPosition = new Vector3(-.1f, this.transform.localPosition.y, this.transform.localPosition.z);

            if (_character.State == CharacterMovementState.Idle)
                this.transform.localPosition = new Vector3(0f, this.transform.localPosition.y, this.transform.localPosition.z);


        }

    }
}

