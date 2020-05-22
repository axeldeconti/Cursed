using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Cursed.UI
{
    public class ControlsBehaviour : MonoBehaviour
    {
        [Header("Referencies")]
        [SerializeField] private GameObject[] _controlsObjects;
        [SerializeField] private GameObject[] _infoObjects;
        [SerializeField] private TMP_Text _headerTxt;
        [SerializeField] private TMP_Text _indexInfoTxt;
        private GameObject _firstSelectedButton;
        [SerializeField] private Button _button5;
        [SerializeField] private Button _button6;
        [SerializeField] private EventSystem _controlSystem;

        [Header("Data")]
        [SerializeField] private string[] _headersName;
        [SerializeField] private int _maxIndex = 2;

        private int _currentIndex;

        private void Start()
        {
            _currentIndex = 0;
            _maxIndex = _controlsObjects.Length - 1;
            _firstSelectedButton = _controlSystem.firstSelectedGameObject;
            CheckIndex();
            UpdateUI();
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
            _controlSystem.SetSelectedGameObject(_firstSelectedButton);
            CheckIndex();
            UpdateUI();
        }

        private void CheckIndex()
        {
            if (_currentIndex > _maxIndex)
                _currentIndex = 0;
            else if (_currentIndex < 0)
                _currentIndex = _maxIndex;

            if (_currentIndex > 0)
            {
                _button5.interactable = false;
                _button6.interactable = false;
                _button5.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                _button6.GetComponent<Image>().color = new Color(1, 1, 1, 0);
            }
            else
            {
                _button5.interactable = true;
                _button6.interactable = true;
                _button5.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                _button6.GetComponent<Image>().color = new Color(1, 1, 1, 1);
            }
        }

        private void UpdateUI()
        {
            for (int i = 0; i < _controlsObjects.Length; i++)
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
