using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateMaxBar : MonoBehaviour
{
    [SerializeField] private IntReference _maxValue;
    private int _lastMaxValue;
    private RectTransform rectTransform;
    private float transformPositionX, transformWidth;

    // Start is called before the first frame update
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        transformPositionX = rectTransform.anchoredPosition.x;
        transformWidth = rectTransform.sizeDelta.x;
        _lastMaxValue = _maxValue;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateValue(int value)
    {
        
        int offsetValue = value - (value - _lastMaxValue);
        float f = (offsetValue * 100) / transformWidth;
        transformWidth += f;
        transformPositionX += (f / 2);

        rectTransform.anchoredPosition = new Vector2(transformPositionX, rectTransform.anchoredPosition.y);
        rectTransform.sizeDelta = new Vector2(transformWidth, rectTransform.sizeDelta.y);
        _lastMaxValue = value;
    }
}
