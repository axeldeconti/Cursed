using UnityEngine;

public class NeonScript : MonoBehaviour
{
    public Animator flickeringNeon;
    private int animClipNumber;

    void Start()
    {
        flickeringNeon = GetComponent<Animator>();
        animClipNumber = 3;
        flickeringNeon.SetInteger("AnimNumber", Random.Range(0, animClipNumber));        
    }
}
