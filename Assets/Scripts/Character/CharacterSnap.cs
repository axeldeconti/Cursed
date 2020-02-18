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
            if(_character.Side == 1)
                this.transform.localPosition = new Vector3(.15f, this.transform.localPosition.y, this.transform.localPosition.z);
            else if(_character.Side == -1)
                this.transform.localPosition = new Vector3(-.15f, this.transform.localPosition.y, this.transform.localPosition.z);
            
            if(_character.State == CharacterMovementState.Idle)
                this.transform.localPosition = new Vector3(0f, this.transform.localPosition.y, this.transform.localPosition.z);


        }

    }
}

