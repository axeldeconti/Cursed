using UnityEngine;
using TMPro;

public class TextChanger : MonoBehaviour
{
    private TextMeshProUGUI remainingAndroid;

    // Start is called before the first frame update
    void Start()
    {
        remainingAndroid = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        remainingAndroid.text = GameObject.FindGameObjectsWithTag("Enemy").Length.ToString();
    }
}
