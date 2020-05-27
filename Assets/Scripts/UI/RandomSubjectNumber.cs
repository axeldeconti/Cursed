using UnityEngine;
using TMPro;

namespace Cursed.UI
{
    public class RandomSubjectNumber : MonoBehaviour
    {
        public string _sentence { get; private set; }
        private TMP_Text _text;
        private int _number;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            _text.text = "";
            _sentence = "subject _00" + ChooseRandomNumber().ToString();
        }

        private int ChooseRandomNumber()
        {
            _number = Random.Range(1000, 9999);
            return _number;
        }
    }
}
