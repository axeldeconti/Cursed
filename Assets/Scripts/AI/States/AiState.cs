using UnityEngine;

namespace Cursed.AI
{
    public abstract class AiState : ScriptableObject
    {
        [SerializeField] private string _name = "New AiState";
        [SerializeField] protected IntReference _aiUpdateFrame = null;

        private string _newState = "None";

        public virtual void OnStateUpdate(AiController controller, ref AIData data)
        {
            //Check if the state needs to transition to a new one
            _newState = "None";
            CheckForTransition(controller, ref _newState);

            if (!_newState.Equals("None"))
            {
                //Go to new state
                controller.SetState(this, _newState);
                return;
            }
        }

        public virtual void OnStateEnter(AiController controller) { }
        public virtual void OnStateExit(AiController controller) { }

        protected abstract void CheckForTransition(AiController controller, ref string newState);

        /// <summary>
        /// Return random true or false depending on the percent
        /// </summary>
        /// <param name="chancePercent">Chances to have true (0 = always false, 100 = always true)</param>
        /// <returns></returns>
        protected bool ChanceToGetTrue(float chancePercent)
        {
            return Random.Range(0, 100) <= chancePercent * _aiUpdateFrame;
        }

        public string Name => _name;
    }
}