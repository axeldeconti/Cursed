using UnityEngine;

public class ExitSpawnTrigger : MonoBehaviour
{
    [SerializeField] private InteractiveDoor[] _interactiveDoors;

    public void ToggleDoors()
    {
        for (int i = 0; i < _interactiveDoors.Length; i++)
        {
            _interactiveDoors[i].ToggleDoor();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            ToggleDoors();
            Destroy(gameObject);
        }
    }
}
