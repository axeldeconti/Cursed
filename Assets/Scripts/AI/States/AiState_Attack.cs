using Cursed.Character;
using UnityEngine;

namespace Cursed.AI
{
    [CreateAssetMenu(fileName = "New AiState Attack", menuName = "AI/States/Attack")]
    public class AiState_Attack : AiState
    {
        [SerializeField] private FloatReference _aggroRange = null;
        [SerializeField] private FloatReference _attackRange = null;

        [Header("Chances")]
        [SerializeField] private FloatReference _chancesToAttack = null;
        [SerializeField] private FloatReference _chancesToReAttack = null;
        [SerializeField] private FloatReference _chancesToDashWhenAttacked = null;

        private bool _attack = false;
        private bool _forceAttack = false;

        public override void OnStateEnter(AiController controller)
        {
            base.OnStateEnter(controller);

            _forceAttack = true;
        }

        public override void OnStateUpdate(AiController controller, ref AIData data)
        {
            base.OnStateUpdate(controller, ref data);

            _attack = false;

            if (controller.Atk.IsAttacking)
            {
                //Is attacking
                if (controller.Atk.CanICombo)
                {
                    //Can combo
                    _attack = ChanceToGetTrue(_chancesToReAttack);
                }
            }
            else
            {
                if (controller.Atk.CanICombo)
                {
                    //Can combo after an attack
                    _attack = ChanceToGetTrue(_chancesToReAttack);
                }
                else
                {
                    //Is not attacking, go attack
                    _attack = ChanceToGetTrue(_chancesToAttack);
                }
            }

            //Check if the target is attacking me
            if(controller.Target.GetComponent<CharacterAttackManager>().IsAttacking && !controller.Health.IsInvincible)
            {
                bool dash = ChanceToGetTrue(_chancesToDashWhenAttacked);
                data.dash = dash;
                _attack = _attack && !dash;
            }

            //Force attack when entering the attack state
            if (_forceAttack)
            {
                _attack = true;
                _forceAttack = false;
            }
            
            if (_attack)
            {
                int nb = ChooseAttack();

                if (nb == 1)
                    data.attack1 = true;
                else
                    data.attack2 = true;
            }
        }

        protected override void CheckForTransition(AiController controller, ref string newState)
        {
            if (!controller.TargetInRange(_aggroRange, false) || controller.Target == null)
                newState = "GroundPatrol";

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

            nb = ChanceToGetTrue(50 / _aiUpdateFrame) ? 1 : 2;

            return nb;
        }
    }
}