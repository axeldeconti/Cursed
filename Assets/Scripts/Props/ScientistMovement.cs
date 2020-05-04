using UnityEngine;

public class ScientistMovement : MonoBehaviour
{
    [HideInInspector] public Transform target;
    [HideInInspector] public int flip;
    private float _speed;


    private void Start()
    {
        _speed = Random.Range(2.5f, 4f);
        transform.localScale = new Vector2(flip * transform.localScale.x, transform.localScale.y);
    }

    private void Update()
    {
        transform.position = Vector2.MoveTowards(this.transform.position, target.position, _speed * Time.deltaTime);
        CheckDistance();
    }

    private void CheckDistance()
    {
        if (transform.position.x == target.position.x)
            Destroy(gameObject);
    }
}
