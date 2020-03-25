using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeonScript : MonoBehaviour
{
    public Animation flickeringNeon;

    void Start()
    {
        flickeringNeon = GetComponent<Animation>();
    }
}
