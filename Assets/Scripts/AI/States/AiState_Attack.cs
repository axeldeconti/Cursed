using UnityEngine;

namespace Cursed.AI
{
    [CreateAssetMenu(fileName = "New AiState Attack", menuName = "AI/States/Attack")]
    public class AiState_Attack : AiState
    {
        [SerializeField] private FloatReference _attackRange = null;

        public override void OnStateUpdate(AiController controller, ref AIData data)
        {
            base.OnStateUpdate(controller, ref data);

            if (controller.Atk.IsAttacking)
            {
                //Is attacking
            }
            else
            {
                //Is not attacking, go attack
                int nb = ChooseAttack();

                if (nb == 1)
                    data.attack1 = true;
                else
                    data.attack2 = true;
            }
        }

        protected override void CheckForTransition(AiController controller, ref string newState)
        {
            if (!controller.TargetInRange(_attackRange, true))
                newState = "Chase";
        }

        /// <summary>
        /// Choose a weapon to attack with
        /// </summary>
        /// <returns>Number of the choosen weapon</returns>
        private int ChooseAttack()
        {
            int nb = 0;

            nb = Random.Range(0, 100) <= 50 ? 1 : 2;

            return nb;
        }
    }
}