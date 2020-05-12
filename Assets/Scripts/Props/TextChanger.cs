using UnityEngine;
using TMPro;
using System.Collections;

public class TextChanger : MonoBehaviour
{
    [SerializeField] private Material _myBaseShader;
    [SerializeField] private float _changingShaderDuration = 1f;
    [SerializeField] private Shader _shaderGlitch;
    [SerializeField] private Shader _shaderTextMeshPro;

    public void OnEnemyDeath()
    {
        StartCoroutine(TimerForSwitchShader());
    }

    IEnumerator TimerForSwitchShader()
    {
        _myBaseShader.shader = _shaderGlitch;
        yield return new WaitForSeconds(_changingShaderDuration);
        _myBaseShader.shader = _shaderTextMeshPro;        
    }
}
