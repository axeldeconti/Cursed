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
                this.transform.position = new Vector3(this.transform.position.x + .1f, this.transform.position.y, this.transform.position.z);
            else if(_character.Side == -1)
                this.transform.position = new Vector3(this.transform.position.x - .1f, this.transform.position.y, this.transform.position.z);
        }

    }
}

