using UnityEngine;
using TMPro;

public class TextChanger : MonoBehaviour
{
    private TextMeshProUGUI _remainingAndroid;
    private static int _numberOfEnemy;
    private int _myNumberOfEnemy;

    private void Awake()
    {
        _remainingAndroid = GetComponent<TextMeshProUGUI>();
        _numberOfEnemy = 0;
        _myNumberOfEnemy = 0;
    }

    private void UpdateText()
    {
        _remainingAndroid.text = (_numberOfEnemy + 1).ToString();
    }

    public void AddEnemy()
    {
        if (_myNumberOfEnemy == _numberOfEnemy)
        {
            _numberOfEnemy++;
            _myNumberOfEnemy++;
        }
        else
        {
            _myNumberOfEnemy = _numberOfEnemy;
        }
        UpdateText();
    }

    public void OnEnemyDeath()
    {
        if (_myNumberOfEnemy == _numberOfEnemy)
        {
            _numberOfEnemy--;
            _myNumberOfEnemy--;
        }
        else
        {
            _myNumberOfEnemy = _numberOfEnemy;
        }
        UpdateText();
    }
}
