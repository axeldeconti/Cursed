using UnityEngine;
using TMPro;
using System.Collections;

namespace Cursed.UI
{
    public class DialogueText : MonoBehaviour
    {
        private TextMeshProUGUI _textDisplay;
        private RandomSubjectNumber _randomSubjectNumber;

        [SerializeField] private float _typingSpeed = .05f;

        private void Start()
        {
            _textDisplay = GetComponent<TextMeshProUGUI>();
            _randomSubjectNumber = GetComponent<RandomSubjectNumber>();
            StartCoroutine(Type());
        }

        private IEnumerator Type()
        {
            foreach(char letter in _randomSubjectNumber._sentence.ToCharArray())
            {
                _textDisplay.text += letter;
                yield return new WaitForSeconds(_typingSpeed);
            }
        }
    }
}
