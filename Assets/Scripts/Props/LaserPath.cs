using UnityEngine;

public class LaserPath : MonoBehaviour
{
    [SerializeField] private float speed;
    public Transform[] moveSpots;
    private int index;

        // Update is called once per frame
    void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, moveSpots[index].position, speed * Time.deltaTime);
        if (transform.position.x == moveSpots[index].position.x)
        {
            if (index >= 1)
                index = 0;
            else
                index++;
        }
            }
}
