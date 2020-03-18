using UnityEngine;
using CodeMonkey.Utils;

namespace Cursed.Creature
{
    public class SonarEnemyDetection : MonoBehaviour
    {
        private Transform _target;
        private Vector2 _targetDirection;

        private void Update()
        {
            _target = GetClosestEnemy();
            _targetDirection = GetEnemyDirection(_target);
            RotateToTarget();

            if (!GetComponentInChildren<SpriteRenderer>().enabled)
                GetComponentInChildren<SpriteRenderer>().enabled = true;
        }

        private void SearchCloserEnemy()
        {
            RaycastHit2D[] obj = Physics2D.CircleCastAll(transform.position, 10000f, new Vector2(0f, 0f));
            float distance = Mathf.Infinity;
            Transform enemyTransform = null;
            foreach (RaycastHit2D hit in obj)
            {
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    Debug.Log(Vector2.Distance(hit.point, transform.position));
                    if (Vector2.Distance(hit.point, transform.position) < distance)
                    {
                        Debug.Log("Hit point : " + hit.point);
                        distance = Vector2.Distance(hit.point, transform.position);
                        enemyTransform = hit.collider.transform;
                    }
                }
            }
            if(enemyTransform != null)
            {
                _target = enemyTransform;
                Debug.Log(_target.name);
            }
        }

        private Transform GetClosestEnemy()
        {
            Transform tMin = null;
            float minDist = Mathf.Infinity;
            Vector3 currentPos = transform.position;
            foreach (GameObject go in GameObject.FindGameObjectsWithTag("Enemy"))
            {
                float dist = Vector3.Distance(go.transform.position, currentPos);
                if (dist < minDist)
                {
                    tMin = go.transform.GetChild(0);
                    minDist = dist;
                }
            }
            return tMin;
        }

        private Vector2 GetEnemyDirection(Transform target)
        {
            Vector2 _direction;
            _direction = target.position - transform.position;
            return _direction;
        }

        private void RotateToTarget()
        {
            if (_targetDirection != null)
            {
                float angle = UtilsClass.GetAngleFromVectorFloat(_targetDirection);
                transform.localEulerAngles = new Vector3(0, 0, angle);
            }
        }

    }
}
    