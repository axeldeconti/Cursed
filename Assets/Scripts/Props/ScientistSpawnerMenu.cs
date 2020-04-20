using UnityEngine;
using System.Collections;

public class ScientistSpawnerMenu : MonoBehaviour
{
    [SerializeField] private bool _flip;
    [SerializeField] private Transform _target;
    [SerializeField] private GameObject _scientist;

    private void Start()
    {
        StartCoroutine(WaitForSpawnScientist(0f));
    }

    private void SpawnScientist()
    {
        ScientistMovement scientist = Instantiate(_scientist, this.transform.position, Quaternion.identity, transform).GetComponent<ScientistMovement>();
        scientist.target = _target;
        scientist.GetComponent<SpriteRenderer>().sortingOrder = Random.Range(-19, 0);


        if (_flip)
            scientist.flip = -1;
        else
            scientist.flip = 1;
    }

    IEnumerator WaitForSpawnScientist(float delay)
    {
        yield return new WaitForSeconds(delay);
        SpawnScientist();
        StartCoroutine(WaitForSpawnScientist(Random.Range(2.5f, 5f)));
    }
}
