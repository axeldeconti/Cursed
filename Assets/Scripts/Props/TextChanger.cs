using UnityEngine;
using TMPro;
using System.Collections;

public class TextChanger : MonoBehaviour
{
/*        [SerializeField] private Material _myBaseShader;
        [SerializeField] private Shader _shaderGlitch;
        [SerializeField] private Shader _shaderTextMeshPro;


        

        IEnumerator TimerForSwitchShader()
        {
            _myBaseShader.shader = _shaderGlitch;
            yield return new WaitForSeconds(_changingShaderDuration);
            _myBaseShader.shader = _shaderTextMeshPro;        
        }*/

    public TMP_FontAsset FontAssetA;
    public TMP_FontAsset FontAssetB;

    public Material _shaderMat;
    public Material _TMPMat;

    private TMP_Text m_TextComponent;

    [SerializeField] private float _changingShaderDuration = 0.5f;


    private void Awake()
    {
        m_TextComponent = GetComponent<TMP_Text>();
    }

    public void HideText()
    {
        m_TextComponent.text = "";
    }

    public void OnEnemyDeath()
    {
        StartCoroutine(TimerForSwitchShader());
    }


    IEnumerator TimerForSwitchShader()
    {
        // Assign the new font asset.
        m_TextComponent.font = FontAssetA;

        // Use a different material preset which was derived from this font asset and created using the Create Material Preset Context Menu.
        m_TextComponent.fontSharedMaterial = _shaderMat;

        yield return new WaitForSeconds(_changingShaderDuration);

        m_TextComponent.font = FontAssetB;
        m_TextComponent.fontSharedMaterial = _TMPMat;
    }
}






