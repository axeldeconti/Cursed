using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextHP : MonoBehaviour
{
    [Tooltip("Value to use as the current ")]
    public FloatReference Variable;

    public Text text;

    private void Start()
    {
        text.text = Variable.Value.ToString();
    }

    private void Update()
    {
        text.text = Variable.Value.ToString();
    }
}
