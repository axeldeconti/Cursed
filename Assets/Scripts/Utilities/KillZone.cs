using UnityEngine;

namespace Cursed.Utilities
{
    public class KillZone : MonoBehaviour
    {
        [SerializeField] private VoidEvent _onDeathEvent;

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
                _onDeathEvent?.Raise();
        }
    }
}
