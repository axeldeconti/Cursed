using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureSonar : MonoBehaviour
    {
        [SerializeField] private GameObject _sonarReference;
        private CreatureInputController _input;
        private GameObject _sonarObject;

        private void Awake()
        {
            _input = GetComponent<CreatureInputController>();
        }

        private void Update()
        {
            UpdateInput();
        }

        private void UpdateInput()
        {
            if (_input.Sonar)
            {
                if(_sonarObject == null)
                    _sonarObject = Instantiate(_sonarReference, transform.position, Quaternion.identity, transform);
            }
            else
                Destroy(_sonarObject);
        }
    }
}
