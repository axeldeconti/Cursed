using UnityEngine;

public class ScientistMovement : MonoBehaviour
{
    [HideInInspector] public Transform target;
    [HideInInspector] public int flip;
    private float _speed;
    private float _currentPosX;

    private void Start()
    {
        _currentPosX = transform.position.x;
        _speed = Random.Range(2.5f, 6f);
        transform.localScale = new Vector2(flip * transform.localScale.x, transform.localScale.y);
    }

    private void Update()
    {
        //transform.localPosition = Vector2.MoveTowards(this.transform.localPosition, target.localPosition, _speed * Time.deltaTime);
        _currentPosX = Mathf.MoveTowards(_currentPosX, target.position.x, _speed * Time.deltaTime);
        transform.position = new Vector3(_currentPosX, transform.position.y, 7.5f);
        CheckDistance();
    }

    private void CheckDistance()
    {
        if (transform.position.x == target.position.x)
            Destroy(gameObject);
    }
}
