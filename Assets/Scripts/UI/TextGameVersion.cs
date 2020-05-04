using UnityEngine;
using TMPro;

namespace Cursed.UI
{
    public class TextGameVersion : MonoBehaviour
    {
        private TMP_Text _text;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
        }

        private void Start()
        {
            _text.text = "v " + GameManager.Instance.GetGameVersion();
        }
    }
}
