using UnityEngine;

namespace Cursed.AI
{
    [CreateAssetMenu(fileName = "New AiState Tuto", menuName = "AI/States/Tuto")]
    public class AiState_Tuto : AiState
    {
        [SerializeField] private FloatReference _timeBetweenAttack = null;

        private float _currentTime = 0;

        public override void OnStateEnter(AiController controller)
        {
            _currentTime = _timeBetweenAttack;
        }

        public override void OnStateUpdate(AiController controller, ref AIData data)
        {
            _currentTime -= Time.deltaTime;

            if(_currentTime  <= 0)
            {
                _currentTime = _timeBetweenAttack;
                data.attack1 = true;
            }
        }

        protected override void CheckForTransition(AiController controller, ref string newState) { }
    }
}