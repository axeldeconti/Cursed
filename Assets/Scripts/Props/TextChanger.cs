using UnityEngine;
using TMPro;
using System.Collections;

public class TextChanger : MonoBehaviour
{
    private TextMeshProUGUI _remainingAndroid;
    private static int _numberOfEnemy;
    private int _myNumberOfEnemy;
    [SerializeField] private Material _myBaseShader;
    [SerializeField] private float _changingShaderDuration = 1f;
    [SerializeField] private Shader _shaderGlitch;
    [SerializeField] private Shader _shaderTextMeshPro;

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
        StartCoroutine(TimerForSwitchShader());
    }

    IEnumerator TimerForSwitchShader()
    {
        _myBaseShader.shader = _shaderGlitch;
        yield return new WaitForSeconds(_changingShaderDuration);
        UpdateText();
        _myBaseShader.shader = _shaderTextMeshPro;        
    }
}
