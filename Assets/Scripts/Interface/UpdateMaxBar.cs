using UnityEngine;

public class UpdateMaxBar : MonoBehaviour
{
    [SerializeField] private IntReference _maxValue;
    private int _lastMaxValue;
    private RectTransform rectTransform;
    private float transformPositionX, transformWidth;

    private bool _canLerp;
    [SerializeField] private float _lerpSpeed = 3f;

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
        if(_canLerp)
        {
            float newPosition = Mathf.Lerp(rectTransform.anchoredPosition.x, transformPositionX, _lerpSpeed * Time.deltaTime);
            float newWidth = Mathf.Lerp(rectTransform.sizeDelta.x, transformWidth, _lerpSpeed * Time.deltaTime);
            rectTransform.anchoredPosition = new Vector2(newPosition, rectTransform.anchoredPosition.y);
            rectTransform.sizeDelta = new Vector2(newWidth, rectTransform.sizeDelta.y);
        }
        
    }

    public void UpdateValue(int value)
    {
        int offsetValue;
        if (value > _lastMaxValue)
            offsetValue = value - (value - _lastMaxValue);
        else
            offsetValue = (value - _lastMaxValue) - value;

        float f = (offsetValue * 100) / transformWidth;
        transformWidth += f;
        transformPositionX += (f / 2);
        _canLerp = true;    
        _lerpSpeed = Mathf.Abs(((float)value - (float)_lastMaxValue) / 10 * 3);
        _lastMaxValue = value;
    }


    #region GETTERS

    public int LastMaxValue => _lastMaxValue;

    #endregion
}
