using UnityEngine;

namespace Cursed.Creature
{
    public class SonarEnemyDetection : MonoBehaviour
    {
        private Transform _target;

        private void Update()
        {
            SearchCloserEnemy();
        }

        private void SearchCloserEnemy()
        {
            RaycastHit2D[] obj = Physics2D.CircleCastAll(transform.position, Mathf.Infinity, new Vector2(0f, 0f));
            float distance = Mathf.Infinity;
            Transform enemyTransform = null;
            foreach (RaycastHit2D hit in obj)
            {
                if (hit.collider.gameObject.CompareTag("Enemy"))
                {
                    if (Vector2.Distance(hit.point, transform.position) <= distance)
                    {
                        distance = Vector2.Distance(hit.point, transform.position);
                        Debug.Log(distance);
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
    }
}
    