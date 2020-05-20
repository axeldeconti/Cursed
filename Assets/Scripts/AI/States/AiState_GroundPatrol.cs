﻿using UnityEngine;

namespace Cursed.AI
{
    [CreateAssetMenu(fileName = "New AiState Ground Patrol", menuName = "AI/States/Ground Patrol")]
    public class AiState_GroundPatrol : AiState
    {
        [SerializeField] private FloatReference _aggroRange = null;
        [SerializeField] private FloatReference _timeToChangePlatformTarget = null;

        private float _currentTimeToChangePlatformTarget = 0f;

        public override void OnStateEnter(AiController controller)
        {
            _currentTimeToChangePlatformTarget = 0;
        }

        public override void OnStateUpdate(AiController controller, ref AIData data)
        {
            base.OnStateUpdate(controller, ref data);

            //Check for a target if I have none
            if (!controller.Target)
            {
                RaycastHit2D hit = controller.RaycastInFront(_aggroRange);
                if (hit)
                {
                    //Set target to the AiTarget if one is hit
                    controller.Target = hit.collider.GetComponent<AiTarget>();
                }
            }

            if (!controller.IsMoving)
                _currentTimeToChangePlatformTarget -= Time.deltaTime;

            //It's time to change target
            if (_currentTimeToChangePlatformTarget <= 0)
            {
                //Change target
                controller.FindRandomPathTarget();

                //Reset timer
                _currentTimeToChangePlatformTarget = _timeToChangePlatformTarget;

                controller.IsMoving = true;
            }

            controller.PathAgent.AiMovement(ref data);
        }

        protected override void CheckForTransition(AiController controller, ref string newState)
        {
            //Switch to chase if player in range
            if (controller.TargetInRange(_aggroRange, true))
                newState = "Chase";
        }
    }
}