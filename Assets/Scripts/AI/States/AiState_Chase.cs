using Cursed.Traps;
using UnityEngine;

namespace Cursed.AI
{
    [CreateAssetMenu(fileName = "New AiState Chase", menuName = "AI/States/Chase")]
    public class AiState_Chase : AiState
    {
        [SerializeField] private FloatReference _aggroRange = null;
        [SerializeField] private FloatReference _attackRange = null;

        public override void OnStateUpdate(AiController controller, ref AIData data)
        {
            base.OnStateUpdate(controller, ref data);

            //Set the target of the path agent
            int side = controller.Target.Position.x > controller.transform.position.x ? -1 : 1;
            controller.PathAgent.Target = controller.Target.Position;
            controller.PathAgent.Target = controller.PathAgent.Target + Vector3.right * side * 4;

            //Retrieve movement data from the path agent
            controller.PathAgent.AiMovement(ref data);

            //Check if there is a laser in front
            //Check if there is a laser in front
            bool shouldDash = false;
            RaycastHit2D[] laserCheck = controller.RaycastInFront(5);
            for (int i = 0; i < laserCheck.Length; i++)
            {
                if (laserCheck[i].collider && !controller.Move.IsDashing)
                    if (laserCheck[i].collider.GetComponent<LaserBeam>())
                        shouldDash = true;
            }
            data.dash = shouldDash;
        }

        protected override void CheckForTransition(AiController controller, ref string newState)
        {
            if (!controller.TargetInRange(_aggroRange, false) || controller.Target == null)
                newState = "GroundPatrol";

            if (controller.TargetInRange(_attackRange, true))
                newState = "Attack";
        }
    }
}