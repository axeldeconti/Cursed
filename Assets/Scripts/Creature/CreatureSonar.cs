using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureSonar : MonoBehaviour
    {
        [SerializeField] private GameObject _sonarReference;
        private CreatureInputController _input;
        private CreatureManager _creature;
        private GameObject _sonarObject;

        private void Awake()
        {
            _input = GetComponent<CreatureInputController>();
            _creature = GetComponent<CreatureManager>();
        }

        private void Update()
        {
            UpdateInput();
        }

        private void UpdateInput()
        {
            if (_creature.CurrentState != CreatureState.OnCharacter)
            {
                if (_sonarObject != null)
                    Destroy(_sonarObject);

                return;
            }

            if (_input.Sonar)
            {
                if (_sonarObject == null)
                {
                    _sonarObject = Instantiate(_sonarReference, GameObject.FindGameObjectWithTag("Player").transform.GetChild(0).position, Quaternion.identity, transform);
                    _sonarObject.GetComponent<CreatureJoystickLine>().LerpSize(false);
                }
                else
                    _sonarObject.GetComponent<CreatureJoystickLine>().LerpSize(false);
            }
            else
            {
                if(_sonarObject != null)
                    _sonarObject.GetComponent<CreatureJoystickLine>().LerpSize(true);
            }
        }
    }
}
