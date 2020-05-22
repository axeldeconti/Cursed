using UnityEngine;
using TMPro;

namespace Cursed.UI
{
    public class ControlsBehaviour : MonoBehaviour
    {
        [Header("Referencies")]
        [SerializeField] private GameObject[] _controlsObjects;
        [SerializeField] private GameObject[] _infoObjects;
        [SerializeField] private TMP_Text _headerTxt;
        [SerializeField] private TMP_Text _indexInfoTxt;

        [Header("Data")]
        [SerializeField] private string[] _headersName;
        [SerializeField] private int _maxIndex = 2;

        private int _currentIndex;

        private void Start()
        {
            _currentIndex = 0;
            _maxIndex = _controlsObjects.Length - 1;
        }

        private void Update()
        {
            if (Input.GetButtonDown("WorldInteraction"))
                IncreaseIndex(1);

            if (Input.GetButtonDown("LeftBumper"))
                IncreaseIndex(-1);
        }

        private void IncreaseIndex(int index)
        {
            _currentIndex += index;
            CheckIndex();
            UpdateUI();
        }

        private void CheckIndex()
        {
            if (_currentIndex > _maxIndex)
                _currentIndex = 0;
            else if (_currentIndex < 0)
                _currentIndex = _maxIndex;
        }

        private void UpdateUI()
        {
            for(int i = 0; i < _controlsObjects.Length; i++)
            {
                _controlsObjects[i].SetActive(false);
                _infoObjects[i].SetActive(false);
            }
            _controlsObjects[_currentIndex].SetActive(true);
            _infoObjects[_currentIndex].SetActive(true);
            _headerTxt.text = _headersName[_currentIndex];
            _indexInfoTxt.text = (_currentIndex + 1).ToString() + " / " + (_maxIndex + 1).ToString();
        }
    }
}
