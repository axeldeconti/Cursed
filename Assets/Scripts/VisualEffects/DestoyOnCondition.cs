using UnityEngine;

namespace Cursed.VisualEffect
{
    public class DestoyOnCondition : MonoBehaviour
    {
        public delegate bool Condition();

        public Condition condition = null;

        private void Update()
        {
            if (condition())
                Destroy(gameObject);
        }
    }
}