using System.Collections;
using UnityEngine;

namespace Cursed.Creature
{
    public class CreatureJoystickDirection : MonoBehaviour
    {
        private Transform _target;
        private float _directionX, _directionY;

        private void Start()
        {
            _directionX = Input.GetAxis("HorizontalRight");
            _directionY = Input.GetAxis("VerticalRight");
            _target = transform.GetChild(0);
        }

        private void Update()
        {
            Vector2 dir = new Vector2(Input.GetAxis("HorizontalRight"), Input.GetAxis("VerticalRight"));
            UpdateTargetPosition(dir);
        }

        private void UpdateTargetPosition(Vector2 dir)
        {
            _target.position = new Vector3(dir.x, dir.y, this.transform.position.z);
        }
    }
}
