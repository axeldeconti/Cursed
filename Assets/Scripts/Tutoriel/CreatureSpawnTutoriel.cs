using UnityEngine;

namespace Cursed.Tutoriel
{
    public class CreatureSpawnTutoriel : MonoBehaviour
    {
        [SerializeField] private GameObject _creature;
        [SerializeField] private GameObject _creatureLife;
        [SerializeField] private GameObject _creatureStamina;

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                Instantiate(_creature, transform.GetChild(0).position, Quaternion.identity);
                _creatureLife.SetActive(true);
                _creatureStamina.SetActive(true);
            }
        }
    }
}
